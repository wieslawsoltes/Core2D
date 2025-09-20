// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Core2D.Util;

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