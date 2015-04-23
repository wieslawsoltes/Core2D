var c = Context.Editor.Container;
var layer = c.CurrentLayer;
var styles = c.Styles;
var ps = c.PointShape;

var styleTextBig = ShapeStyle.Create(
    "Gate-Big-Text", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", 14.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextBig);

var styleLineThick = ShapeStyle.Create(
    "Gate-Line-Thick", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, null, 
    "Consolas", 12.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleLineThick);

var styleConnector = ShapeStyle.Create(
    "Connector", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    1.0, 
    null, 
    "Consolas", 12.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleConnector);

{
    var g = XGroup.Create("AND");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "&", false, "");
    g.Shapes.Add(label);
    
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    g.Shapes.Add(frame);

    var connectorShape = XEllipse.Create(-3, -3, 3, 3, styleConnector, null, true, "");
    
    var cl = XPoint.Create(0, 15, connectorShape, "L");
    g.Connectors.Add(cl);
    
    var cr = XPoint.Create(30, 15, connectorShape, "R");
    g.Connectors.Add(cr);
    
    var ct = XPoint.Create(15, 0, connectorShape, "T");
    g.Connectors.Add(ct);
    
    var cb = XPoint.Create(15, 30, connectorShape, "B");
    g.Connectors.Add(cb);
    
    layer.Shapes.Add(g);
}

{
    var g = XGroup.Create("OR");

    var label = XText.Create(0, 0, 30, 30, styleTextBig, ps, "≥1", false, "");
    g.Shapes.Add(label);
    
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    g.Shapes.Add(frame);

    var connectorShape = XEllipse.Create(-3, -3, 3, 3, styleConnector, null, true, "");
    
    var cl = XPoint.Create(0, 15, connectorShape, "L");
    g.Connectors.Add(cl);
    
    var cr = XPoint.Create(30, 15, connectorShape, "R");
    g.Connectors.Add(cr);
    
    var ct = XPoint.Create(15, 0, connectorShape, "T");
    g.Connectors.Add(ct);
    
    var cb = XPoint.Create(15, 30, connectorShape, "B");
    g.Connectors.Add(cb);
    
    layer.Shapes.Add(g);
}

layer.Invalidate();