// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void LeftDown(double x, double y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void LeftUp(double x, double y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void RightDown(double x, double y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void RightUp(double x, double y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void Move(double x, double y);

        /// <summary>
        /// 
        /// </summary>
        public abstract void ToStateOne();

        /// <summary>
        /// 
        /// </summary>
        public abstract void ToStateTwo();

        /// <summary>
        /// 
        /// </summary>
        public abstract void ToStateThree();

        /// <summary>
        /// 
        /// </summary>
        public abstract void ToStateFour();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public abstract void Move(BaseShape shape);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public abstract void Finalize(BaseShape shape);

        /// <summary>
        /// 
        /// </summary>
        public abstract void Remove();
    }
}
