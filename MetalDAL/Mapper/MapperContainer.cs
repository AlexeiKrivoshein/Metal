using AutoMapper;
using MetalDAL.Manager;
using MetalDAL.Model;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetalDAL.Mapper
{
    public static class MapperContainer
    {
        private static Lazy<MapperConfig> _instance =
            new Lazy<MapperConfig>(() => new MapperConfig());

        public static IMapper Instance { get => _instance.Value.Mapper; }

        private class MapperConfig
        {
            private MapperConfiguration _config;

            public IMapper Mapper { get; }

            public MapperConfig()
            {
                _config = new MapperConfiguration(cfg =>
                {
                    //заказ
                    cfg.CreateMap<Order, OrderDTO>();
                    cfg.CreateMap<Order, Order>();
                    cfg.CreateMap<Order, OrderListItemDTO>().
                        ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer.Name)).
                        ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.OrderGroup.Name));
                    cfg.CreateMap<OrderDTO, Order>().
                        ForMember(dest => dest.Customer, opt => opt.Ignore()).
                        ForMember(dest => dest.OrderGroup, opt => opt.Ignore());

                    //заказчики
                    cfg.CreateMap<Customer, CustomerDTO>();
                    cfg.CreateMap<Customer, Customer>();
                    cfg.CreateMap<Customer, VersionListItemDTO>();
                    cfg.CreateMap<CustomerDTO, Customer>();

                    //сотрудники
                    cfg.CreateMap<Employee, EmployeeDTO>();
                    cfg.CreateMap<Employee, Employee>();
                    cfg.CreateMap<Employee, VersionListItemDTO>().
                        ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ShortName));
                    cfg.CreateMap<EmployeeDTO, Employee>().
                        ForMember(dest => dest.UserGroup, opt => opt.Ignore()).
                        ForMember(dest => dest.Post, opt => opt.Ignore());

                    //должности
                    cfg.CreateMap<Post, PostDTO>();
                    cfg.CreateMap<Post, Post>();
                    cfg.CreateMap<Post, VersionListItemDTO>();
                    cfg.CreateMap<PostDTO, Post>();

                    //операции
                    cfg.CreateMap<Operation, OperationDTO>();
                    cfg.CreateMap<Operation, Operation>();
                    cfg.CreateMap<Operation, VersionListItemDTO>();
                    cfg.CreateMap<OperationDTO, Operation>();
                    cfg.CreateMap<VersionListItemDTO, Operation>();

                    //материалы
                    cfg.CreateMap<Material, MaterialDTO>();
                    cfg.CreateMap<Material, Material>();
                    cfg.CreateMap<Material, VersionListItemDTO>();
                    cfg.CreateMap<MaterialDTO, Material>();
                    cfg.CreateMap<VersionListItemDTO, Material>();

                    //группы заказов
                    cfg.CreateMap<OrderGroup, OrderGroupDTO>();
                    cfg.CreateMap<OrderGroup, OrderGroup>();
                    cfg.CreateMap<OrderGroup, VersionListItemDTO>();
                    cfg.CreateMap<OrderGroupDTO, OrderGroup>();
                    cfg.CreateMap<VersionListItemDTO, OrderGroup>();

                    //работы заказа
                    cfg.CreateMap<OrderOperation, OrderOperationDTO>();
                    cfg.CreateMap<OrderOperation, OrderOperation>();
                    cfg.CreateMap<OrderOperationDTO, OrderOperation>();

                    //файлы
                    cfg.CreateMap<MetalFile, MetalFileDTO>();
                    cfg.CreateMap<MetalFile, MetalFile>();
                    cfg.CreateMap<MetalFile, BaseListItemDTO>();
                    cfg.CreateMap<MetalFileDTO, MetalFile>();

                    //пользователи
                    cfg.CreateMap<UserGroup, UserGroupDTO>();
                    cfg.CreateMap<UserGroup, UserGroup>();
                    cfg.CreateMap<UserGroup, VersionListItemDTO>();
                    cfg.CreateMap<UserGroupDTO, UserGroup>();

                    //материалы в лимитной карточке
                    cfg.CreateMap<LimitCardMaterial, LimitCardMaterialDTO>().
                        ForMember(dest => dest.FactMaterials, opt => opt.MapFrom(src => src.FactMaterials.ToList()));
                    cfg.CreateMap<LimitCardMaterial, LimitCardMaterial>();
                    cfg.CreateMap<LimitCardMaterialDTO, LimitCardMaterial>().
                        ForMember(dest => dest.FactMaterials, opt => opt.MapFrom(src => src.FactMaterials.ToList()));

                    //фактические материалы
                    cfg.CreateMap<LimitCardFactMaterial, LimitCardFactMaterialDTO>();
                    cfg.CreateMap<LimitCardFactMaterial, LimitCardFactMaterial>();
                    cfg.CreateMap<LimitCardFactMaterialDTO, LimitCardFactMaterial>();

                    //операции в лимитной карточке
                    cfg.CreateMap<LimitCardOperation, LimitCardOperationDTO>();
                    cfg.CreateMap<LimitCardOperation, LimitCardOperation>();
                    cfg.CreateMap<LimitCardOperationDTO, LimitCardOperation>().
                        ForMember(dest => dest.Order, opt => opt.Ignore());
                });

                Mapper = _config.CreateMapper();
            }
        }
    }


}
