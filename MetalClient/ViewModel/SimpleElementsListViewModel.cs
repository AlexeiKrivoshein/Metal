using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetalClient.ViewModel
{
    /// <summary>
    /// Модель для списков не поддерживающих форму редактирования
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleElementsListViewModel<T>
        : ElementsListViewModelBase<T>
        where T : BaseListItemDTO, new()
    {
        public event Action<Guid> OnSave;

        //Команда сохранение измененного списка (для элементов не имеющих собственной формы)
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                  (_saveCommand = new RelayCommand(obj =>
                  {
                      Save().ContinueWith(t => OnSave?.Invoke(_id));
                  }, (obj) => Selected != null));
            }
        }

        private RelayCommand _applyCommand;
        public RelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand ??
                  (_applyCommand = new RelayCommand(obj =>
                  {
                      ApplySelected();
                  }, (obj) => Selected != null));
            }
        }

        public SimpleElementsListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(id, dataManager, selectType) { }

        protected override Task<Guid> AddElement()
        {
            var element = new T();
            element.State = BaseListItemDTO.ItemState.New;

            _dispatcher.Invoke(
              new Action(() =>
              {
                  Elements.Add(element);
                  Selected = element;
              }));

            return TaskHelper.FromResult(element.Id);
        }

        protected override Task<bool> Select()
        {
            if (SelectType == ElementListSelectType.Select)
            {
                return InnerSelect();
            }
            else
            {
                return TaskHelper.FromResult(true);
            }
        }

        private Task<bool> Save()
        {
            if (Selected is BaseListItemDTO named &&
                string.IsNullOrEmpty(named.Name))
            {
                _id = Selected.Id;
                return Load(true);
            }

            var factory = new DatagramFactory();
            var request = factory.WithType(SetDatagramType)
                                 .WithDTOObject(Selected)
                                 .Build();

            var token = PrgShow?.Invoke("Сохранение").Token ?? CancellationToken.None;
            return DataManager.ExcuteRequestAsync<HandledDTO>(request, token).ContinueWith(t1 =>
            {
                PrgHide?.Invoke();
                if (!TaskHelper.CheckError(t1, out var error))
                {
                    ErrorInvoke(error);
                    return false;
                }

                _id = Selected.Id;
                IsModify = true;

                //обновление списка элементов
                Load(true);
                //.ContinueWith(t2 => OnSave)

                return true;
            });
        }

        public void DefaultEnter()
        {
            if (Selected is null) return;

            if (Selected.State == BaseListItemDTO.ItemState.Modify ||
                Selected.State == BaseListItemDTO.ItemState.New)
            {
                if (SaveCommand.CanExecute(null)) SaveCommand.Execute(null);
            }
            else 
            {
                if (SelectCommand.CanExecute(null)) SelectCommand.Execute(null);
            }
        }

        public void ApplySelected()
        {
            if (Selected is null) return;

            if (Selected.State == BaseListItemDTO.ItemState.New && 
                string.IsNullOrEmpty(Selected.Name))
            {
                var index = Elements.IndexOf(Selected);

                _dispatcher.Invoke(
                    new Action(() =>
                    {
                        Elements.Remove(Selected);
                    })
                );

                if (index >= 0 && index < Elements.Count)
                {
                    Selected = Elements[index];
                }
                else
                {
                    if (Elements.Count > 0)
                    {
                        Selected = Elements[0];
                    }
                    else
                    {
                        Selected = null;
                    }
                }
            }
            else
            {
                DefaultEnter();
            }
        }
    }
}
