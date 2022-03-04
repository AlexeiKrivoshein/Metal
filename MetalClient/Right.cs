using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MetalClient
{
    public class Right : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isHeader;
        public bool IsHeader
        {
            get { return _isHeader; }
            set { _isHeader = value; OnPropertyChanged("IsHeader"); }
        }

        private byte _value;
        public byte Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public Right(string name, byte value, bool isHeader)
        {
            _value = value;
            _name = name;
            _isHeader = isHeader;
        }

        void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
