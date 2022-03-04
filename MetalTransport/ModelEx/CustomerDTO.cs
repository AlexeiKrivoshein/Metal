using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class CustomerDTO 
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

        private string _employee = "";
        public string Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged("Employee");
            }
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged("Phone");
            }
        }

        private string _mail = "";
        public string Mail
        {
            get => _mail;
            set
            {
                _mail = value;
                OnPropertyChanged("Mail");
            }
        }

        private string _fax = "";
        public string Fax
        {
            get => _fax;
            set
            {
                _fax = value;
                OnPropertyChanged("Fax");
            }
        }

        public bool Deleted { get; set; }

        public long Version { get; set; } = 0;

        public void CopyFrom(BaseDTO value)
        {
            if (value is CustomerDTO source)
            {
                Id = source.Id;
                Name = source.Name;
                Employee = source.Employee;
                Phone = source.Phone;
                Mail = source.Mail;
                Fax = source.Fax;
                Version = source.Version;
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is CustomerDTO other))
                return false;

            return this.Employee == other.Employee &&
                    this.Fax == other.Fax &&
                    this.Id == other.Id &&
                    this.Mail == other.Mail &&
                    this.Name == other.Name &&
                    this.Phone == other.Phone;
        }
    }
}
