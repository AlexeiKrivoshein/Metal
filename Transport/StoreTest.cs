using System;
using System.Threading;
using MetalTransport.Datagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MetalTransport.ModelEx;
using MetalTransport.Datagram.Properties;
using System.Linq;
using MetalTransport.Helper;
using FileHelper = MetalClient.Helper.FileHelper;
using System.IO;
using System.Security.Cryptography;
using MetalTransport.Datagram.Security;
using Transport;

namespace StoreTest
{
    [TestClass]
    public class StoreTest 
        : TestBase
    {
        [TestMethod]
        public void CustomerTest()
        {
            var factory = new DatagramFactory();

            //сохранение
            var customer_master = GenerateAndStoreCustomer();
            
            var request = factory.WithType(DatagramType.GetCustomerElement).WithDTOObject(new GetElementData(customer_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<CustomerDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                    {
                        var customer_store = t1.Result;
                        Assert.AreEqual(customer_master, customer_store);
                    }).Wait();

            var customer_modify = GenerateCustomer();
            customer_modify.Id = customer_master.Id;
            //изменение
            StoreCustomer(customer_modify);

            request = factory.WithType(DatagramType.GetCustomerElement).WithDTOObject(new GetElementData(customer_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<CustomerDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                    {
                        var customer_store = t1.Result;
                        Assert.AreEqual(customer_modify, customer_store);
                    }).Wait();
            //удаление
            RemoveCustomer(customer_modify.Id, false);
            
            request = factory.WithType(DatagramType.GetCustomerElement).WithDTOObject(new GetElementData(customer_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<CustomerDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                    {
                        var customer_store = t1.Result;
                        Assert.IsTrue(customer_store.Deleted);
                    }).Wait();

            RemoveCustomer(customer_modify.Id, true);
            //чтение списка справочника
            var customer1 = GenerateAndStoreCustomer();
            var customer2 = GenerateAndStoreCustomer();
            var customer3 = GenerateAndStoreCustomer();

            request = factory.WithType(DatagramType.GetCustomerNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var customer_list = t1.Result;
                    Assert.AreEqual(customer_list.Elements.Count, 3);
                }).Wait();

            RemoveCustomer(customer1.Id, true);
            RemoveCustomer(customer2.Id, true);
            RemoveCustomer(customer3.Id, true);
            
            request = factory.WithType(DatagramType.GetCustomerNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var customer_list = t1.Result;
                    Assert.AreEqual(customer_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void EmployeeTest()
        {
            var factory = new DatagramFactory();

            var post = GenerateAndStorePost();
            var userGroup = GenerateAndStoreUserGroup();

            //сохранение
            var employee_master = GenerateAndStoreEmployee(post.Id, userGroup.Id);

            var request = factory.WithType(DatagramType.GetEmployeeElement).WithDTOObject(new GetElementData(employee_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<EmployeeDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var employee_store = t1.Result;
                    Assert.AreEqual(employee_master, employee_store);
                }).Wait();

            var employee_modify = GenerateEmployee(post.Id, userGroup.Id);
            employee_modify.Id = employee_master.Id;
            //изменение
            StoreEmployee(employee_modify);

            request = factory.WithType(DatagramType.GetEmployeeElement).WithDTOObject(new GetElementData(employee_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<EmployeeDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var employee_store = t1.Result;
                    Assert.AreEqual(employee_modify, employee_store);
                }).Wait();
            //удаление
            RemoveEmployee(employee_modify.Id, false);

            request = factory.WithType(DatagramType.GetEmployeeElement).WithDTOObject(new GetElementData(employee_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<EmployeeDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var employee_store = t1.Result;
                    Assert.IsTrue(employee_store.Deleted);
                }).Wait();

            RemoveEmployee(employee_modify.Id, true);

            //чтение списка справочника
            var employee1 = GenerateAndStoreEmployee(post.Id, userGroup.Id);
            var employee2 = GenerateAndStoreEmployee(post.Id, userGroup.Id);
            var employee3 = GenerateAndStoreEmployee(post.Id, userGroup.Id);

            request = factory.WithType(DatagramType.GetEmployeeNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var employee_list = t1.Result;
                    Assert.AreEqual(employee_list.Elements.Count, 3);
                }).Wait();

            RemoveEmployee(employee1.Id, true);
            RemoveEmployee(employee2.Id, true);
            RemoveEmployee(employee3.Id, true);

            request = factory.WithType(DatagramType.GetEmployeeNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var employee_list = t1.Result;
                    Assert.AreEqual(employee_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void PostTest()
        {
            var factory = new DatagramFactory();

            //сохранение
            var post_master = GenerateAndStorePost();
            
            var request = factory.WithType(DatagramType.GetPostElement).WithDTOObject(new GetElementData(post_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<PostDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var post_store = t1.Result;
                    Assert.AreEqual(post_master, post_store);
                }).Wait();

            var post_modify = GeneratePost();
            post_modify.Id = post_master.Id;
            //изменение
            StorePost(post_modify);

            request = factory.WithType(DatagramType.GetPostElement).WithDTOObject(new GetElementData(post_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<PostDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var post_store = t1.Result;
                    Assert.AreEqual(post_modify, post_store);
                }).Wait();
            //удаление
            RemovePost(post_modify.Id, false);

            request = factory.WithType(DatagramType.GetPostElement).WithDTOObject(new GetElementData(post_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<PostDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var post_store = t1.Result;
                    Assert.IsTrue(post_store.Deleted);
                }).Wait();

            RemovePost(post_modify.Id, true);

            //чтение списка справочника
            var post1 = GenerateAndStorePost();
            var post2 = GenerateAndStorePost();
            var post3 = GenerateAndStorePost();

            request = factory.WithType(DatagramType.GetPostList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var post_list = t1.Result;
                    Assert.AreEqual(post_list.Elements.Count, 3);
                }).Wait();

            RemovePost(post1.Id, true);
            RemovePost(post2.Id, true);
            RemovePost(post3.Id, true);

            request = factory.WithType(DatagramType.GetPostList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var post_list = t1.Result;
                    Assert.AreEqual(post_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void OperationTest()
        {
            var factory = new DatagramFactory();

            //сохранение
            var operation_master = GenerateAndStoreOperation();

            var request = factory.WithType(DatagramType.GetOperationElement).WithDTOObject(new GetElementData(operation_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.AreEqual(operation_master, operation_store);
                }).Wait();

            var operation_modify = GenerateOperation();
            operation_modify.Id = operation_master.Id;
            //изменение
            StoreOperation(operation_modify);

            request = factory.WithType(DatagramType.GetOperationElement).WithDTOObject(new GetElementData(operation_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<OperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.AreEqual(operation_modify, operation_store);
                }).Wait();
            //удаление
            RemoveOperation(operation_modify.Id);

            request = factory.WithType(DatagramType.GetOperationElement).WithDTOObject(new GetElementData(operation_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.IsTrue(operation_store.Deleted);
                }).Wait();

            RemoveOperation(operation_modify.Id, true);
            //чтение списка справочника
            var operation1 = GenerateAndStoreOperation();
            var operation2 = GenerateAndStoreOperation();
            var operation3 = GenerateAndStoreOperation();

            request = factory.WithType(DatagramType.GetOperationNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_list = t1.Result;
                    Assert.AreEqual(operation_list.Elements.Count, 3);
                }).Wait();

            RemoveOperation(operation1.Id, true);
            RemoveOperation(operation2.Id, true);
            RemoveOperation(operation3.Id, true);

            request = factory.WithType(DatagramType.GetOperationNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_list = t1.Result;
                    Assert.AreEqual(operation_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void MaterialTest()
        {
            var factory = new DatagramFactory();

            //сохранение
            var material_master = GenerateAndStoreMaterial();

            var request = factory.WithType(DatagramType.GetMaterialElement).WithDTOObject(new GetElementData(material_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<MaterialDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var material_store = t1.Result;
                    Assert.AreEqual(material_master, material_store);
                }).Wait();

            var material_modify = GenerateMaterial();
            material_modify.Id = material_master.Id;
            //изменение
            StoreMaterial(material_modify);

            request = factory.WithType(DatagramType.GetMaterialElement).WithDTOObject(new GetElementData(material_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<MaterialDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var material_store = t1.Result;
                    Assert.AreEqual(material_modify, material_store);
                }).Wait();
            //удаление
            RemoveMaterial(material_modify.Id, false);
            
            request = factory.WithType(DatagramType.GetMaterialElement).WithDTOObject(new GetElementData(material_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<MaterialDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var material_store = t1.Result;
                    Assert.IsTrue(material_store.Deleted);
                }).Wait();

            RemoveMaterial(material_modify.Id, true);
            
            //чтение списка справочника
            var material1 = GenerateAndStoreMaterial();
            var material2 = GenerateAndStoreMaterial();
            var material3 = GenerateAndStoreMaterial();

            request = factory.WithType(DatagramType.GetMaterialNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var material_list = t1.Result;
                    Assert.AreEqual(material_list.Elements.Count, 3);
                }).Wait();

            RemoveMaterial(material1.Id, true);
            RemoveMaterial(material2.Id, true);
            RemoveMaterial(material3.Id, true);
            
            request = factory.WithType(DatagramType.GetMaterialNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var material_list = t1.Result;
                    Assert.AreEqual(material_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void OrderGroupTest()
        {
            var factory = new DatagramFactory();
            //сохранение
            var group_master = GenerateAndStoreOrderGroup();

            var request = factory.WithType(DatagramType.GetOrderGroupElement).WithDTOObject(new GetElementData(group_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var group_store = t1.Result;
                    Assert.AreEqual(group_master, group_store);
                }).Wait();

            var group_modify = GenerateOrderGroup();
            group_modify.Id = group_master.Id;
            //изменение
            StoreOrderGroup(group_modify);

            request = factory.WithType(DatagramType.GetOrderGroupElement).WithDTOObject(new GetElementData(group_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var group_store = t1.Result;
                    Assert.AreEqual(group_modify, group_store);
                }).Wait();
            //удаление
            RemoveOrderGroup(group_modify.Id);

            request = factory.WithType(DatagramType.GetOrderGroupElement).WithDTOObject(new GetElementData(group_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var group_store = t1.Result;
                    Assert.IsTrue(group_store.Deleted);
                }).Wait();

            RemoveOrderGroup(group_modify.Id, true);

            //чтение списка справочника
            var group1 = GenerateAndStoreOrderGroup();
            var group2 = GenerateAndStoreOrderGroup();
            var group3 = GenerateAndStoreOrderGroup();

            request = factory.WithType(DatagramType.GetOrderGroupNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var group_list = t1.Result;
                    Assert.AreEqual(group_list.Elements.Count, 3);
                }).Wait();

            RemoveOrderGroup(group1.Id, true);
            RemoveOrderGroup(group2.Id, true);
            RemoveOrderGroup(group3.Id, true);

            request = factory.WithType(DatagramType.GetOrderGroupNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var group_list = t1.Result;
                    Assert.AreEqual(group_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void OrderOperationTest()
        {
            var factory = new DatagramFactory();

            var operation = GenerateAndStoreOperation();
            var post = GenerateAndStorePost();
            var group = GenerateAndStoreUserGroup();

            var employee = GenerateAndStoreEmployee(post.Id, group.Id);
            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            //сохранение
            var operation_master = GenerateAndStorOrderOperation(operation.Id, employee.Id, order.Id);


            var request = factory.WithType(DatagramType.GetOrderOperationElement).WithDTOObject(new GetElementData(operation_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderOperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.AreEqual(operation_master, operation_store);
                }).Wait();

            var operation_modify = GenerateOrderOperation(operation.Id, employee.Id, order.Id);
            operation_modify.Id = operation_master.Id;
            //изменение
            SyncStoreOrderOperation(operation_modify);

            request = factory.WithType(DatagramType.GetOrderOperationElement).WithDTOObject(new GetElementData(operation_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderOperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.AreEqual(operation_modify, operation_store);
                }).Wait();
            //удаление
            RemoveOrderOperation(operation_modify.Id);

            request = factory.WithType(DatagramType.GetOrderOperationElement).WithDTOObject(new GetElementData(operation_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderOperationDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_store = t1.Result;
                    Assert.IsTrue(operation_store.Deleted);
                }).Wait();

            RemoveOrderOperation(operation_modify.Id, true);
            //чтение списка
            var operation1 = GenerateAndStorOrderOperation(operation.Id, employee.Id, order.Id);
            var operation2 = GenerateAndStorOrderOperation(operation.Id, employee.Id, order.Id);
            var operation3 = GenerateAndStorOrderOperation(operation.Id, employee.Id, order.Id);

            request = factory.WithType(DatagramType.GetOrderOperationList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_list = t1.Result;
                    Assert.AreEqual(operation_list.Elements.Count, 3);
                }).Wait();

            RemoveOrderOperation(operation1.Id, true);
            RemoveOrderOperation(operation2.Id, true);
            RemoveOrderOperation(operation3.Id, true);

            request = factory.WithType(DatagramType.GetOrderOperationList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var operation_list = t1.Result;
                    Assert.AreEqual(operation_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void LimitMaterialTest()
        {
            var factory = new DatagramFactory();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var material1 = GenerateAndStoreMaterial();
            var material2 = GenerateAndStoreMaterial();

            var limitCardMaterial_master = GenerateLimitCardMaterial(material1.Id, order.Id);
            var factMaterial1 = GenerateLimitCardFactMaterial(limitCardMaterial_master.Id, limitCardMaterial_master.MaterialId);
            var factMaterial2 = GenerateLimitCardFactMaterial(limitCardMaterial_master.Id, material2.Id);

            limitCardMaterial_master.FactMaterials.Add(factMaterial1);
            limitCardMaterial_master.FactMaterials.Add(factMaterial2);

            //сохранение
            SyncStoreLimitMaterial(limitCardMaterial_master);

            var request = factory.WithType(DatagramType.GetLimitMaterialList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var data = t1.Result;
                    Assert.AreEqual(data.Elements.Count, 1);

                    var limitCardMaterial_store = data.Elements[0] as LimitCardMaterialDTO;

                    Assert.AreEqual(limitCardMaterial_master, limitCardMaterial_store);
                }).Wait();

            //изменение
            var limitCardMaterial_modify = GenerateLimitCardMaterial(material1.Id, order.Id);
            limitCardMaterial_modify.Id = limitCardMaterial_master.Id;

            SyncStoreLimitMaterial(limitCardMaterial_modify);

            //удаление
            RemoveLimitMaterial(limitCardMaterial_modify.Id, true);

            //чтение списка
            var limitCardMaterial1 = GenerateAndStoreLimitMaterial(material1.Id, order.Id);
            var limitCardMaterial2 = GenerateAndStoreLimitMaterial(material1.Id, order.Id);
            var limitCardMaterial3 = GenerateAndStoreLimitMaterial(material1.Id, order.Id);

            request = factory.WithType(DatagramType.GetLimitMaterialList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var limitCardMaterial_list = t1.Result;
                    Assert.AreEqual(limitCardMaterial_list.Elements.Count, 3);
                }).Wait();

            RemoveLimitMaterial(limitCardMaterial1.Id, true);
            RemoveLimitMaterial(limitCardMaterial2.Id, true);
            RemoveLimitMaterial(limitCardMaterial3.Id, true);


            request = factory.WithType(DatagramType.GetLimitMaterialList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var limitCardMaterial_list = t1.Result;
                    Assert.AreEqual(limitCardMaterial_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void LimitOperationTest()
        {
            var factory = new DatagramFactory();
            DatagramBase request = null;

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var operation = GenerateAndStoreOperation();

            //сохранение
            var limitCardOperation_master = GenerateAndStoreLimitOperation(operation.Id, order.Id);

            request = factory.WithType(DatagramType.GetLimitOperationList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var data = t1.Result;
                    Assert.AreEqual(data.Elements.Count, 1);

                    var limitCardOperation_store = data.Elements[0] as LimitCardOperationDTO;

                    Assert.AreEqual(limitCardOperation_master, limitCardOperation_store);
                }).Wait();

            var limitCardOperation_modify = GenerateLimitCardOperation(operation.Id, order.Id);
            limitCardOperation_modify.Id = limitCardOperation_master.Id;
            //изменение
            SyncStoreLimitOperation(limitCardOperation_modify);

            //удаление
            RemoveLimitOperation(limitCardOperation_modify.Id, true);
            
            //чтение списка
            var limitCardOperation1 = GenerateAndStoreLimitOperation(operation.Id, order.Id);
            var limitCardOperation2 = GenerateAndStoreLimitOperation(operation.Id, order.Id);
            var limitCardOperation3 = GenerateAndStoreLimitOperation(operation.Id, order.Id);

            request = factory.WithType(DatagramType.GetLimitOperationList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var limitCardOperation_list = t1.Result;
                    Assert.AreEqual(limitCardOperation_list.Elements.Count, 3);
                }).Wait();

            RemoveLimitOperation(limitCardOperation1.Id, true);
            RemoveLimitOperation(limitCardOperation2.Id, true);
            RemoveLimitOperation(limitCardOperation3.Id, true);

            request = factory.WithType(DatagramType.GetLimitOperationList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.ORDER_ID, order.Id.ToString()).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var limitCardOperation_list = t1.Result;
                    Assert.AreEqual(limitCardOperation_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void UserGroupTest()
        {
            var factory = new DatagramFactory();

            //сохранение
            var user_group_master = GenerateAndStoreUserGroup();

            var request = factory.WithType(DatagramType.GetUserGroupElement).WithDTOObject(new GetElementData(user_group_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<UserGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var user_group_store = t1.Result;
                    Assert.AreEqual(user_group_master, user_group_store);
                }).Wait();

            //изменение
            var user_group_modify = GenerateUserGroup();
            user_group_modify.Id = user_group_master.Id;
            StoreUserGroup(user_group_modify);

            request = factory.WithType(DatagramType.GetUserGroupElement).WithDTOObject(new GetElementData(user_group_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<UserGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var user_group_store = t1.Result;
                    Assert.AreEqual(user_group_modify, user_group_store);
                }).Wait();
            //удаление
            RemoveUserGroup(user_group_modify.Id, false);

            request = factory.WithType(DatagramType.GetUserGroupElement).WithDTOObject(new GetElementData(user_group_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<UserGroupDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var user_group_store = t1.Result;
                    Assert.IsTrue(user_group_store.Deleted);
                }).Wait();

            RemoveUserGroup(user_group_modify.Id, true);

            //чтение списка справочника
            var user_group1 = GenerateAndStoreUserGroup();
            var user_group2 = GenerateAndStoreUserGroup();
            var user_group3 = GenerateAndStoreUserGroup();

            request = factory.WithType(DatagramType.GetUserGroupNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var user_group_list = t1.Result;
                    Assert.AreEqual(user_group_list.Elements.Count, 3);
                }).Wait();

            RemoveUserGroup(user_group1.Id, true);
            RemoveUserGroup(user_group2.Id, true);
            RemoveUserGroup(user_group3.Id, true);

            request = factory.WithType(DatagramType.GetUserGroupNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                WithProperty(Constants.CACHE_VERSION, "0").
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var user_group_list = t1.Result;
                    Assert.AreEqual(user_group_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void OrderTest()
        {
            var factory = new DatagramFactory();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();

            //сохранение
            var order_master = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            var request = factory.WithType(DatagramType.GetOrderElement).WithDTOObject(new GetElementData(order_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_store = t1.Result;
                    Assert.AreEqual(order_master, order_store);
                }).Wait();

            //изменение
            var order_modify = GenerateOrder(customer.Id, null, orderGroup.Id);
            order_modify.Id = order_master.Id;
            SyncStoreOrder(order_modify);

            request = factory.WithType(DatagramType.GetOrderElement).WithDTOObject(new GetElementData(order_modify.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_store = t1.Result;
                    Assert.AreEqual(order_modify, order_store);
                }).Wait();
            //удаление
            RemoveOrder(order_modify.Id, false);
            
            request = factory.WithType(DatagramType.GetOrderElement).WithDTOObject(new GetElementData(order_master.Id)).Build();
            _dataManager.ExcuteRequestAsync<OrderDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_store = t1.Result;
                    Assert.IsTrue(order_store.Deleted);
                }).Wait();

            RemoveOrder(order_modify.Id, true);

            //чтение списка справочника
            var order1 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var order2 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var order3 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            request = factory.WithType(DatagramType.GetOrderList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_list = t1.Result;
                    Assert.AreEqual(order_list.Elements.Count, 3);
                }).Wait();

            RemoveOrder(order1.Id, true);
            RemoveOrder(order2.Id, true);
            RemoveOrder(order3.Id, true);
            
            request = factory.WithType(DatagramType.GetOrderList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                Build();
            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_list = t1.Result;
                    Assert.AreEqual(order_list.Elements.Count, 0);
                }).Wait();
        }

        [TestMethod]
        public void PlanTest()
        {
            var factory = new DatagramFactory();

            var customer = GenerateAndStoreCustomer();

            var orderGroup = GenerateAndStoreOrderGroup();

            //чтение списка справочника
            var order1 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var order2 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);
            var order3 = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            var request = factory.WithType(DatagramType.GetPlanElement).WithDTOObject(new GetPlanElement(order1.Date.Month, order1.Date.Year)).Build();
            _dataManager.ExcuteRequestAsync<SetPlanElement>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var data = t1.Result;
                    Assert.AreEqual(data.Elements.Count, 3);
                }).Wait();
        }

        [TestMethod]
        public void FileTest()
        {
            var factory = new DatagramFactory();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            var gen_dir = $@"{AppDomain.CurrentDomain.BaseDirectory}\filesGenerate\";
            if (!Directory.Exists(gen_dir))
                Directory.CreateDirectory(gen_dir);

            var fileName = $@"{gen_dir}{Guid.NewGuid()}.dat";
            File.WriteAllBytes(fileName, GetByteArray(SMALL_FILE));
            var fileId = FileHelper.SaveFile(fileName, order.Id, _dataManager, CancellationToken.None);

            order.FileId = fileId;
            SyncStoreOrder(order);

            var path = FileHelper.LoadFile(order.FileId, _dataManager, CancellationToken.None).Result;

            var store_file = File.ReadAllBytes(path);
            var master_file = File.ReadAllBytes(fileName);
            
            Assert.IsTrue(store_file.SequenceEqual(master_file));

            //очистка
            RemoveOrder(order.Id, true);
            RemoveOrderGroup(orderGroup.Id, true);
            
            if (Directory.Exists(gen_dir))
                (new DirectoryInfo(gen_dir)).Delete(true);

            var store_dir = $@"{AppDomain.CurrentDomain.BaseDirectory}\files\";
            if (Directory.Exists(store_dir))
                (new DirectoryInfo(store_dir)).Delete(true);
        }

        [TestMethod]
        public void UsersTest()
        {
            var factory = new DatagramFactory();

            var post = GenerateAndStorePost();
            var userGroup = GenerateAndStoreUserGroup();

            var employee1 = GenerateAndStoreEmployee(post.Id, userGroup.Id);
            var employee2 = GenerateAndStoreEmployee(post.Id, userGroup.Id);
            var employee3 = GenerateAndStoreEmployee(post.Id, userGroup.Id);

            var request = factory.WithType(DatagramType.GetUsersList).WithDTOObject(new GetListData(0, int.MaxValue, true)).Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.AreEqual(t1.Result.Elements.Count, 4);  //3 добавленных + администратор
                }).Wait();

            RemoveEmployee(employee1.Id, false);
            RemoveEmployee(employee2.Id, false);
            RemoveEmployee(employee3.Id, false);

            request = factory.WithType(DatagramType.GetUsersList).WithDTOObject(new GetListData(0, int.MaxValue, true)).Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    Assert.AreEqual(t1.Result.Elements.Count, 1);  //администратор
                }).Wait();
        }

        [TestMethod]
        public void LoginTest()
        {
            var factory = new DatagramFactory();
            DatagramBase request = null;

            var post = GenerateAndStorePost();
            var userGroup = GenerateAndStoreUserGroup();
            var employee1 = GenerateAndStoreEmployee(post.Id, userGroup.Id);

            request = factory.WithType(DatagramType.UserLogin).WithDTOObject(new GetLoginElement(employee1.Id, employee1.Password)).Build();

            _dataManager.ExcuteRequestAsync<SecurityContext>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var result = t1.Result;
                    Assert.IsTrue(result.UserId == employee1.Id && employee1.Id != Guid.Empty);
                }).Wait();

            using (MD5 md5Hash = MD5.Create())
            {
                request = factory.WithType(DatagramType.UserLogin).WithDTOObject(new GetLoginElement(employee1.Id, SecurityHelper.GetMd5Hash(md5Hash, GetRandomString(STRING_FIELD_LENGTH)))).Build();
            }

            _dataManager.ExcuteRequestAsync<SecurityContext>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var result = t1.Result;
                    Assert.IsFalse(result.UserId == employee1.Id && employee1.Id != Guid.Empty);
                }).Wait();
        }
    }
}