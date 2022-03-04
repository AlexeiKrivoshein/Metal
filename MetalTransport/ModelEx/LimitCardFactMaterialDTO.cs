using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class LimitCardFactMaterialDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private Guid _limitCardMaterialId;
        public Guid LimitCardMaterialId
        {
            get => _limitCardMaterialId;
            set
            {
                _limitCardMaterialId = value;
                OnPropertyChanged("LimitCardMaterialId");
            }
        }

        private Guid _materialId;
        public Guid MaterialId
        {
            get => _materialId;
            set
            {
                _materialId = value;
                OnPropertyChanged("MaterialId");
            }
        }


        private double _count = 0D;
        public double Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
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

        public LimitCardFactMaterialDTO(Guid limitCardMaterialId)
        {
            _limitCardMaterialId = limitCardMaterialId;
        }

        public LimitCardFactMaterialDTO Clone()
        {
            return new LimitCardFactMaterialDTO(LimitCardMaterialId)
            {
                Id = Id,
                MaterialId = MaterialId,
                Count = Count,
                Price = Price
            };
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is LimitCardFactMaterialDTO other))
                return false;

            return this.Id == other.Id &&
                    this.LimitCardMaterialId == other.LimitCardMaterialId &&
                    this.MaterialId == other.MaterialId &&
                    this.Count == other.Count &&
                    this.Price == other.Price;
        }
    }
}
