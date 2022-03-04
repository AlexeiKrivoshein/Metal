using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MetalTransport.Datagram.GetListData;
using MetalTransport.Datagram.Properties;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class FilterDTO
        : BaseDTO
    {
        private DateTime _dateFrom = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата заказа с
        /// </summary>
        public DateTime DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }

        private DateTime _dateTo = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата заказа по
        /// </summary>
        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged("DateTo");
            }
        }

        private int _numberFrom;
        /// <summary>
        /// Номер заказа с
        /// </summary>
        public int NumberFrom
        {
            get => _numberFrom;
            set
            {
                _numberFrom = value;
                OnPropertyChanged("NumberFrom");
            }
        }

        private int _numberTo;
        /// <summary>
        /// Номер заказа по
        /// </summary>
        public int NumberTo
        {
            get => _numberTo;
            set
            {
                _numberTo = value;
                OnPropertyChanged("NumberTo");
            }
        }

        private Guid _orderGroupId;
        /// <summary>
        /// Идентификатор группы заказов
        /// </summary>
        public Guid OrderGroupId
        {
            get => _orderGroupId;
            set
            {
                _orderGroupId = value;
                OnPropertyChanged("OrderGroupId");
            }
        }

        private string _name;
        /// <summary>
        /// Наименование заказа
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _countFrom;
        /// <summary>
        /// Количество ед. изготовления с
        /// </summary>
        public int CountFrom
        {
            get => _countFrom;
            set
            {
                _countFrom = value;
                OnPropertyChanged("CountFrom");
            }
        }

        private int _countTo;
        /// <summary>
        /// Количество ед. изготовления по
        /// </summary>
        public int CountTo
        {
            get => _countTo;
            set
            {
                _countTo = value;
                OnPropertyChanged("CountTo");
            }
        }

        private Guid _customerId;
        /// <summary>
        /// Идентификатор заказчика
        /// </summary>
        public Guid CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged("CustomerId");
            }
        }

        private OrderState _orderStateFrom = OrderState.Create;
        /// <summary>
        /// Состояние заказа с
        /// </summary>
        public OrderState OrderStateFrom
        {
            get => _orderStateFrom;
            set
            {
                _orderStateFrom = value;
                OnPropertyChanged("OrderStateFrom");
            }
        }

        private OrderState _orderStateTo = OrderState.Shipping;
        /// <summary>
        /// Состояние заказа по
        /// </summary>
        public OrderState OrderStateTo
        {
            get => _orderStateTo;
            set
            {
                _orderStateTo = value;
                OnPropertyChanged("OrderStateTo");
            }
        }

        private double _сalcPriceFrom;
        /// <summary>
        /// Расчетная цена с
        /// </summary>
        public double CalcPriceFrom
        {
            get => _сalcPriceFrom;
            set
            {
                _сalcPriceFrom = value;
                OnPropertyChanged("CalcPriceFrom");
            }
        }

        private double _сalcPriceTo;
        /// <summary>
        /// Расчетная цена по
        /// </summary>
        public double CalcPriceTo
        {
            get => _сalcPriceTo;
            set
            {
                _сalcPriceTo = value;
                OnPropertyChanged("CalcPriceTo");
            }
        }

        private double _expectedPriceFrom;
        /// <summary>
        /// Предлагаемая цена с
        /// </summary>
        public double ExpectedPriceFrom
        {
            get => _expectedPriceFrom;
            set
            {
                _expectedPriceFrom = value;
                OnPropertyChanged("ExpectedPriceFrom");
            }
        }

        private double _expectedPriceTo;
        /// <summary>
        /// Предлагаемая цена по
        /// </summary>
        public double ExpectedPriceTo
        {
            get => _expectedPriceTo;
            set
            {
                _expectedPriceTo = value;
                OnPropertyChanged("ExpectedPriceTo");
            }
        }

        private double _salesPriceFrom;
        /// <summary>
        /// Отпускная цена с
        /// </summary>
        public double SalesPriceFrom
        {
            get => _salesPriceFrom;
            set
            {
                _salesPriceFrom = value;
                OnPropertyChanged("SalesPriceFrom");
            }
        }

        private double _salesPriceTo;
        /// <summary>
        /// Отпускная цена по
        /// </summary>
        public double SalesPriceTo
        {
            get => _salesPriceTo;
            set
            {
                _salesPriceTo = value;
                OnPropertyChanged("SalesPriceTo");
            }
        }

        private DateTime _readyDateFrom = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Срок изготовления с
        /// </summary>
        public DateTime ReadyDateFrom
        {
            get => _readyDateFrom;
            set
            {
                _readyDateFrom = value;
                OnPropertyChanged("ReadyDateFrom");
            }
        }

        private DateTime _readyDateTo = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Срок изготовления по
        /// </summary>
        public DateTime ReadyDateTo
        {
            get => _readyDateTo;
            set
            {
                _readyDateTo = value;
                OnPropertyChanged("ReadyDateTo");
            }
        }

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        private List<Sort> _sort = new List<Sort>();
        public List<Sort> Sort
        {
            get => _sort;
            set
            {
                _sort = value;
                OnPropertyChanged("Sort");
            }
        }
    }
}
