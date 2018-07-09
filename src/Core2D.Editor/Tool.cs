// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using Core2D.Editor.Input;
using Core2D.Shape;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines tool contract.
    /// </summary>
    public abstract class Tool : ObservableObject
    {
        internal static bool s_enableDebug = false;

        /// <summary>
        /// Gets the tool title.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public virtual void LeftDown(InputArgs args)
        {
            Debug.WriteLineIf(s_enableDebug, $"[{Title}] LeftDown X={args.X} Y={args.Y}, Modifier {args.Modifier}");
        }

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public virtual void LeftUp(InputArgs args)
        {
            Debug.WriteLineIf(s_enableDebug, $"[{Title}] LeftDown X={args.X} Y={args.Y}, Modifier {args.Modifier}");
        }

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public virtual void RightDown(InputArgs args)
        {
            Debug.WriteLineIf(s_enableDebug, $"[{Title}] LeftDown X={args.X} Y={args.Y}, Modifier {args.Modifier}");
        }

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public virtual void RightUp(InputArgs args)
        {
            Debug.WriteLineIf(s_enableDebug, $"[{Title}] LeftDown X={args.X} Y={args.Y}, Modifier {args.Modifier}");
        }

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public virtual void Move(InputArgs args)
        {
            Debug.WriteLineIf(s_enableDebug, $"[{Title}] LeftDown X={args.X} Y={args.Y}, Modifier {args.Modifier}");
        }

        /// <summary>
        /// Move edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public virtual void Move(BaseShape shape)
        {
        }

        /// <summary>
        /// Finalize edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public virtual void Finalize(BaseShape shape)
        {
        }

        /// <summary>
        /// Remove edited shape.
        /// </summary>
        public virtual void Remove()
        {
        }
    }
}
