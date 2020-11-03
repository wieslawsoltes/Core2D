using System;
using System.Collections.Generic;
using ObjectType = System.Int32;
using ObjectID = System.Int64;
using Property = System.String;
using Value = System.Object;

namespace DbDemo
{
    public class PropertiesDictionary : Dictionary<Property, Value>
    {
    }

    public class ObjectsDictionary : Dictionary<ObjectID, PropertiesDictionary>
    {
    }

    public class Store
    {
        public Dictionary<ObjectID, Dictionary<Property, Value>> Objects { get; set; }

        public Dictionary<ObjectID, Value> Cache { get; set; }

        public Value GetValue(ObjectID id, ObjectType type)
        {
            switch (type)
            {
                case Types.Point:
                    {
                        if (Cache.TryGetValue(id, out var value))
                        {
                            return value;
                        }

                        var point = new Point(this, id);

                        Cache[id] = point;

                        return point;
                    }
                case Types.Line:
                    {
                        if (Cache.TryGetValue(id, out var value))
                        {
                            return value;
                        }

                        var line = new Line(this, id);

                        Cache[id] = line;

                        return line;
                    }
            }
            return null;
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
            get => (ObjectType)Store.Objects[Id][nameof(Type)];
            set => Store.Objects[Id][nameof(Type)] = value;
        }
    }

    public class Point : Base
    {
        public Point(Store store, ObjectID id) : base(store, id)
        {
        }

        public double X
        {
            get => (double)Store.Objects[Id][nameof(X)];
            set => Store.Objects[Id][nameof(X)] = value;
        }

        public double Y
        {
            get => (double)Store.Objects[Id][nameof(Y)];
            set => Store.Objects[Id][nameof(Y)] = value;
        }
    }

    public class Line : Base
    {
        public Line(Store store, ObjectID id) : base(store, id)
        {
        }

        public ObjectID StartID
        {
            get => (ObjectID)Store.Objects[Id][nameof(Start)];
            set => Store.Objects[Id][nameof(Start)] = value;
        }

        public ObjectID EndID
        {
            get => (ObjectID)Store.Objects[Id][nameof(End)];
            set => Store.Objects[Id][nameof(End)] = value;
        }

        public Point Start => (Point)Store.GetValue(StartID, Types.Point);

        public Point End => (Point)Store.GetValue(EndID, Types.Point);
    }

    public static class Program
    {
        public static void Main()
        {
            var store = new Store()
            {
                Objects = new Dictionary<ObjectID, Dictionary<Property, Value>>(),
                Cache = new Dictionary<ObjectID, object>()
            };

            // Point, Start
            store.Objects[0] = new Dictionary<Property, Value>()
            {
                ["Type"] = Types.Point, // ObjectType
                ["X"] = 5.0, // double
                ["Y"] = 10.0 // double
            };

            // Point, End
            store.Objects[1] = new Dictionary<Property, Value>()
            {
                ["Type"] = Types.Point, // ObjectType
                ["X"] = 27.0, // double
                ["Y"] = 32.0 // double
            };

            // Line
            store.Objects[2] = new Dictionary<Property, Value>()
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

            var line = (Line)store.GetValue(2, Types.Line);
            Console.WriteLine($"{line.Start.X}");
            Console.WriteLine($"{line.Start.Y}");
            Console.WriteLine($"{line.End.X}");
            Console.WriteLine($"{line.End.Y}");
        }
    }
}