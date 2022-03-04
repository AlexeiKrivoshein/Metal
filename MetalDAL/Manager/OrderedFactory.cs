using MetalDAL.Model;
using MetalDAL.ModelEx;
using MetalTransport.Datagram.GetListData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalDAL.Manager
{
    internal static class OrderedFactory
    {
        public static IQueryable<T> GetOrdered<T>(IQueryable<Order> query, List<Sort> sort)
            where T : class, IVersionModelElement<T>, new()
        {
            IOrderedQueryable<Order> sorted = null;

            foreach (var item in sort)
            {
                switch (item.Field)
                {
                    case SortField.OrderNumber:
                        if(sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.Number);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.Number);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.Number);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.Number);
                            }
                        }

                        break;
                    case SortField.OrderDate:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.Date);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.Date);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.Date);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.Date);
                            }
                        }

                        break;
                    case SortField.OrderName:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.Name);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.Name);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.Name);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.Name);
                            }
                        }

                        break;
                    case SortField.OrderCount:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.Count);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.Count);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.Count);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.Count);
                            }
                        }

                        break;
                    case SortField.OrderCustomer:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.Customer.Name);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.Customer.Name);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.Customer.Name);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.Customer.Name);
                            }
                        }

                        break;
                    case SortField.OrderReadyDate:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.ReadyDate);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.ReadyDate);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.ReadyDate);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.ReadyDate);
                            }
                        }

                        break;
                    case SortField.OrderSalesPrice:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.SalesPrice);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.SalesPrice);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.SalesPrice);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.SalesPrice);
                            }
                        }

                        break;
                    case SortField.OrderOrderState:
                        if (sorted is null)
                        {
                            if (item.IsDesc)
                            {
                                sorted = query.OrderByDescending(x => x.OrderState);
                            }
                            else
                            {
                                sorted = query.OrderBy(x => x.OrderState);
                            }
                        }
                        else
                        {
                            if (item.IsDesc)
                            {
                                sorted = sorted.ThenByDescending(x => x.OrderState);
                            }
                            else
                            {
                                sorted = sorted.ThenBy(x => x.OrderState);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            if(sorted is null)
            {
                sorted = query.OrderBy(x => x.Id);
            }

            return sorted.Cast<T>();
        }
    }
}
