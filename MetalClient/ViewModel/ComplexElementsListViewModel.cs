using MetalClient.DataManager;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    /// <summary>
    /// Модель для списков поддерживающих форму редактирования
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComplexElementsListViewModel<T>
        : ElementsListViewModelBase<T>
        where T : BaseDTO, new()
    {

        //Команда открыть
        public event Action OnOpen;
        protected void OpenInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnOpen?.Invoke()));
        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ??
                  (_openCommand = new RelayCommand(obj =>
                  {
                      if (Open())
                      {
                          OnOpen?.Invoke();
                      }
                  }));
            }
        }

        public ComplexElementsListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            :base(id, dataManager, selectType) { }

        protected override Task<Guid> AddElement()
        {
            var element = CreateElement();

            if (element == null) 
                return TaskHelper.FromResult(Guid.Empty);

            _id = element.Id;
            IsModify = true;

            //обновление списка элементов
            return Load(true).ContinueWith(t => _id);
        }

        /// <summary>
        /// Реализованный в потомках метод должен создавать, сохранять и возвращать элемент
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateElement();

        protected override Task<bool> Select()
        {
            if (SelectType == ElementListSelectType.Select)
            {
                return InnerSelect();
            }
            else
            {
                Open();
                return TaskHelper.FromResult(true);
            }
        }

        private bool Open()
        {
            var element = OpenElement();
            //TODO возвращать ElementViewModelBase, ставить отметку об изменении только при сохранении

            if (element == null) return false;

            _id = element.Id;

            IsModify = true;
            Load(true);

            return true;
        }

        /// <summary>
        /// Реализованный в потомках метод должен открывать окно редактирования, если для справочника предусмотрен сложная форма, сохранять и возвращать элемент
        /// </summary>
        /// <returns></returns>
        protected abstract T OpenElement();
    }
}
