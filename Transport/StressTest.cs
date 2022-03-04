using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetalClient.Client;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using MetalClient.DataManager;
using Transport;
using System.Threading;
using System.IO;
using MetalClient.Helper;
using MetalTransport.Datagram.Properties;
using System.Diagnostics;

namespace StressTest
{
    [TestClass]
    public class StressTest
        : TestBase
    {
        //Настройки стресс теста заказ
        private const int CLIENT_COUNT = 5;

        private const int ORDER_COUNT = 10;
        private const int ORDER_TIMEOUT_MS = 100;
        
        private const int LIMIT_OPERATION_COUNT = 10;
        private const int LIMIT_MATERIAL_COUNT = 10;
        private const int ORDER_OPERATIONS_COUNT = 10;

        private const int EMPLOYEES_COUNT = 3;
        private const int OPERATIONS_COUNT = 10;
        private const int MATERIALS_COUNT = 10;

        //Настройки стресс теста справочник клиенты
        private const int CUSTOMER_COUNT = 10_000;
        private const int CUSTOMER_TIMEOUT_MS = 100;

        //Настройки стресс теста файлы
        private const int FILE_COUNT = 10;

        //тест падения сервера и клиента
        private const int SERVER_DROP_TIMEOUT_MS = 10_000;
        private const int CLIENT_DROP_TIMEOUT_MS = 200;
        private const int DROP_TEST_TIMEOUT = 60_000;

        [TestMethod]
        public void OrderStressTest()
        {
            var factory = new DatagramFactory();
            List<OrderDTO> orders = new List<OrderDTO>();
            
            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();

            for (int index = 0; index < ORDER_COUNT; index++)
                orders.Add(GenerateOrder(customer.Id, null, orderGroup.Id));

            var tasks = new List<Task>();
            for (int index = 0; index < orders.Count; index++)
            {
                tasks.Add(AsyncStoreOrder(orders[index]));
                Task.Delay(ORDER_TIMEOUT_MS);
            }

            Task.WaitAll(tasks.ToArray());

            var request = factory.WithType(DatagramType.GetOrderList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_list = t1.Result.Response;
                    Assert.AreEqual(order_list.Elements.Count, ORDER_COUNT);

                    foreach (var order in orders)
                    {
                        var elements = order_list.Elements;
                        Assert.IsTrue(elements.Any(elm => elm.Id.Equals(order.Id)));

                        var storeOrder = elements.First(elm => elm.Id.Equals(order.Id)) as OrderDTO;
                        Assert.IsNotNull(storeOrder);

                        Assert.AreEqual(storeOrder, order);
                    }
                }).Wait();

            //генерируем справочник операций
            var operations = new List<OperationDTO>();
            for (int operationIndex = 0; operationIndex < OPERATIONS_COUNT; operationIndex++)
            {
                var operation = GenerateOperation();
                operations.Add(operation);

                StoreOperation(operation);
            }

            //генерируем справочник материалов
            var materials = new List<MaterialDTO>();
            for (int materialIndex = 0; materialIndex < MATERIALS_COUNT; materialIndex++)
            {
                var material = GenerateMaterial();
                materials.Add(material);

                StoreMaterial(material);
            }

            //элемент справочника групп пользователей
            var group = GenerateAndStoreUserGroup();
            //элемент справочника должностей
            var post = GenerateAndStorePost();

            //генерируем справочник сотрудников
            var employees = new List<EmployeeDTO>();
            for (int employeeIndex = 0; employeeIndex < EMPLOYEES_COUNT; employeeIndex++)
            {
                var employee = GenerateEmployee(post.Id, group.Id);
                employees.Add(employee);

                StoreEmployee(employee);
            }

            //добавление материалов, работ и фактически выполняемых работ
            for (int orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                var employeeIndex = 0;
                var operationIndex = 0;
                var materialIndex = 0;

                operationIndex = 0;
                for (var limitOperationIndex = 0; limitOperationIndex < LIMIT_OPERATION_COUNT; limitOperationIndex++)
                {
                    SyncStoreLimitOperation(GenerateLimitCardOperation(operations[operationIndex].Id, orders[orderIndex].Id));

                    operationIndex++;
                    if (operationIndex == OPERATIONS_COUNT)
                        operationIndex = 0;
                }

                materialIndex = 0;
                for (var limitMaterialIndex = 0; limitMaterialIndex < LIMIT_MATERIAL_COUNT; limitMaterialIndex++)
                {
                    SyncStoreLimitMaterial(GenerateLimitCardMaterial(materials[materialIndex].Id, orders[orderIndex].Id));
                    
                    materialIndex++;
                    if (materialIndex == MATERIALS_COUNT)
                        materialIndex = 0;
                }

                employeeIndex = 0;
                operationIndex = 0;
                for (var orderOperationIndex = 0; orderOperationIndex < ORDER_OPERATIONS_COUNT; orderOperationIndex++)
                {
                    SyncStoreOrderOperation(GenerateOrderOperation(operations[operationIndex].Id, employees[employeeIndex].Id, orders[orderIndex].Id));

                    operationIndex++;
                    if (operationIndex == OPERATIONS_COUNT)
                        operationIndex = 0;

                    employeeIndex++;
                    if (employeeIndex == EMPLOYEES_COUNT)
                        employeeIndex = 0;
                }
            }

            //чтение и проверка
            for (int orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                request = factory.WithType(DatagramType.GetLimitMaterialList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == LIMIT_MATERIAL_COUNT);
                    });

