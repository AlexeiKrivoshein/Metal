using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MetalTransport.ModelEx
{
    public enum OrderState : int
    {
        [Description("Создан")]
        Create = 0,

        [Description("Подготовлено комерческое предложение")]
        CommercialOffer,

        [Description("Спецификация выставлена")]
        Specification,

        [Description("Выставлен счёт")]
        Invoice,

        [Description("Материалы заказаны")]
        MaterialsOrdered,

        [Description("Поставлен в план")]
        InPlan,

        [Description("Изготавливается")]
        Process,

        [Description("В ОТК")]
        OTK,

        [Description("Доработка")]
        Defect,

        [Description("ОТК пройдено")]
        NoDefect,

        [Description("Документы переданы в бухгалтерию")]
        AccountingDep,

        [Description("Заказчик проинформирован")]
        CustomerInform,

        [Description("Оплачено")]
        Paid,

        [Description("Отгружено")]
        Shipping, 
    }
}
