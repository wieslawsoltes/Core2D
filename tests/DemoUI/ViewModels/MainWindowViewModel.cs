using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace DemoUI.ViewModels
{
    public class Item : ReactiveObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public override string ToString()
        {
            return $"{_name}";
        }
    }

    public class Layer : Item
    {
        public ObservableCollection<Item> Items { get; set; }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private Layer _layer;
        private Stack<IChangeSet<Item>> _changes = new Stack<IChangeSet<Item>>();
        private int _count;

        public MainWindowViewModel()
        {
            _layer = new Layer()
            {
                Items = new ObservableCollection<Item>()
            };
            _count = 0;

            var undoRedoStack = new Stack<(Action undo, Action redo)>();

            var observableItems = _layer.Items.ToObservableChangeSet().AsObservableList();

            observableItems.Connect().Subscribe((IChangeSet<Item> changeSet) =>
            {
                _changes.Push(changeSet);
            });

            observableItems.Connect().WhenPropertyChanged((Item x) => x.Name, notifyOnInitialValue: true).Subscribe(x =>
            {
                var item = x.Sender;
                var value = x.Value;
                var previous = item.Name;
                //Debug.WriteLine($"previous: {previous}, next: {item.Name}");
                undoRedoStack.Push((() => item.Name = value, () => item.Name = previous));
            });

            UndoCommand = ReactiveCommand.Create(() =>
            {
                var undo = undoRedoStack.Pop();
            });

            RedoCommand = ReactiveCommand.Create(() =>
            {

            });

            AddItemCommand = ReactiveCommand.Create<Unit, Item>((param) =>
            {
                var item = new Item() { Name = $"Item{_count++}" };

                item.WhenAnyValue(x => x.Name).Skip(0).Subscribe(x =>
                {

                    Debug.WriteLine($"previous: {x}, next: {x}");
                });

                _layer.Items.Add(item);

                return item;
            });

            RemoveItemCommand = ReactiveCommand.Create<Item, Unit>((item) =>
            {
                _layer.Items.Remove(item);

                return Unit.Default;
            });

            InsertItemCommand = ReactiveCommand.Create<int, Item>((index) =>
            {
                var item = new Item() { Name = $"Item{_count++}" };

                item.WhenAnyValue(x => x.Name).Skip(0).Subscribe(x =>
                {
                    //
                });

                _layer.Items.Insert(index, item);

                return item;
            });
        }

        private void Undo(IChangeSet<Item> changeSet, ObservableCollection<Item> items)
        {
            foreach (Change<Item> change in changeSet)
            {
                switch (change.Reason)
                {
                    case ListChangeReason.Add:
                        break;

                    case ListChangeReason.AddRange:
                        break;

                    case ListChangeReason.Replace:
                        break;

                    case ListChangeReason.Remove:
                        break;

                    case ListChangeReason.RemoveRange:
                        break;

                    case ListChangeReason.Refresh:
                        break;

                    case ListChangeReason.Moved:
                        break;

                    case ListChangeReason.Clear:
                        break;
                }
                Console.WriteLine($"  Change: {change.Reason} {change}");
            }
        }

        public ReactiveCommand<Unit, Unit> UndoCommand { get; }

        public ReactiveCommand<Unit, Unit> RedoCommand { get; }

        public ReactiveCommand<Unit, Item> AddItemCommand { get; }

        public ReactiveCommand<Item, Unit> RemoveItemCommand { get; }

        public ReactiveCommand<int, Item> InsertItemCommand { get; }

        public Layer Layer
        {
            get => _layer;
            set => this.RaiseAndSetIfChanged(ref _layer, value);
        }
    }
}
