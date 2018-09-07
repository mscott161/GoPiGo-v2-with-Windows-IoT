﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Dexter.Controller.App
{
    public static class DispatcherHelper
    {
        public static async Task RunAndAwaitAsync(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<Task> asyncAction)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            await dispatcher.RunAsync(priority, async () =>
            {
                try
                {
                    await asyncAction().ConfigureAwait(false);

                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.TrySetException(ex);
                }
            });

            await taskCompletionSource.Task.ConfigureAwait(false);
        }
    }
}
