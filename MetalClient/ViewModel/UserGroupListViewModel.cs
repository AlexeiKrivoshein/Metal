using MetalClient.DataManager;
using MetalClient.Helper;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class UserGroupListViewModel
        : ComplexElementsListViewModel<VersionListItemDTO>
    {
        protected override DatagramType RemoveDatagramType => DatagramType.RemUserGroupElement;

        protected override DatagramType SetDatagramType => DatagramType.SetUserGroupElement;

        public UserGroupListViewModel(Guid id, ClientDataManager dataManager,  ElementListSelectType selectType)
            : base(id, dataManager, selectType) { }

        protected override Task<List<VersionListItemDTO>> InnerLoad(CancellationToken token)
        {
            return DataManager.InitUserGroupCache(token).ContinueWith((t) =>
            {
                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return null;
                }

                return DataManager.UserGroupNameCache.Values.ToList();
            }, token);
        }

        protected override VersionListItemDTO CreateElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new UserGroupViewModel(Guid.Empty, DataManager, ElementState.Created);
            viewModel.Element.Rights = new byte[UserGroupHelper.LAST];
            viewModel.PrepareRightsView();

            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                return new VersionListItemDTO()
                {
                    Id = viewModel.Element.Id,
                    Name = viewModel.Element.Name
                };
            }
            else
            {
                return null;
            }
        }

        protected override VersionListItemDTO OpenElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new UserGroupViewModel(Selected.Id, DataManager, ElementState.Loading);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                return new VersionListItemDTO()
                {
                    Id = viewModel.Element.Id,
                    Name = viewModel.Element.Name
                };
            }
            else
            {
                return null;
            }
        }

        protected override Task<bool> Remove()
        {
            if (Selected == null || Selected.Id == SecurityHelper.AdministratorGroupId)
            {
                return TaskHelper.FromResult(false);
            }

            return base.Remove();
        }
    }
}
