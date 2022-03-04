using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.Dialog
{
    public class StartProgressResult
    {
        public CancellationToken Token { get; set; }
        public Task StartTask { get; set; }

        public StartProgressResult(Task startTask, CancellationToken token)
        {
            StartTask = startTask;
            Token = token;
        }
    }
}
