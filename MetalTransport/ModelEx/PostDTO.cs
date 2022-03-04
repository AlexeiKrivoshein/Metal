using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class PostDTO
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

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is PostDTO other))
                return false;

            return this.Name == other.Name;
        }
    }
 }
