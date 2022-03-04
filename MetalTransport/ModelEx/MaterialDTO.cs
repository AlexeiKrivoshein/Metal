using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class MaterialDTO 
        : BaseDTO
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

        private long _version;
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
            if (!(obj is MaterialDTO other))
                return false;

            return this.Name == other.Name &&
                    this.Deleted == other.Deleted;
        }
    }
}
