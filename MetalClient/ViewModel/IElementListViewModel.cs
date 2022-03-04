using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MetalClient.ViewModel
{
    /// <summary>
    /// Интерфейс модели списков
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IElementListViewModel<T>
        : IViewModel
        where T : class
    {
        bool IsSelected { get; set; }

        bool IsModify { get; set; }

        ObservableCollection<T> Elements { get; }

        ICollectionView DefaultView { get; }

        RelayCommand AddCommand { get; }

        RelayCommand SelectCommand { get; }

        RelayCommand RemoveCommand { get; }

        RelayCommand FilterCommand { get; }
    }
}
