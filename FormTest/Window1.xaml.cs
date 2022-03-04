using System;
using System.ComponentModel;
using System.Windows;


namespace Presentation
{
    public partial class Window1 : Window
    {
        public Department _department;

        public Window1()

        {
            InitializeComponent();

            _department = FindResource("department") as Department;

        }

        public void GiveRaise_Click(object sender, EventArgs e)

        {

            _department.Manager.Salary += 10;

        }

        public void ChangeManager_Click(object sender, EventArgs e)

        {

            if (_department.Manager == Manager.Kim)

                _department.Manager = Manager.Jim;

            else

                _department.Manager = Manager.Kim;

        }

    }

    public class Manager : INotifyPropertyChanged

    {

        public Manager(string name, decimal salary)

        {

            _name = name;

            _salary = salary;

        }

        public decimal Salary

        {

            get

            {

                return _salary;

            }

            set

            {

                if (_salary != value)

                {

                    _salary = value;

                    OnPropertyChanged("Salary");

                }

            }

        }

        public string Name

        {

            get

            {

                return _name;

            }

            set

            {

                if (_name != value)

                {

                    _name = value;

                    OnPropertyChanged("Name");

                }

            }

        }

        public static readonly Manager Jim = new Manager("Jim", 55);

        public static readonly Manager Kim = new Manager("Kim", 65);

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(String propertyName)

        {

            if (PropertyChanged != null)

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        private string _name;

        private decimal _salary;

    }

    public class Department : INotifyPropertyChanged

    {

        private Manager _manager;

        public Department()

        {

            _manager = new Manager("Вася", 10);

        }

        public Manager Manager

        {

            get

            {

                return _manager;

            }

            set

            {

                if (_manager != value)

                {

                    _manager = value;

                    OnPropertyChanged("Manager");

                }

            }

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(String propertyName)

        {

            if (PropertyChanged != null)

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}