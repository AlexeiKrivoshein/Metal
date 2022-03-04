using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class BaseListItemDTO 
        : BaseDTO
        , INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value && State == ItemState.Stored)
                {
                    State = ItemState.Modify;
                }

                _name = value;
                OnPropertyChanged("Name");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is BaseListItemDTO typed)) return false;

            return Name == typed.Name && Id == typed.Id;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Новая запись, не сохранена на сервере
        /// </summary>
        [field: NonSerialized]
        public ItemState State { get; set; } = ItemState.Stored;

        public enum ItemState 
        { 
            Stored,
            Modify,
            New
        }
    }
}
