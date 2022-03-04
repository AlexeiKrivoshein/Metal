using System;
using System.ComponentModel;
using System.Linq;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class UserGroupDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        public string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private byte[] _rights { get; set; } = new byte[24];
        public byte[] Rights
        {
            get => _rights;
            set
            {
                _rights = value;
                OnPropertyChanged("Rights");
            }
        }

        private long _version { get; set; }
        public long Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        private bool _deleted { get; set; }
        public bool Deleted
        {
            get => _deleted;
            set
            {
                _deleted = value;
                OnPropertyChanged("Deleted");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is UserGroupDTO other))
                return false;

            return this.Name == other.Name &&
                    Enumerable.SequenceEqual(this.Rights, other.Rights) &&
                    this.Deleted == other.Deleted;
        }
    }
}
