using System.Collections.Generic;
using Core2D.Editor.Input;
using Core2D.Shapes;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines decorator contract.
    /// </summary>
    public interface IDecorator : IObservableObject, IDrawable
    {
        /// <summary>
        ///  Gets or sets decorated shapes.
        /// </summary>
        IList<IBaseShape> Shapes { get; set; }

        /// <summary>
        /// Update decorator.
        /// </summary>
        /// <param name="rebuild">The flag indicating whether to rebuild box.</param>
        void Update(bool rebuild);

        /// <summary>
        /// Show decorator.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide decorator.
        /// </summary>
        void Hide();

        /// <summary>
        /// Hit test decorator.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>True if handle is available.</returns>
        bool HitTest(InputArgs args);

        /// <summary>
        /// Move decorator.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void Move(InputArgs args);
    }
}
