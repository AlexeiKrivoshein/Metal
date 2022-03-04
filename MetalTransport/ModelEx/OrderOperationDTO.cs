using MetalTransport.Datagram.Properties;
using System;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public sealed class OrderOperationDTO 
        : BaseDTO, INotifyPropertyChanged
    {
        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged("Index");
            }
        }

        private Guid _orderId;
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
        public Guid OperationId
        {
            get => _operationId;
            set
            {
                _operationId = value;
                OnPropertyChanged("OperationId");
            }
        }

        private int _count = 1;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        private DateTime _startDate = Constants.EMPTY_DATETIME;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime _endDate = Constants.EMPTY_DATETIME;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        private string _comment = "";
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        private Nullable<Guid> _employeeId;
        public Nullable<Guid> EmployeeId
        {
            get => _employeeId;
            set
            {
                _employeeId = value;
                OnPropertyChanged("EmployeeId");
            }
        }

        public bool Deleted { get; set; }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is OrderOperationDTO other))
                return false;

            return Index == Index &&
                   OrderId == OrderId &&
                   OperationId == OperationId &&
                   Count == Count &&
                   StartDate == StartDate &&
                   EndDate == EndDate &&
                   Comment == Comment &&
                   EmployeeId == EmployeeId;
        }
    }
}
