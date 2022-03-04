using MetalServer.Handler;

namespace MetalServer.Server
{
    public interface IServer
    {
        bool IsWork { get; }

        void Start();

        void Stop();

        void AddDatagramHandler(IDatagramHandler handler);

        string ServerName { get; }

        string ServerPort { get; }
    }
}
