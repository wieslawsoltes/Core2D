#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.Model.Editor;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels;

public abstract partial class ViewModelBase : INotifyPropertyChanged
{
    private bool _isDirty;
    [AutoNotify] private ViewModelBase? _owner;
    [AutoNotify] private string _name = "";

    protected ViewModelBase(IServiceProvider? serviceProvider)
    {
        ServiceProvider = serviceProvider;

        CutCommand = new RelayCommand<object?>(x => ServiceProvider.GetService<IClipboardService>()?.OnCut(x));

        CopyCommand = new RelayCommand<object?>(x => ServiceProvider.GetService<IClipboardService>()?.OnCut(x));

        PasteCommand = new RelayCommand<object?>(x => ServiceProvider.GetService<IClipboardService>()?.OnCut(x));

        DeleteCommand = new RelayCommand<object?>(x => ServiceProvider.GetService<IClipboardService>()?.OnDelete(x));

        ExportCommand = new RelayCommand<object?>(x => ServiceProvider.GetService<IProjectEditorPlatform>()?.OnExport(x));
    }

    [IgnoreDataMember]
    public ICommand CutCommand { get; }
        
    [IgnoreDataMember]
    public ICommand CopyCommand { get; }

    [IgnoreDataMember]
    public ICommand PasteCommand { get; }

    [IgnoreDataMember]
    public ICommand DeleteCommand { get; }

    [IgnoreDataMember]
    public ICommand ExportCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    [IgnoreDataMember]
    protected IServiceProvider? ServiceProvider { get; }

    public virtual bool IsDirty() => _isDirty;

    public virtual void Invalidate() => _isDirty = false;

    public virtual void MarkAsDirty() => _isDirty = true;

    public abstract object Copy(IDictionary<object, object>? shared);

    protected void RaisePropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    protected void RaiseAndSetIfChanged<T>(ref T field, T value, PropertyChangedEventArgs e)
    {
        if (!Equals(field, value))
        {
            field = value;
            _isDirty = true;
            PropertyChanged?.Invoke(this, e);
        }
    }

    protected void ObserveSelf(
        PropertyChangedEventHandler handler,
        ref IDisposable? propertyDisposable,
        CompositeDisposable? mainDisposable)
    {
        if (propertyDisposable is { })
        {
            mainDisposable?.Remove(propertyDisposable);

            propertyDisposable.Dispose();
            propertyDisposable = default;
        }

        PropertyChanged += handler;

        void Dispose() => PropertyChanged -= handler;

        propertyDisposable = Disposable.Create(Dispose);

        mainDisposable?.Add(propertyDisposable);
    }

    protected void ObserveObject<T>(
        T? obj,
        ref IDisposable? objDisposable,
        CompositeDisposable? mainDisposable,
        IObserver<(object? sender, PropertyChangedEventArgs e)> observer) where T : ViewModelBase
    {
        if (objDisposable is { })
        {
            mainDisposable?.Remove(objDisposable);

            objDisposable.Dispose();
            objDisposable = default;
        }

        if (obj is { })
        {
            objDisposable = obj.Subscribe(observer);

            if (mainDisposable is { } && objDisposable is { })
            {
                mainDisposable.Add(objDisposable);
            }
        }
    }

    protected void ObserveList<T>(
        IEnumerable<T>? list,
        ref CompositeDisposable? listDisposable,
        CompositeDisposable? mainDisposable,
        IObserver<(object? sender, PropertyChangedEventArgs e)> observer) where T : ViewModelBase
    {
        if (listDisposable is { })
        {
            mainDisposable?.Remove(listDisposable);

            listDisposable.Dispose();
            listDisposable = default;
        }

        if (list is { })
        {
            listDisposable = new CompositeDisposable();

            foreach (var item in list)
            {
                var itemDisposable = item.Subscribe(observer);
                if (itemDisposable is { })
                {
                    listDisposable.Add(itemDisposable);
                }
            }

            mainDisposable?.Add(listDisposable);
        }
    }

    public virtual IDisposable? Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var disposablePropertyChanged = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, default);

        return disposablePropertyChanged;

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            observer.OnNext((sender, e));
        }
    }
}
