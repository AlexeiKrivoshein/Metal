using MetalClient.DataManager;
using MetalClient.Dialog;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace MetalClient.ViewModel
{

    public abstract partial class ElementsListViewModelBase<T>
        : IElementListViewModel<T>
        where T : BaseDTO, new()
    {
        public ClientDataManager DataManager;
        protected Guid _id;
        protected Dispatcher _dispatcher;

        private ICollectionView _defaultView;
        public ICollectionView DefaultView
        {
            get => _defaultView;
            set
            {
                _defaultView = value;
                OnPropertyChanged(nameof(DefaultView));
            }
        }

        public ElementListSelectType SelectType { get; set; }

        public bool IsSelected { get; set; } = false;

        public bool IsModify { get; set; } = false;

        //управление прогрессом загрузки данных
        public Func<string, StartProgressResult> PrgShow;
        public Action PrgHide;

        //уведомления
        public event Action<string> OnError;
        public event Action<string> OnInform;
        public event Action<bool> OnValidChange;

        //метод отображения окна элемента
        public Action<IViewModel> ShowElement;

        //доступ к уведомлениям для потомков
        protected void ErrorInvoke(string message) => OnError?.Invoke(message);
        protected void InformInvoke(string message) => OnInform?.Invoke(message);

        //типы датаграм списка
        protected abstract DatagramType SetDatagramType { get; }
        protected abstract DatagramType RemoveDatagramType { get; }

        //список элементов
        private ObservableCollection<T> _elements;
        public ObservableCollection<T> Elements
        {
            get => _elements;
            set
            {
                _elements = value;
                OnPropertyChanged(nameof(Elements));
            }
        }

        //объект доступа
        public Locker LockObject { get; set; }

        //заголвок
        private string _header = "";
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        //выбранный элемент
        private T _selected;
        public T Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                _id = _selected?.Id ?? Guid.Empty;
                OnPropertyChanged(nameof(Selected));
            }
        }

        protected ElementsListViewModelBase(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
        {
            _id = id;
            _dispatcher = Application.Current.Dispatcher;

            DataManager = dataManager;
            Elements = new ObservableCollection<T>();
            DefaultView = CollectionViewSource.GetDefaultView(Elements);
            LockObject = new Locker(dataManager.SecurityContext.Rights);
            SelectType = selectType;

            Elements.CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        /// <summary>
        /// Инициирование загрузки списка элементов
        /// </summary>
        /// <returns></returns>
        public Task<bool> Load(bool silent = false)
        {
            var tcs = new TaskCompletionSource<bool>();
            //прогресс
            var token = CancellationToken.None;
            if (!silent)
            {
                token = PrgShow?.Invoke("Загрузка...").Token ?? CancellationToken.None;
            }
            
            InnerLoad(token).ContinueWith((t1) =>
            {
                if (!TaskHelper.CheckError(t1, out var error))
                {
                    ErrorInvoke(error);
                    return false;
                }

                var list = t1.Result;

                SetElements(list);
                OnDataLoaded?.Invoke(list);

                return true;
            }).ContinueWith((t2) =>
            {
                if (t2.Result)
                {
                    AfterLoad(token).ContinueWith((t3) =>
                    {
                        if (!silent) 
                            PrgHide?.Invoke();

                        tcs.SetResult(t3.Result);
                    });
                } 
                else
                {
                    if (!silent) 
                        PrgHide?.Invoke();

                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        public event Action<List<T>> OnDataLoaded;

        protected void SetElements(List<T> list)
        {
            _dispatcher.Invoke(
                new Action(() =>
                {
                    var elements = new ObservableCollection<T>();
                    if (list is List<BaseListItemDTO> bList)
                    {
                        foreach (var item in bList.OrderBy(x => x.Name))
                        {
                            elements.Add(item as T);
                        }
                    }
                    else if (list is List<VersionListItemDTO> vList)
                    {
                        foreach (var item in vList.OrderBy(x => x.Name))
                        {
                            elements.Add(item as T);
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            elements.Add(item);
                        }
                    }

                    Elements.CollectionChanged -= FullObservableCollectionCollectionChanged;
                    Elements = elements;

                    T selected = null;
                    if (_id != Guid.Empty)
                    {
                        selected = Elements.FirstOrDefault(e => e.Id == _id);
                    }
                    else
                    {
                        selected = Elements.FirstOrDefault();
                        _id = selected?.Id ?? Guid.Empty;
                    }

                    if (selected != null)
                    {
                        Selected = selected;
                    }

                    DefaultView = CollectionViewSource.GetDefaultView(Elements);
                    Elements.CollectionChanged += FullObservableCollectionCollectionChanged;
                    SetOrder();
                }));
        }

        /// <summary>
        /// Реализованный в потомках метод должен загружать и возвращать список элементов
        /// </summary>
        /// <param name="token">Токен отмены из прогресс бара</param>
        /// <returns></returns>
        protected abstract Task<List<T>> InnerLoad(CancellationToken token);

        /// <summary>
        /// Действия после загрузки
        /// </summary>
        /// <param name="token">Токен отмены выполнения данной операции</param>
        /// <returns></returns>
        protected virtual Task<bool> AfterLoad(CancellationToken token) { return TaskHelper.FromResult(true); }

        protected abstract Task<Guid> AddElement();

        protected abstract Task<bool> Select();

        protected Task<bool> InnerSelect()
        {
            if (Selected is BaseListItemDTO named &&
                string.IsNullOrEmpty(named.Name))
            {
                return TaskHelper.FromResult(false);
            }

            var token = PrgShow?.Invoke("Обновление данных справочника").Token ?? CancellationToken.None;
            return InnerLoad(token).ContinueWith((t) =>
            {
                PrgHide?.Invoke();
                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return false;
                }

                IsSelected = true;
                return true;
            });
        }

        protected virtual Task<bool> Remove()
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(RemoveDatagramType)
                                 .WithDTOObject(new RemElementData(Selected.Id))
                                 .Build();

            var token = PrgShow?.Invoke("Удаление").Token ?? CancellationToken.None;
            return DataManager.ExcuteRequestAsync<HandledDTO>(request, token).ContinueWith(t1 =>
                {
                    PrgHide?.Invoke();
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        return false;
                    }

                    IsModify = true;
                    Load(true);

                    return true;
                });
        }

        /// <summary>
        /// Метод реализуемый справочниками, добавляет/обновляет элемент в локальном кеше
        /// </summary>
        /// <param name="element"></param>
        protected virtual void SetDictionaryElement(T element) { return; }

        /// <summary>
        /// Метод реализуемый справочниками, удаляет элемент в локальном кеше
        /// </summary>
        /// <param name="element"></param>
        protected virtual void RemoveDictionaryElement(T element) { return; }

        private void DropFilter()
        {
            DefaultView.Filter = null;
        }

        private void SetFilter(string filter)
        {
            DefaultView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(filter))
                    return true;
                else
                    return ((BaseListItemDTO)item).Name.ToUpper().Contains(filter.ToUpper());
            };
        }

        private void SetOrder()
        {
            var sortDescription = GetSortDescription();
            if (sortDescription.HasValue)
            {
                DefaultView.SortDescriptions.Add(sortDescription.Value);
            }
        }

        protected virtual SortDescription? GetSortDescription()
        {
            return null;
        }

        private void ValidateElements(object sender, PropertyChangedEventArgs e)
        {
            IsValid = !Elements.Any() || Elements.All(element => element.IsValid);
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if(_isValid != value)
                {
                    _isValid = value;
                    OnValidChange?.Invoke(_isValid);
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ValidateElements;
                }
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ValidateElements;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}