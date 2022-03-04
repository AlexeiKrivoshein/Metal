using MetalClient.DataManager;
using MetalClient.Helper;
using MetalTransport.ModelEx;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace MetalClient
{
    /// <summary>
    /// Логика взаимодействия для frmOrderPrint.xaml
    /// </summary>
    public partial class frmOrderPrint
        : Window
    {
        private OrderDTO _order;
        private List<OrderOperationDTO> _operations;
        private ConcurrentDictionary<Guid, VersionListItemDTO> _employeeNames;
        private ConcurrentDictionary<Guid, VersionListItemDTO> _operationNames;

        public frmOrderPrint(ClientDataManager _dataManager, OrderDTO order, List<OrderOperationDTO> operations, Window owner)
        {
            Owner = owner;
            _order = order;
            _operations = operations;
            _employeeNames = _dataManager.EmployeeNameCache;
            _operationNames = _dataManager.OperationNameCache;

            InitializeComponent();
        }

        private void btnComercialOffer_Click(object sender, RoutedEventArgs e)
        {
            var buffer = Properties.Resources.order;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $@"{folder}\Metal\Заказ_{_order.Number}_{_order.Date:dd.MM.yyyy}";
            var fi = new FileInfo($@"{fileName}.xlsx");
            
            var number = 0;
            while (File.Exists(fi.FullName))
                fi = new FileInfo($@"{fileName}_{number++}.xlsx");

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                ExcelRichText readyText = null;

                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["на 2х листах"];
                    //Номер заказа
                    readyText = worksheet.Cells["I3"].RichText.Add($"  {_order.Number}  ");
                    readyText.UnderLine = true;
                    worksheet.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //Дата заказа
                    worksheet.Cells["L3"].Value = "от ";
                    readyText = worksheet.Cells["L3"].RichText.Add($"  {_order.Date:dd.MM.yyyy}  ");
                    readyText.UnderLine = true;
                    //Наименование
                    worksheet.Cells["E7"].Value = _order.Name;
                    //Количество
                    if (_order.Count > 0) 
                        worksheet.Cells["E9"].Value = _order.Count;
                    //Наименование заказчика
                    worksheet.Cells["E11"].Value = _order.Customer.Name;
                    //Контактное лицо заказчика
                    worksheet.Cells["E13"].Value = _order.Customer.Employee;
                    //Телефон заказчика
                    if (!_order.Customer.Phone.IsEmpty())
                        worksheet.Cells["L13"].Value = $"тел. {_order.Customer.Phone}";
                    //Email заказчика
                    worksheet.Cells["E15"].Value = $"Mail: {_order.Customer.Mail}";
                    //Факс заказчика
                    worksheet.Cells["L15"].Value = $"факс. {_order.Customer.Fax}";
                    //Срок изготовления до
                    if (!Funcs.DateIsEmpty(_order.ReadyDate))
                        worksheet.Cells["E17"].Value = $"{_order.ReadyDate.ToString("dd MMMM yyyy", new CultureInfo("ru-RU"))}";
                    //Заказ принят
                    if (!Funcs.DateIsEmpty(_order.AcceptedDate))
                        worksheet.Cells["C20"].Value = $"заказ принят    дата {_order.AcceptedDate:dd.MM.yyyy}   подпись_____________";
                    //Номер чертежа
                    if (!_order.DrawingNumber.IsEmpty())
                        worksheet.Cells["C22"].Value = $"чертеж   № {_order.DrawingNumber}";
                    //Тип готовности чертежа
                    if (_order.DrawingState == DrawingType.Ready)
                        worksheet.Cells["H22"].Value = "V";
                    else if (_order.DrawingState == DrawingType.NeedToMake)
                        worksheet.Cells["J22"].Value = "V";
                    //Материалы заказчика
                    if (!_order.CustomerMaterial.IsEmpty())
                        worksheet.Cells["C24"].Value = $"материал заказчика {_order.CustomerMaterial}";
                    //Желаемый срок изготовления
                    if (!Funcs.DateIsEmpty(_order.CustomerReadyDate))
                        worksheet.Cells["C26"].Value = $"желаемый срок изготовления {_order.CustomerReadyDate:dd.MM.yyyy}   подпись________________";
                    //Согласование материала с заказчиком и снабжением
                    if (!Funcs.DateIsEmpty(_order.MaterialAgreed))
                        worksheet.Cells["L28"].Value = $"дата {_order.MaterialAgreed:dd.MM.yyyy}";
                    //Расчетная цена
                    if (_order.TechCalcPrice > 0)
                        worksheet.Cells["C34"].Value = $"расчетная цена      {_order.TechCalcPrice:N2} руб.,";
                    //Расчетное время
                    if (_order.TechCalcHour > 0 || _order.TechCalcMinutes > 0)
                        worksheet.Cells["F34"].Value = $"без НДС, время {_order.TechCalcHour}:{_order.TechCalcMinutes}ч.";
                    //Дата определения рассчетной цены
                    if (!Funcs.DateIsEmpty(_order.TechCalcPriceDate))
                        worksheet.Cells["L34"].Value = $"дата {_order.TechCalcPriceDate:dd.MM.yyyy}";
                    //Дата приложения заявки на материалы и инструменты
                    if (!Funcs.DateIsEmpty(_order.TechMaterialReqDate))
                        worksheet.Cells["L36"].Value = $"дата {_order.TechMaterialReqDate:dd.MM.yyyy}";
                    //Наличие материалов
                    if (_order.SalesMaterialAvailable)
                        worksheet.Cells["F40"].Value = "V";
                    else
                        worksheet.Cells["H40"].Value = "V";
                    //Дата определения наличия материалов
                    if (!Funcs.DateIsEmpty(_order.SalesMaterialAvailableDate))
                        worksheet.Cells["L40"].Value = $"дата {_order.SalesMaterialAvailableDate:dd.MM.yyyy}";
                    //Предлагаемая цена
                    if (_order.DirectorExpectedPrice > 0)
                        worksheet.Cells["C42"].Value = $"предлагаемая цена  {_order.DirectorExpectedPrice:N2} руб.,";
                    //Приме. срок изготовления дней
                    if (_order.DirectorExpectedDay > 0)
                        worksheet.Cells["F42"].Value = $"без НДС,   (прим.срок изг.{_order.DirectorExpectedDay} раб.дней)";
                    //Дата определения предполоагаемой цены
                    if (!Funcs.DateIsEmpty(_order.DirectorExpectedDate))
                        worksheet.Cells["L42"].Value = $"дата {_order.DirectorExpectedDate:dd.MM.yyyy}";
                    //Тип предоплаты
                    if (_order.SalesPrepaymentType == PaymentType.Full)
                    {
                        worksheet.Cells["E46"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["E46"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SalesPrepaymentType == PaymentType.Half)
                    {
                        worksheet.Cells["D46"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["D46"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SalesPrepaymentType == PaymentType.None)
                    {
                        worksheet.Cells["G46"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["G46"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    //Отгрузить при типе оплаты
                    if (_order.SalesPaymentType == PaymentType.Full)
                    {
                        worksheet.Cells["D48"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["D48"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SalesPaymentType == PaymentType.Half)
                    {
                        worksheet.Cells["E48"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["E48"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SalesPaymentType == PaymentType.None)
                    {
                        worksheet.Cells["G48"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["G48"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    //Отпускная цена
                    if (_order.SalesPrice > 0)
                        worksheet.Cells["D50"].Value = _order.SalesPrice.ToString("N2");
                    //Дата определения отпускной цены
                    if (!Funcs.DateIsEmpty(_order.SalesPriceDate))
                        worksheet.Cells["L51"].Value = $"дата {_order.SalesPriceDate:dd.MM.yyyy}";
                    //Номер коммерческого предложения
                    if (! _order.SalesComOfferNumber.IsEmpty())
                        worksheet.Cells["I55"].Value = $"       № {_order.SalesComOfferNumber}";
                    //Дата коммерческого предложения
                    if (!Funcs.DateIsEmpty(_order.SalesComOfferDate))
                        worksheet.Cells["L55"].Value = $"дата {_order.SalesComOfferDate:dd.MM.yyyy}";
                    //Номер спецификации
                    if (! _order.AccSpecifNumber.IsEmpty())
                        worksheet.Cells["I57"].Value = $"       № {_order.AccSpecifNumber}";
                    //Дата спецификации
                    if (!Funcs.DateIsEmpty(_order.AccSpecifDate))
                        worksheet.Cells["L57"].Value = $"дата {_order.AccSpecifDate:dd.MM.yyyy}";
                    //Номер подписанной спецификации
                    if (!_order.AccSpecifNumber.IsEmpty())
                        worksheet.Cells["I61"].Value = $"       № {_order.AccSpecifNumber}";
                    //Дата подписанной спецификации
                    if (!Funcs.DateIsEmpty(_order.AccSpecifDate))
                        worksheet.Cells["L61"].Value = $"дата {_order.AccSpecifDate:dd.MM.yyyy}";
                    //Номер счета
                    if (! _order.AccBillNumber.IsEmpty())
                        worksheet.Cells["I59"].Value = $"       № {_order.AccBillNumber}";
                    //Дата счета
                    if (!Funcs.DateIsEmpty(_order.AccBillDate))
                        worksheet.Cells["L59"].Value = $"дата {_order.AccBillDate:dd.MM.yyyy}";
                    //Тип оплаты заказа
                    if (_order.AccPaymType == PaymentType.Full)
                        worksheet.Cells["J63"].Value = "V";
                    else if (_order.AccPaymType == PaymentType.Half)
                        worksheet.Cells["H63"].Value = "V";
                    else if (_order.AccPaymType == PaymentType.None)
                        worksheet.Cells["F63"].Value = "V";
                    //Дата оплаты заказа
                    if (!Funcs.DateIsEmpty(_order.AccPaymDate))
                        worksheet.Cells["L63"].Value = $"дата {_order.AccPaymDate:dd.MM.yyyy}";
                    //Дата заказа инструментов и материалов
                    if (!Funcs.DateIsEmpty(_order.SalesMaterialOrderDate))
                        worksheet.Cells["L71"].Value = $"дата {_order.SalesMaterialOrderDate:dd.MM.yyyy}";
                    //Срок поставки материалов и инструментов
                    if (!Funcs.DateIsEmpty(_order.SalesMaterialOrderReadyDate))
                        worksheet.Cells["L75"].Value = $"дата {_order.SalesMaterialOrderReadyDate:dd.MM.yyyy}";
                    //Заказ поставлен в план
                    if (!Funcs.DateIsEmpty(_order.DirectorOrderPlanedDate))
                        worksheet.Cells["E82"].Value = $"дата {_order.DirectorOrderPlanedDate:dd.MM.yyyy}";
                    //Заказ получен в производство
                    if (!Funcs.DateIsEmpty(_order.OrderInManufactureDate))
                        worksheet.Cells["L86"].Value = $"дата {_order.OrderInManufactureDate:dd.MM.yyyy}";
                    //Материалы получены в производство
                    if (!Funcs.DateIsEmpty(_order.MaterialInManufactureDate))
                        worksheet.Cells["L88"].Value = $"дата {_order.MaterialInManufactureDate:dd.MM.yyyy}";
                    //Заказ запущен в работу
                    if (!Funcs.DateIsEmpty(_order.OrderInWorkDate))
                        worksheet.Cells["L90"].Value = $"дата {_order.OrderInWorkDate:dd.MM.yyyy}";
                    //Изготовление закончено, передано в ОТК
                    if (!Funcs.DateIsEmpty(_order.ManMadeDate))
                        worksheet.Cells[$"L98"].Value = $"дата {_order.ManMadeDate:dd.MM.yyyy}";
                    //Получено в ОТК
                    if (!Funcs.DateIsEmpty(_order.OTKProductGetDate))
                        worksheet.Cells[$"C102"].Value = $"дата получения {_order.OTKProductGetDate:dd.MM.yyyy}";
                    //Возвращено на доработку
                    if (!Funcs.DateIsEmpty(_order.OTKProductDefectDate))
                        worksheet.Cells[$"L104"].Value = $"дата {_order.OTKProductDefectDate:dd.MM.yyyy}";
                    //Причина возврата в доработку
                    var infoLength = _order.OTKProductDefectInfo.Length;
                    worksheet.Cells[$"E106"].Value = _order.OTKProductDefectInfo.Substring(0, Math.Min(infoLength, 90));

                    if (infoLength > 90)
                        worksheet.Cells[$"C108"].Value = _order.OTKProductDefectInfo.Substring(90, Math.Min(infoLength - 90, 110));

                    if (infoLength > 200)
                        worksheet.Cells[$"C108"].Value = _order.OTKProductDefectInfo.Substring(200);
                    //Дата принятия корректного изделия
                    if (!Funcs.DateIsEmpty(_order.OTKProductCorrectDate))
                        worksheet.Cells[$"L114"].Value = $"дата {_order.OTKProductCorrectDate:dd.MM.yyyy}";
                    //Дата принятия изделия в произв. участок
                    if (!Funcs.DateIsEmpty(_order.ManProductApplyDate))
                        worksheet.Cells[$"L118"].Value = $"дата {_order.ManProductApplyDate:dd.MM.yyyy}";
                    //Дата заполнения лимитки
                    if (!Funcs.DateIsEmpty(_order.ManLimitCreateDate))
                        worksheet.Cells[$"L120"].Value = $"дата {_order.ManLimitCreateDate:dd.MM.yyyy}";
                    //Дата и время информирования заказчика
                    if (!Funcs.DateIsEmpty(_order.AccCustomerInformedDate))
                    {
                        worksheet.Cells[$"I124"].Value = $"время {_order.AccCustomerInformedDate:HH:mm}";
                        worksheet.Cells[$"L124"].Value = $"дата {_order.AccCustomerInformedDate:dd.MM.yyyy}";
                    }
                    //Дата оплаты изделия
                    if (!Funcs.DateIsEmpty(_order.AccOrderPaidDate))
                        worksheet.Cells[$"L126"].Value = $"дата {_order.AccOrderPaidDate:dd.MM.yyyy}";
                    //Дата передачи документов на отгрузку
                    if (!Funcs.DateIsEmpty(_order.AccDocumentsToSendDate))
                        worksheet.Cells[$"L128"].Value = $"дата {_order.AccDocumentsToSendDate:dd.MM.yyyy}";
                    //Тип отгрузки товара
                    if (_order.SendOrderDeliveryType == DeliveryType.Self)
                    {
                        worksheet.Cells["C134"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C134"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SendOrderDeliveryType == DeliveryType.OurCompany)
                    {
                        worksheet.Cells["D134"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["D134"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    else if (_order.SendOrderDeliveryType == DeliveryType.Company)
                    {
                        worksheet.Cells["F134"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["F134"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    }
                    //Дата отгрузки
                    if (!Funcs.DateIsEmpty(_order.SendDeliveryDate))
                        worksheet.Cells[$"L134"].Value = $"дата {_order.SendDeliveryDate:dd.MM.yyyy}";
                    //Изделие изготовлено
                    readyText = null;
                    if (_order.OrderReadyType == OrderReadyType.Sucess)
                    {
                        readyText = worksheet.Cells["G141"].RichText[0];
                    }
                    else if (_order.OrderReadyType == OrderReadyType.Holded)
                    {
                        worksheet.Cells["M141"].Value = $"задержано на {_order.OrderHoldDay} дней";
                        readyText = worksheet.Cells["M141"].RichText[0];
                    }
                    else if (_order.OrderReadyType == OrderReadyType.Canceled)
                    {
                        readyText = worksheet.Cells["K141"].RichText[0];
                    }

                    readyText.UnderLine = true;
                    //Причина невыполненного заказа
                    worksheet.Cells["E142"].Value = _order.OrderCorruptReason;
                    //Табличная часть операций
                    var position = 94;
                    for (int i = 0; i < _operations.Count; i++)
                    {
                        if (i < (_operations.Count - 1))
                            worksheet.InsertRow(position, 1, position + 1);

                        worksheet.Cells[$"C{position}:D{position}"].Merge = true;
                        worksheet.Cells[$"C{position}:D{position}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        worksheet.Cells[$"E{position}:H{position}"].Merge = true;
                        worksheet.Cells[$"E{position}:H{position}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells[$"I{position}:J{position}"].Merge = true;
                        worksheet.Cells[$"I{position}:J{position}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (_operations[i].EmployeeId.HasValue &&
                            _employeeNames.TryGetValue(_operations[i].EmployeeId.Value, out var employee))
                        {
                            worksheet.Cells[$"C{position}"].Value = employee.Name;
                        }

                        if (_operationNames.TryGetValue(_operations[i].OperationId, out var operation))
                            worksheet.Cells[$"E{position}"].Value = operation.Name;
                        
                        worksheet.Cells[$"I{position}"].Value = _operations[i].Count;
                        worksheet.Cells[$"K{position}"].Value = _operations[i].StartDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[$"L{position}"].Value = _operations[i].StartDate.ToString("HH:mm");
                        worksheet.Cells[$"M{position}"].Value = _operations[i].EndDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[$"N{position}"].Value = _operations[i].EndDate.ToString("HH:mm");
                        worksheet.Cells[$"O{position}"].Value = _operations[i].Comment;

                        position++;
                    }

                    if (!Directory.Exists(fi.Directory.FullName))
                        Directory.CreateDirectory(fi.Directory.FullName);
                    
                    excelPackage.SaveAs(fi);
                }
            }

            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
                MessageBoxHelper.InformationMesage("Печать", $"Не установленн Excel. Файл сохранен в каталог {fi.FullName}", this);
            else
                System.Diagnostics.Process.Start(fi.FullName);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
