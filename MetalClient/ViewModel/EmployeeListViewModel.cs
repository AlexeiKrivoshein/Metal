using MetalClient.DataManager;
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
    public class EmployeeListViewModel
        : ComplexElementsListViewModel<VersionListItemDTO>
    {
        protected override DatagramType RemoveDatagramType => DatagramType.RemEmployeeElement;

        protected override DatagramType SetDatagramType => DatagramType.SetEmployeeElement;

        public EmployeeListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(id, dataManager, selectType) { }

        protected override Task<List<VersionListItemDTO>> InnerLoad(CancellationToken token)
        {
            return DataManager.InitEmployeerCache(token).ContinueWith((t) =>
            {
                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return null;
                }

                return DataManager.EmployeeNameCache.Values.ToList();
            }, token);
        }

        protected override VersionListItemDTO CreateElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new EmployeeViewModel(Guid.Empty, DataManager, ElementState.Created);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                return new VersionListItemDTO()
                {
                    Id = viewModel.Element.Id,
                    Name = string.Concat(viewModel.Element.Secondname, " ", viewModel.Element.Name, " ", viewModel.Element.Patronymic)
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

            var viewModel = new EmployeeViewModel(Selected.Id, DataManager, ElementState.Loading);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                return new VersionListItemDTO()
                {
                    Id = viewModel.Element.Id,
                    Name = string.Concat(viewModel.Element.Secondname, " ", viewModel.Element.Name, " ", viewModel.Element.Patronymic)
                };
            }
            else
            {
                return null;
            }
        }

        protected override Task<bool> Remove()
        {
            if (Selected == null || Selected.Id == SecurityHelper.AdministratorId)
            {
                return TaskHelper.FromResult(false);
            }

            return base.Remove();
        }
    }
}
