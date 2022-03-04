using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public interface IViewModel
        : INotifyPropertyChanged
    {
        event Action<string> OnError;

        event Action<string> OnInform;

        Task<bool> Load(bool silent = false);
    }
}
