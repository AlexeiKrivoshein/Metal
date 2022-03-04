using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class VersionListItemDTO
        : BaseListItemDTO
    {

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

        private bool _deleted;
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
            if (!(obj is VersionListItemDTO typed)) return false;

            return Name == typed.Name && 
                   Id == typed.Id &&
                   Version == typed.Version &&
                   Deleted == typed.Deleted;
        }
    }
}
