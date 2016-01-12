// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Minimum translation vector for Separating Axis Theorem implementation.
    /// </summary>
    public struct MinimumTranslationVector
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector2 Smallest { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double Overlap { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallest"></param>
        /// <param name="overlap"></param>
        public MinimumTranslationVector(Vector2 smallest, double overlap)
            : this()
        {
            this.Smallest = smallest;
            this.Overlap = overlap;
        }
    }
}
