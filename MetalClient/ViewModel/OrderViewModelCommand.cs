using MetalClient.Helper;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClient.ViewModel
{
    public partial class OrderViewModel
    {
        //Команда добавить операцию заказа
        private RelayCommand _addOrderOperationCommand;
        public RelayCommand AddOrderOperationCommand
        {
            get
            {
                return _addOrderOperationCommand ??
                  (_addOrderOperationCommand = new RelayCommand(obj =>
                  {
                      AddOperation();
                  }));
            }
        }

        //Команда выбрать операцию заказа
        public Action<OperationListViewModel> ShowOperationList;
        private RelayCommand _selectOrderOperationCommand;
        public RelayCommand SelectOrderOperationCommand
        {
            get
            {
                return _selectOrderOperationCommand ??
                  (_selectOrderOperationCommand = new RelayCommand(obj =>
                  {
                      SelectOrderOperation();
                  }, (obj) => ShowOperationList != null));
            }
        }

        //Команда удалить операцию заказа
        private RelayCommand _removeOrderOperationCommand;
        public RelayCommand RemoveOrderOperationCommand
        {
            get
            {
                return _removeOrderOperationCommand ??
                  (_removeOrderOperationCommand = new RelayCommand(obj =>
                  {
                      _removedOrderOperations.Add(SelectedOrderOperation.Id);
                      OrderOperations.Remove(SelectedOrderOperation);
                  }, (obj) => SelectedOrderOperation != null));
            }
        }

        //Команда выбрать сотрудника для операции заказа
        public Action<EmployeeListViewModel> ShowEmployeeList;
        private RelayCommand _selectOrderOperationEmployeeCommand;
        public RelayCommand SelectOrderOperationEmployeeCommand
        {
            get
            {
                return _selectOrderOperationEmployeeCommand ??
                  (_selectOrderOperationEmployeeCommand = new RelayCommand(obj =>
                  {
                      SelectOrderOperationEmployee();
                  }, (obj) => ShowEmployeeList != null));
            }
        }

        //Комманда печать
        public Action<OrderDTO, List<OrderOperationDTO>> ShowPrint;
        private RelayCommand _printCommand;
        public RelayCommand PrintCommand
        {
            get
            {
                return _printCommand ??
                  (_printCommand = new RelayCommand(obj =>
                  {
                      ShowPrint(Element, OrderOperations.ToList());
                  }, (obj) => ShowPrint != null));
            }
        }

        //Комманда выбор заказчика
        public Action<CustomerListViewModel> ShowCustomerList;
        private RelayCommand _selectCustomerCommand;
        public RelayCommand SelectCustomerCommand
        {
            get
            {
                return _selectCustomerCommand ??
                  (_selectCustomerCommand = new RelayCommand(obj =>
                  {
                      SelectCustomer();
                  }, (obj) => ShowCustomerList != null));
            }
        }

        private RelayCommand _handleCustomerCommand;
        public RelayCommand HandleCustomerCommand
        {
            get
            {
                return _handleCustomerCommand ??
                  (_handleCustomerCommand = new RelayCommand(obj =>
                  {
                      HandleCustomer(Element.CustomerId);
                  }, (obj) => Element.CustomerId != Guid.Empty));
            }
        }

        //Комманда выбор группы заказов
        public Action<OrderGroupListViewModel> ShowOrderGroupList;
        private RelayCommand _selectOrderGroupCommand;
        public RelayCommand SelectOrderGroupCommand
        {
            get
            {
                return _selectOrderGroupCommand ??
                  (_selectOrderGroupCommand = new RelayCommand(obj =>
                  {
                      SelectOrderGroup();
                  }, (obj) => ShowCustomerList != null));
            }
        }

        //Комманда выбор чертежа
        public Action<OrderDrawingListViewModel> ShowDrawingList;
        private RelayCommand _editDrawingCommand;
        public RelayCommand EditDrawingCommand
        {
            get
            {
                return _editDrawingCommand ??
                  (_editDrawingCommand = new RelayCommand(obj =>
                  {
                      EditDrawingList();
                  }, (obj) => (Element?.DrawingState ?? DrawingType.None) == DrawingType.Ready));
            }
        }

        //Команда показать лимитную карту
        public Action<LimitCardViewModel> ShowLimitCard;
        private RelayCommand _openLimitCardCommand;
        public RelayCommand OpenLimitCardCommand
        {
            get
            {
                return _openLimitCardCommand ??
                  (_openLimitCardCommand = new RelayCommand(obj =>
                  {
                      if (obj is string value &&
                          bool.TryParse(value, out var isFact))
                      {
                          OpenLimitCard(isFact);
                      }

                  }, (obj) => ShowLimitCard != null));
            }
        }

        //Комманды заполнения/очистки даты
        private RelayCommand _setAcceptedDate;
        public RelayCommand SetAcceptedDate
        {
            get
            {
                return _setAcceptedDate ??
                  (_setAcceptedDate = new RelayCommand(obj =>
                  {
                      Element.AcceptedDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setMaterialAgreed;
        public RelayCommand SetMaterialAgreed
        {
            get
            {
                return _setMaterialAgreed ??
                  (_setMaterialAgreed = new RelayCommand(obj =>
                  {
                      Element.MaterialAgreed = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setSalesMaterialOrderDate;
        public RelayCommand SetSalesMaterialOrderDate
        {
            get
            {
                return _setSalesMaterialOrderDate ??
                  (_setSalesMaterialOrderDate = new RelayCommand(obj =>
                  {
                      Element.SalesMaterialOrderDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setMaterialReqDate;
        public RelayCommand SetMaterialReqDate
        {
            get
            {
                return _setMaterialReqDate ??
                  (_setMaterialReqDate = new RelayCommand(obj =>
                  {
                      Element.TechMaterialReqDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setDirectorOrderPlanedDate;
        public RelayCommand SetDirectorOrderPlanedDate
        {
            get
            {
                return _setDirectorOrderPlanedDate ??
                  (_setDirectorOrderPlanedDate = new RelayCommand(obj =>
                  {
                      Element.DirectorOrderPlanedDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setOrderInManufactureDate;
        public RelayCommand SetOrderInManufactureDate
        {
            get
            {
                return _setOrderInManufactureDate ??
                  (_setOrderInManufactureDate = new RelayCommand(obj =>
                  {
                      Element.OrderInManufactureDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setMaterialInManufactureDate;
        public RelayCommand SetMaterialInManufactureDate
        {
            get
            {
                return _setMaterialInManufactureDate ??
                  (_setMaterialInManufactureDate = new RelayCommand(obj =>
                  {
                      Element.MaterialInManufactureDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setOrderInWorkDate;
        public RelayCommand SetOrderInWorkDate
        {
            get
            {
                return _setOrderInWorkDate ??
                  (_setOrderInWorkDate = new RelayCommand(obj =>
                  {
                      Element.OrderInWorkDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setManMadeDate;
        public RelayCommand SetManMadeDate
        {
            get
            {
                return _setManMadeDate ??
                  (_setManMadeDate = new RelayCommand(obj =>
                  {
                      Element.ManMadeDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setOTKProductDefectDate;
        public RelayCommand SetOTKProductDefectDate
        {
            get
            {
                return _setOTKProductDefectDate ??
                  (_setOTKProductDefectDate = new RelayCommand(obj =>
                  {
                      Element.OTKProductDefectDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setOTKProductCorrectDate;
        public RelayCommand SetOTKProductCorrectDate
        {
            get
            {
                return _setOTKProductCorrectDate ??
                  (_setOTKProductCorrectDate = new RelayCommand(obj =>
                  {
                      Element.OTKProductCorrectDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setManProductApplyDate;
        public RelayCommand SetManProductApplyDate
        {
            get
            {
                return _setManProductApplyDate ??
                  (_setManProductApplyDate = new RelayCommand(obj =>
                  {
                      Element.ManProductApplyDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setManLimitCreateDate;
        public RelayCommand SetManLimitCreateDate
        {
            get
            {
                return _setManLimitCreateDate ??
                  (_setManLimitCreateDate = new RelayCommand(obj =>
                  {
                      Element.ManLimitCreateDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setAccCustomerInformedDate;
        public RelayCommand SetAccCustomerInformedDate
        {
            get
            {
                return _setAccCustomerInformedDate ??
                  (_setAccCustomerInformedDate = new RelayCommand(obj =>
                  {
                      Element.AccCustomerInformedDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setAccOrderPaidDate;
        public RelayCommand SetAccOrderPaidDate
        {
            get
            {
                return _setAccOrderPaidDate ??
                  (_setAccOrderPaidDate = new RelayCommand(obj =>
                  {
                      Element.AccOrderPaidDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setAccDocumentsToSendDate;
        public RelayCommand SetAccDocumentsToSendDate
        {
            get
            {
                return _setAccDocumentsToSendDate ??
                  (_setAccDocumentsToSendDate = new RelayCommand(obj =>
                  {
                      Element.AccDocumentsToSendDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setSendDeliveryDate;
        public RelayCommand SetSendDeliveryDate
        {
            get
            {
                return _setSendDeliveryDate ??
                  (_setSendDeliveryDate = new RelayCommand(obj =>
                  {
                      Element.SendDeliveryDate = Funcs.GetDateTimeValue(obj);
                  }));
            }
        }

        private RelayCommand _setMaterialAvailableDate;
        public RelayCommand SetMaterialAvailableDate
        {
            get
            {
                return _setMaterialAvailableDate ??
                  (_setMaterialAvailableDate = new RelayCommand(obj =>
                  {
                      if (  obj is string value &&
                            bool.TryParse(value, out var available))
                      {
                          Element.SalesMaterialAvailable = available;
                          Element.SalesMaterialAvailableDate = DateTime.Now;
                      }
                  }));
            }
        }

        private RelayCommand _setSalesPrepaymentType;
        public RelayCommand SetSalesPrepaymentType
        {
            get
            {
                return _setSalesPrepaymentType ??
                  (_setSalesPrepaymentType = new RelayCommand(obj =>
                  {
                      PaymentType paymentType;

                      if (obj is string value && 
                          Enum.TryParse(value, out paymentType))
                      { 
                        Element.SalesPrepaymentType = paymentType;
                      }
                  }));
            }
        }

        private RelayCommand _setSalesPriceDate;
        public RelayCommand SetSalesPriceDate
        {
            get
            {
                return _setSalesPriceDate ??
                  (_setSalesPriceDate = new RelayCommand(obj =>
                  {
                      Element.SalesPriceDate = Funcs.GetDateTimeValue(Element.SalesPrice);
                  }));
            }
        }

        private RelayCommand _setSalesComOfferDate;
        public RelayCommand SetSalesComOfferDate
        {
            get
            {
                return _setSalesComOfferDate ??
                  (_setSalesComOfferDate = new RelayCommand(obj =>
                  {
                      Element.SalesComOfferDate = Funcs.GetDateTimeValue(Element.SalesComOfferNumber);
                  }));
            }
        }

        private RelayCommand _setAccSpecifDate;
        public RelayCommand SetAccSpecifDate
        {
            get
            {
                return _setAccSpecifDate ??
                  (_setAccSpecifDate = new RelayCommand(obj =>
                  {
                      Element.AccSpecifDate = Funcs.GetDateTimeValue(Element.AccSpecifNumber);
                  }));
            }
        }

        private RelayCommand _setAccBillDate;
        public RelayCommand SetAccBillDate
        {
            get
            {
                return _setAccBillDate ??
                  (_setAccBillDate = new RelayCommand(obj =>
                  {
                      Element.AccBillDate = Funcs.GetDateTimeValue(Element.AccBillNumber);
                  }));
            }
        }

        private RelayCommand _setAccPaymDate;
        public RelayCommand SetAccPaymDate
        {
            get
            {
                return _setAccPaymDate ??
                  (_setAccPaymDate = new RelayCommand(obj =>
                  {
                        Element.AccPaymDate = DateTime.Now;
                  }));
            }
        }
    }
}
