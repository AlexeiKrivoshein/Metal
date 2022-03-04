﻿using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class OrderGroupListViewModel
        : ComplexElementsListViewModel<VersionListItemDTO>
    {
        protected override DatagramType RemoveDatagramType => DatagramType.RemOrderGroupElement;

        protected override DatagramType SetDatagramType => DatagramType.SetOrderGroupElement;

        public OrderGroupListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(id, dataManager, selectType) { }

        protected override Task<List<VersionListItemDTO>> InnerLoad(CancellationToken token)
        {
            return DataManager.InitOrderGroupCache(token).ContinueWith((t) =>
            {
                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return null;
                }

                return DataManager.OrderGroupNameCache.Values.ToList();
            }, token);
        }

        protected override VersionListItemDTO CreateElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new OrderGroupViewModel(Guid.Empty, DataManager, ElementState.Created);
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

            var viewModel = new OrderGroupViewModel(Selected.Id, DataManager, ElementState.Loading);
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
    }
}
