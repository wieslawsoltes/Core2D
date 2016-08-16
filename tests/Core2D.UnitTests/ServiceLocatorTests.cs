// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Xunit;

namespace Core2D.UnitTests
{
    public class ServiceLocatorTests
    {
        [Fact]
        [Trait("Core2D", "Base")]
        public void Instance_Is_Initialized()
        {
            Assert.NotNull(ServiceLocator.Instance);
            Assert.NotNull(ServiceLocator.Instance._singletons);
            Assert.NotNull(ServiceLocator.Instance._singletonsCache);
            Assert.NotNull(ServiceLocator.Instance._transients);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Instance_Is_Singleton()
        {
            Assert.Same(ServiceLocator.Instance, ServiceLocator.Instance);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Factory_Method_Added_To_Transient_Registry()
        {
            Func<Class1> func = () => new Class1();
            ServiceLocator.Instance.RegisterTransient<ITest1>(func);
            Assert.Same(ServiceLocator.Instance._transients[typeof(ITest1)], func);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Factory_Method_Added_To_Singleton_Registry()
        {
            Func<Class2> func = () => new Class2();
            ServiceLocator.Instance.RegisterSingleton<ITest2>(func);
            Assert.Same(ServiceLocator.Instance._singletons[typeof(ITest2)], func);
            Assert.True(ServiceLocator.Instance._singletonTypes.Contains(typeof(ITest2)));
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resolve_Multiple_Instances()
        {
            ServiceLocator.Instance.RegisterTransient<ITest3>(() => new Class3());
            Assert.NotSame(
                ServiceLocator.Instance.Resolve<ITest3>(),
                ServiceLocator.Instance.Resolve<ITest3>());
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resolve_Multiple_Instances_Array()
        {
            ServiceLocator.Instance.RegisterTransient<IEnumerable<ITest3>>(() => new Class3[] { new Class3(), new Class3() });
            Assert.NotSame(
                ServiceLocator.Instance.Resolve<IEnumerable<ITest3>>(),
                ServiceLocator.Instance.Resolve<IEnumerable<ITest3>>());
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resolve_Singletons()
        {
            ServiceLocator.Instance.RegisterSingleton<ITest4>(() => new Class4());
            Assert.Same(
                ServiceLocator.Instance.Resolve<ITest4>(),
                ServiceLocator.Instance.Resolve<ITest4>());
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resolve_Singletons_Array()
        {
            ServiceLocator.Instance.RegisterSingleton<IEnumerable<ITest4>>(() => new Class4[] { new Class4(), new Class4() });
            Assert.Same(
                ServiceLocator.Instance.Resolve<IEnumerable<ITest4>>(),
                ServiceLocator.Instance.Resolve<IEnumerable<ITest4>>());
        }

        [Fact(Skip = "Resolving cycling dependencies is not supported. Use the Lazy<T> initialization instead.")]
        [Trait("Core2D", "Base")]
        public void Resolving_Cyclic_Depenencies_Throws()
        {
            ServiceLocator.Instance.RegisterTransient<CyclicClassA>(() => new CyclicClassA());
            ServiceLocator.Instance.RegisterTransient<CyclicClassB>(() => new CyclicClassB());
            Assert.NotNull(ServiceLocator.Instance.Resolve<CyclicClassA>());
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resolving_Cyclic_Depenencies_Using_LazyOfT()
        {
            ServiceLocator.Instance.RegisterTransient<LazyClassA>(() => new LazyClassA());
            ServiceLocator.Instance.RegisterTransient<LazyClassB>(() => new LazyClassB());

            var a = ServiceLocator.Instance.Resolve<LazyClassA>();
            var b = ServiceLocator.Instance.Resolve<LazyClassB>();
            Assert.NotNull(a);
            Assert.NotNull(b);
            Assert.NotNull(a.B);
            Assert.NotNull(b.A);
        }

        interface ITest1 { }
        interface ITest2 { }
        interface ITest3 { }
        interface ITest4 { }

        class Class1 : ITest1 { }
        class Class2 : ITest2 { }
        class Class3 : ITest3 { }
        class Class4 : ITest4 { }

        class CyclicClassA
        {
            public CyclicClassB B { get; set; }

            public CyclicClassA()
            {
                B = ServiceLocator.Instance.Resolve<CyclicClassB>();
            }
        }

        class CyclicClassB
        {
            public CyclicClassA A { get; set; }

            public CyclicClassB()
            {
                A = ServiceLocator.Instance.Resolve<CyclicClassA>();
            }
        }

        class LazyClassA
        {
            private readonly Lazy<LazyClassB> _b =
                new Lazy<LazyClassB>(() => ServiceLocator.Instance.Resolve<LazyClassB>());

            public LazyClassB B
            {
                get { return _b.Value; }
            }

            public LazyClassA()
            {
            }
        }

        class LazyClassB
        {
            private readonly Lazy<LazyClassA> _a =
                new Lazy<LazyClassA>(() => ServiceLocator.Instance.Resolve<LazyClassA>());

            public LazyClassA A
            {
                get { return _a.Value; }
            }

            public LazyClassB()
            {
            }
        }
    }
}
