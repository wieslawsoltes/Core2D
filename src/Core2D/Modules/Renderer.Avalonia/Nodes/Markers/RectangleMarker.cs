namespace Core2D.Renderer
{
    internal class RectangleMarker : Marker
    {
        public Avalonia.Rect Rect { get; set; }

        public override void Draw(object dc)
        {
            var context = dc as Avalonia.Media.DrawingContext;

            using var rotationDisposable = context.PushPreTransform(Rotation);

            if (Shape.IsFilled)
            {
                context.FillRectangle(Brush, Rect);
            }

            if (Shape.IsStroked)
            {
                context.DrawRectangle(Pen, Rect);
            }
        }
    }
}
