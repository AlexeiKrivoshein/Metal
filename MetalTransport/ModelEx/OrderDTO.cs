using MetalTransport.Datagram.GetListData;
using MetalTransport.Datagram.Properties;
using MetalTransport.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class OrderDTO
        : BaseDTO, INotifyPropertyChanged
    {
        [NonSerialized]
        public static List<Sort> ListSort = new List<Sort>
        {
            new Sort(SortField.OrderNumber, true)
        };

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

        private string _name = "";
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

        private int _count = 0;
        /// <summary>
        /// Количество ед. изготовления
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
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

        private DateTime _readyDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Срок изготовления до
        /// </summary>
        public DateTime ReadyDate
        {
            get => _readyDate;
            set
            {
                _readyDate = value;
                OnPropertyChanged("ReadyDate");
            }
        }

        private DateTime _acceptedDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата принятия заказа
        /// </summary>
        public DateTime AcceptedDate
        {
            get => _acceptedDate;
            set
            {
                _acceptedDate = value;
                OnPropertyChanged("AcceptedDate");
            }
        }

        private string _drawingNumber = "";
        /// <summary>
        /// Номер чертежа
        /// </summary>
        public string DrawingNumber
        {
            get => _drawingNumber;
            set
            {
                _drawingNumber = value;
                OnPropertyChanged("DrawingNumber");
            }
        }

        private DrawingType _drawingState = DrawingType.NeedToMake;
        /// <summary>
        /// Тип состояния чертежа
        /// </summary>
        public DrawingType DrawingState
        {
            get => _drawingState;
            set
            {
                _drawingState = value;
                OnPropertyChanged("DrawingState");
            }
        }

        private bool _isCustomerMaterial = false;
        /// <summary>
        /// Материал заказчика
        /// </summary>
        public bool IsCustomerMaterial
        {
            get => _isCustomerMaterial;
            set
            {
                _isCustomerMaterial = value;
                OnPropertyChanged("IsCustomerMaterial");
            }
        }

        private string _customerMaterial = "";
        /// <summary>
        /// Описание материлов заказчикаы
        /// </summary>
        public string CustomerMaterial
        {
            get => _customerMaterial;
            set
            {
                _customerMaterial = value;
                OnPropertyChanged("CustomerMaterial");
            }
        }

        private DateTime _customerReadyDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата желаемого срока изготовления
        /// </summary>
        public DateTime CustomerReadyDate
        {
            get => _customerReadyDate;
            set
            {
                _customerReadyDate = value;
                OnPropertyChanged("CustomerReadyDate");
            }
        }

        private DateTime _materialAgreed = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата согласования материала с заказчиком
        /// </summary>
        public DateTime MaterialAgreed
        {
            get => _materialAgreed;
            set
            {
                _materialAgreed = value;
                OnPropertyChanged("MaterialAgreed");
            }
        }

        private double _techCalcPrice = 0D;
        /// <summary>
        /// Расчетная цена
        /// </summary>
        public double TechCalcPrice
        {
            get => _techCalcPrice;
            set
            {
                _techCalcPrice = value;
                OnPropertyChanged("TechCalcPrice");
            }
        }

        private int _techCalcHour = 0;
        /// <summary>
        /// Расчетное время часы
        /// </summary>
        public int TechCalcHour
        {
            get => _techCalcHour;
            set
            {
                _techCalcHour = value;
                OnPropertyChanged("TechCalcHour");
            }
        }

        private int _techCalcMinutes = 0;
        /// <summary>
        /// Расчетное время минуты
        /// </summary>
        public int TechCalcMinutes
        {
            get => _techCalcMinutes;
            set
            {
                _techCalcMinutes = value;
                OnPropertyChanged("TechCalcMinutes");
            }
        }

        private DateTime _techCalcPriceDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата определения расчетной цены
        /// </summary>
        public DateTime TechCalcPriceDate
        {
            get => _techCalcPriceDate;
            set
            {
                _techCalcPriceDate = value;
                OnPropertyChanged("TechCalcPriceDate");
            }
        }

        private double _techCalcMultiplier = 0D;
        /// <summary>
        /// Множитель цены
        /// </summary>
        public double TechCalcMultiplier
        {
            get => _techCalcMultiplier;
            set
            {
                _techCalcMultiplier = value;
                OnPropertyChanged("TechCalcMultiplier");
            }
        }

        private DateTime _techMaterialReqDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата приложения заявки на материалы и инструменты
        /// </summary>
        public DateTime TechMaterialReqDate
        {
            get => _techMaterialReqDate;
            set
            {
                _techMaterialReqDate = value;
                OnPropertyChanged("TechMaterialReqDate");
            }
        }

        private double _directorExpectedPrice = 0D;
        /// <summary>
        /// Предлагаемая цена
        /// </summary>
        public double DirectorExpectedPrice
        {
            get => _directorExpectedPrice;
            set
            {
                _directorExpectedPrice = value;
                OnPropertyChanged("DirectorExpectedPrice");
            }
        }

        private int _directorExpectedDay = 0;
        /// <summary>
        /// Прим. срок изготовления дней
        /// </summary>
        public int DirectorExpectedDay
        {
            get => _directorExpectedDay;
            set
            {
                _directorExpectedDay = value;
                OnPropertyChanged("DirectorExpectedDay");
            }
        }

        private DateTime _directorExpectedDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата определения предлагаемой цены
        /// </summary>
        public DateTime DirectorExpectedDate
        {
            get => _directorExpectedDate;
            set
            {
                _directorExpectedDate = value;
                OnPropertyChanged("DirectorExpectedDate");
            }
        }

        private bool _salesMaterialAvailable = false;
        /// <summary>
        /// Основные и вспомагательные материалы доступны
        /// </summary>
        public bool SalesMaterialAvailable
        {
            get => _salesMaterialAvailable;
            set
            {
                _salesMaterialAvailable = value;
                OnPropertyChanged("SalesMaterialAvailable");
            }
        }

        private DateTime _salesMaterialAvailableDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата определения доступности основных и вспомагательных материалов
        /// </summary>
        public DateTime SalesMaterialAvailableDate
        {
            get => _salesMaterialAvailableDate;
            set
            {
                _salesMaterialAvailableDate = value;
                OnPropertyChanged("SalesMaterialAvailableDate");
            }
        }

        private PaymentType _salesPrepaymentType;
        /// <summary>
        /// Тип предоплаты заказа
        /// </summary>
        public PaymentType SalesPrepaymentType
        {
            get => _salesPrepaymentType;
            set
            {
                _salesPrepaymentType = value;
                OnPropertyChanged("SalesPrepaymentType");
            }
        }

        private PaymentType _salesPaymentType;
        /// <summary>
        /// Тип отгрузки заказа
        /// </summary>
        public PaymentType SalesPaymentType
        {
            get => _salesPaymentType;
            set
            {
                _salesPaymentType = value;
                OnPropertyChanged("SalesPaymentType");
            }
        }

        private double _salesPrice = 0D;
        /// <summary>
        /// Отпускная цена
        /// </summary>
        public double SalesPrice
        {
            get => _salesPrice;
            set
            {
                _salesPrice = value;
                OnPropertyChanged("SalesPrice");
            }
        }

        private DateTime _salesPriceDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата определения отпускной цены
        /// </summary>
        public DateTime SalesPriceDate
        {
            get => _salesPriceDate;
            set
            {
                _salesPriceDate = value;
                OnPropertyChanged("SalesPriceDate");
            }
        }

        private string _salesComOfferNumber = "";
        /// <summary>
        /// Номер коммерческого предложения
        /// </summary>
        public string SalesComOfferNumber
        {
            get => _salesComOfferNumber;
            set
            {
                _salesComOfferNumber = value;
                OnPropertyChanged("SalesComOfferNumber");
            }
        }

        private DateTime _salesComOfferDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата коммерческого предложения
        /// </summary>
        public DateTime SalesComOfferDate
        {
            get => _salesComOfferDate;
            set
            {
                _salesComOfferDate = value;
                ProcessOrderState();
                OnPropertyChanged("SalesComOfferDate");
            }
        }

        private string _accSpecifNumber = "";
        /// <summary>
        /// Номер спецификации
        /// </summary>
        public string AccSpecifNumber
        {
            get => _accSpecifNumber;
            set
            {
                _accSpecifNumber = value;
                OnPropertyChanged("AccSpecifNumber");
            }
        }

        private DateTime _accSpecifDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата спецификации
        /// </summary>
        public DateTime AccSpecifDate
        {
            get => _accSpecifDate;
            set
            {
                _accSpecifDate = value;
                ProcessOrderState();
                OnPropertyChanged("AccSpecifDate");
            }
        }

        private string _accBillNumber = "";
        /// <summary>
        /// Номер счета
        /// </summary>
        public string AccBillNumber
        {
            get => _accBillNumber;
            set
            {
                _accBillNumber = value;
                OnPropertyChanged("AccBillNumber");
            }
        }

        private DateTime _accBillDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата счета
        /// </summary>
        public DateTime AccBillDate
        {
            get => _accBillDate;
            set
            {
                _accBillDate = value;
                ProcessOrderState();
                OnPropertyChanged("AccBillDate");
            }
        }

        private PaymentType _accPaymType;
        /// <summary>
        /// Заказ оплачен
        /// </summary>
        public PaymentType AccPaymType
        {
            get => _accPaymType;
            set
            {
                _accPaymType = value;
                OnPropertyChanged("AccPaymType");
            }
        }

        private DateTime _accPaymDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата оплаты заказа
        /// </summary>
        public DateTime AccPaymDate
        {
            get => _accPaymDate;
            set
            {
                _accPaymDate = value;
                OnPropertyChanged("AccPaymDate");
            }
        }

        private DateTime _salesMaterialOrderDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата заказа инструментов и материалов
        /// </summary>
        public DateTime SalesMaterialOrderDate
        {
            get => _salesMaterialOrderDate;
            set
            {
                _salesMaterialOrderDate = value;
                ProcessOrderState();
                OnPropertyChanged("SalesMaterialOrderDate");
            }
        }

        private DateTime _salesMaterialOrderReadyDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата поставки материалов и инструметов
        /// </summary>
        public DateTime SalesMaterialOrderReadyDate
        {
            get => _salesMaterialOrderReadyDate;
            set
            {
                _salesMaterialOrderReadyDate = value;
                OnPropertyChanged("SalesMaterialOrderReadyDate");
            }
        }

        private DateTime _directorOrderPlanedDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата постановки заказа в план
        /// </summary>
        public DateTime DirectorOrderPlanedDate
        {
            get => _directorOrderPlanedDate;
            set
            {
                _directorOrderPlanedDate = value;
                ProcessOrderState();
                OnPropertyChanged("DirectorOrderPlanedDate");
            }
        }

        private DateTime _orderInManufactureDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата получения заказа в производство
        /// </summary>
        public DateTime OrderInManufactureDate
        {
            get => _orderInManufactureDate;
            set
            {
                _orderInManufactureDate = value;
                OnPropertyChanged("OrderInManufactureDate");
            }
        }

        private DateTime _materialInManufactureDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата получения материалов и инструментов в производство
        /// </summary>
        public DateTime MaterialInManufactureDate
        {
            get => _materialInManufactureDate;
            set
            {
                _materialInManufactureDate = value;
                OnPropertyChanged("MaterialInManufactureDate");
            }
        }

        private DateTime _orderInWorkDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата запуска заказа в работу
        /// </summary>
        public DateTime OrderInWorkDate
        {
            get => _orderInWorkDate;
            set
            {
                _orderInWorkDate = value;
                ProcessOrderState();
                OnPropertyChanged("OrderInWorkDate");
            }
        }

        private DateTime _manMadeDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата изготовления
        /// </summary>
        public DateTime ManMadeDate
        {
            get => _manMadeDate;
            set
            {
                _manMadeDate = value;
                OnPropertyChanged("ManMadeDate");
            }
        }

        private DateTime _OTKProductGetDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата получения заказа в ОТК
        /// </summary>
        public DateTime OTKProductGetDate
        {
            get => _OTKProductGetDate;
            set
            {
                _OTKProductGetDate = value;
                ProcessOrderState();
                OnPropertyChanged("OTKProductGetDate");
            }
        }

        private DateTime _OTKProductDefectDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата возвращения изделия в доработку
        /// </summary>
        public DateTime OTKProductDefectDate
        {
            get => _OTKProductDefectDate;
            set
            {
                _OTKProductDefectDate = value;
                ProcessOrderState();
                OnPropertyChanged("OTKProductDefectDate");
            }
        }

        private string _OTKProductDefectInfo = "";
        /// <summary>
        /// Информация о дефекте
        /// </summary>
        public string OTKProductDefectInfo
        {
            get => _OTKProductDefectInfo;
            set
            {
                _OTKProductDefectInfo = value;
                OnPropertyChanged("OTKProductDefectInfo");
            }
        }

        private DateTime _OTKProductCorrectDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата проверки корректного изделия
        /// </summary>
        public DateTime OTKProductCorrectDate
        {
            get => _OTKProductCorrectDate;
            set
            {
                _OTKProductCorrectDate = value;
                ProcessOrderState();
                OnPropertyChanged("OTKProductCorrectDate");
            }
        }

        private DateTime _manProductApplyDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата принятия изделия в производстве
        /// </summary>
        public DateTime ManProductApplyDate
        {
            get => _manProductApplyDate;
            set
            {
                _manProductApplyDate = value;
                OnPropertyChanged("ManProductApplyDate");
            }
        }

        private DateTime _manLimitCreateDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата передачи лимитки в бухгалтерию
        /// </summary>
        public DateTime ManLimitCreateDate
        {
            get => _manLimitCreateDate;
            set
            {
                _manLimitCreateDate = value;
                ProcessOrderState();
                OnPropertyChanged("ManLimitCreateDate");
            }
        }

        private DateTime _accCustomerInformedDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата и время информирования заказчика
        /// </summary>
        public DateTime AccCustomerInformedDate
        {
            get => _accCustomerInformedDate;
            set
            {
                _accCustomerInformedDate = value;
                ProcessOrderState();
                OnPropertyChanged("AccCustomerInformedDate");
            }
        }

        public DateTime _accOrderPaidDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата оплаты заказа
        /// </summary>
        public DateTime AccOrderPaidDate
        {
            get => _accOrderPaidDate;
            set
            {
                _accOrderPaidDate = value;
                ProcessOrderState();
                OnPropertyChanged("AccOrderPaidDate");
            }
        }

        private DateTime _accDocumentsToSendDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата передачи документов на отгрузку
        /// </summary>
        public DateTime AccDocumentsToSendDate
        {
            get => _accDocumentsToSendDate;
            set
            {
                _accDocumentsToSendDate = value;
                OnPropertyChanged("AccDocumentsToSendDate");
            }
        }

        public DeliveryType _sendOrderDeliveryType;
        /// <summary>
        /// Тип доставки товара
        /// </summary>
        public DeliveryType SendOrderDeliveryType
        {
            get => _sendOrderDeliveryType;
            set
            {
                _sendOrderDeliveryType = value;
                OnPropertyChanged("SendOrderDeliveryType");
            }
        }

        private DateTime _sendDeliveryDate = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата отгрузки товара
        /// </summary>
        public DateTime SendDeliveryDate
        {
            get => _sendDeliveryDate;
            set
            {
                _sendDeliveryDate = value;
                ProcessOrderState();
                OnPropertyChanged("SendDeliveryDate");
            }
        }

        private OrderReadyType _orderReadyType;
        /// <summary>
        /// Тип говности заказа
        /// </summary>
        public OrderReadyType OrderReadyType
        {
            get => _orderReadyType;
            set
            {
                _orderReadyType = value;
                ProcessOrderState();
                OnPropertyChanged("OrderReadyType");
            }
        }

        private int _orderHoldDay = 0;
        /// <summary>
        /// Количество дней на которые задержан заказ
        /// </summary>
        public int OrderHoldDay
        {
            get => _orderHoldDay;
            set
            {
                _orderHoldDay = value;
                OnPropertyChanged("OrderHoldDay");
            }
        }

        private OrderState _orderState = OrderState.Create;
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

        private DateTime _dateRec = Constants.EMPTY_DATETIME;
        /// <summary>
        /// Дата внесения/обновления в БД
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

        private bool _deleted = false;
        /// <summary>
        /// Удалено
        /// </summary>
        public bool Deleted
        {
            get => _deleted;
            set
            {
                _deleted = value;
                OnPropertyChanged("Deleted");
            }
        }

        private string _orderCorruptReason = "";
        /// <summary>
        /// Причина невыполненного заказа
        /// </summary>
        public string OrderCorruptReason
        {
            get => _orderCorruptReason;
            set
            {
                _orderCorruptReason = value;
                OnPropertyChanged("OrderCorruptReason");
            }
        }

        private CustomerDTO _customer = null;
        /// <summary>
        /// Контрагент
        /// </summary>
        public CustomerDTO Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                if (_customer != value)

                {
                    _customer = value;
                    OnPropertyChanged("Сustomer");
                }
            }
        }

        private OrderGroupDTO _orderGroup = null;
        /// <summary>
        /// Группа заказа
        /// </summary>
        public OrderGroupDTO OrderGroup
        {
            get => _orderGroup;
            set
            {
                _orderGroup = value;
                OnPropertyChanged("OrderGroup");
            }
        }

        public OrderDTO()
        {
            _customer = new CustomerDTO();
            _orderGroup = new OrderGroupDTO();
        }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is OrderDTO other))
                return false;

            return Id == other.Id &&
                   Number == other.Number &&
                   Date == other.Date &&
                   Name == other.Name &&
                   Count == other.Count &&
                   CustomerId == other.CustomerId &&
                   ReadyDate == other.ReadyDate &&
                   AcceptedDate == other.AcceptedDate &&
                   DrawingNumber == other.DrawingNumber &&
                   DrawingState == other.DrawingState &&
                   IsCustomerMaterial == other.IsCustomerMaterial &&
                   CustomerMaterial == other.CustomerMaterial &&
                   CustomerReadyDate == other.CustomerReadyDate &&
                   MaterialAgreed == other.MaterialAgreed &&
                   TechCalcPrice == other.TechCalcPrice &&
                   TechCalcHour == other.TechCalcHour &&
                   TechCalcMinutes == other.TechCalcMinutes &&
                   TechCalcPriceDate == other.TechCalcPriceDate &&
                   TechCalcMultiplier == other.TechCalcMultiplier &&
                   TechMaterialReqDate == other.TechMaterialReqDate &&
                   DirectorExpectedPrice == other.DirectorExpectedPrice &&
                   DirectorExpectedDay == other.DirectorExpectedDay &&
                   DirectorExpectedDate == other.DirectorExpectedDate &&
                   SalesMaterialAvailable == other.SalesMaterialAvailable &&
                   SalesMaterialAvailableDate == other.SalesMaterialAvailableDate &&
                   SalesPrepaymentType == other.SalesPrepaymentType &&
                   SalesPaymentType == other.SalesPaymentType &&
                   SalesPrice == other.SalesPrice &&
                   SalesPriceDate == other.SalesPriceDate &&
                   SalesComOfferNumber == other.SalesComOfferNumber &&
                   SalesComOfferDate == other.SalesComOfferDate &&
                   AccSpecifNumber == other.AccSpecifNumber &&
                   AccSpecifDate == other.AccSpecifDate &&
                   AccBillNumber == other.AccBillNumber &&
                   AccBillDate == other.AccBillDate &&
                   AccPaymType == other.AccPaymType &&
                   AccPaymDate == other.AccPaymDate &&
                   SalesMaterialOrderDate == other.SalesMaterialOrderDate &&
                   SalesMaterialOrderReadyDate == other.SalesMaterialOrderReadyDate &&
                   DirectorOrderPlanedDate == other.DirectorOrderPlanedDate &&
                   OrderInManufactureDate == other.OrderInManufactureDate &&
                   MaterialInManufactureDate == other.MaterialInManufactureDate &&
                   OrderInWorkDate == other.OrderInWorkDate &&
                   ManMadeDate == other.ManMadeDate &&
                   OTKProductGetDate == other.OTKProductGetDate &&
                   OTKProductDefectDate == other.OTKProductDefectDate &&
                   OTKProductDefectInfo == other.OTKProductDefectInfo &&
                   OTKProductCorrectDate == other.OTKProductCorrectDate &&
                   ManProductApplyDate == other.ManProductApplyDate &&
                   ManLimitCreateDate == other.ManLimitCreateDate &&
                   AccCustomerInformedDate == other.AccCustomerInformedDate &&
                   AccOrderPaidDate == other.AccOrderPaidDate &&
                   AccDocumentsToSendDate == other.AccDocumentsToSendDate &&
                   SendOrderDeliveryType == other.SendOrderDeliveryType &&
                   SendDeliveryDate == other.SendDeliveryDate &&
                   OrderReadyType == other.OrderReadyType &&
                   OrderHoldDay == other.OrderHoldDay &&
                   OrderCorruptReason == other.OrderCorruptReason &&
                   OrderState == other.OrderState &&
                   OrderGroupId == other.OrderGroupId &&
                   Deleted == other.Deleted;
        }

        public OrderListItemDTO ToListItem()
        {
            return new OrderListItemDTO
            {
                Id = Id,
                Number = Number,
                Date = Date,
                Name = Name,
                Customer = Customer.Name,
                Group = OrderGroup?.Name ?? "",
                DateRec = DateRec,
                OrderState = OrderState
            };
        }

        private void ProcessOrderState()
        {
            //Создан
            OrderState state = OrderState.Create;

            //Подготовлено комерческое предложение
            if (SalesComOfferDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.CommercialOffer;
            }

            //Спецификация выставлена
            if (AccSpecifDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Specification;
            }

            //Выставлен счёт
            if (AccBillDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Invoice;
            }

            //Материалы заказаны
            if (SalesMaterialOrderDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.MaterialsOrdered;
            }

            //Поставлен в план
            if (DirectorOrderPlanedDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.InPlan;
            }

            //Изготавливается
            if (OrderInWorkDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Process;
            }

            //В ОТК
            if (OTKProductGetDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.OTK;
            }

            //Доработка
            if (OTKProductDefectDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Defect;
            }

            //ОТК пройдено
            if (OTKProductCorrectDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.NoDefect;
            }

            // Документы переданы в бухгалтерию
            if (ManLimitCreateDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.AccountingDep;
            }

            // Заказчик проинформирован
            if (AccCustomerInformedDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.CustomerInform;
            }

            // Оплачено
            if (AccOrderPaidDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Paid;
            }

            //Отгружено
            if (SendDeliveryDate != Constants.EMPTY_DATETIME)
            {
                state = OrderState.Shipping;
            }

            OrderState = state;
        }

        public static PlanItemDTO ToPlanDTO(BaseDTO dto)
        {
            if(! (dto is OrderDTO orderDTO))
            {
                return null;
            }

            var plan = new PlanItemDTO()
            {
                Id = orderDTO.Id,
                Number = orderDTO.Number,
                Customer = orderDTO.Customer.Name,
                Name = orderDTO.Name,
                Count = orderDTO.Count,
                Price = Math.Round(orderDTO.SalesPrice, 2),
                Sum = Math.Round(orderDTO.SalesPrice * orderDTO.Count, 2),
                Maked = orderDTO.Count,
                Week1 = DateTimeHelper.GetWeekOfMonth(orderDTO.OTKProductCorrectDate) == 1 ? orderDTO.Count : 0,
                Week2 = DateTimeHelper.GetWeekOfMonth(orderDTO.OTKProductCorrectDate) == 2 ? orderDTO.Count : 0,
                Week3 = DateTimeHelper.GetWeekOfMonth(orderDTO.OTKProductCorrectDate) == 3 ? orderDTO.Count : 0,
                Week4 = DateTimeHelper.GetWeekOfMonth(orderDTO.OTKProductCorrectDate) > 3 ? orderDTO.Count : 0,
                MakedSum = Math.Round(orderDTO.SalesPrice * orderDTO.Count, 2),
                OrderInManufactureDate = orderDTO.OrderInManufactureDate,
                ReadyDate = orderDTO.ReadyDate
            };

            return plan;
        }
    }
}
