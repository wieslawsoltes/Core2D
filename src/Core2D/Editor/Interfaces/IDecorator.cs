using System.Collections.Generic;
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
    }
}
