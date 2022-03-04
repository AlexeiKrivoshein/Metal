using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using MetalTransport.ModelEx.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalTransport.Helper
{
    public static class TaskHelper
    {
        public static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                tcs.TrySetResult(true);
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }

        public static Task WaitOneAsync(this WaitHandle waitHandle)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith((antecedent) => rwh.Unregister(null));
            return t;
        }

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        public static bool CheckError(Task task, out string error)
        {
            error = "";

            if (task.IsFaulted)
            {
                error = task.Exception.Message;
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckError<T>(Task<T> task, out string error)
        {
            error = "";

            if (task.IsFaulted)
            {
                error = Funcs.GetInnerExceptions(task.Exception);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Task Complite()
        {
            return Task.Factory.StartNew(() => { });
        }

        public static CheckState CheckTask(Task<bool> task, CancellationToken token, out string error)
        {
            error = string.Empty;

            if (!task.Result)
            {
                return CheckState.Cancelled;
            }
            else if (token.IsCancellationRequested)
            {
                return CheckState.Cancelled;
            }
            else if (!CheckError(task, out error))
            {
                return CheckState.Error;
            }

            return CheckState.Success;
        }
    }

    public enum CheckState
    {
        Success,
        Error,
        Cancelled
    }

}
