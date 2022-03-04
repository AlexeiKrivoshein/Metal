using log4net;
using MetalDAL.Manager;
using MetalDAL.Mapper;
using MetalDiagnostic.Logger;
using MetalServer.Handler;
using MetalServer.Server;
using MetalTransport.Helper;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MetalServer
{
    public class MetallServerHost : ServiceBase 
    {
        private static ILog _log;

        private ConnectionBroker _connectionBroker;
        private IServer _server;
        private ModelManager _manager;
        
        public MetallServerHost()
        {
            LogService.InitLogger(LoggerType.Server);
            _log = LogService.GetLogger(nameof(MetallServerHost));

            var drawingPath = System.Configuration.ConfigurationManager.AppSettings["DrawingPath"];
            _manager = new ModelManager(drawingPath);
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            StopService();
        }

        public void StartService()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
               UnhandledException(s, e, (str, ex) => {
                   File.WriteAllText(@"unhandled.log", str);
                   File.AppendAllText(@"unhandled.log", ex.Message);
                   File.AppendAllText(@"unhandled.log", ex.StackTrace);
               });

            try
            {
                _server = new TcpMetalServer();

                _log.Info("Запуск TCP сервера Metall.");
                _manager.Start();

                _connectionBroker = new ConnectionBroker();
                _connectionBroker.Start();
                _connectionBroker.AddServer(_server);

                var assembly = typeof(MetallServerHost).Assembly;
                var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<HandlerAttribute>() != null);

                foreach (var type in types)
                {
                    var handler = Activator.CreateInstance(type) as IDatagramHandler;
                    handler.Manager = _manager;
                    _server.AddDatagramHandler(handler);
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var validError in error.ValidationErrors)
                    {
                        _log.Error($"{validError.PropertyName}-{validError.ErrorMessage}");
                    }
                }

                throw;
            }

            _log.Info($"Сервер Metall запущен {_server.ServerName}:{_server.ServerPort}.");
        }

        public void StopService()
        {
            _log.Info("Остановка сервера Metall.");

            _connectionBroker.Stop();
            _server.Stop();
            Task.Delay(1000);

            _log.Info("Сервер Metall остановлен.");
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args, Action<string, Exception> log)
        {
            var ex = args.ExceptionObject as Exception;

            var error = Funcs.GetInnerExceptions(ex);

            try
            {
                log.Invoke($"Произошло необработанное исключение. Программа будет закрыта. {error}", ex);
            }
            catch (Exception)
            {
                Console.Error.WriteLine(error.ToString());
            }

            Environment.Exit(1);
        }
    }
}
