using MetalClient.DataManager;
using MetalClient.Helper;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class PlanListViewModel
        : ComplexElementsListViewModel<PlanItemDTO>
    {
        public int PlanMonth = DateTimeHelper.DateTimeNow().Month;
        public int PlanYear = DateTimeHelper.DateTimeNow().Year;

        private RelayCommand _getNextPlanCommand;
        public RelayCommand GetNextPlanCommand
        {
            get
            {
                return _getNextPlanCommand ??
                  (_getNextPlanCommand = new RelayCommand(obj =>
                  {
                      if (PlanMonth < 12)
                      {
                          PlanMonth++;
                      }
                      else
                      {
                          PlanMonth = 1;
                          PlanYear++;
                      }
                      Load();
                  }));
            }
        }

        private RelayCommand _getPrevPlanCommand;
        public RelayCommand GetPrevPlanCommand
        {
            get
            {
                return _getPrevPlanCommand ??
                  (_getPrevPlanCommand = new RelayCommand(obj =>
                  {
                      if (PlanMonth > 1)
                      {
                          PlanMonth--;
                      }
                      else
                      {
                          PlanMonth = 12;
                          PlanYear--;
                      }
                      Load();
                  }));
            }
        }

        private PlanFooter _footer = new PlanFooter();
        public PlanFooter Footer
        {
            get => _footer;
            set
            {
                _footer = value;
                OnPropertyChanged(nameof(Footer));
            }
        }

        protected override DatagramType RemoveDatagramType => DatagramType.NONE;

        protected override DatagramType SetDatagramType => DatagramType.NONE;

        public PlanListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            :base(id, dataManager, selectType)
        { }

        protected override Task<List<PlanItemDTO>> InnerLoad(CancellationToken token)
        {
            string monthName = Helper.Funcs.GetMonthName(PlanMonth);
            Header = $"План: {monthName} {PlanYear}";

            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetPlanElement).WithDTOObject(new GetPlanElementsList(PlanMonth, PlanYear)).Build();

            return DataManager.ExcuteRequestAsync<SetListData<PlanItemDTO>>(request, token).
                ContinueWith((t1) =>
                {
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        return null;
                    }

                    var data = t1.Result;
                    return data.Elements.Cast<PlanItemDTO>().ToList();
                }, token);
        }

        protected override Task<bool> AfterLoad(CancellationToken token)
        {
            var totalSum = Elements.Sum(item => item.Sum);
            var totalMakedSum = Elements.Sum(item => item.MakedSum);

            var week1Sum = Elements.Sum(item => item.Week1);
            var week2Sum = Elements.Sum(item => item.Week2);
            var week3Sum = Elements.Sum(item => item.Week3);
            var week4Sum = Elements.Sum(item => item.Week4);

            var totalCount = Elements.Sum(item => item.Maked);

            Footer = new PlanFooter() {
                Sum = totalSum,
                MakedSum = totalMakedSum,
                Week1Precent = Math.Round((float)week1Sum / totalCount * 100, 2),
                Week2Precent = Math.Round((float)week2Sum / totalCount * 100, 2),
                Week3Precent = Math.Round((float)week3Sum / totalCount * 100, 2),
                Week4Precent = Math.Round((float)week4Sum / totalCount * 100, 2)
            };

            return TaskHelper.FromResult(true);
        }

        protected override PlanItemDTO CreateElement()
        {
            return null;
        }

        protected override PlanItemDTO OpenElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new OrderViewModel(Selected.Id, DataManager, ElementState.Loading);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                _id = viewModel.Element.Id;
                return OrderDTO.ToPlanDTO(viewModel.Element);
            }
            else
            {
                return null;
            }
        }
    }
}