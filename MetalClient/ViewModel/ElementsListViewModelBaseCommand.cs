using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MetalClient.ViewModel
{
    public abstract partial class ElementsListViewModelBase<T>
    {
        //Команда добавление элемента в список
        public event Action<Guid> OnAdd;
        protected void AddInvoke(T added) =>
            _dispatcher.BeginInvoke(new Action(() => OnAdd?.Invoke(added.Id)));
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new RelayCommand(obj =>
                  {
                      AddElement().ContinueWith(t =>
                      {
                          if (!t.IsFaulted && !t.IsCanceled && t.Result != Guid.Empty)
                              OnAdd?.Invoke(t.Result);
                      });
                  }));
            }
        }

        //Команда выбор
        public event Action OnSelected;
        protected void SelectedInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnSelected?.Invoke()));
        private RelayCommand _selectCommand;
        public RelayCommand SelectCommand
        {
            get
            {
                return _selectCommand ??
                  (_selectCommand = new RelayCommand(obj =>
                  {
                      Select().ContinueWith((t) =>
                      {
                          if (t.Result &&
                                SelectType == ElementListSelectType.Select)
                          {
                              Application.Current.Dispatcher.BeginInvoke(
                                        DispatcherPriority.Background,
                                        new Action(() => OnSelected?.Invoke()));
                          }
                      });

                  }, (obj) => Selected != null));
            }
        }

        //Команда удаление
        public event Action OnRemove;
        protected void RemoveInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnRemove?.Invoke()));
        private RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                  (_removeCommand = new RelayCommand(obj =>
                  {
                      Remove().ContinueWith((t) =>
                      {
                          if (t.Result)
                          {
                              OnRemove?.Invoke();
                          }
                      });
                  }, (obj) => Selected != null));
            }
        }

        //Команда фильтр
        public event Action OnFilter;
        protected void FilterInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnFilter?.Invoke()));
        private RelayCommand _filterCommand;
        public RelayCommand FilterCommand
        {
            get
            {
                return _filterCommand ??
                  (_filterCommand = new RelayCommand(obj =>
                  {
                      if (obj is string filter)
                      {
                          if (string.IsNullOrEmpty(filter))
                          {
                              DropFilter();
                          }
                          else
                          {
                              SetFilter(filter);
                          }

                          DefaultView.Refresh();
                          OnFilter?.Invoke();
                      }
                  }));
            }
        }
    }
}
