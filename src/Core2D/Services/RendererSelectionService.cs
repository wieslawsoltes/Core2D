// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Input;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Rendering;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Renderer;

namespace Core2D.Services;

public sealed class RendererSelectionService : ViewModelBase, IRendererSelectionService
{
    private static readonly PropertyChangedEventArgs SelectedOptionPropertyChangedEventArgs = new(nameof(SelectedOption));
    private static readonly PropertyChangedEventArgs RendererPropertyChangedEventArgs = new(nameof(Renderer));

    private readonly ImmutableArray<RendererOption> _options;
    private readonly Dictionary<string, RendererOption> _optionById;
    private readonly Dictionary<string, IShapeRenderer> _rendererCache;
    private readonly IServiceProvider? _serviceProvider;
    private readonly IRendererProvider _rendererProvider;
    private readonly ShapeRendererStateViewModel? _state;

    private RendererOption? _selectedOption;
    private IShapeRenderer? _currentRenderer;

    public ICommand SelectRendererCommand { get; }

    public RendererSelectionService(IServiceProvider? serviceProvider, IRendererProvider rendererProvider)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _rendererProvider = rendererProvider ?? throw new ArgumentNullException(nameof(rendererProvider));
        _rendererCache = new Dictionary<string, IShapeRenderer>(StringComparer.OrdinalIgnoreCase);
        _state = serviceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();

        _options = rendererProvider.Options;

        _optionById = new Dictionary<string, RendererOption>(StringComparer.OrdinalIgnoreCase);
        foreach (var option in _options)
        {
            _optionById[option.Id] = option;
        }

        SelectRendererCommand = new DelegateCommand(parameter => OnSelectRenderer(parameter));

        if (!_options.IsDefaultOrEmpty)
        {
            SelectedOption = _options[0];
        }
    }

    public ImmutableArray<RendererOption> Options => _options;

    public RendererOption? SelectedOption
    {
        get => _selectedOption;
        private set
        {
            if (Equals(_selectedOption, value))
            {
                return;
            }

            _selectedOption = value;
            ActivateRenderer(value);
            RaisePropertyChanged(SelectedOptionPropertyChangedEventArgs);
        }
    }

    public IShapeRenderer? Renderer => _currentRenderer;

    public void OnSelectRenderer(object? parameter)
    {
        RendererOption? target = parameter switch
        {
            RendererOption option => option,
            string id when _optionById.TryGetValue(id, out var option) => option,
            _ => null
        };

        if (target is null)
        {
            return;
        }

        SelectedOption = target;
    }

    private void ActivateRenderer(RendererOption? option)
    {
        var previousRenderer = _currentRenderer;

        if (option is null)
        {
            _currentRenderer = null;
        }
        else
        {
            _currentRenderer = GetOrCreateRenderer(option);
            if (_state is { } state && _currentRenderer is { })
            {
                _currentRenderer.State = state;
            }
        }

        if (!ReferenceEquals(previousRenderer, _currentRenderer))
        {
            RaisePropertyChanged(RendererPropertyChangedEventArgs);
        }

        _serviceProvider.GetService<IEditorCanvasPlatform>()?.InvalidateControl?.Invoke();
    }

    private IShapeRenderer GetOrCreateRenderer(RendererOption option)
    {
        if (_rendererCache.TryGetValue(option.Id, out var renderer))
        {
            return renderer;
        }

        renderer = CreateRenderer(option.Id);
        if (_state is { })
        {
            renderer.State = _state;
        }

        _rendererCache[option.Id] = renderer;
        return renderer;
    }

    private IShapeRenderer CreateRenderer(string id) => _rendererProvider.CreateRenderer(id);

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public DelegateCommand(Action<object?> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged;
    }
}
