using MetalClient.DataManager;
using MetalTransport.Datagram.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class LoadViewModel
        : INotifyPropertyChanged
    {
        private const int INIT_DATA_COUNT = 6;
        private const int DEFAULT_PROGRESS_STEP = 1;

        private ClientDataManager _dataManager;
        private object _lock = new object();

        public bool IsSucces { get; set; }

        private int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private int _maximum = INIT_DATA_COUNT;
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                OnPropertyChanged("Maximum");
            }
        }

        public string Header => "Загрузка данных программы";

        public LoadViewModel(ClientDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        /// <summary>
        /// Инициирование загрузки справочников
        /// </summary>
        /// <returns></returns>
        public Task Load(CancellationToken token)
        {
            var task = new Task(() =>
            {
                IsSucces = false;

                //TODO асинхронная загрузка сразу всех справочников, в данный момент периодически в канал поступают не десериализуеммые данные от сервера
                try
                {
                    _dataManager.InitUserGroupCache(token).ContinueWith((t) => Increase()).Wait(token);
                    _dataManager.InitMaterialCache(token).ContinueWith((t) => Increase()).Wait(token);
                    _dataManager.InitOperationCache(token).ContinueWith((t) => Increase()).Wait(token);
                    _dataManager.InitCustomerCache(token).ContinueWith((t) => Increase()).Wait(token);
                    _dataManager.InitEmployeerCache(token).ContinueWith((t) => Increase()).Wait(token);
                    _dataManager.InitOrderGroupCache(token).ContinueWith((t) => Increase()).Wait(token);
                }
                catch (TaskCanceledException) { return; }
                catch (OperationCanceledException) { return; }
                catch (IOException) { return; }

                IsSucces = true;
            }, token);

            task.Start();

            return task;
        }

        /// <summary>
        /// Шаг прогресса, только для шкалы-прогресс
        /// </summary>
        private void Increase()
        {
            lock (_lock)
            {
                Value = Math.Min(Value + DEFAULT_PROGRESS_STEP, _maximum);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
