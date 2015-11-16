// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Helper class for <see cref="Tool.None"/> editor.
    /// </summary>
    public class NoneHelper : Helper
    {
        private Editor _editor;

        /// <summary>
        /// Initialize new instance of <see cref="NoneHelper"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="NoneHelper"/> object.</param>
        public NoneHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void LeftUp(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void RightUp(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateFour()
        {
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public override void Remove()
        {
        }
    }
}
