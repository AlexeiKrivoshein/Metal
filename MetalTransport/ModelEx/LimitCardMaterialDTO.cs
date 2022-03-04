using MetalTransport.ModelEx.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class LimitCardMaterialDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private Guid _orderId;
        [Required("Необходимо заполнить поле \"Заказ\"")]
        public Guid OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged("OrderId");
            }
        }

        private Guid _materialId;
        [Required("Необходимо заполнить поле \"Материал\"")]
        public Guid MaterialId
        {
            get => _materialId;
            set
            {
                _materialId = value;
                OnPropertyChanged("MaterialId");
            }
        }

        private string _units = "";
        public string Units
        {
            get => _units;
            set
            {
                _units = value;
                OnPropertyChanged("Units");
            }
        }

        private double _usagePerUnits = 0D;
        public double UsagePerUnits
        {
            get => _usagePerUnits;
            set
            {
                _usagePerUnits = value;
                OnPropertyChanged("UsagePerUnits");
            }
        }

        private double _usagePerOrder = 0D;
        public double UsagePerOrder
        {
            get => _usagePerOrder;
            set
            {
                _usagePerOrder = value;
                OnPropertyChanged("UsagePerOrder");
            }
        }

        private double _multiplicity = 0D;
        public double Multiplicity
        {
            get => _multiplicity;
            set
            {
                _multiplicity = value;
                OnPropertyChanged("Multiplicity");
            }
        }

        private double _price = 0D;
        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public ObservableCollection<LimitCardFactMaterialDTO> FactMaterials { get; set; } = new ObservableCollection<LimitCardFactMaterialDTO>();

        static LimitCardMaterialDTO()
        {
            InitValidation(typeof(LimitCardMaterialDTO).GetProperties());
        }

        public LimitCardMaterialDTO Clone()
        {
            var limitCardMaterialRow = new LimitCardMaterialDTO()
                {
                    Id = Id,
                    OrderId = OrderId,
                    MaterialId = MaterialId,
                    Units = Units,
                    UsagePerUnits = UsagePerUnits,
                    UsagePerOrder = UsagePerOrder,
                    Multiplicity = Multiplicity,
                    Price = Price
                };

            foreach (var factMaterial in FactMaterials)
            {
                limitCardMaterialRow.FactMaterials.Add(factMaterial.Clone() as LimitCardFactMaterialDTO);
            }

            return limitCardMaterialRow;
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is LimitCardMaterialDTO other))
                return false;

            if (FactMaterials.Count != other.FactMaterials.Count)
                return false;

            foreach (var myfactMaterial in FactMaterials)
            {
                var otherFact = other.FactMaterials.FirstOrDefault(fm => fm.Id == myfactMaterial.Id);

                if (otherFact == null)
                    return false;

                if (!otherFact.Equals(myfactMaterial))
                    return false;
            }

            return OrderId == OrderId &&
                   MaterialId == MaterialId &&
                   Units == Units &&
                   UsagePerUnits == UsagePerUnits &&
                   UsagePerOrder == UsagePerOrder &&
                   Multiplicity == Multiplicity &&
                   Price == Price;
        }
    }
}
