using MetalClient.Client;
using MetalClient.DataManager;
using MetalDAL.Manager;
using MetalDAL.Mapper;
using MetalDiagnostic.Logger;
using MetalServer;
using MetalServer.Handler;
using MetalServer.Server;
using MetalTransport.Datagram;
using MetalTransport.Datagram.GetListData;
using MetalTransport.Datagram.Properties;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using MetalTransport.ModelEx.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester
{
    [TestClass]
    public class TestBase
    {
        protected const uint STRING_FIELD_LENGTH = 30;
        protected const int INT_MAX = 30;

        public const int HALF_CHUNK_SIZE = 255 * 512;

        protected const uint EMPLOYEE_NAME_LENGTH = 60;
        protected const uint SMALL_FILE = 1024 * 1024 * 1; //1MB
        protected const uint BIG_FILE = 1024 * 1024 * 10; //10MB
        protected readonly Random _rnd = new Random();

        protected IServer _server;
        protected IClient _client;
        protected ClientDataManager _dataManager;
        protected ModelManager modelManager;

        [TestInitialize]
        public void Init()
        {
            LogService.InitLogger(LoggerType.Test);
            var drawingPath = @"C:\Temp\DrawingPath\";

            modelManager = new ModelManager(drawingPath);
            modelManager.Start();

            _server = new TcpMetalServer();
            _client = new TcpMetalClient();

            var assembly = typeof(MetallServerHost).Assembly;
            var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<HandlerAttribute>() != null);

            foreach (var type in types)
            {
                var handler = Activator.CreateInstance(type) as IDatagramHandler;
                handler.Manager = modelManager;
                _server.AddDatagramHandler(handler);
            }

            _server.Start();
            _client.Start();

            _dataManager = new ClientDataManager(_client);
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            ClearAll();

            _client.Stop();
            _server.Stop();
        }

        private void ClearAll()
        {
            var factory = new DatagramFactory();

            //заказы
            var request = factory.WithType(DatagramType.GetOrdersNext)
                                 .WithDTOObject(new GetPaginationElementsList(0, int.MaxValue, false, OrderDTO.ListSort))
                                 .Build();

            _dataManager.ExcuteRequestAsync<SetListData<OrderListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var orders = t1.Result.Elements;

                       foreach (var order in orders)
                       {
                           //материалы лимитки
                           request = factory.WithType(DatagramType.GetLimitMaterialList)
                                            .WithDTOObject(new GetOrderIdFilteredElementsList(order.Id))
                                            .Build();

                           _dataManager.ExcuteRequestAsync<SetListData<LimitCardMaterialDTO>>(request, CancellationToken.None).
                                    ContinueWith((m1) =>
                                    {
                                        var materials = m1.Result.Elements;

                                        foreach (var limitMaterial in materials)
                                            RemoveLimitMaterial(limitMaterial.Id, true);
                                    }).Wait();

                           //операции лимитки
                           request = factory.WithType(DatagramType.GetLimitOperationList)
                                            .WithDTOObject(new GetOrderIdFilteredElementsList( order.Id))
                                            .Build();

                           _dataManager.ExcuteRequestAsync<SetListData<LimitCardOperationDTO>>(request, CancellationToken.None).
                                    ContinueWith((o1) =>
                                    {
                                        var operations = o1.Result.Elements;

                                        foreach (var limitOperation in operations)
                                            RemoveLimitOperation(limitOperation.Id, true);
                                    }).Wait();

                           //операции заказа
                           request = factory.WithType(DatagramType.GetOrderOperationList)
                                            .WithDTOObject(new GetOrderIdFilteredElementsList(order.Id))
                                            .Build();

                           _dataManager.ExcuteRequestAsync<SetListData<OrderOperationDTO>>(request, CancellationToken.None).
                                    ContinueWith((o1) =>
                                    {
                                        var operations = o1.Result.Elements;

                                        foreach (var operation in operations)
                                            RemoveOrderOperation(operation.Id, true);
                                    }).Wait();

                           RemoveOrder(order.Id, true);
                       }
                   }).Wait();

            //группы заказов
            request = factory.WithType(DatagramType.GetOrderGroupNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var ordersGroups = t1.Result.Elements;

                       foreach (var group in ordersGroups)
                           RemoveOrderGroup(group.Id, true);
                   }).Wait();

            //заказчики
            request = factory.WithType(DatagramType.GetCustomerNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var customers = t1.Result.Elements;

                       foreach (var customer in customers)
                           RemoveCustomer(customer.Id, true);
                   }).Wait();

            //сотрудники
            request = factory.WithType(DatagramType.GetEmployeeNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var employees = t1.Result.Elements;

                       foreach (var employee in employees)
                           RemoveEmployee(employee.Id, true);
                   }).Wait();

            //материалы
            request = factory.WithType(DatagramType.GetMaterialNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var materials = t1.Result.Elements;

                       foreach (var material in materials)
                           RemoveMaterial(material.Id, true);
                   }).Wait();

            //операции
            request = factory.WithType(DatagramType.GetOperationNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var operations = t1.Result.Elements;

                       foreach (var operation in operations)
                           RemoveOperation(operation.Id, true);
                   }).Wait();

            //должности
            request = factory.WithType(DatagramType.GetPostList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var posts = t1.Result.Elements;

                       foreach (var post in posts)
                           RemovePost(post.Id, true);
                   }).Wait();

            //группы пользователей
            request = factory.WithType(DatagramType.GetUserGroupNameList)
                             .WithDTOObject(new GetAllVersionElementsList(false, -1))
                             .Build();

            _dataManager.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, CancellationToken.None).
                   ContinueWith((t1) =>
                   {
                       var usersGroups = t1.Result.Elements;

                       foreach (var group in usersGroups)
                           RemoveUserGroup(group.Id, true);
                   }).Wait();
        }

        protected CustomerDTO GenerateAndStoreCustomer()
        {
            var customer = GenerateCustomer();
            StoreCustomer(customer);

            return customer;
        }

        protected void StoreCustomer(CustomerDTO customer)
        {
            AsyncStoreCustomer(customer).Wait();
        }

        protected Task AsyncStoreCustomer(CustomerDTO customer)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetCustomerElement).WithDTOObject(customer).Build();
            var task = _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                });

            return task;
        }

        protected OrderGroupDTO GenerateAndStoreOrderGroup()
        {
            var orderGroup = GenerateOrderGroup();
            StoreOrderGroup(orderGroup);

            return orderGroup;
        }

        protected void StoreOrderGroup(OrderGroupDTO orderGroup)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetOrderGroupElement).WithDTOObject(orderGroup).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected PostDTO GenerateAndStorePost()
        {
            var post = GeneratePost();
            StorePost(post);

            return post;
        }

        protected void StorePost(PostDTO post)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetPostElement).WithDTOObject(post).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected OperationDTO GenerateAndStoreOperation()
        {
            var operation = GenerateOperation();
            StoreOperation(operation);

            return operation;
        }

        protected void StoreOperation(OperationDTO operation)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetOperationElement).WithDTOObject(operation).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected UserGroupDTO GenerateAndStoreUserGroup()
        {
            var userGroup = GenerateUserGroup();
            StoreUserGroup(userGroup);

            return userGroup;
        }

        protected void StoreUserGroup(UserGroupDTO userGroup)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetUserGroupElement).WithDTOObject(userGroup).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected EmployeeDTO GenerateAndStoreEmployee(PostDTO post, UserGroupDTO group)
        {
            var employee = GenerateEmployee(post, group);
            StoreEmployee(employee);

            return employee;
        }

        protected void StoreEmployee(EmployeeDTO employee)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetEmployeeElement).WithDTOObject(employee).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected MaterialDTO GenerateAndStoreMaterial()
        {
            var material = GenerateMaterial();
            StoreMaterial(material);

            return material;
        }

        protected void StoreMaterial(MaterialDTO material)
        {
            AsyncStoreMaterial(material).Wait();
        }

        protected Task AsyncStoreMaterial(MaterialDTO material)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetMaterialElement).WithDTOObject(material).Build();
            var task = _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                });

            return task;
        }

        protected OrderDTO GenerateAndStoreOrder(Guid customerId, Guid orderGroupId)
        {
            var order = GenerateOrder(customerId, orderGroupId);
            SyncStoreOrder(order);

            return order;
        }

        protected void SyncStoreOrder(OrderDTO order)
        {
            AsyncStoreOrder(order).Wait();
        }

        protected Task AsyncStoreOrder(OrderDTO order, ClientDataManager manager = null)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetOrderElement)
                                 .WithDTOObject(order)
                                 .Build();

            var requestManager = manager ?? _dataManager;
            var task = requestManager.ExcuteRequestAsync<OrderHandledDTO>(request, CancellationToken.None).
                ContinueWith((t) =>
                {
                    Assert.IsTrue(CheckError(t));
                    order.Number = t.Result.OrderNumber;
                });

            return task;
        }

        protected OrderOperationDTO GenerateAndStorOrderOperation(Guid operationId, Guid employeeId, Guid orderId)
        {
            var orderOperation = GenerateOrderOperation(operationId, employeeId, orderId);
            SyncStoreOrderOperation(orderOperation);

            return orderOperation;
        }

        protected void SyncStoreOrderOperation(OrderOperationDTO operation)
        {
            AsyncStoreOrderOperation(operation).Wait();
        }

        protected Task AsyncStoreOrderOperation(OrderOperationDTO operation, ClientDataManager manager = null)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetOrderOperationElement).WithDTOObject(operation).Build();
            var requestManager = manager ?? _dataManager;
            var task = requestManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                });

            return task;
        }

        protected LimitCardMaterialDTO GenerateAndStoreLimitMaterial(Guid materialId, Guid orderId)
        {
            var limitMaterial = GenerateLimitCardMaterial(materialId, orderId);
            SyncStoreLimitMaterial(limitMaterial);

            return limitMaterial;
        }

        protected void SyncStoreLimitMaterial(LimitCardMaterialDTO limitMaterial)
        {
            AsyncStoreLimitMaterial(limitMaterial).Wait(); ;
        }

        protected Task AsyncStoreLimitMaterial(LimitCardMaterialDTO limitMaterial, ClientDataManager manager = null)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetLimitMaterialElement).WithDTOObject(limitMaterial).Build();
            var requestManager = manager ?? _dataManager;
            var task = requestManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                });

            return task;
        }

        protected void RemoveOrder(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemOrderElement)
                                 .WithDTOObject(new RemElementData(id, permannet))
                                 .Build();

            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected void RemoveMaterial(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemMaterialElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                }).Wait();
        }

        protected void RemoveCustomer(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemCustomerElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveEmployee(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemEmployeeElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemovePost(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemPostElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveUserGroup(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemUserGroupElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveOrderGroup(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemOrderGroupElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveOperation(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemOperationElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveOrderOperation(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemOrderOperationElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveLimitMaterial(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemLimitMaterialElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveLimitOperation(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemLimitOperationElement).WithDTOObject(new RemElementData(id, permannet)).Build();
            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }

        protected void RemoveFile(Guid id, bool permannet = false)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.RemFile)
                                 .WithDTOObject(new RemElementData(id, permannet))
                                 .Build();

            _dataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        Assert.IsTrue(CheckError(t1));
                    }).Wait();
        }


        protected LimitCardOperationDTO GenerateAndStoreLimitOperation(Guid operationId, Guid orderId)
        {
            var limitOperation = GenerateLimitCardOperation(operationId, orderId);
            SyncStoreLimitOperation(limitOperation);

            return limitOperation;
        }

        protected void SyncStoreLimitOperation(LimitCardOperationDTO limitMaterial)
        {
            AsyncStoreLimitOperation(limitMaterial).Wait();
        }

        protected Task AsyncStoreLimitOperation(LimitCardOperationDTO limitMaterial, ClientDataManager manager = null)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.SetLimitOperationElement).WithDTOObject(limitMaterial).Build();
            var requestManager = manager ?? _dataManager;
            var task = requestManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.IsTrue(CheckError(t1));
                });

            return task;
        }

        protected LimitCardOperationDTO GenerateLimitCardOperation(Guid operationId, Guid orderId)
        {
            var limitCardOpertion = new LimitCardOperationDTO();
            limitCardOpertion.ElapsedHours = (short)GetRandomInt(23);
            limitCardOpertion.ElapsedMinutes = (short)GetRandomInt(59);
            limitCardOpertion.OperationId = operationId;
            limitCardOpertion.OrderId = orderId;
            limitCardOpertion.PricePerHour = GetRandomDouble();
            return limitCardOpertion;
        }

        protected LimitCardFactMaterialDTO GenerateLimitCardFactMaterial(Guid limitCardMaterialId, Guid materialId)
        {
            var limitCardFactMaterial = new LimitCardFactMaterialDTO(limitCardMaterialId);
            limitCardFactMaterial.Count = GetRandomDouble();
            limitCardFactMaterial.MaterialId = materialId;
            limitCardFactMaterial.Price = GetRandomDouble();

            return limitCardFactMaterial;
        }

        protected LimitCardMaterialDTO GenerateLimitCardMaterial(Guid materialId, Guid orderId)
        {
            var limit_material = new LimitCardMaterialDTO();
            limit_material.MaterialId = materialId;
            limit_material.Multiplicity = GetRandomDouble();
            limit_material.OrderId = orderId;
            limit_material.Price = GetRandomDouble();
            limit_material.Units = GetRandomString(STRING_FIELD_LENGTH);
            limit_material.UsagePerOrder = GetRandomDouble();
            limit_material.UsagePerUnits = GetRandomDouble();

            return limit_material;
        }

        protected OrderDTO GenerateOrder(Guid customerId, Guid orderGroupId)
        {
            var order = new OrderDTO();
            order.Date = DateTimeHelper.DateTimeNow();
            order.Name = GetRandomString(STRING_FIELD_LENGTH);
            order.Count = GetRandomInt(INT_MAX);
            order.CustomerId = customerId;
            order.ReadyDate = DateTimeHelper.DateTimeNow();
            order.AcceptedDate = DateTimeHelper.DateTimeNow();
            order.DrawingNumber = GetRandomString(STRING_FIELD_LENGTH);
            order.DrawingState = GetRandomDrawingType();
            order.IsCustomerMaterial = true;
            order.CustomerMaterial = GetRandomString(STRING_FIELD_LENGTH);
            order.CustomerReadyDate = DateTimeHelper.DateTimeNow();
            order.MaterialAgreed = DateTimeHelper.DateTimeNow();
            order.TechCalcPrice = GetRandomDouble();
            order.TechCalcHour = GetRandomInt(24);
            order.TechCalcMinutes = GetRandomInt(59);
            order.TechCalcPriceDate = DateTimeHelper.DateTimeNow();
            order.TechCalcMultiplier = GetRandomDouble();
            order.TechMaterialReqDate = DateTimeHelper.DateTimeNow();
            order.DirectorExpectedPrice = GetRandomDouble();
            order.DirectorExpectedDay = GetRandomInt(5);
            order.DirectorExpectedDate = DateTimeHelper.DateTimeNow();
            order.SalesMaterialAvailable = true;
            order.SalesMaterialAvailableDate = DateTimeHelper.DateTimeNow();
            order.SalesPrepaymentType = GetRandomPaymentType();
            order.SalesPaymentType = GetRandomPaymentType();
            order.SalesPrice = GetRandomDouble();
            order.SalesPriceDate = DateTimeHelper.DateTimeNow();
            order.SalesComOfferNumber = GetRandomString(STRING_FIELD_LENGTH);
            order.SalesComOfferDate = DateTimeHelper.DateTimeNow();
            order.AccSpecifNumber = GetRandomString(STRING_FIELD_LENGTH);
            order.AccSpecifDate = DateTimeHelper.DateTimeNow();
            order.AccBillNumber = GetRandomString(STRING_FIELD_LENGTH);
            order.AccBillDate = DateTimeHelper.DateTimeNow();
            order.AccPaymType = GetRandomPaymentType();
            order.AccPaymDate = DateTimeHelper.DateTimeNow();
            order.SalesMaterialOrderDate = DateTimeHelper.DateTimeNow();
            order.SalesMaterialOrderReadyDate = DateTimeHelper.DateTimeNow();
            order.DirectorOrderPlanedDate = DateTimeHelper.DateTimeNow();
            order.OrderInManufactureDate = DateTimeHelper.DateTimeNow();
            order.MaterialInManufactureDate = DateTimeHelper.DateTimeNow();
            order.OrderInWorkDate = DateTimeHelper.DateTimeNow();
            order.ManMadeDate = DateTimeHelper.DateTimeNow();
            order.OTKProductGetDate = DateTimeHelper.DateTimeNow();
            order.OTKProductDefectDate = DateTimeHelper.DateTimeNow();
            order.OTKProductDefectInfo = GetRandomString(STRING_FIELD_LENGTH);
            order.OTKProductCorrectDate = DateTimeHelper.DateTimeNow();
            order.ManProductApplyDate = DateTimeHelper.DateTimeNow();
            order.ManLimitCreateDate = DateTimeHelper.DateTimeNow();
            order.AccCustomerInformedDate = DateTimeHelper.DateTimeNow();
            order.AccOrderPaidDate = DateTimeHelper.DateTimeNow();
            order.AccDocumentsToSendDate = DateTimeHelper.DateTimeNow();
            order.SendOrderDeliveryType = GetRandomDeliveryType();
            order.SendDeliveryDate = GetRandomInt(1) == 1 ? DateTimeHelper.DateTimeNow() : Constants.EMPTY_DATETIME;
            order.OrderReadyType = GetRandomOrderReadyType();
            order.OrderHoldDay = GetRandomInt(5);
            order.OrderCorruptReason = GetRandomString(STRING_FIELD_LENGTH);
            order.OrderState = GetRandomOrderState();
            order.OrderGroupId = orderGroupId;

            return order;
        }

        protected OrderOperationDTO GenerateOrderOperation(Guid operationId, Guid employeeId, Guid orderId)
        {
            var orderOperation = new OrderOperationDTO();
            orderOperation.Comment = GetRandomString(STRING_FIELD_LENGTH);
            orderOperation.Count = GetRandomInt(INT_MAX);
            orderOperation.EmployeeId = employeeId;
            orderOperation.EndDate = DateTimeHelper.DateTimeNow();
            orderOperation.Index = 0;
            orderOperation.OperationId = operationId;
            orderOperation.OrderId = orderId;
            orderOperation.StartDate = DateTimeHelper.DateTimeNow();

            return orderOperation;
        }

        protected OrderGroupDTO GenerateOrderGroup()
        {
            var group = new OrderGroupDTO();
            group.Name = GetRandomString(STRING_FIELD_LENGTH);

            return group;
        }

        protected MaterialDTO GenerateMaterial()
        {
            var material = new MaterialDTO();
            material.Name = GetRandomString(STRING_FIELD_LENGTH);

            return material;
        }

        protected OperationDTO GenerateOperation()
        {
            var operation = new OperationDTO();
            operation.Name = GetRandomString(STRING_FIELD_LENGTH);

            return operation;
        }

        protected CustomerDTO GenerateCustomer()
        {
            var customer = new CustomerDTO();
            customer.Employee = GenerateEmployeeName();
            customer.Fax = GetRandomString(STRING_FIELD_LENGTH);
            customer.Name = GetRandomString(STRING_FIELD_LENGTH);
            customer.Phone = GetRandomString(STRING_FIELD_LENGTH);
            customer.Mail = GetRandomString(STRING_FIELD_LENGTH);

            return customer;
        }

        protected EmployeeDTO GenerateEmployee(PostDTO post, UserGroupDTO group)
        {
            var employee = new EmployeeDTO();
            employee.Name = GetRandomString(STRING_FIELD_LENGTH);
            employee.Patronymic = GetRandomString(STRING_FIELD_LENGTH);
            employee.Secondname = GetRandomString(STRING_FIELD_LENGTH);

            using (MD5 md5Hash = MD5.Create())
            {
                employee.Password = SecurityHelper.GetMd5Hash(md5Hash, GetRandomString(STRING_FIELD_LENGTH));
            }

            employee.UseForLogin = true;
            employee.PostId = post.Id;
            employee.Post = post;

            employee.UserGroupId = group.Id;
            employee.UserGroup = group;

            return employee;
        }

        protected UserGroupDTO GenerateUserGroup()
        {
            var userGroup = new UserGroupDTO();
            userGroup.Name = GetRandomString(STRING_FIELD_LENGTH);
            userGroup.Rights = new byte[13];

            for (int i = 0; i < 13; i++)
                userGroup.Rights[i] = 2;

            return userGroup;
        }

        protected PostDTO GeneratePost()
        {
            var post = new PostDTO();
            post.Name = GetRandomString(STRING_FIELD_LENGTH); ;
            return post;
        }

        protected bool CheckError(Task<OrderHandledDTO> task)
        {
            if (!task.IsCompleted ||
                 task.Result.Type != HandledType.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool CheckError(Task<HandledDTO> task)
        {
            if (!task.IsCompleted ||
                 task.Result.Type != HandledType.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected string GenerateEmployeeName()
        {
            return GetRandomString(EMPLOYEE_NAME_LENGTH);
        }

        protected int GetRandomInt(int max)
        {
            return _rnd.Next(max);
        }

        protected double GetRandomDouble()
        {
            return _rnd.NextDouble();
        }

        protected DrawingType GetRandomDrawingType()
        {
            var max = Enum.GetValues(typeof(DrawingType)).Cast<int>().Max();
            return (DrawingType)Enum.ToObject(typeof(DrawingType), GetRandomInt(max));
        }

        protected PaymentType GetRandomPaymentType()
        {
            var max = Enum.GetValues(typeof(PaymentType)).Cast<int>().Max();
            return (PaymentType)Enum.ToObject(typeof(PaymentType), GetRandomInt(max));
        }

        protected DeliveryType GetRandomDeliveryType()
        {
            var max = Enum.GetValues(typeof(DeliveryType)).Cast<int>().Max();
            return (DeliveryType)Enum.ToObject(typeof(DeliveryType), GetRandomInt(max));
        }

        protected OrderReadyType GetRandomOrderReadyType()
        {
            var max = Enum.GetValues(typeof(OrderReadyType)).Cast<int>().Max();
            return (OrderReadyType)Enum.ToObject(typeof(OrderReadyType), GetRandomInt(max));
        }

        protected OrderState GetRandomOrderState()
        {
            var max = Enum.GetValues(typeof(OrderState)).Cast<int>().Max();
            return (OrderState)Enum.ToObject(typeof(OrderState), GetRandomInt(max));
        }

        protected string GetRandomString(uint length)
        {
            var sb = new StringBuilder();
            for (uint i = 0; i < length; ++i)
            {
                sb.Append((char)GetRandomInt(128));
            }
            return sb.ToString();
        }

        protected byte[] GetByteArray(uint length)
        {
            var arr = new byte[length];
            _rnd.NextBytes(arr);

            return arr;
        }
    }
}
