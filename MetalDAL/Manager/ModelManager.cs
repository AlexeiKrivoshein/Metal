using MetalDAL.Model;
using MetalDiagnostic.Logger;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data.Entity;
using System.Security.Cryptography;
using MetalTransport.Helper;
using System.Data;
using MetalDAL.ModelEx;
using log4net;
using MetalTransport;
using MetalCore.ModelEx;
using System.Text;
using MetalTransport.ModelEx.Enums;
using MetalTransport.Datagram.GetListData;

namespace MetalDAL.Manager
{
    public sealed class ModelManager
        : IStartable
    {
        private ILog _log = LogService.GetLogger(nameof(ModelManager));
        public string FilePath { get; set; }

        private const int DATABASE_VERSION = 3;
        private const string UNKNOWN_SET = "Не найдена коллекция элементов.";
        private const string UNKNOWN_ELEMENT = "В базе данных не найден элемент";

        private int _orderNumber = 1;

        private bool _isStarted = false;

        private Dictionary<int, string> _versionCommand = new Dictionary<int, string>
        {
            {2, "IF OBJECT_ID(N'LockedSet','U') IS NOT NULL DROP TABLE LockedSet" },
            {3, @"IF OBJECT_ID(N'MetalFileSet','U') IS NOT NULL
                BEGIN
                    DROP TABLE MetalFileSet

                    CREATE TABLE [dbo].[MetalFileSet] (
                    [Id] uniqueidentifier  NOT NULL,
                    [FileId] uniqueidentifier  NOT NULL,
                    [Path] nvarchar(max)  NOT NULL,
                    [Name] nvarchar(max)  NOT NULL,
                    [Index] int  NOT NULL,
                    [ChunkCount] int  NOT NULL,
                    [OrderId] uniqueidentifier  NOT NULL
	                );

                    ALTER TABLE[dbo].[MetalFileSet]
                        ADD CONSTRAINT[PK_MetalFileSet]
                    PRIMARY KEY CLUSTERED([Id] ASC);

                    ALTER TABLE[dbo].[MetalFileSet]
                        ADD CONSTRAINT[FK_MetalFileOrder]
                    FOREIGN KEY([OrderId])
                    REFERENCES[dbo].[OrderSet]
                        ([Id])
                   ON DELETE NO ACTION ON UPDATE NO ACTION;
                END;
                ALTER TABLE OrderSet ALTER COLUMN OrderGroupId uniqueidentifier NULL;"}
        };

        private ManualResetEventSlim _load = new ManualResetEventSlim(false);
        public VersionManager VersionManager { get; private set; }

        public ModelManager(string path)
        {
            FilePath = path;
        }

        public void Start()
        {
            if (_isStarted) _log.Warn("Уже запущен" + Environment.NewLine + Environment.StackTrace);

            _load.Reset();

            try
            {
                ValidateDataBase();
                InitVersions();
            }
            catch (Exception ex)
            {
                _log.Error($"Во время старта менеджера данных возникло исключение {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                throw;
            }
            finally
            {
                _load.Set();
            }
        }

        private void ValidateDataBase()
        {
            using (var container = new MetalEDMContainer())
            {
                container.Database.CreateIfNotExists();

                //изменим версию БД
                if (container.SysTabSet.Count() == 1)
                {
                    var modify = container.SysTabSet.First();

                    var oldVersion = modify.DataBaseVersion;

                    for (int version = ++oldVersion; version <= DATABASE_VERSION; version++)
                        ModifyDataBaseVersion(version, container);

                    if (modify.DataBaseVersion != DATABASE_VERSION)
                    {
                        modify.DataBaseVersion = DATABASE_VERSION;
                        container.SaveChanges();
                    }
                }
                else
                {
                    while (container.SysTabSet.Any())
                    {
                        var removed = container.SysTabSet.First();
                        container.SysTabSet.Remove(removed); ;
                    }

                    container.SysTabSet.Add(new SysTab() { Id = Guid.NewGuid(), DataBaseVersion = DATABASE_VERSION });
                    container.SaveChanges();
                }

                //добавим группу администраторов, если её нет
                var group = container.UserGroupSet.Find(SecurityHelper.AdministratorGroupId);
                if (group is null)
                {
                    group = new UserGroup();
                    {
                        //TODO список прав - как перечисление, границы перечисления определяются динамически
                        group.Name = "Администраторы";
                        group.Rights = new byte[13];

                        for (int i = 0; i < 13; i++)
                            group.Rights[i] = 2;

                        group.Id = SecurityHelper.AdministratorGroupId;
                    }

                    container.UserGroupSet.Add(group);
                    container.SaveChanges();
                }

                //добавим пользователя - администратора
                if (container.EmployeeSet.Any(g => g.Id == SecurityHelper.AdministratorId))
                {
                    var administrator = container.EmployeeSet.Find(SecurityHelper.AdministratorId);

                    if (string.IsNullOrEmpty(administrator.Secondname)) //фикс перенос наименования
                    {
                        administrator.Secondname = "Администратор";
                        administrator.Name = "";
                        administrator.Patronymic = "";
                        administrator.UserGroupId = group.Id;
                        administrator.UserGroup = group;

                        container.SaveChanges();
                    }
                }
                else
                {
                    Employee administrator = new Employee();
                    {
                        administrator.Id = SecurityHelper.AdministratorId;
                        administrator.Secondname = "Администратор";
                        administrator.Name = "";
                        administrator.Patronymic = "";
                        administrator.UserGroupId = group.Id;
                        administrator.UserGroup = group;

                        using (MD5 md5Hash = MD5.Create())
                            administrator.Password = SecurityHelper.GetMd5Hash(md5Hash, "123456");

                        administrator.UseForLogin = true;
                    }

                    container.EmployeeSet.Add(administrator);
                    container.SaveChanges();
                }
            }
        }

        private void InitVersions()
        {
            VersionManager = new VersionManager();
            VersionManager.AddVersionCache<Order>();

            using (var container = new MetalEDMContainer())
            {
                var version = container.MaterialSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<Material>(version);

                version = container.OperationSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<Operation>(version);

                version = container.CustomerSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<Customer>(version);

                version = container.EmployeeSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<Employee>(version);

                version = container.OrderGroupSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<OrderGroup>(version);

                version = container.UserGroupSet.Max(x => (long?)x.Version) ?? 0;
                VersionManager.SetVersion<UserGroup>(version);

                version = container.OrderSet.Max(o => (long?)o.Version) ?? 0;
                VersionManager.SetVersion<Order>(version);

                version = container.PostSet.Max(o => (long?)o.Version) ?? 0;
                VersionManager.SetVersion<Post>(version);

                //номер заказа
                _orderNumber = container.OrderSet.Where(o => !o.Deleted).Max(o => (int?)o.Number) ?? 0;
            }
        }



        /// <summary>
        /// Получение списка для справочных элементов ведущих версии
        /// </summary>
        public List<BaseDTO> GetModelElements<T>(GetAllVersionElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, out int count, out long version)
            where T : class, IVersionModelElement<T>, new()
        {
            return GetModelElements(data, order, null, null, out count, out version);
        }

        /// <summary>
        /// Получение списка для справочных элементов ведущих версии
        /// </summary>
        public List<BaseDTO> GetModelElements<T>(GetAllVersionElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, Func<IQueryable<T>, IQueryable<T>> where, out int count, out long version)
            where T : class, IVersionModelElement<T>, new()
        {
            return GetModelElements(data, order, where, null, out count, out version);
        }

        /// <summary>
        /// Получение списка для справочных элементов ведущих версии
        /// так как используются для отображения в выпадающих списках и формах выбора,
        /// то необходимо первый раз получать все, далее только изменненные, 
        /// т.е. с версией выше послденей полученной
        /// </summary>
        /// <typeparam name="T">Тип реализующий IVersionModelElement</typeparam>
        /// <param name="data">Данные-запрос списка</param>
        /// <param name="count">Общее количество в базе данных</param>
        /// <returns></returns>
        private List<BaseDTO> GetModelElements<T>(GetAllVersionElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, Func<IQueryable<T>, IQueryable<T>> where, Func<IQueryable<T>, IQueryable<T>>  include, out int count, out long version)
            where T: class, IVersionModelElement<T>, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _load.Wait();
            version = 0;
            count = 0;

            if (!VersionManager.TryGetActual<T>(out version))
                throw new ArgumentException($"Не удалось определить версию для типа {data.GetType()}");

            using (var context = new MetalEDMContainer())
            {
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                var query = set.Where(x => (!data.ExcludeDelete || !x.Deleted) && (x.Version > data.CacheVersion));

                if (where != null)
                    query = where(query);

                if (include != null)
                    query = include(query);

                var ordered = order(query);

                var elements = ordered.ToList();

                if (VersionManager.TryGetVersionCache<T>(out var cache))
                {
                    cache.SetObjectsVersions(elements.ToDictionary(x => x.Id, x => x.Version));
                }

                count = set.Count();

                return elements.Select(element => element.ToListItemDTO()).ToList();
            }
        }

        /// <summary>
        /// Получение списка заказов с пагинацией
        /// </summary>
        /// <typeparam name="T">Тип реализующий IVersionModelElement</typeparam>
        /// <param name="data">Данные-запрос списка</param>
        /// <param name="count">Общее количество в базе данных</param>
        /// <returns></returns>
        public List<BaseDTO> GetModelElements<T>(GetPaginationElementsList data, out int count)
            where T : class, IVersionModelElement<T>, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _load.Wait();
            count = 0;

            using (var context = new MetalEDMContainer())
            {
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                var query = set.Where(x => (!data.ExcludeDelete || !x.Deleted));

                if (query is IQueryable<Order> orders)
                {
                    query = OrderedFactory.GetOrdered<T>(orders, data.Sort);
                }
                else
                {
                    query = query.OrderBy(x => x.Id).AsQueryable();
                }

                var elements = query
                                .Skip(data.PageIndex * data.PageSize)
                                .Take(data.PageSize)
                                .ToList();

                var result = elements.Select(element => element.ToListItemDTO())
                                .ToList();

                count = set.Count();

                if(VersionManager.TryGetVersionCache<T>(out var cache))
                {
                    cache.SetObjectsVersions(elements.ToDictionary(x => x.Id, x => x.Version));
                }

                return result;
            }
        }



        /// <summary>
        /// Получение списка для элементов заказа (имеют свойства OrderId, Order)
        /// </summary>
        public List<BaseDTO> GetModelElements<T>(GetOrderIdFilteredElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, out int count)
            where T : class, IPartOfOrderModelElement<T>, new()
        {
            return GetModelElements<T>(data, order, null, null, out count);
        }

        /// <summary>
        /// Получение списка для элементов заказа (имеют свойства OrderId, Order)
        /// </summary>
        public List<BaseDTO> GetModelElements<T>(GetOrderIdFilteredElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, Func<IQueryable<T>, IQueryable<T>> where, out int count)
            where T : class, IPartOfOrderModelElement<T>, new()
        {
            return GetModelElements<T>(data, order, where, null, out count);
        }

        /// <summary>
        /// Получение списка для элементов заказа (имеют свойства OrderId, Order)
        /// </summary>
        /// <typeparam name="T">Тип реализующий IPartOfOrderModelElement</typeparam>
        /// <param name="data">Данные-запрос списка<</param>
        /// <param name="count">Общее количество в базе данных</param>
        /// <returns></returns>
        public List<BaseDTO> GetModelElements<T>(GetOrderIdFilteredElementsList data, Func<IQueryable<T>, IOrderedQueryable<T>> order, Func<IQueryable<T>, IQueryable<T>> where, Func<IQueryable<T>, IQueryable<T>> include, out int count)
            where T : class, IPartOfOrderModelElement<T>, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _load.Wait();

            count = 0;

            using (var context = new MetalEDMContainer())
            {
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                var query = set.Where(x => x.OrderId == data.OrderId);

                if (where != null)
                    query = where(query);

                if (include != null)
                    query = include(query);

                var ordered = order(query);

                count = set.Count();

                return ordered.ToList().Select(element => element.ToDTO()).ToList();
            }
        }


