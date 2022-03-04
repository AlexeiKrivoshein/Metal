using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;

namespace MetalClient.ViewModel
{
    public abstract class ElementViewModelBase<T> 
        : IElementViewModel<T>
        where T : BaseDTO, new()
    {
        public ClientDataManager DataManager;
        protected Guid _id;
        protected Dispatcher _dispatcher;

        public ElementState State { get; set; }

        //управление прогрессом загрузки данных
        public Func<string, CancellationToken> PrgShow;
        public Action PrgHide;

        public event Action<string> OnError;
        public event Action<string> OnInform;

        protected void ErrorInvoke(string message) => OnError?.BeginInvoke(message, null, null);
        protected void InformInvoke(string message) => OnInform?.BeginInvoke(message, null, null);

        protected abstract DatagramType SetDatagramType { get; }
        protected abstract DatagramType GetDatagramType { get; }

        private T _element;
        public T Element
        {
            get { return _element; }
            set
            {
                _element = value;
                OnPropertyChanged("Element");
            }
        }

        private Locker _lockObject;
        public Locker LockObject
        {
            get => _lockObject;
            set
            {
                _lockObject = value;
                OnPropertyChanged(nameof(LockObject));
            }
        }

        private string _header;
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        #region Комманды
        // Сохранение контрагента
        public event Action OnSave;
        protected void SaveInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnSave?.Invoke()));
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {
                      Save();
                  }, (x) => SaveValidation()));
            }
        }

        // Сохранение контрагента
        public event Action OnClose;
        protected void CloseInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnClose?.Invoke()));
        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                  (_closeCommand = new RelayCommand(obj =>
                  {
                      Close();
                      OnClose?.Invoke();
                  }));
            }
        }
        #endregion

        public ElementViewModelBase(Guid id, ClientDataManager dataManager, ElementState state)
        {
            _id = id;
            DataManager = dataManager;
            LockObject = new Locker(DataManager.SecurityContext.Rights);
            _dispatcher = Application.Current.Dispatcher;
            State = state;

            if (State == ElementState.Created)
            {
                Element = new T();
            }
        }

        private bool CheckLoadProcess(Task task)
        {
            if (!TaskHelper.CheckError(task, out var error))
            {
                PrgHide?.Invoke();
                ErrorInvoke(error);
                
                return false;
            }
            else
            {
                State = ElementState.Loaded;
                return true;
            }
        }

        private bool CheckSaveProcess(Task task, TaskCompletionSource<bool> tcs)
        {
            if (!TaskHelper.CheckError(task, out var error))
            {
                PrgHide?.Invoke();
                ErrorInvoke(error);
                tcs.SetResult(false);

                return false;
            }
            else
            {
                State = ElementState.Saved;
                return true;
            }
        }

        protected bool CheckSaveProcess(Task task)
        {
            if (!TaskHelper.CheckError(task, out var error))
            {
                PrgHide?.Invoke();
                ErrorInvoke(error);
                
                return false;
            }
            else
            {
                State = ElementState.Saved;
                return true;
            }
        }

        private bool CheckCancelled(CancellationToken token, TaskCompletionSource<bool> tcs)
        {
            if (token.IsCancellationRequested)
            {
                PrgHide?.Invoke();
                tcs.SetCanceled();
                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool CheckCancelled(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                PrgHide?.Invoke();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Инициализация модели (напр справочники) 
        /// выполняется как при создании нового элемента так и при загрузке существующего
        /// </summary>
        protected virtual Task<bool> InitViewModel(CancellationToken token) { return TaskHelper.FromResult(true); }

        #region Загрузка
        /// <summary>
        /// Загрузка элемента
        /// </summary>
        /// <returns></returns>
        public Task<bool> Load(bool silent = false)
        {
            var tcs = new TaskCompletionSource<bool>();
            //прогресс
            var token = CancellationToken.None;
            if (!silent)
            {
                token = PrgShow?.Invoke("Загрузка...") ?? CancellationToken.None;
            }

            if (State == ElementState.Created)
            {
                InitViewModel(token).ContinueWith((t0) =>
                {
                    if (ValidateStateToContinue(t0, tcs, token))
                    {
                        Element = new T();
                        if (!silent) PrgHide?.Invoke();
                        tcs.SetResult(true);
                    }
                });
            }
            else
            {
                InitViewModel(token)
                .ContinueWith((t0) =>
                {
                    if (!ValidateStateToContinue(t0, tcs, token))
                    {
                        return;
                    }

                    BeforeLoad(token)
                    .ContinueWith((t1) =>
                    {
                        if (!ValidateStateToContinue(t1, tcs, token))
                        {
                            return;
                        }

                        State = ElementState.Loading;

                        InnerLoad(token)
                        .ContinueWith((t2) =>
                        {
                            if (!ValidateStateToContinue(t2, tcs, token))
                            {
                                return;
                            }

                            AfterLoad(token)
                            .ContinueWith((t3) =>
                            {
                                if (!ValidateStateToContinue(t3, tcs, token))
                                {
                                    return;
                                }

                                State = ElementState.Loaded;

                                if (!silent) PrgHide?.Invoke();
                                tcs.SetResult(true);
                            });
                        });
                    });
                });
            }

            return tcs.Task;
        }

        private bool ValidateStateToContinue(Task<bool> t0, TaskCompletionSource<bool> tcs, CancellationToken token)
        {
            var check = TaskHelper.CheckTask(t0, token, out var error);
            if (check != CheckState.Success)
            {
                PrgHide?.Invoke();

                if (check == CheckState.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (check == CheckState.Error)
                {
                    ErrorInvoke(error);
                    tcs.SetResult(false);
                }

                return false;
            }

            return true;
        }



        /// <summary>
        /// Действия перед началом загрузки (подготовка объекта)
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <param name="processToken">Токен отмены последущих ContinueWith операций</param>
        /// <returns></returns>
        protected virtual Task<bool> BeforeLoad(CancellationToken token) { return TaskHelper.FromResult(true); }

        /// <summary>
        /// Основной алгоритм загрузки
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <param name="processToken">Токен отмены последущих ContinueWith операций</param>
        /// <returns></returns>
        private Task<bool> InnerLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(GetDatagramType).WithDTOObject(new GetElementData(_id)).Build();

            return DataManager.ExcuteRequestAsync<T>(request, token)
            .ContinueWith<bool>((t) =>
            {
                if (!CheckCancelled(token) ||
                    !CheckLoadProcess(t))
                {
                    return false;
                }

                Element = t.Result;
                return true;
            }); 
        }

        /// <summary>
        /// Действия после загрузки
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <returns></returns>
        protected virtual Task<bool> AfterLoad(CancellationToken token) { return TaskHelper.FromResult(true); }
        #endregion

        #region Сохранение
        /// <summary>
        /// Сохранение
        /// </summary>
        /// <returns></returns>
        protected virtual Task Save()
        {
            var tcs = new TaskCompletionSource<bool>();

            //источник токенов отмены цепочки операций
            var cts = new CancellationTokenSource();
            var processToken = cts.Token;
            //прогресс
            var token = PrgShow?.Invoke("Сохранение...") ?? CancellationToken.None;

            BeforeSave(token, cts)
                .ContinueWith((t1) =>
                {
                    State = ElementState.Saving;

                    if (!CheckCancelled(token, tcs) ||
                        !CheckSaveProcess(t1, tcs))
                    {
                        return;
                    }

                    InnerSave(token, cts)
                    .ContinueWith((t2) =>
                    {
                        if (!CheckCancelled(token, tcs) ||
                            !CheckSaveProcess(t1, tcs))
                        {
                            return;
                        }

                        AfterSave(token, t2.Result)
                        .ContinueWith((t3) =>
                        {
                            if (!CheckCancelled(token, tcs) ||
                                !CheckSaveProcess(t1, tcs))
                            {
                                return;
                            }

                            PrgHide?.Invoke();
                            State = ElementState.Saved;
                            tcs.SetResult(true);

                            OnSave?.Invoke();
                        });
                    }, processToken);
                }, processToken);
            return tcs.Task;
        }

        /// <summary>
        /// Действия перед началом сохранения (подготовка данных)
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <param name="processToken">Токен отмены последущих ContinueWith операций</param>
        /// <returns></returns>
        protected virtual Task BeforeSave(CancellationToken token, CancellationTokenSource processToken) { return TaskHelper.Complite(); }

        /// <summary>
        /// Основной алгоритм сохранения
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <param name="processToken">Токен отмены последущих ContinueWith операций</param>
        /// <returns></returns>
        protected virtual Task<HandledDTO> InnerSave(CancellationToken token, CancellationTokenSource processToken)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(SetDatagramType).WithDTOObject(Element).Build();

            var task = DataManager.ExcuteRequestAsync<HandledDTO>(request, token)
            .ContinueWith((t) =>
            {
                if (!CheckCancelled(token) ||
                    !CheckSaveProcess(t))
                {
                    processToken.Cancel();
                    return null;
                }
                else
                {
                    return t.Result;
                }
            });

            return task;
        }

        /// <summary>
        /// Действия после сохранения
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <returns></returns>
        protected virtual Task AfterSave(CancellationToken token, HandledDTO parameter) { return TaskHelper.Complite(); }

        /// <summary>
        /// Действия после загрузки
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <returns></returns>
        protected virtual Task Close() { return TaskHelper.Complite(); }

        /// <summary>
        /// Валидация сохранения
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveValidation() { return true; }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
