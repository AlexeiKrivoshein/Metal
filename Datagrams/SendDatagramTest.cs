using System;
using MetalClient.Client;
using MetalDAL.Manager;
using MetalServer.Handler;
using MetalServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MetalTransport.Handler;
using MetalClient.Client.Pipe;
using MetalClient.DataManager;
using MetalTransport.Datagram;

namespace Datagrams
{
    [TestClass]
    public class SendDatagramTest
    {
        private IServer _server;
        private IClient _client;
        private ClientDataManager _dataManager;

        [ClassInitialize]
        public void Initialize(TestContext context)
        {
            var manager = new ModelManager();

            DatagramHandlerMappingContainer.Instance.AddHandler(new FileHandler(manager));
            DatagramHandlerMappingContainer.Instance.AddHandler(new GetElementHandler(manager));
            DatagramHandlerMappingContainer.Instance.AddHandler(new GetElementsListHandler(manager));
            DatagramHandlerMappingContainer.Instance.AddHandler(new RemoveElementHandler(manager));
            DatagramHandlerMappingContainer.Instance.AddHandler(new SecurityHandler());
            DatagramHandlerMappingContainer.Instance.AddHandler(new SetElementHandler(manager));
            DatagramHandlerMappingContainer.Instance.AddHandler(new ViewHandler(manager));

            _server = new TcpMetalServer();
            _client = new TcpMetalClient();

            _server.StartServer();
            _client.Start();

            _dataManager = new ClientDataManager(_client);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Stop();
            _server.StopServer();
        }
    }
}
