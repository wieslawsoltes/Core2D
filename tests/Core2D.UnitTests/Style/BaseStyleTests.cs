// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Style;
using Xunit;

namespace Core2D.UnitTests
{
    public class BaseStyleTests
    {
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Class1();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDoubleArrayToDashes_Should_Return_Dashes_String()
        {
            var dashes = new double[] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 };
            var expected = "2 2 0 2 0 2";

            var target = BaseStyle.ConvertDoubleArrayToDashes(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertFloatArrayToDashes_Should_Return_Dashes_String()
        {
            var dashes = new float[] { 2.0f, 2.0f, 0.0f, 2.0f, 0.0f, 2.0f };
            var expected = "2 2 0 2 0 2";

            var target = BaseStyle.ConvertFloatArrayToDashes(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToDoubleArray_Should_Return_Dashes_Double_Array()
        {
            var dashes = "2 2 0 2 0 2";
            var expected = new double[] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 };

            var target = BaseStyle.ConvertDashesToDoubleArray(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToFloatArray_Should_Return_Dashes_Float_Array()
        {
            var dashes = "2 2 0 2 0 2";
            var expected = new float[] { 2.0f, 2.0f, 0.0f, 2.0f, 0.0f, 2.0f };

            var target = BaseStyle.ConvertDashesToFloatArray(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDoubleArrayToDashes_Should_Not_Throw()
        {
            var target = BaseStyle.ConvertDoubleArrayToDashes(null);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertFloatArrayToDashes_Should_Not_Throw()
        {
            var target = BaseStyle.ConvertFloatArrayToDashes(null);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToDoubleArray_Should_Not_Throw()
        {
            var dashes = "0 A";
            var target = BaseStyle.ConvertDashesToDoubleArray(dashes);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToFloatArray_Should_Not_Throw()
        {
            var dashes = "0 A";
            var target = BaseStyle.ConvertDashesToFloatArray(dashes);
            Assert.Null(target);
        }

        private class Class1 : BaseStyle
        {
            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }
        }
    }
}
