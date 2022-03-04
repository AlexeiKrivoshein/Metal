namespace MetalClient.ViewModel
{
    public interface IElementViewModel<T> 
        : IViewModel 
        where T: class
    {
        ElementState State { get; }

        T Element { get; }

        RelayCommand SaveCommand { get; }
    }
}