        /// <summary>
        /// Обновление списка элементов, ведущих версию
        /// </summary>
        public List<BaseDTO> GetModelElements<T>(GetActualVersionElementsList data, out int count)
            where T : class, IVersionModelElement<T>, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _load.Wait();
            count = 0;

            var cached = VersionManager.TryGetVersionCache<T>(out var cache);

            using (var context = new MetalEDMContainer())
            {
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                //получение изменных элементов
                List<T> updatedElements;
                if (cached)
                {
                    var updatedIds = cache.GetUpdatedIds(data.ObjectsVersions);
                    updatedElements = set.Where(x => updatedIds.Contains(x.Id))
                                    .ToList();
                }
                else
                {
                    updatedElements = new List<T>();
                }

                //получение новых элементов
                var query = set.Where(x => (!data.ExcludeDelete || !x.Deleted));
                if (query is IQueryable<Order> orders)
                {
                    query = OrderedFactory.GetOrdered<T>(orders, data.Sort);
                }
                else
                {
                    query = query.OrderBy(x => x.Id).AsQueryable();
                }

                var addedIds = query
                                .Take((data.PageIndex + 1) * data.PageSize)
                                .Select(x => x.Id)
                                .ToList();

                addedIds = addedIds.Except(data.ObjectsVersions.Keys).ToList();

                List<T> newElements = null;
                if (addedIds.Any())
                {
                    newElements = set.Where(x => addedIds.Contains(x.Id))
                                    .ToList();
                }
                else
                {
                    newElements = new List<T>();
                }

                var result = updatedElements.Concat(newElements).Select(element => element.ToListItemDTO())
                                .ToList();


                count = set.Count();

                if (cached)
                {
                    cache.SetObjectsVersions(newElements.ToDictionary(x => x.Id, x => x.Version));
                    cache.SetObjectsVersions(updatedElements.Where(x => !x.Deleted).ToDictionary(x => x.Id, x => x.Version));
                }

                return result;
            }
        }

        /// <summary>
        /// Получение отдельного элемента из базы данных в контексте 
        /// </summary>
        /// <param name="data">Данные-запрос элемента</param>
        /// <param name="type">Тип датаграммы</param>
        /// <returns></returns>
        public BaseDTO GetElement<T>(Guid id)
            where T : class, IModelElement<T>, new()
        {
            _load.Wait();
            using (var context = new MetalEDMContainer())
            {
                if (context is null)
                    throw new ArgumentNullException(nameof(context));

                //получение из базы
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                var element = set.Find(id);

                if (element is null)
                    throw new Exception($"{UNKNOWN_ELEMENT} [{id}]");

                if (element is IVersionModelElement<T> version && 
                    VersionManager.TryGetVersionCache<T>(out var cache))
                {
                    cache.SetObjectVersion(version.Id, version.Version);
                }

                element.LoadNested(context, this);

                return element.ToDTO();
            }
        }

        /// <summary>
        /// TODO: Получение плана
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PlanItemDTO> GetPlanElements(GetPlanElementsList data, out int count)
        {
            _load.Wait();

            if (data == null) 
                throw new ArgumentNullException(nameof(data));

            using (var context = new MetalEDMContainer())
            {
                var elements = context.OrderSet
                            .Where(x => x.Deleted == false && x.Date.Year == data.Year && x.Date.Month == data.Month)
                            .OrderByDescending(x => x.Date)
                            .ThenBy(x => x.Number)
                            .ToList()
                            .Select(x => x.ToPlanDTO())
                            .ToList();

                count = elements.Count;

                return elements;
            }
        }

        /// <summary>
        /// Сохранение/обновление заказа
        /// </summary>
        /// <typeparam name="T">Тип сохраняемого элемента</typeparam>
        /// <param name="element">Элемент</param>
        /// <param name="version">Версия (для типов ведущих версии, иначе -1)</param>
        public OrderHandledDTO SetElement(Order element)
        {
            _load.Wait();

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element.Number == 0)
            {
                element.Number = Interlocked.Increment(ref _orderNumber);
                element.DateRec = DateTime.UtcNow;
            }

            InnerSetElement(element);

            return new OrderHandledDTO()
            {
                Id = element.Id,
                Type = HandledType.OK,
                OrderNumber = element.Number
            };
        }

        /// <summary>
        /// Сохранение/обновление лимитки
        /// </summary>
        /// <typeparam name="T">Тип сохраняемого элемента</typeparam>
        /// <param name="element">Элемент</param>
        /// <param name="version">Версия (для типов ведущих версии, иначе -1)</param>
        public HandledDTO SetElement(LimitCardMaterial element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _load.Wait();

            using (var context = new MetalEDMContainer())
            {
                if (element.Id != Guid.Empty)
                {
                    var exists = context.LimitCardMaterialSet.Find(element.Id);
                    if (exists != null && exists.FactMaterials.Any())
                    {
                        context.LimitCardFactMaterialSet.RemoveRange(exists.FactMaterials);
                    }

                    if (!context.GetValidationErrors().Any(v => !v.IsValid))
                    {
                        context.SaveChanges();
                    }
                    else
                    {
                        var error = new StringBuilder();

                        foreach (var exception in context.GetValidationErrors())
                        {
                            foreach (var validation in exception.ValidationErrors)
                            {
                                error.AppendLine($"{validation.PropertyName} {validation.ErrorMessage}");
                            }
                        }

                        throw new Exception(error.ToString());
                    }
                }
            }

            return InnerSetElement(element);
        }

        /// <summary>
        /// Сохранение/обновление элемента БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">Элемент</param>
        /// <returns></returns>
        public HandledDTO SetElement<T>(T element)
            where T : class, IModelElement<T>
        {
            return InnerSetElement(element);
        }       

        /// <summary>
        /// Сохранение/обновление элемента
        /// </summary>
        /// <typeparam name="T">Тип сохраняемого элемента</typeparam>
        /// <param name="element">Элемент</param>
        /// <param name="version">Версия (для типов ведущих версии, иначе -1)</param>
        private HandledDTO InnerSetElement<T>(T element)
            where T: class, IModelElement<T>
        {
            _load.Wait();

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            using (var context = new MetalEDMContainer())
            {
                var set = context.Set<T>();

                if (set is null)
                    throw new Exception(UNKNOWN_SET);

                IVersionModelElement<T> versioning = null;
                if (element is IVersionModelElement<T>)
                {
                    versioning = element as IVersionModelElement<T>;
                    if (!VersionManager.TryGetNext<T>(out var version))
                        throw new ArgumentException($"Не удалось определить версию для типа {element.GetType()}");

                    versioning.Version = version;
                }

                element.LoadNested(context, this);

                var id = element.Id;

                if (id != Guid.Empty)
                {
                    var update = set.Find(id);

                    if (update != null)
                        context.Entry(update).CurrentValues.SetValues(element);
                    else
                        set.Add(element);
                }
                else
                {
                    element.Id = Guid.NewGuid();
                    set.Add(element);
                }

                if (!context.GetValidationErrors().Any(v => !v.IsValid))
                {
                    context.SaveChanges();

                    if(VersionManager.TryGetVersionCache<T>(out var cache))
                    {
                        cache.SetObjectVersion(versioning.Id, versioning.Version);
                    }
                }
                else
                {
                    var error = new StringBuilder();

                    foreach (var exception in context.GetValidationErrors())
                    {
                        foreach (var validation in exception.ValidationErrors)
                        {
                            error.AppendLine($"{validation.PropertyName} {validation.ErrorMessage}");
                        }
                    }

                    throw new Exception(error.ToString());
                }
            }

            return HandledDTO.Success(element.Id);
        }

        /// <summary>
        /// Удаление версионируемого элемента из банка данных
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public HandledDTO RemoveVersioningElement<T>(Guid id, bool permanent)
            where T : class, IVersionModelElement<T>
        {
            _load.Wait();

            using (var context = new MetalEDMContainer())
            {
                return RemoveVersioningElement<T>(id, context, permanent);
            }
        }

        /// <summary>
        /// Удаление версионируемого элемента из банка данных в контексте context
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <param name="permanent"></param>
        /// <returns></returns>
        public HandledDTO RemoveVersioningElement<T>(Guid id, MetalEDMContainer context, bool permanent)
            where T : class, IVersionModelElement<T>
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _load.Wait();

            var set = context.Set<T>();

            if (set is null)
                throw new Exception(UNKNOWN_SET);

            var element = set.Find(id);

            if (!VersionManager.TryGetNext<T>(out var version))
                throw new ArgumentException($"Не удалось определить версию для типа {element.GetType()}");

            element.RemoveNested(context, this, permanent);

            if (permanent)
            {
                set.Remove(element);
            }
            else
            {
                element.Version = version;
                element.Deleted = true;
            }

            if (!context.GetValidationErrors().Any(v => !v.IsValid))
            {
                context.SaveChanges();

                if (VersionManager.TryGetVersionCache<T>(out var cache))
                {
                    if (permanent)
                    {
                        cache.RemoveObjectVersion(element.Id);
                    }
                    else
                    {
                        cache.SetObjectVersion(element.Id, element.Version);
                    }
                }
            }
            else
            {
                var error = new StringBuilder();

                foreach (var exception in context.GetValidationErrors())
                {
                    foreach (var validation in exception.ValidationErrors)
                    {
                        error.AppendLine($"{validation.PropertyName} {validation.ErrorMessage}");
                    }
                }

                throw new Exception(error.ToString());
            }

            return HandledDTO.Success(id);
        }

        /// <summary>
        /// Удаление версионируемого элемента из банка данных
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public HandledDTO RemovePartOfOrderElement<T>(Guid id)
            where T : class, IPartOfOrderModelElement<T>
        {
            using (var context = new MetalEDMContainer())
            {
                return RemovePartOfOrderElement<T>(id, context);
            }
        }

        /// <summary>
        /// Удаление версионируемого элемента из банка данных в контексте context
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public HandledDTO RemovePartOfOrderElement<T>(Guid id, MetalEDMContainer context)
            where T : class, IPartOfOrderModelElement<T>
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _load.Wait();

            var set = context.Set<T>();

            if (set is null)
                throw new Exception(UNKNOWN_SET);

            var element = set.Find(id);

            element.RemoveNested(context, this, true);
            set.Remove(element);

            if (!context.GetValidationErrors().Any(v => !v.IsValid))
            {
                context.SaveChanges();
            }
            else
            {
                var error = new StringBuilder();

                foreach (var exception in context.GetValidationErrors())
                {
                    foreach (var validation in exception.ValidationErrors)
                    {
                        error.AppendLine($"{validation.PropertyName} {validation.ErrorMessage}");
                    }
                }

                throw new Exception(error.ToString());
            }

            return HandledDTO.Success(id);
        }

        /// <summary>
        /// Получить отфильтрованные данные
        /// </summary>
        /// <param name="filter">Параметры фильтрации</param>
        /// <param name="count">Количество отбранных данных</param>
        /// <returns></returns>
        public List<BaseDTO> GetFiltered(FilterDTO filter, out int count)
        {
            _load.Wait();

            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            using (var context = new MetalEDMContainer())
            {
                count = context.OrderSet.Count(x => x.Deleted == false);

                if (filter.DateTo != Constants.EMPTY_DATETIME)
                {
                    filter.DateTo = ToEndDate(filter.DateTo);
                }
                if (filter.ReadyDateTo != Constants.EMPTY_DATETIME)
                {
                    filter.ReadyDateTo = ToEndDate(filter.ReadyDateTo);
                }

                var query = context.OrderSet
                      .Where(x => !x.Deleted &&
                            (x.Date >= filter.DateFrom && (x.Date <= filter.DateTo || filter.DateTo == Constants.EMPTY_DATETIME)) &&
                            (x.Number >= filter.NumberFrom && (x.Number <= filter.NumberTo || filter.NumberTo == default)) &&
                            (x.OrderGroupId == filter.OrderGroupId || filter.OrderGroupId == Guid.Empty) &&
                            (x.Name == filter.Name || string.IsNullOrEmpty(filter.Name)) &&
                            (x.Count >= filter.CountFrom && (x.Count <= filter.CountTo || filter.CountTo == default)) &&
                            (x.CustomerId == filter.CustomerId || filter.CustomerId == Guid.Empty) &&
                            ((x.OrderState >= filter.OrderStateFrom || filter.OrderStateFrom == OrderState.Create) && (x.OrderState <= filter.OrderStateTo || filter.OrderStateTo == OrderState.Create)) &&
                            (x.TechCalcPrice >= filter.CalcPriceFrom && (x.TechCalcPrice <= filter.CalcPriceTo || filter.CalcPriceTo == default)) &&
                            (x.DirectorExpectedPrice >= filter.ExpectedPriceFrom && (x.DirectorExpectedPrice <= filter.ExpectedPriceTo || filter.ExpectedPriceTo == default)) &&
                            (x.SalesPrice >= filter.SalesPriceFrom && (x.SalesPrice <= filter.SalesPriceTo || filter.SalesPriceTo == default)) &&
                            (x.ReadyDate >= filter.ReadyDateFrom && (x.ReadyDate <= filter.ReadyDateTo || filter.ReadyDateTo == Constants.EMPTY_DATETIME)));

                query = OrderedFactory.GetOrdered<Order>(query, filter.Sort);

                var elements = query.ToList();

                var result = elements.Select(element => element.ToListItemDTO())
                                .ToList();

                if (VersionManager.TryGetVersionCache<Order>(out var cache))
                {
                    cache.SetObjectsVersions(elements.ToDictionary(x => x.Id, x => x.Version));
                }

                return result;
            }
        }

        private DateTime ToEndDate(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        private void ModifyDataBaseVersion(int scriptVersion, MetalEDMContainer container)
        {
            if (_versionCommand.TryGetValue(scriptVersion, out var script))
                container.Database.ExecuteSqlCommand(script);
        }

        public void Stop()
        {
            if (!_isStarted) _log.Warn("Уже остановлен" + Environment.NewLine + Environment.StackTrace);
        }
    }
}