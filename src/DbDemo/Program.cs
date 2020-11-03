using System;
using System.Collections.Generic;
using ObjectType = System.Int32;
using ObjectID = System.Int64;
using Property = System.String;
using Value = System.Object;

namespace DbDemo
{
    public class PropertyDictionary : Dictionary<Property, Value>
    {
    }

    public class ObjectDictionary : Dictionary<ObjectID, PropertyDictionary>
    {
    }

    public class ValueDictionary : Dictionary<ObjectID, Value>
    {
    }

    public class Store
    {
        public ObjectDictionary Objects { get; set; }

        public ValueDictionary Cache { get; set; }

        public T GetValueObject<T>(ObjectID id, ObjectType type) where T : Base
        {
            switch (type)
            {
                case Types.Point:
                    {
                        if (Cache.TryGetValue(id, out var value))
                        {
                            return value as T;
                        }

                        var point = new Point(this, id);

                        Cache[id] = point;

                        return  point as T;
                    }
                case Types.Line:
                    {
                        if (Cache.TryGetValue(id, out var value))
                        {
                            return value as T;
                        }

                        var line = new Line(this, id);

                        Cache[id] = line;

                        return line as T;
                    }
            }
            return default(T);
        }
    }

    public static class Types
    {
        public const ObjectType Point = 1;
        public const ObjectType Line = 2;
    }

    public abstract class Base
    {
        private readonly Store _store;
        private readonly ObjectID _id;

        public Base(Store store, ObjectID id)
        {
            _id = id;
            _store = store;
        }

        public Store Store => _store;

        public ObjectID Id => _id;

        public ObjectType Type
        {
            get => GetValue<ObjectType>(Id, nameof(Type));
            set => SetValue(Id, nameof(Type), value);
        }
  
        public T GetValue<T>(ObjectID id, string key)
        {
            return (T)_store.Objects[id][key];
        }
        public void SetValue<T>(ObjectID id, string key, T value)
        {
            _store.Objects[id][key] = value;
        }
    }

    public class Point : Base
    {
        public Point(Store store, ObjectID id) : base(store, id)
        {
        }

        public double X
        {
            get => GetValue<double>(Id, nameof(X));
            set => SetValue(Id, nameof(X), value);
        }

        public double Y
        {
            get => GetValue<double>(Id, nameof(Y));
            set => SetValue(Id, nameof(Y), value);
        }
    }

    public class Line : Base
    {
        public Line(Store store, ObjectID id) : base(store, id)
        {
        }

        public ObjectID StartID
        {
            get => GetValue<ObjectID>(Id, nameof(Start));
            set => SetValue(Id, nameof(Start), value);
        }

        public ObjectID EndID
        {
            get => GetValue<ObjectID>(Id, nameof(End));
            set => SetValue(Id, nameof(End), value);
        }

        public Point Start => Store.GetValueObject<Point>(StartID, Types.Point);

        public Point End => Store.GetValueObject<Point>(EndID, Types.Point);
    }

    public static class Program
    {
        public static void Main()
        {
            var store = new Store()
            {
                Objects = new ObjectDictionary(),
                Cache = new ValueDictionary()
            };

            // Point, Start
            store.Objects[0] = new PropertyDictionary()
            {
                ["Type"] = Types.Point, // ObjectType
                ["X"] = 5.0, // double
                ["Y"] = 10.0 // double
            };

            // Point, End
            store.Objects[1] = new PropertyDictionary()
            {
                ["Type"] = Types.Point, // ObjectType
                ["X"] = 27.0, // double
                ["Y"] = 32.0 // double
            };

            // Line
            store.Objects[2] = new PropertyDictionary()
            {
                ["Type"] = Types.Line, // ObjectType
                ["Start"] = (ObjectID)0, // Point, ObjectID
                ["End"] = (ObjectID)1 // Point, ObjectID
            };

            foreach (var obj in store.Objects)
            {
                Console.WriteLine($"[{obj.Key}]={obj.Value}");
                foreach (var prop in obj.Value)
                {
                    Console.WriteLine($"  [{obj.Key}:{prop.Key}]={prop.Value}");
                }
            }

            var line = store.GetValueObject<Line>(2, Types.Line);
            Console.WriteLine($"{line.Start.X}");
            Console.WriteLine($"{line.Start.Y}");
            Console.WriteLine($"{line.End.X}");
            Console.WriteLine($"{line.End.Y}");
        }
    }
}