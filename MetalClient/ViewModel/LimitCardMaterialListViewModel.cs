using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class LimitCardMaterialListViewModel
        : ComplexElementsListViewModel<LimitCardMaterialDTO>
    {
        private Guid _orderId;

        private bool _isFactMaterial = false;
        public bool IsFactMaterial
        {
            get => _isFactMaterial;
            set
            {
                _isFactMaterial = value;
                OnPropertyChanged(nameof(IsFactMaterial));
            }
        }

        public bool NotIsFactMaterial => !_isFactMaterial;

        //удаленные элементы
        public List<Guid> RemovedMaterials { get; private set; }

        protected override DatagramType RemoveDatagramType => DatagramType.NONE;

        protected override DatagramType SetDatagramType => DatagramType.NONE;

        public ObservableCollection<BaseListItemDTO> UnitsCache => DataManager.UnitsCache;

        //кэш
        public ICollection<VersionListItemDTO> _materialCache;
        public ICollection<VersionListItemDTO> MaterialCache
        {
            get => _materialCache;
            private set
            {
                _materialCache = value;
                OnPropertyChanged(nameof(MaterialCache));
            }
        }

        #region Комманды
        // Выбор материала
        public Action<MaterialListViewModel> ShowMaterialList;
        private RelayCommand _selectMaterialCommand;
        public RelayCommand SelectMaterialCommand
        {
            get
            {
                return _selectMaterialCommand ??
                  (_selectMaterialCommand = new RelayCommand(obj => SelectMaterial(), 
                  (obj) => ShowMaterialList != null && Selected != null));
            }
        }

        // Добавить фактический материал
        private RelayCommand _addFactMaterialCommand;
        public RelayCommand AddFactMaterialCommand
        {
            get
            {
                return _addFactMaterialCommand ??
                  (_addFactMaterialCommand = new RelayCommand(obj => AddFactMaterial(), 
                  (obj) => Selected != null));
            }
        }

        // Удалить фактический материал
        private RelayCommand _removeFactMaterialCommand;
        public RelayCommand RemoveFactMaterialCommand
        {
            get
            {
                return _removeFactMaterialCommand ??
                  (_removeFactMaterialCommand = new RelayCommand(obj =>
                  {
                      if (obj is LimitCardFactMaterialDTO fact)
                      {
                          if (Selected.FactMaterials.Count > 1)
                          {
                              Selected.FactMaterials.Remove(fact);
                          }
                          else if (Selected.FactMaterials.Count == 1)
                          {
                              Selected.FactMaterials[0] = new LimitCardFactMaterialDTO(Selected.Id);
                          }
                      }
                  }, (obj) => Selected != null));
            }
        }

        // Выбор фактического материала
        private RelayCommand _selectFactMaterialCommand;
        public RelayCommand SelectFactMaterialCommand
        {
            get
            {
                return _selectFactMaterialCommand ??
                  (_selectFactMaterialCommand = new RelayCommand(obj =>
                  {
                      SelectFactMaterial(obj);
                  }, (obj) => ShowMaterialList != null &&
                              Selected != null));
            }
        }
        #endregion

        public LimitCardMaterialListViewModel(Guid orderId, bool isFactMaterial, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(Guid.Empty, dataManager, selectType) 
        {
            _orderId = orderId;
            IsFactMaterial = isFactMaterial;
            RemovedMaterials = new List<Guid>();
        }

        protected override Task<List<LimitCardMaterialDTO>> InnerLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetLimitMaterialList)
                                 .WithDTOObject(new GetOrderIdFilteredElementsList(_orderId))
                                 .Build();

            return DataManager.ExcuteRequestAsync<SetListData<LimitCardMaterialDTO>>(request, token).ContinueWith((t1) =>
            {
                if (!TaskHelper.CheckError(t1, out var error))
                {
                    ErrorInvoke(error);
                }

                RefreshMaterialCache();

                var data = t1.Result;
                var materials = data.Elements;

                foreach (var material in materials)
                {
                    if (!material.FactMaterials.Any())
                    {
                        material.FactMaterials.Add(new LimitCardFactMaterialDTO(material.Id));
                    }
                }

                return materials;
            }, token);
        }

        protected override LimitCardMaterialDTO OpenElement()
        {
            return null;
        }

        protected override LimitCardMaterialDTO CreateElement()
        {
            var limitCardMaterial = new LimitCardMaterialDTO 
            {
                OrderId = _orderId
            };

            limitCardMaterial.FactMaterials.Add(new LimitCardFactMaterialDTO(limitCardMaterial.Id));

            return limitCardMaterial;
        }

        protected override Task<bool> Remove()
        {
            RemovedMaterials.Add(Selected.Id);
            _dispatcher.Invoke(
                        new Action(() =>
                        {
                            if (! Elements.Any())
                            {
                                return;
                            }

                            int index;
                            for (index = 0; Elements[index].Id != Selected.Id; index++) { }

                            if (Elements[index].Id == Selected.Id)
                            {
                                Elements.RemoveAt(index);
                            }
                        }));

            return TaskHelper.FromResult(true);
        }

        private void SelectMaterial()
        {
            var viewModel = new MaterialListViewModel(Selected.Id, DataManager, ElementListSelectType.Select);
            ShowMaterialList(viewModel);

            if (viewModel.IsModify)
            {
                RefreshMaterialCache();
            }

            if (viewModel.IsSelected &&
                  viewModel.Selected.Id != null &&
                  viewModel.Selected.Id != Guid.Empty)
            {
                Selected.MaterialId = viewModel.Selected.Id;
            }
        }

        private void SelectFactMaterial(object obj)
        {
            if (obj is LimitCardFactMaterialDTO fact)
            {
                var viewModel = new MaterialListViewModel(fact.MaterialId, DataManager, ElementListSelectType.Select);
                ShowMaterialList(viewModel);

                if (viewModel.IsModify)
                {
                    RefreshMaterialCache();
                }

                if (viewModel.IsSelected &&
                      viewModel.Selected.Id != null &&
                      viewModel.Selected.Id != Guid.Empty)
                {
                    fact.MaterialId = viewModel.Selected.Id;
                }
            }
        }

        private void AddFactMaterial()
        {
            var factMaterial = new LimitCardFactMaterialDTO(Selected.Id);
            Selected.FactMaterials.Add(factMaterial);
        }

        private void RefreshMaterialCache()
        {
            MaterialCache = DataManager.MaterialNameCache.Values.OrderBy(x => x.Name).ToList();
        }

        protected override Task<Guid> AddElement()
        {
            var element = CreateElement();

            if (element == null) return null;

            _id = element.Id;
            IsModify = true;

            //обновление списка элементов
            Elements.Add(element);
            SetElements(Elements.ToList());

            return TaskHelper.FromResult(element.Id);
        }
    }
}
