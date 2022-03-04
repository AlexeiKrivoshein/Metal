using MetalClient.DataManager;
using MetalClient.Exceptions;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class OrderDrawingListViewModel
        : ElementsListViewModelBase<BaseListItemDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetFileData;
        protected override DatagramType RemoveDatagramType => DatagramType.RemFile;

        public List<Guid> Removed;
        public List<Guid> Adding;

        private Guid _orderId;

        //Команда открыть
        public event Action OnOpen;
        protected void OpenInvoke() =>
            _dispatcher.BeginInvoke(new Action(() => OnOpen?.Invoke()));
        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ??
                  (_openCommand = new RelayCommand(obj =>
                      {
                          Select().ContinueWith(t =>
                          {
                              OnOpen?.Invoke();
                          });
                      }
                  ));
            }
        }

        public OrderDrawingListViewModel(ClientDataManager dataManager, Guid orderId, ElementListSelectType selectType)
            : base(Guid.Empty, dataManager, ElementListSelectType.Show)
        {
            _orderId = orderId;
            Removed = new List<Guid>();
            Adding = new List<Guid>();
        }

        protected override Task<Guid> AddElement()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            BaseListItemDTO element = null;
            if (openFileDialog.ShowDialog() == true)
            {
                var token = PrgShow?.Invoke("Сохранение чертежа").Token ?? CancellationToken.None;
                try
                {
                    FileInfo info = new FileInfo(openFileDialog.FileName);

                    var maxSize = Constants.MAX_FILE_SIZE_MB * 1024 * 1024;
                    if (maxSize < info.Length)
                    {
                        PrgHide?.Invoke();
                        ErrorInvoke($"Слишком большой файл, поддерживаются файлы до {Constants.MAX_FILE_SIZE_MB} Мб.");
                        return null;
                    }

                    var fileId = Helper.FileHelper.SaveFile(openFileDialog.FileName, _orderId, DataManager, token);
                    element = new VersionListItemDTO { Id = fileId, Name = Path.GetFileName(openFileDialog.FileName), Deleted = false };
                }
                catch (OperationCanceledException) { return null; }
                finally
                {
                    PrgHide?.Invoke();
                }
                
                _dispatcher.Invoke(
                    new Action(() =>
                    {
                        Adding.Add(element.Id);
                        Elements.Add(element);
                        Selected = element;
                    }));
            }

            return TaskHelper.FromResult(element.Id);
        }

        protected override Task<List<BaseListItemDTO>> InnerLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetFileList)
                                 .WithDTOObject(new GetOrderIdFilteredElementsList(_orderId))
                                 .Build();

            return DataManager.ExcuteRequestAsync<SetListData<BaseListItemDTO>>(request, token).
                ContinueWith(t1 =>
                {
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        return null;
                    }

                    var data = t1.Result;
                    return data.Elements;
                }, token);
        }

        protected override Task<bool> Select()
        {
            var success = TaskHelper.FromResult(true);

            try
            {
                var token = PrgShow?.Invoke("Загрузка чертежа").Token ?? CancellationToken.None;
                var task = Helper.FileHelper.LoadFile(Selected.Id, DataManager, token);

                task.ContinueWith((t1) =>
                {
                    PrgHide?.Invoke();
                    if (!TaskHelper.CheckError(task, out var error))
                    {
                        ErrorInvoke(error);
                        return;
                    }

                    try
                    {
                        Process.Start(t1.Result);
                    }
                    catch
                    {
                        ErrorInvoke($"Не удалось открыть файл {Path.GetFileName(t1.Result)}");
                    }
                }, token);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    return success;
                }
                else if (ex is AggregateException)
                {
                    var innerEx = ex.GetBaseException();
                    if (innerEx is RequestException requestException)
                        ErrorInvoke(requestException.Message);
                    else
                        throw ex;
                }
                else
                {
                    throw ex;
                }
            }

            return success;
        }

        protected override Task<bool> Remove()
        {
            _dispatcher.Invoke(
                new Action(() =>
                {
                    var index = Elements.IndexOf(Selected);

                    Removed.Add(Selected.Id);
                    Elements.Remove(Selected);

                    if (index > 0)
                    {
                        Selected = Elements[index - 1];
                    }
                    else if (Elements.Any())
                    {
                        Selected = Elements[0];
                    }
                    else
                    {
                        Selected = null;
                    }
                }));

            return TaskHelper.FromResult(true);
        }
    }
}
