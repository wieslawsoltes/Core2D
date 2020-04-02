#r "Core2D"
#r "Math.Spatial"
using static System.Math;
using Core2D.Editor.Layout;
using Core2D.Renderer;
using Spatial;

Matrix2 Get(IMatrixObject t) { return new Matrix2(t.M11, t.M12, t.M21, t.M22, t.OffsetX, t.OffsetY); }
void Set(IMatrixObject t, Matrix2 m) { t.M11 = m.M11; t.M12 = m.M12; t.M21 = m.M21;  t.M22 = m.M22; t.OffsetX = m.OffsetX; t.OffsetY = m.OffsetY; }

var s = Editor.PageState.SelectedShape;
var b = new ShapeBox(s);
var t = s.Transform;

//Set(t, Matrix2.Translate(30, 0));
//Set(t, Matrix2.TranslatePrepend(Get(t), 30, 0));
//Set(t, Matrix2.Scale(2, 2));
//Set(t, Matrix2.ScaleAt(2, 2, 0, 0));
//Set(t, Matrix2.ScaleAtPrepend(Get(t), 2, 2, 0, 0));
//Set(t, Matrix2.Skew(PI/4, 0));
//Set(t, Matrix2.Rotation(PI/4));
//Set(t, Matrix2.Rotation(PI/4, 0, 0));
Set(t, Matrix2.Rotation(PI/4, b.CenterX, b.CenterY));
