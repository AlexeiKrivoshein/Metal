using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class OperationDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool Deleted { get; set; }

        private long _version = 0;
        public long Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is OperationDTO other))
                return false;

            return this.Name == other.Name &&
                    this.Deleted == other.Deleted;
        }
    }
}
