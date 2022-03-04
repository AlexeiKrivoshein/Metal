using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FormTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //---------
        //FOR XAML BINDING.    
        //---------
        //Records for datagrid.  
        public ObservableCollection<House> houses { get; set; }

        //List of owners.  Each house record gets an owner object assigned.    
        public ObservableCollection<Owner> owners { get; set; }

        public House House { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            owners = new ObservableCollection<Owner>
            {
                new Owner {id = 1, name = "owner 1"},
                new Owner {id = 2, name = "owner 2"}
            };

            houses = new ObservableCollection<House>
            {
                new House {name = "house 1", ownerObj = owners[0]},
                new House {name = "house 2", ownerObj = owners[1]}
            };

            House = new House()
            {
                name = "House",
                ownerObj = new Owner()
                {
                    name = "Owner",
                    id = 1
                }
            };

            txtValue.DataContext = House.ownerObj;
        }

    private void CheckValue_Click(object sender, RoutedEventArgs e)
    {
        owners = new ObservableCollection<Owner>
        {
            new Owner {id = 1, name = "owner 1"},
            new Owner {id = 2, name = "owner 2"}
        };

        houses = new ObservableCollection<House>
        {
            new House {name = "house 1", ownerObj = owners[0]},
            new House {name = "house 2", ownerObj = owners[1]}
        };

        var message = "";
        foreach (var item in houses)
        {
            message += item.name + "\t" + item.ownerObj.id + Environment.NewLine;
        }
        MessageBox.Show(message);
    }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            House.ownerObj.name = "test_test";
        }
    }

    public class House : INotifyPropertyChanged
    {
        private string _name;

        public string name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                NotifyPropertyChanged("name");
            }
        }


        private Owner _ownerObj;

        public Owner ownerObj
        {
            get
            {
                return _ownerObj;
            }

            set
            {
                _ownerObj = value;
                NotifyPropertyChanged("ownerObj");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    //Owner is a combobox choice.  Each house is assigned an owner.    
    public class Owner : INotifyPropertyChanged
    {
        private int _id;

        public int id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                NotifyPropertyChanged("id");
            }
        }

        private string _name;

        public string name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                NotifyPropertyChanged(name);
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
