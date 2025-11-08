// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Core2D.Model;
using Core2D.ViewModels.Wizard.Export.Destination;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;
using Core2D.ViewModels.Wizard.Export.Steps;

namespace Core2D.ViewModels.Wizard.Export.Execution;

public sealed class ExportJobRunner
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly IReadOnlyDictionary<string, IFileWriter> _writers;

    public ExportJobRunner(IServiceProvider? serviceProvider, IEnumerable<IFileWriter> writers)
    {
        _serviceProvider = serviceProvider;
        _writers = writers.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    }

    public async Task RunAsync(
        IReadOnlyList<ExportJobItemViewModel> jobs,
        ExportWizardContext context,
        IProgress<double>? progress,
        CancellationToken cancellationToken)
    {
        if (jobs.Count == 0)
        {
            return;
        }

        var log = _serviceProvider?.GetService<ILog>();
        var total = jobs.Count;
        var completed = 0;

        foreach (var job in jobs)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                UpdateJob(job, ExportJobStatus.Cancelled, "Cancelled");
                continue;
            }

            UpdateJob(job, ExportJobStatus.Running, null);

            try
            {
                if (!_writers.TryGetValue(job.WriterName, out var writer))
                {
                    throw new InvalidOperationException($"Writer '{job.WriterName}' is not registered.");
                }

                var target = ResolveTarget(job.Scope);
                if (target is null)
                {
                    throw new InvalidOperationException("Unable to resolve export scope.");
                }

                var path = ExportPathBuilder.BuildPath(context, job.Scope, job.Exporter);
                var directory = System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                if (System.IO.File.Exists(path) && !context.OverwriteExisting)
                {
                    throw new System.IO.IOException($"File '{path}' already exists.");
                }

                await using var stream = new System.IO.FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                writer.Save(stream, target, context.Project);
                UpdateJob(job, ExportJobStatus.Completed, null);
            }
            catch (Exception ex)
            {
                log?.LogException(ex);
                UpdateJob(job, ExportJobStatus.Failed, ex.Message);
            }

            completed++;
            progress?.Report((double)completed / total);
        }
    }

    private static object? ResolveTarget(ExportScopeSelection scope)
    {
        return scope.Kind switch
        {
            ExportScopeKind.Project => scope.Project,
            ExportScopeKind.Document => scope.Document,
            ExportScopeKind.Page => scope.Page,
            _ => null
        };
    }

    private static void UpdateJob(ExportJobItemViewModel job, ExportJobStatus status, string? message)
    {
        void Apply()
        {
            job.Status = status;
            job.Message = message;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Apply();
        }
        else
        {
            Dispatcher.UIThread.Post(Apply);
        }
    }
}
