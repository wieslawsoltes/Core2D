using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using DynamicData;
using ReactiveUI;
using Xunit;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using System.Diagnostics;

namespace Demo
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
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();
    }

    public class LayerUnitTest
    {
        [Fact]
        public void TestChanges()
        {
            var layer = new Layer();

            var changes = new Stack<IChangeSet<Item>>();

            var propertyChanges = new Stack<(Action, Item, object)>();

            var observableItems = layer.Items.ToObservableChangeSet().AsObservableList();

            observableItems.Connect().Subscribe((IChangeSet<Item> changeSet) =>
            {
                changes.Push(changeSet);
                Console.WriteLine($"Changes: {changeSet}");
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
            });

            observableItems.Connect().WhenPropertyChanged((Item x) => x.Name).Subscribe(x =>
            {
                var item = x.Sender;
                var value = x.Value;
                propertyChanges.Push((() => item.Name = value, item, value));
            });



            var item0 = new Item() { Name = "Item0" };
            var item1 = new Item() { Name = "Item1" };

            layer.Items.Add(item0);
            layer.Items.RemoveAt(0);
            layer.Items.Add(item1);



            foreach (var change in propertyChanges)
            {
                Console.WriteLine($"propertyChange: {change.Item3}");
            }



            item0.WhenAnyValue(x => x.Name).Skip(1).Subscribe(x => Console.WriteLine($"item0.Name = {x}"));
            item0.Name = "Item0-0";
            item0.Name = "Item0-1";
            item0.Name = "Item0-2";


        }
    }
}
