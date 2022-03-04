using MetalClient.Client;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using MetalTransport.Datagram.Security;
using MetalTransport.Datagram.GetListData;

namespace MetalClient.DataManager
{
    /// <summary>
    /// Менеджер для работы с данными, содержит КЕШи словарей, 
    /// загружает словари,
    /// фасад для исполнения запросов
    /// </summary>
    public sealed class ClientDataManager
    {
        private ClientRequestExecutorHandler _executor;

        public const int CACHE_COUNT = 6;

        public IClient Client { get; }

        private ManualResetEventSlim _lockMaterial = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockOperation = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockCustomer = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockEmployee = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockOrderGroup = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockUserGroup = new ManualResetEventSlim(true);
        private ManualResetEventSlim _lockOrder = new ManualResetEventSlim(true);

        private long _materialCacheVersion = -1;
        private long _operationCacheVersion = -1;
        private long _customerCacheVersion = -1;
        private long _employeeCacheVersion = -1;
        private long _orderGroupCacheVersion = -1;
        private long _userGroupCacheVersion = -1;

        private ConcurrentDictionary<Guid, VersionListItemDTO> _materialNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, VersionListItemDTO> _operationNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, VersionListItemDTO> _customerNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, VersionListItemDTO> _employeeNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, VersionListItemDTO> _orderGroupNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, VersionListItemDTO> _userGroupNameCache = new ConcurrentDictionary<Guid, VersionListItemDTO>();
        private ConcurrentDictionary<Guid, BaseListItemDTO> _orderCache = new ConcurrentDictionary<Guid, BaseListItemDTO>();

        private ConcurrentDictionary<Guid, long> _ordersVersions = new ConcurrentDictionary<Guid, long>();

        public SecurityContext SecurityContext { get; set; }

        public ConcurrentDictionary<Guid, VersionListItemDTO> MaterialNameCache
        {
            get
            {
                _lockMaterial.Wait();
                return _materialNameCache;
            }
        }

        public ConcurrentDictionary<Guid, VersionListItemDTO> OperationNameCache
        {
            get
            {
                _lockOperation.Wait();
                return _operationNameCache;
            }
        }

        public ConcurrentDictionary<Guid, VersionListItemDTO> CustomerNameCache
        {
            get
            {
                _lockCustomer.Wait();
                return _customerNameCache;
            }
        }

        public ConcurrentDictionary<Guid, VersionListItemDTO> EmployeeNameCache
        {
            get
            {
                _lockEmployee.Wait();
                return _employeeNameCache;
            }
        }

        public ConcurrentDictionary<Guid, VersionListItemDTO> OrderGroupNameCache
        {
            get
            {
                _lockOrderGroup.Wait();
                return _orderGroupNameCache;
            }
        }

        public ConcurrentDictionary<Guid, VersionListItemDTO> UserGroupNameCache
        {
            get
            {
                _lockUserGroup.Wait();
                return _userGroupNameCache;
            }
        }

        public ObservableCollection<BaseListItemDTO> UnitsCache
        {
            get
            {
                return new ObservableCollection<BaseListItemDTO>()
                {
                    new BaseListItemDTO() { Name = "шт" },
                    new BaseListItemDTO() { Name = "кг" },
                    new BaseListItemDTO() { Name = "м2" },
                    new BaseListItemDTO() { Name = "пог. м" },

                    new BaseListItemDTO() { Name = "м" },
                    new BaseListItemDTO() { Name = "л" },
                    new BaseListItemDTO() { Name = "г" },
                    new BaseListItemDTO() { Name = "м3" },
                };
            }
        }

        public ConcurrentDictionary<Guid, BaseListItemDTO> OrderCache
        {
            get
            {
                _lockOrder.Wait();
                return _orderCache;
            }
        }

        public ConcurrentDictionary<Guid, long> OrdersVersions
        {
            get
            {
                _lockOrder.Wait();
                return _ordersVersions;
            }
        }


        public ClientDataManager(IClient client)
        {
            Client = client;
            SecurityContext = new SecurityContext();
            _executor = new ClientRequestExecutorHandler(Client);
        }

