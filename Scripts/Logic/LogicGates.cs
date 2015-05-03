var c = Context.Editor.Container;
var sg = c.StyleGroups.Where(x => x.Name == "Logic").FirstOrDefault();
if (sg == null)
{
    sg = ShapeStyleGroup.Create("Logic");
    c.StyleGroups.Add(sg);
}
var styles = sg.Styles;
var ps = c.PointShape;

var styleTextBig = ShapeStyle.Create(
    "Logic-Big-Text", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", 14.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextBig);

var styleLineThick = ShapeStyle.Create(
    "Logic-Line-Thick", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, null, 
    "Consolas", 12.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleLineThick);

sg.CurrentStyle = sg.Styles.FirstOrDefault();

void SetShapeState(BaseShape shape, XGroup owner)
{
    shape.Owner = owner;
    shape.State &= ~ShapeState.Standalone;
}

void SetConnectorAsNone(XPoint point, XGroup owner)
{
    point.Owner = owner;
    point.State |= ShapeState.Connector | ShapeState.None;
    point.State &= ~ShapeState.Standalone;
}

void SetConnectorAsInput(XPoint point, XGroup owner)
{
    point.Owner = owner;
    point.State |= ShapeState.Connector | ShapeState.Input;
    point.State &= ~ShapeState.Standalone;
}

void SetConnectorAsOutput(XPoint point, XGroup owner)
{
    point.Owner = owner;
    point.State |= ShapeState.Connector | ShapeState.Output;
    point.State &= ~ShapeState.Standalone;
}

// INPUT signal
XGroup CreateInputSignal()
{
    var g = XGroup.Create("INPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "IN", false, "");
    SetShapeState(label, g);
    g.Shapes.Add(label);

    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    SetShapeState(frame, g);
    g.Shapes.Add(frame);

    var co = XPoint.Create(30, 15, ps, "O");
    SetConnectorAsOutput(co, g);
    g.Connectors.Add(co);

    return g;
}

// OUTPUT signal
XGroup CreateOutputSignal()
{
    var g = XGroup.Create("OUTPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "OUT", false, "");
    SetShapeState(label, g);
    g.Shapes.Add(label);

    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    SetShapeState(frame, g);
    g.Shapes.Add(frame);

    var ci = XPoint.Create(0, 15, ps, "I");
    SetConnectorAsInput(ci, g);
    g.Connectors.Add(ci);

    return g;
}

// AND gate
XGroup CreateAndGate()
{
    var g = XGroup.Create("AND");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "&", false, "");
    SetShapeState(label, g);
    g.Shapes.Add(label);

    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    SetShapeState(frame, g);
    g.Shapes.Add(frame);

    var cl = XPoint.Create(0, 15, ps, "L");
    SetConnectorAsNone(cl, g);
    g.Connectors.Add(cl);

    var cr = XPoint.Create(30, 15, ps, "R");
    SetConnectorAsNone(cr, g);
    g.Connectors.Add(cr);

    var ct = XPoint.Create(15, 0, ps, "T");
    SetConnectorAsNone(ct, g);
    g.Connectors.Add(ct);

    var cb = XPoint.Create(15, 30, ps, "B");
    SetConnectorAsNone(cb, g);
    g.Connectors.Add(cb);

    return g;
}

// OR gate
XGroup CreateOrGate()
{
    var g = XGroup.Create("OR");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "≥1", false, "");
    SetShapeState(label, g);
    g.Shapes.Add(label);

    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    SetShapeState(frame, g);
    g.Shapes.Add(frame);

    var cl = XPoint.Create(0, 15, ps, "L");
    SetConnectorAsNone(cl, g);
    g.Connectors.Add(cl);

    var cr = XPoint.Create(30, 15, ps, "R");
    SetConnectorAsNone(cr, g);
    g.Connectors.Add(cr);

    var ct = XPoint.Create(15, 0, ps, "T");
    SetConnectorAsNone(ct, g);
    g.Connectors.Add(ct);

    var cb = XPoint.Create(15, 30, ps, "B");
    SetConnectorAsNone(cb, g);
    g.Connectors.Add(cb);

    return g;
}

c.Groups.Add(CreateInputSignal());
c.Groups.Add(CreateOutputSignal());
c.Groups.Add(CreateAndGate());
c.Groups.Add(CreateOrGate());