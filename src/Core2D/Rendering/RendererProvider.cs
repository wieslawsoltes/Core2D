// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Avalonia;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.Modules.Renderer.SparseStrips;
using Core2D.Modules.Renderer.VelloSharp;
using Core2D.ViewModels.Renderer;

namespace Core2D.Rendering;

internal sealed record RendererRegistration(RendererOption Option, Func<IServiceProvider, IShapeRenderer> Factory);

public sealed class RendererProvider : IRendererProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ImmutableArray<RendererRegistration> _registrations;
    private readonly Dictionary<string, RendererRegistration> _registrationsById;

    public RendererProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _registrations = CreateDefaultRegistrations().ToImmutableArray();
        _registrationsById = new Dictionary<string, RendererRegistration>(StringComparer.OrdinalIgnoreCase);
        foreach (var registration in _registrations)
        {
            _registrationsById[registration.Option.Id] = registration;
        }
    }

    public ImmutableArray<RendererOption> Options => _registrations.Select(static x => x.Option).ToImmutableArray();

    public RendererOption? GetOption(string id)
        => _registrationsById.TryGetValue(id, out var registration) ? registration.Option : null;

    public IShapeRenderer CreateRenderer(string id)
    {
        if (_registrationsById.TryGetValue(id, out var registration))
        {
            return registration.Factory(_serviceProvider);
        }

        return _registrations.Length > 0
            ? _registrations[0].Factory(_serviceProvider)
            : throw new InvalidOperationException("No renderer registrations found.");
    }

    private static IEnumerable<RendererRegistration> CreateDefaultRegistrations()
    {
        yield return new RendererRegistration(
            new RendererOption("avalonia", "Avalonia Renderer", "Avalonia immediate renderer with draw-node caching."),
            sp => new AvaloniaRendererViewModel(sp));

        yield return new RendererRegistration(
            new RendererOption("skiasharp", "SkiaSharp Renderer", "SkiaSharp vector renderer backed by draw-node caching."),
            sp => new SkiaSharpRendererViewModel(sp));

        yield return new RendererRegistration(
            new RendererOption("sparse-strips", "Sparse Strips Renderer", "Vello sparse-strip renderer optimized for CPU workloads."),
            sp => new SparseStripsRenderer(sp));

        yield return new RendererRegistration(
            new RendererOption("vello-sharp", "VelloSharp Renderer", "CPU Vello renderer using VelloSharp rasterizer."),
            sp => new VelloSharpRenderer(sp));
    }
}
