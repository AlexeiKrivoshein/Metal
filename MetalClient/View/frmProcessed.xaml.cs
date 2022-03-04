using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MetalClient.Dialog;
using System.Threading.Tasks;
using MetalTransport.Helper;
using log4net;
using MetalDiagnostic.Logger;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for frmProcessed.xaml
    /// </summary>
    public partial class frmProcessed 
        : Window
        , INotifyPropertyChanged
        , IDisposable
    {
        private const int DEFAULT_PROCESSED_LENGTH = 80;
        private const int DEFAULT_PROCESSED_STEP = 10;
        private const int ANIMATION_TIMEOUT_MS = 25;

        private ILog _log = LogService.GetLogger(nameof(frmProcessed));

        private int _processedPosition = (0 - DEFAULT_PROCESSED_LENGTH);
        private object _lock = new object();

        //обработка
        private Timer _processAnimationTimer;
        private CancellationTokenSource _cts;
        private bool _isStarted = false;

        private string _header;
        public string Header 
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public frmProcessed(Window owner, string title = "Загрузка...")
        {
            InitializeComponent();

            Owner = owner;
            Title = title;

            _processAnimationTimer = new Timer(Process, null, ANIMATION_TIMEOUT_MS, ANIMATION_TIMEOUT_MS);
        }

        /// <summary>
        /// Анимация процесса в шкале без прогресса
        /// </summary>
        /// <param name="state"></param>
        private void Process(object state)
        {
            _processedPosition = Math.Min((int)cnvProgress.ActualWidth, _processedPosition + DEFAULT_PROCESSED_STEP);

            //сдвианем индикатор процесса
            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                //урезаем начало
                rctProgress.Width = DEFAULT_PROCESSED_LENGTH - (_processedPosition < 0 ? Math.Abs(_processedPosition) : 0);

                //урезаем конец
                if (_processedPosition + DEFAULT_PROCESSED_LENGTH > cnvProgress.ActualWidth)
                    rctProgress.Width = DEFAULT_PROCESSED_LENGTH - ((_processedPosition + DEFAULT_PROCESSED_LENGTH) - cnvProgress.ActualWidth);

                Canvas.SetLeft(rctProgress, Math.Max(_processedPosition, 0));
            }));

            //начинаем с начала
            if (_processedPosition == cnvProgress.ActualWidth)
                _processedPosition = (0 - DEFAULT_PROCESSED_LENGTH);
        }

        public StartProgressResult Start(string description)
        {
            CancellationToken token;
            Task startTask;

            lock (_lock)
            {
                if (_isStarted)
                {
                    _log.Warn("Выполение прогресса уже запущено." + Environment.NewLine + Environment.StackTrace);
                    token = _cts.Token;
                    startTask = TaskHelper.Complite();
                }
                else
                {
                    _isStarted = true;
                    Header = description;
                    _cts = new CancellationTokenSource();
                    token = _cts.Token;

                    _processAnimationTimer.Change(ANIMATION_TIMEOUT_MS, ANIMATION_TIMEOUT_MS);

                    var tcs = new TaskCompletionSource<bool>();
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tcs.SetResult(true);
                        ShowDialog();
                    }));
                    startTask = tcs.Task;
                }
            }

            return new StartProgressResult(startTask, token);
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isStarted)
                {
                    _log.Warn("Выполение прогресса не запущено." + Environment.NewLine + Environment.StackTrace);
                    return;
                }

                _isStarted = false;

                _processedPosition = (0 - DEFAULT_PROCESSED_LENGTH);
                _processAnimationTimer.Change(Timeout.Infinite, Timeout.Infinite);

                _cts.Cancel();

                Application.Current.Dispatcher.Invoke(new Action(() => Hide()));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        public void Dispose()
        {
            Stop();
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
