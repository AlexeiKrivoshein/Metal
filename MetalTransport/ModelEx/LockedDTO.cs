using MetalTransport.Helper;
using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
 
    [Serializable]
    public sealed class LockedDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private Guid _lockedObject;
        public Guid LockedObject
        {
            get => _lockedObject;
            set
            {
                _lockedObject = value;
                OnPropertyChanged("LockedObject");
            }
        }

        private DateTime _lockDate = DateTimeHelper.DateTimeNow();
        public DateTime LockDate
        {
            get => _lockDate;
            set
            {
                _lockDate = value;
                OnPropertyChanged("LockDate");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is LockedDTO other))
                return false;

            return this.LockedObject == other.LockedObject &&
                   this.LockDate == other.LockDate &&
                   this.Id == other.Id;
        }
    }
}
