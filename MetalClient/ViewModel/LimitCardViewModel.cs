using MetalClient.DataManager;
using MetalClient.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class LimitCardViewModel
        : IViewModel, INotifyPropertyChanged
    {
        private bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public bool IsEditing => !_isReadOnly;

        //Суммы и затраченное время
        private double _sumMaterial = 0.0;
        public double SumMaterial
        {
            get => _sumMaterial;
            set
            {
                _sumMaterial = value;
                OnPropertyChanged(nameof(SumMaterial));
            }
        }

        private double _sumOperation = 0.0;
        public double SumOperation
        {
            get => _sumOperation;
            set
            {
                _sumOperation = value;
                OnPropertyChanged(nameof(SumOperation));
            }
        }

        public int _sumHour = 0;
        public int SumHour
        {
            get => _sumHour;
            set
            {
                _sumHour = value;
                OnPropertyChanged(nameof(SumHour));
            }
        }

        private int _sumMinutes = 0;
        public int SumMinutes
        {
            get => _sumMinutes;
            set
            {
                _sumMinutes = value;
                OnPropertyChanged(nameof(SumMinutes));
            }
        }

        private double _multiplier = 1.0;
        public double Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                OnPropertyChanged(nameof(Multiplier));
            }
        }

        private string _totalString = "";
        public string TotalString
        {
            get => _totalString;
            set
            {
                _totalString = value;
                OnPropertyChanged(nameof(TotalString));
            }
        }

        private bool _isFact = false;
        public bool IsFact
        {
            get => _isFact;
            set
            {
                _isFact = value;
                MaterialVM.IsFactMaterial = _isFact;
                OnPropertyChanged(nameof(IsFact));
            }
        }



        private ClientDataManager _dataManager;

        public Action<MaterialListViewModel> ShowMaterialList
        {
            get => MaterialVM.ShowMaterialList;
            set
            {
                MaterialVM.ShowMaterialList = value;
            }
        }

        public LimitCardMaterialListViewModel MaterialVM { get; set; }

        public Action<OperationListViewModel> ShowOperationList
        {
            get => OperationVM.ShowOperationList;
            set
            {
                OperationVM.ShowOperationList = value;
            }
        }

        public LimitCardOperationListViewModel OperationVM { get; set; }

        public ObservableCollection<LimitCardMaterialDTO> Materials => MaterialVM.Elements;
        public ObservableCollection<LimitCardOperationDTO> Operations => OperationVM.Elements;

        //удаленные элементы
        public List<Guid> RemovedMaterials => MaterialVM.RemovedMaterials;
        public List<Guid> RemovedOperations => OperationVM.RemovedOperations;

        public Func<string, CancellationToken> PrgShow;
        public Action PrgHide;

        // Сохранение
        public Action Hide;
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                  (_saveCommand = new RelayCommand(obj =>
                  {
                      IsModify = true;
                      Hide?.Invoke();
                  }));
            }
        }

        public RelayCommand SelectOperation => OperationVM.SelectOperationCommand;

        public RelayCommand SelectMaterial => MaterialVM.SelectMaterialCommand;

        public RelayCommand AddFactMaterial => MaterialVM.AddFactMaterialCommand;
        public RelayCommand SelectFactMaterial => MaterialVM.SelectFactMaterialCommand;
        public RelayCommand RemoveFactMaterial => MaterialVM.RemoveFactMaterialCommand;

        public bool IsModify { get; private set; } = false;

        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                  (_closeCommand = new RelayCommand(obj =>
                  {
                      IsModify = false;
                      Hide();
                  }));
            }
        }

        int _count;

        public LimitCardViewModel(
            ClientDataManager dataManager,
            Guid id,
            double multiplier,
            bool isFactMaterial,
            bool isReadOnly,
            int count)
        {
            _dataManager = dataManager;
            _count = count;

            MaterialVM = new LimitCardMaterialListViewModel(id, isFactMaterial, _dataManager, ElementListSelectType.Show);
            OperationVM = new LimitCardOperationListViewModel(id, isFactMaterial, _dataManager, ElementListSelectType.Show);

            IsReadOnly = isReadOnly;
            Multiplier = multiplier;
            IsFact = isFactMaterial;

            MaterialVM.OnValidChange += ValidateMaterialVM;
            OperationVM.OnValidChange += ValidateOperationVM;
        }

        public event Action<string> OnError
        {
            add
            {
                MaterialVM.OnError += value;
                OperationVM.OnError += value;
            }

            remove
            {
                MaterialVM.OnError -= value;
                OperationVM.OnError -= value;
            }
        }

        public event Action<string> OnInform
        {
            add
            {
                MaterialVM.OnInform += value;
                OperationVM.OnInform += value;
            }

            remove
            {
                MaterialVM.OnInform -= value;
                OperationVM.OnInform -= value;
            }
        }

        public Task<bool> Load(bool silent = false)
        {
            return Task.Factory.StartNew(() =>
            {
                var token = CancellationToken.None;
                if (!silent)
                {
                    token = PrgShow?.Invoke("Загрузка лимитной карточки") ?? CancellationToken.None;
                }

                var material = MaterialVM.Load();
                var operation = OperationVM.Load();

                Task.WaitAll(material, operation);

                if (!silent) PrgHide?.Invoke();

                if (material.Result && operation.Result)
                {
                    CalcOperation();
                    CalcMaterial();
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Расчет стоимости работ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CalcOperation()
        {
            SumHour = 0;
            SumMinutes = 0;
            SumOperation = 0.0;
            foreach (var operation in Operations)
            {
                SumHour += (operation.ElapsedHours);
                SumMinutes += (operation.ElapsedMinutes);
                double AvgOperation = ((operation.ElapsedHours * operation.PricePerHour) +
                    ((operation.ElapsedMinutes * operation.PricePerHour) / 60)) * _count;
                SumOperation += AvgOperation * (Multiplier == 0 ? 1 : Multiplier);
            }

            TotalString = $"Итого: {(SumMaterial + SumOperation).ToString("N2")} руб. из них материалы: {SumMaterial.ToString("N2")} руб., работы: {SumOperation.ToString("N2")}, затрачено времени: {SumHour}:{SumMinutes}.";
        }

        /// <summary>
        /// Расчет стоимости материалов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CalcMaterial()
        {
            SumMaterial = 0.0;
            foreach (var material in MaterialVM.Elements.OrEmpty())
            {
                material.UsagePerOrder = material.UsagePerUnits * _count;
                SumMaterial += (material.UsagePerOrder * material.Price);
            }

            TotalString = $"Итого: {(SumMaterial + SumOperation).ToString("N2")} руб. из них материалы: {SumMaterial.ToString("N2")} руб., работы: {SumOperation.ToString("N2")}, затрачено времени: {SumHour}:{SumMinutes}.";
        }

        private void ValidateMaterialVM(bool isValid)
        {
            IsValid = OperationVM.IsValid && isValid;
        }

        private void ValidateOperationVM(bool isValid)
        {
            IsValid = MaterialVM.IsValid && isValid;
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
