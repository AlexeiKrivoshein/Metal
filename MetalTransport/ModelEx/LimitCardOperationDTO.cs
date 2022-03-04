using MetalTransport.ModelEx.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class LimitCardOperationDTO
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

        private Guid _operationId;
        [Required("Необходимо заполнить поле \"Операция\"")]
        public Guid OperationId
        {
            get => _operationId;
            set
            {
                _operationId = value;
                OnPropertyChanged("OperationId");
            }
        }

        private short _elapsedHours = 0;
        [NumberRange(0, 999, "Допустимые значения от 0 до 999")]
        public short ElapsedHours
        {
            get => _elapsedHours;
            set
            {
                _elapsedHours = value;
                OnPropertyChanged("ElapsedHours");
            }
        }

        private short _elapsedMinutes = 0;
        [NumberRange(0, 59, "Допустимые значения от 0 до 59")]
        public short ElapsedMinutes
        {
            get => _elapsedMinutes;
            set
            {
                _elapsedMinutes = value;
                OnPropertyChanged("ElapsedMinutes");
            }
        }

        private double _pricePerHour = 0D;
        public double PricePerHour
        {
            get => _pricePerHour;
            set
            {
                _pricePerHour = value;
                OnPropertyChanged("PricePerHour");
            }
        }

        private int _position = 0;
        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        static LimitCardOperationDTO()
        {
            InitValidation(typeof(LimitCardOperationDTO).GetProperties());
        }

        public LimitCardOperationDTO Clone()
        {
            return new LimitCardOperationDTO()
            {
                Id = Id,
                OrderId = OrderId,
                OperationId = OperationId,
                ElapsedHours = ElapsedHours,
                ElapsedMinutes = ElapsedMinutes,
                PricePerHour = PricePerHour,
                Position = Position
            };
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is LimitCardOperationDTO other))
                return false;

            return Id == other.Id &&
                   OrderId == other.OrderId &&
                   OperationId == other.OperationId &&
                   ElapsedHours == other.ElapsedHours &&
                   ElapsedMinutes == other.ElapsedMinutes &&
                   PricePerHour == other.PricePerHour &&
                   Position == other.Position;
        }
    }
}
