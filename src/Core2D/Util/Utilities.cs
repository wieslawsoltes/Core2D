using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Core2D.Util
{
    public static class Utilities
    {
        public static async Task RunUiJob(Action action)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                action.Invoke();
                Dispatcher.UIThread.RunJobs();
            });
        }
    }
}
