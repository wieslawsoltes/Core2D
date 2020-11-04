using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Core2D.UnitTests
{
    public class ObservableObjectTests
    {
        [Fact]
        [Trait("Core2D", "Base")]
        public void Implements_INotifyPropertyChanged_Interface()
        {
            var target = new Class1();
            Assert.True(target is INotifyPropertyChanged);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Notify_PropertyChanged_Event_Is_Raised()
        {
            var target = new Class1();
            string actual = null;

            target.PropertyChanged += (sender, e) =>
            {
                actual = e.PropertyName;
            };

            target.TestProperty = "Test";
            Assert.Equal("TestProperty", actual);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Update_Backing_Field_And_Raise_PropertyChanged_Event()
        {
            var target = new Class1();
            bool raised = false;

            target.PropertyChanged += (sender, e) =>
            {
                raised = true;
            };

            target.TestProperty = "Test";

            Assert.True(raised);
            Assert.Equal("Test", target.TestProperty);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Do_Not_Update_Backing_Field_If_Value_Is_Equal()
        {
            var target = new Class1();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            target.TestProperty = "Test";
            target.TestProperty = "Test";

            Assert.Equal(1, count);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Update_Returns_Boolean()
        {
            var target = new Class1();

            target.TestProperty = "Test";
            Assert.True(target.TestPropertyUpdated);

            target.TestProperty = "Test";
            Assert.False(target.TestPropertyUpdated);
        }

        private class Class1 : ObservableObject
        {
            public bool TestPropertyUpdated;

            private string _testProperty;
            public string TestProperty
            {
                get { return _testProperty; }
                set
                {
                    TestPropertyUpdated = RaiseAndSetIfChanged(ref _testProperty, value);
                }
            }

            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }
        }
    }
}