        public Task InitMaterialCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetMaterialNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _materialCacheVersion))
                                 .Build();
            
            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);

            return task.ContinueWith((t1) =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_materialCacheVersion >= actualVersion) return;

                _lockMaterial.Reset();
                Parallel.ForEach(datagram.Elements, (item) =>
                {
                    if (item.Deleted)
                        _materialNameCache.TryRemove(item.Id, out var _);
                    else
                        _materialNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                });
                _lockMaterial.Set();

                _materialCacheVersion = actualVersion;
            }, token);
        }

        public Task InitOperationCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetOperationNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _operationCacheVersion))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);
            return task.ContinueWith((t1) =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_operationCacheVersion >= actualVersion) return;

                _lockOperation.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                            _operationNameCache.TryRemove(item.Id, out var _);
                        else
                            _operationNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                    });
                }
                finally
                {
                    _lockOperation.Set();
                }

                _operationCacheVersion = actualVersion;
            }, token);
        }

        public Task InitCustomerCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetCustomerNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _customerCacheVersion))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);
            return task.ContinueWith((t1) =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_customerCacheVersion >= actualVersion) return;

                _lockCustomer.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                            _customerNameCache.TryRemove(item.Id, out var _);
                        else
                            _customerNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                    });
                }
                finally
                {
                    _lockCustomer.Set();
                }

                _customerCacheVersion = actualVersion;
            }, token);
        }

        public Task InitEmployeerCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetEmployeeNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _employeeCacheVersion))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);
            return task.ContinueWith((t1) =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_employeeCacheVersion >= actualVersion) return;

                _lockEmployee.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                            _employeeNameCache.TryRemove(item.Id, out var _);
                        else
                            _employeeNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                    });
                }
                finally
                {
                    _lockEmployee.Set();
                }

                _employeeCacheVersion = actualVersion;
            }, token);
        }

        public Task InitOrderGroupCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetOrderGroupNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _orderGroupCacheVersion))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);
            return task.ContinueWith((t1) =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_orderGroupCacheVersion >= actualVersion) return;

                _lockOrderGroup.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                            _orderGroupNameCache.TryRemove(item.Id, out var _);
                        else
                            _orderGroupNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                    });
                }
                finally
                {
                    _lockOrderGroup.Set();
                }

                _orderGroupCacheVersion = actualVersion;
            }, token);
        }

        public Task InitUserGroupCache(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetUserGroupNameList)
                                 .WithDTOObject(new GetAllVersionElementsList(false, _userGroupCacheVersion))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<VersionListItemDTO>>(request, token);
            return task.ContinueWith(t1 =>
            {
                var datagram = t1.Result;

                long actualVersion = datagram.CacheVersion;

                if (_userGroupCacheVersion >= actualVersion) return;

                _lockUserGroup.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                            _userGroupNameCache.TryRemove(item.Id, out var _);
                        else
                            _userGroupNameCache.AddOrUpdate(item.Id, item, (k, v) => item);
                    });
                }
                finally
                {
                    _lockUserGroup.Set();
                }

                _userGroupCacheVersion = actualVersion;
            }, token);
        }

        public Task GetOrders(int pageIndex, int pageSize, CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetOrdersNext)
                                 .WithDTOObject(new GetPaginationElementsList(pageIndex, pageSize, false, OrderDTO.ListSort))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<OrderListItemDTO>>(request, token);
            return task.ContinueWith(t1 =>
            {
                var datagram = t1.Result;

                _lockOrder.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                        {
                            _orderCache.TryRemove(item.Id, out var _);
                            _ordersVersions.TryRemove(item.Id, out var _);
                        }
                        else
                        {
                            _orderCache.AddOrUpdate(item.Id, item, (k, v) => item);
                            _ordersVersions.AddOrUpdate(item.Id, item.Version, (k, v) => item.Version);
                        }
                    });
                }
                finally
                {
                    _lockOrder.Set();
                }
            }, token);
        }

        public Task RefreshOrders(int pageIndex, int pageSize, CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetOrdersActual)
                                 .WithDTOObject(new GetActualVersionElementsList(pageIndex, pageSize, true, OrderDTO.ListSort, OrdersVersions))
                                 .Build();

            var task = _executor.ExcuteRequestAsync<SetListData<OrderListItemDTO>>(request, token);
            return task.ContinueWith(t1 =>
            {
                var datagram = t1.Result;

                _lockOrder.Reset();
                try
                {
                    Parallel.ForEach(datagram.Elements, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
                    {
                        if (item.Deleted)
                        {
                            _orderCache.TryRemove(item.Id, out var _);
                            _ordersVersions.TryRemove(item.Id, out var _);
                        }
                        else
                        {
                            _orderCache.AddOrUpdate(item.Id, item, (k, v) => item);
                            _ordersVersions.AddOrUpdate(item.Id, item.Version, (k, v) => item.Version);
                        }
                    });
                }
                finally
                {
                    _lockOrder.Set();
                }
            }, token);
        }

        public Task<T> ExcuteRequestAsync<T>(DatagramBase request, CancellationToken token) 
            where T: BaseDTO
        {
            return _executor.ExcuteRequestAsync<T>(request, token);
        }
    }
}
