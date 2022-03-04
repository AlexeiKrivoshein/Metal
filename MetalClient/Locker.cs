using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MetalClient
{
    public sealed class Locker
        : INotifyPropertyChanged
    {
        private bool _notSaved = false;
        public bool NotSaved
        {
            get { return _notSaved; }
            set
            {
                _notSaved = value;
                OnPropertyChanged(nameof(NotSaved));
            }
        }

        private bool _stateLock = false;
        public bool StateLock
        {
            get { return _stateLock; }
            set
            {
                _stateLock = value;
                OnPropertyChanged(nameof(StateLock));
            }
        }

        private bool _userLock = false;
        public bool UserLock
        {
            get { return _userLock; }
            set
            {
                _userLock = value;
                OnPropertyChanged(nameof(UserLock));
            }
        }

        private byte[] _rights;
        public byte[] Rights
        {
            get { return _rights; }
            set
            {
                _rights = value;
                OnPropertyChanged(nameof(Rights));
            }
        }

        public Locker(byte[] rights)
        {
            Rights = rights;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
