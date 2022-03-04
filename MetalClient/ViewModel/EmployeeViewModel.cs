using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class EmployeeViewModel
        : ElementViewModelBase<EmployeeDTO>
    {
        public Func<string> TakePassword;

        protected override DatagramType SetDatagramType => DatagramType.SetEmployeeElement;

        protected override DatagramType GetDatagramType => DatagramType.GetEmployeeElement;

        public ICollection<VersionListItemDTO> _userGroupCache;
        public ICollection<VersionListItemDTO> UserGroupCache
        {
            get => _userGroupCache;
            private set
            {
                _userGroupCache = value;
                OnPropertyChanged(nameof(UserGroupCache));
            }
        }

        public bool IsSystemAdministrator => (Element.Id == SecurityHelper.AdministratorId);

        //Команда выбрать операцию заказа
        public Action<UserGroupListViewModel> ShowUserGroupList;
        private RelayCommand _selectUserGroupCommand;
        public RelayCommand SelectUserGroupCommand
        {
            get
            {
                return _selectUserGroupCommand ??
                  (_selectUserGroupCommand = new RelayCommand(obj =>
                  {
                      SelectUserGroup();
                  }, (obj) => ShowUserGroupList != null));
            }
        }

        private void SelectUserGroup()
        {
            var id = Element.UserGroupId.HasValue ? Element.UserGroupId.Value : Guid.Empty;
            var viewModel = new UserGroupListViewModel(id, DataManager, ElementListSelectType.Select);
            ShowUserGroupList.Invoke(viewModel);

            if (viewModel.IsModify)
            {
                RefreshUserGroupCache();
            }

            if (viewModel.IsSelected &&
                  viewModel.Selected.Id != null &&
                  viewModel.Selected.Id != Guid.Empty)
            {
                var factory = new DatagramFactory();
                var request = factory.WithType(DatagramType.GetUserGroupElement).WithDTOObject(new GetElementData(viewModel.Selected.Id)).Build();

                DataManager.ExcuteRequestAsync<UserGroupDTO>(request, CancellationToken.None).
                  ContinueWith((t) =>
                  {
                      if (!TaskHelper.CheckError(t, out var error))
                      {
                          ErrorInvoke(error);
                          return;
                      }
                      else
                      {
                          State = ElementState.Saved;
                          Element.UserGroupId = t.Result.Id;
                          Element.UserGroup = t.Result;
                      }
                  });
            }
        }

        public EmployeeViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) 
        {
            RefreshUserGroupCache();
        }

        protected override Task BeforeSave(CancellationToken token, CancellationTokenSource cts)
        {
            return Task.Factory.StartNew(() =>
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    Element.Password = SecurityHelper.GetMd5Hash(md5Hash, TakePassword());
                }
            });
        }

        protected override Task AfterSave(CancellationToken token, HandledDTO handled)
        {
            return Task.Factory.StartNew(() => { Element.Password = ""; });
        }

        /*
        protected override Task<bool> BeforeLoad(CancellationToken token)
        {
            RefreshUserGroupCache();
            return TaskHelper.FromResult(true);
        }*/

        private void RefreshUserGroupCache()
        {
            UserGroupCache = DataManager.UserGroupNameCache.Values.OrderBy(x => x.Name).ToList();
        }
    }
}
