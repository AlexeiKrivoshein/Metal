using System.Collections.Generic;

namespace MetalTransport.Datagram
{
    public enum DatagramType
    {
        //Служебные
        NONE,
        Response,
        Error,

        //Файл
        GetFileData,
        SetFileData,
        RemFile,
        GetFileList,

        //Фильтрация
        Filter,

        //План
        GetPlanElement,

        //Заказ
        GetOrdersNext,
        GetOrdersActual,
        GetOrderElement,
        SetOrderElement,
        RemOrderElement,

        GetCustomerNameList,
        GetCustomerElement,
        SetCustomerElement,
        RemCustomerElement,

        GetEmployeeNameList,
        GetEmployeeElement,
        SetEmployeeElement,
        RemEmployeeElement,

        GetPostList,
        GetPostElement,
        SetPostElement,
        RemPostElement,

        GetOperationNameList,
        GetOperationElement,
        SetOperationElement,
        RemOperationElement,

        GetMaterialNameList,
        GetMaterialElement,
        SetMaterialElement,
        RemMaterialElement,

        GetOrderGroupNameList,
        GetOrderGroupElement,
        SetOrderGroupElement,
        RemOrderGroupElement,

        GetLockedList,
        GetLockedElement,
        SetLockedElement,
        RemLockedElement,

        GetOrderOperationList,
        GetOrderOperationElement,
        SetOrderOperationElement,
        RemOrderOperationElement,

        GetLimitMaterialList,
        SetLimitMaterialElement,
        RemLimitMaterialElement,

        GetLimitOperationList,
        SetLimitOperationElement,
        RemLimitOperationElement,

        GetUserGroupNameList,
        GetUserGroupElement,
        SetUserGroupElement,
        RemUserGroupElement,

        GetUsersList,

        UserLogin
    }
}
