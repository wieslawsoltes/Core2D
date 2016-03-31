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

namespace netDxf.Entities
{
    /// <summary>
    /// Defines the vertical position of a leader with a text annotation.
    /// </summary>
    public enum LeaderTextVerticalPosition
    {
        /// <summary>
        /// The text annotation is placed in the leader hook with respect of its alignment point.
        /// </summary>
        Centered = 0,
        /// <summary>
        /// The text annotation is placed above the leader hook line.
        /// </summary>
        Above = 1,
        /// <summary>
        /// This options seems to have no effect.
        /// </summary>
        Outside = 2,
        /// <summary>
        /// This options seems to have no effect.
        /// </summary>
        JIS = 3,
        /// <summary>
        /// This options seems to have no effect.
        /// </summary>
        Bellow = 4
    }
}
