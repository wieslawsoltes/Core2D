#region netDxf, Copyright(C) 2015 Daniel Carvajal, Licensed under LGPL.
// 
//                         netDxf library
//  Copyright (C) 2009-2015 Daniel Carvajal (haplokuon@gmail.com)
//  
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace netDxf
{
    /// <summary>
    /// Represent a clipping boundary to display specific portions of
    /// an <see cref="netDxf.Entities.Image">Image</see>,
    /// an <see cref="netDxf.Entities.Underlay">Underlay</see>,
    /// or a <see cref="netDxf.Entities.Wipeout">Wipeout</see>.
    /// </summary>
    public class ClippingBoundary :
        ICloneable
    {
        #region private fields

        private readonly ClippingBoundaryType type;
        private readonly List<Vector2> vertexes;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ClippingBoundary</c> class as a rectangular clipping boundary.
        /// </summary>
        /// <param name="x">Rectangle x-coordinate of the bottom-left corner in image local coordinates.</param>
        /// <param name="y">Rectangle y-coordinate of the bottom-left corner in image local coordinates.</param>
        /// <param name="width">Rectangle width in image local coordinates.</param>
        /// <param name="height">Rectangle height in image local coordinates.</param>
        public ClippingBoundary(double x, double y, double width, double height)
        {
            this.type = ClippingBoundaryType.Rectangular;
            this.vertexes = new List<Vector2> { new Vector2(x, y), new Vector2(x + width, y + height) };
        }

        /// <summary>
        /// Initializes a new instance of the <c>ClippingBoundary</c> class as a rectangular clipping boundary from two opposite corners.
        /// </summary>
        /// <param name="firstCorner">Rectangle top-left corner in image local coordinates.</param>
        /// <param name="secondCorner">Rectangle secondCorner in local coordinates.</param>
        public ClippingBoundary(Vector2 firstCorner, Vector2 secondCorner)
        {
            this.type = ClippingBoundaryType.Rectangular;
            this.vertexes = new List<Vector2> { firstCorner, secondCorner };
        }

        /// <summary>
        /// Initializes a new instance of the <c>ClippingBoundary</c> class as a polygonal clipping boundary.
        /// </summary>
        /// <param name="vertexes">The list of vertexes of the polygonal boundary.</param>
        public ClippingBoundary(IList<Vector2> vertexes)
        {
            if (vertexes.Count < 3)
                throw new ArgumentException("The number of vertexes for the polygonal clipping boundary must be equal or greater than three.", nameof(vertexes));

            this.type = ClippingBoundaryType.Polygonal;
            this.vertexes = new List<Vector2>(vertexes);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the clipping boundary type, rectangular or polygonal.
        /// </summary>
        public ClippingBoundaryType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the list of vertexes of the polygonal boundary, or the opposite vertexes if the boundary is rectangular.
        /// </summary>
        public ReadOnlyCollection<Vector2> Vertexes
        {
            get { return this.vertexes.AsReadOnly(); }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new ClippingBoundary that is a copy of the current instance.
        /// </summary>
        /// <returns>A new ClippingBoundary that is a copy of this instance.</returns>
        public object Clone()
        {
            return this.type == ClippingBoundaryType.Rectangular ? new ClippingBoundary(this.vertexes[0], this.vertexes[1]) : new ClippingBoundary(this.vertexes);
        }

        #endregion
    }
}
