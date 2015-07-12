var g = Geometry();
g.BeginFigure(Point(30, 30), false);
g.BezierTo(Point(30, 60), Point(60, 60), Point(60, 30));
Path(g);