                request = factory.WithType(DatagramType.GetLimitOperationList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == LIMIT_OPERATION_COUNT);
                    });

                request = factory.WithType(DatagramType.GetOrderOperationList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == ORDER_OPERATIONS_COUNT);
                    });
            }
        }

        [TestMethod]
        public void CustomerStressTest()
        {
            var factory = new DatagramFactory();
            List<CustomerDTO> customers = new List<CustomerDTO>();

            for (int index = 0; index < CUSTOMER_COUNT; index++)
                customers.Add(GenerateCustomer());

            var tasks = new List<Task>();
            for (int index = 0; index < customers.Count; index++)
            {
                tasks.Add(AsyncStoreCustomer(customers[index]));
                Task.Delay(CUSTOMER_TIMEOUT_MS);
            }

            Task.WaitAll(tasks.ToArray());

            var request = factory.WithType(DatagramType.GetCustomerNameList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var customer_list = t1.Result.Response;
                    Assert.AreEqual(customer_list.Elements.Count, CUSTOMER_COUNT);

                    foreach (var customer in customers)
                    {
                        var elements = customer_list.Elements;
                        Assert.IsTrue(elements.Any(elm => elm.Id.Equals(customer.Id)));

                        var storeCustomer = elements.First(elm => elm.Id.Equals(customer.Id)) as ElementNameDTO;
                        Assert.IsNotNull(storeCustomer);

                        Assert.IsTrue(storeCustomer.Name == customer.Name);
                    }
                }).Wait();
        }


        [TestMethod]
        public void OrderStressMultiClientTest()
        {
            var factory = new DatagramFactory();
            List<OrderDTO> orders = new List<OrderDTO>();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();

            for (int index = 0; index < ORDER_COUNT; index++)
                orders.Add(GenerateOrder(customer.Id, null, orderGroup.Id));

            var managers = new List<ClientDataManager>();
            managers.Add(_dataManager);

            for (int index = 1; index < CLIENT_COUNT; index++)
            {
                var client = new TcpMetalClient();
                client.Start();

                var manager = new ClientDataManager(client);

                managers.Add(manager);
            }

            var clientIndex = 0;
            var tasks = new List<Task>();
            for (int index = 0; index < orders.Count; index++)
            {
                tasks.Add(AsyncStoreOrder(orders[index], managers[clientIndex]));

                clientIndex++;
                if (clientIndex == CLIENT_COUNT)
                    clientIndex = 0;

                Task.Delay(ORDER_TIMEOUT_MS);
            }

            Task.WaitAll(tasks.ToArray());

            var request = factory.WithType(DatagramType.GetOrderList).
                WithDTOObject(new GetListData(0, int.MaxValue, true)).
                Build();

            _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    var order_list = t1.Result.Response;
                    Assert.AreEqual(order_list.Elements.Count, ORDER_COUNT);

                    foreach (var order in orders)
                    {
                        var elements = order_list.Elements;
                        Assert.IsTrue(elements.Any(elm => elm.Id.Equals(order.Id)));

                        var storeOrder = elements.First(elm => elm.Id.Equals(order.Id)) as OrderDTO;
                        Assert.IsNotNull(storeOrder);

                        Assert.AreEqual(storeOrder, order);
                    }
                }).Wait();

            //генерируем справочник операций
            var operations = new List<OperationDTO>();
            for (int operationIndex = 0; operationIndex < OPERATIONS_COUNT; operationIndex++)
            {
                var operation = GenerateOperation();
                operations.Add(operation);

                StoreOperation(operation);
            }

            //генерируем справочник материалов
            var materials = new List<MaterialDTO>();
            for (int materialIndex = 0; materialIndex < MATERIALS_COUNT; materialIndex++)
            {
                var material = GenerateMaterial();
                materials.Add(material);

                StoreMaterial(material);
            }

            //элемент справочника групп пользователей
            var group = GenerateAndStoreUserGroup();
            //элемент справочника должностей
            var post = GenerateAndStorePost();

            //генерируем справочник сотрудников
            var employees = new List<EmployeeDTO>();
            for (int employeeIndex = 0; employeeIndex < EMPLOYEES_COUNT; employeeIndex++)
            {
                var employee = GenerateEmployee(post.Id, group.Id);
                employees.Add(employee);

                StoreEmployee(employee);
            }

            tasks.Clear();
            //добавление материалов, работ и фактически выполняемых работ
            for (int orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                var employeeIndex = 0;
                var operationIndex = 0;
                var materialIndex = 0;

                operationIndex = 0;
                for (var limitOperationIndex = 0; limitOperationIndex < LIMIT_OPERATION_COUNT; limitOperationIndex++)
                {
                    tasks.Add(AsyncStoreLimitOperation(GenerateLimitCardOperation(operations[operationIndex].Id, orders[orderIndex].Id), managers[clientIndex]));

                    operationIndex++;
                    if (operationIndex == OPERATIONS_COUNT)
                        operationIndex = 0;
                }

                materialIndex = 0;
                for (var limitMaterialIndex = 0; limitMaterialIndex < LIMIT_MATERIAL_COUNT; limitMaterialIndex++)
                {
                    tasks.Add(AsyncStoreLimitMaterial(GenerateLimitCardMaterial(materials[materialIndex].Id, orders[orderIndex].Id), managers[clientIndex]));

                    materialIndex++;
                    if (materialIndex == MATERIALS_COUNT)
                        materialIndex = 0;
                }

                employeeIndex = 0;
                operationIndex = 0;
                for (var orderOperationIndex = 0; orderOperationIndex < ORDER_OPERATIONS_COUNT; orderOperationIndex++)
                {
                    tasks.Add(AsyncStoreOrderOperation(GenerateOrderOperation(operations[operationIndex].Id, employees[employeeIndex].Id, orders[orderIndex].Id), managers[clientIndex]));

                    operationIndex++;
                    if (operationIndex == OPERATIONS_COUNT)
                        operationIndex = 0;

                    employeeIndex++;
                    if (employeeIndex == EMPLOYEES_COUNT)
                        employeeIndex = 0;
                }

                clientIndex++;
                if (clientIndex == CLIENT_COUNT)
                    clientIndex = 0;

                Task.Delay(ORDER_TIMEOUT_MS);
            }
            Task.WaitAll(tasks.ToArray());

            //чтение и проверка
            for (int orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                request = factory.WithType(DatagramType.GetLimitMaterialList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == LIMIT_MATERIAL_COUNT);
                    });

                request = factory.WithType(DatagramType.GetLimitOperationList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == LIMIT_OPERATION_COUNT);
                    });

                request = factory.WithType(DatagramType.GetOrderOperationList).WithDTOObject(new GetListData(0, int.MaxValue, true)).WithProperty(Constants.ORDER_ID, orders[orderIndex].ToString()).Build();
                _dataManager.ExcuteRequestAsync<SetListData>(request, CancellationToken.None).
                    ContinueWith((t1) =>
                    {
                        var data = t1.Result.Response;
                        Assert.IsTrue(data.Elements.Count == ORDER_OPERATIONS_COUNT);
                    });
            }

            for (clientIndex = 1; clientIndex < CLIENT_COUNT; clientIndex++)
                managers[clientIndex].Client.Stop();
        }


        [TestMethod]
        public void FileStressMultiClientTest()
        {
            var factory = new DatagramFactory();
            List<OrderDTO> orders = new List<OrderDTO>();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();

            for (int index = 0; index < FILE_COUNT; index++)
                orders.Add(GenerateOrder(customer.Id, null, orderGroup.Id));

            var managers = new List<ClientDataManager>();
            managers.Add(_dataManager);

            for (int index = 1; index < CLIENT_COUNT; index++)
            {
                var client = new TcpMetalClient();
                client.Start();
                managers.Add(new ClientDataManager(client));
            }

            var clientIndex = 0;
            var tasks = new List<Task>();
            for (int index = 0; index < orders.Count; index++)
            {
                tasks.Add(AsyncStoreOrder(orders[index], managers[clientIndex]));

                clientIndex++;
                if (clientIndex == CLIENT_COUNT)
                    clientIndex = 0;

                Task.Delay(ORDER_TIMEOUT_MS);
            }

            Task.WaitAll(tasks.ToArray());

            clientIndex = 0;
            var gen_dir = $@"{AppDomain.CurrentDomain.BaseDirectory}\filesGenerate\";
            if (!Directory.Exists(gen_dir))
                Directory.CreateDirectory(gen_dir);

            foreach (var order in orders)
            {
                var fileName = $@"{gen_dir}{Guid.NewGuid()}.dat";
                File.WriteAllBytes(fileName, GetByteArray(BIG_FILE));
                var fileId = FileHelper.SaveFile(fileName, order.Id, managers[clientIndex], CancellationToken.None);

                order.FileId = fileId;
                AsyncStoreOrder(order, managers[clientIndex]);

                clientIndex++;
                if (clientIndex == CLIENT_COUNT)
                    clientIndex = 0;

                File.Delete(fileName);
                Task.Delay(ORDER_TIMEOUT_MS);
            }

            foreach (var order in orders)
            {
                var path = FileHelper.LoadFile(order.FileId, _dataManager, CancellationToken.None).Result;
                Assert.IsTrue((uint)File.ReadAllBytes(path).Count() == BIG_FILE);
                File.Delete(path);
            }

            for (clientIndex = 1; clientIndex < CLIENT_COUNT; clientIndex++)
                managers[clientIndex].Client.Stop();
        }

        //Два клиента поочереди читают и сохраняют один заказ
        //один запрос уходит в работающий сервер, второй в остановленнный
        //клиенты должны продолжать успешно получать заказ после старта сервера
        [TestMethod]
        public void DropServerStressTest()
        {
            var factory = new DatagramFactory();

            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            var client = new TcpMetalClient();
            client.Start();

            var secondManager = new ClientDataManager(client);
            var firstManager = _dataManager;

            var start = new Stopwatch();
            start.Start();
            var first = true;
            var stopped = false;
            while (start.ElapsedMilliseconds <= DROP_TEST_TIMEOUT)
            {
                ClientDataManager dataManager = null;
                if (first)
                    dataManager = firstManager;
                else
                    dataManager = secondManager;

                var cts = new CancellationTokenSource();
                var request = factory.WithType(DatagramType.GetOrderElement).WithDTOObject(new GetElementData(order.Id)).Build();
                var inTime = dataManager.ExcuteRequestAsync<OrderDTO>(request, cts.Token).
                    ContinueWith((t1) =>
                    {
                        if (_server.IsWork)
                        {
                            Assert.IsFalse(t1.IsFaulted);
                            Assert.IsFalse(t1.IsCanceled);
                            Assert.IsTrue(t1.IsCompleted);
                            Assert.IsNull(t1.Exception);
                        }
                        else
                        {
                            Assert.IsTrue(t1.IsFaulted || t1.IsCanceled);
                        }
                    }).Wait(2_000);


                //TODO оптимизация выполнения запроса
                //Assert.IsTrue(inTime); 

                if (!stopped)
                {
                    _server.StopServer();
                    stopped = true;
                }
                else
                {
                    _server.StartServer();
                    stopped = false;
                }

                Thread.Sleep(3_000);
            }

            if (!_server.IsWork)
                _server.StartServer();
        }


        [TestMethod]
        public void DropClientStressTest()
        {
            var factory = new DatagramFactory();
            var customer = GenerateAndStoreCustomer();
            var orderGroup = GenerateAndStoreOrderGroup();
            var order = GenerateAndStoreOrder(customer.Id, null, orderGroup.Id);

            var start = new Stopwatch();
            start.Start();
            while (start.ElapsedMilliseconds <= DROP_TEST_TIMEOUT)
            {
                var client = new TcpMetalClient();
                client.Start();

                var dataManager = new ClientDataManager(client);

                var cts = new CancellationTokenSource();
                var request = factory.WithType(DatagramType.GetOrderElement).WithDTOObject(new GetElementData(order.Id)).Build();
                var inTime = dataManager.ExcuteRequestAsync<OrderDTO>(request, cts.Token).
                    ContinueWith((t1) =>
                    {
                        Assert.IsFalse(t1.IsFaulted);
                        Assert.IsFalse(t1.IsCanceled);
                        Assert.IsTrue(t1.IsCompleted);
                        Assert.IsNull(t1.Exception);
                    }).Wait(1_000);

                Assert.IsTrue(inTime);

                client.Stop();
                Thread.Sleep(500);
            }

            if (!_server.IsWork)
                _server.StartServer();
        }
    }
}
