using MetalTransport.Datagram.Properties;
using MetalTransport.Helper;
using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class OrderListItemDTO
        : VersionListItemDTO, INotifyPropertyChanged
    {
        private int _number = 0;
        /// <summary>
        /// Номер заказа
        /// </summary>
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        private DateTime _date = DateTimeHelper.DateTimeNow();
        /// <summary>
        /// Дата заказа
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        private string _customer;
        /// <summary>
        /// Заказчик
        /// </summary>
        public string Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }

        private string _group;
        /// <summary>
        /// Группа
        /// </summary>
        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged("Group");
            }
        }

        private DateTime _dateRec;
        /// <summary>
        /// Дата редактирования
        /// </summary>
        public DateTime DateRec
        {
            get => _dateRec;
            set
            {
                _dateRec = value;
                OnPropertyChanged("DateRec");
            }
        }

        private OrderState _orderState;
        /// <summary>
        /// Состояние заказа
        /// </summary>
        public OrderState OrderState
        {
            get => _orderState;
            set
            {
                _orderState = value;
                OnPropertyChanged("OrderState");
            }
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is OrderListItemDTO other))
                return false;

            return this.Id == other.Id;
        }
    }
}
