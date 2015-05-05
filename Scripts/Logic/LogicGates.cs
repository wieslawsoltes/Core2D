var c = Context.Editor.Container;
var sg = c.StyleGroups.Where(x => x.Name == "Logic").FirstOrDefault();
if (sg == null)
{
    sg = ShapeStyleGroup.Create("Logic");
    c.StyleGroups.Add(sg);
}
var styles = sg.Styles;
var ps = c.PointShape;

var styleTextMediumLC = ShapeStyle.Create(
    "Logic-Text-Medium-LC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", 11.0, TextHAlignment.Left, TextVAlignment.Center);
styles.Add(styleTextMediumLC);

var styleTextMediumCC = ShapeStyle.Create(
    "Logic-Text-Medium-CC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", 11.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextMediumCC);

var styleTextBigCC = ShapeStyle.Create(
    "Logic-Text-Big-CC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", 14.0, TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextBigCC);

var styleLineThick = ShapeStyle.Create(
    "Logic-Line-Thick", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null,
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

// INPUT
XGroup CreateInputSignal()
{
    var g = XGroup.Create("INPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var co = XPoint.Create(30, 15, ps, "O");

    var labelProperty = ShapeProperty.Create("IN");
    g.AddProperty("Label", labelProperty);
    label.Properties = new [] { labelProperty };

    SetShapeState(label, g);
    SetShapeState(frame, g);
    SetConnectorAsOutput(co, g);
    
    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(co);

    return g;
}

// OUTPUT
XGroup CreateOutputSignal()
{
    var g = XGroup.Create("OUTPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var ci = XPoint.Create(0, 15, ps, "I");
    
    var labelProperty = ShapeProperty.Create("OUT");
    g.AddProperty("Label", labelProperty);
    label.Properties = new [] { labelProperty };

    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(ci);
    
    SetShapeState(label, g);
    SetConnectorAsInput(ci, g);
    SetShapeState(frame, g);
    
    return g;
}

// SIGNAL
XGroup CreateSignal()
{
    var g = XGroup.Create("SIGNAL");

    var labelDesignation = XText.Create(5, 0, 205, 15, styleTextMediumLC, ps, "{0}", false, "");
    var labelDescription = XText.Create(5, 15, 205, 30, styleTextMediumLC, ps, "{0}", false, "");
    var labelSignal = XText.Create(215, 0, 295, 15, styleTextMediumLC, ps, "{0}", false, "");
    var labelCondition = XText.Create(215, 15, 295, 30, styleTextMediumLC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 300, 30, styleLineThick, ps, false, "");
    var separator = XLine.Create(210, 0, 210, 30, styleLineThick, ps);
    var ci = XPoint.Create(0, 15, ps, "I");
    var co = XPoint.Create(300, 15, ps, "O");
   
    var designationProperty = ShapeProperty.Create("Designation");
    g.AddProperty("Designation", designationProperty);
    labelDesignation.Properties = new [] { designationProperty };

    var descriptionProperty = ShapeProperty.Create("Description");
    g.AddProperty("Description", descriptionProperty);
    labelDescription.Properties = new [] { descriptionProperty };
    
    var signalProperty = ShapeProperty.Create("Signal");
    g.AddProperty("Signal", signalProperty);
    labelSignal.Properties = new [] { signalProperty };
    
    var conditionProperty = ShapeProperty.Create("Condition");
    g.AddProperty("Condition", conditionProperty);
    labelCondition.Properties = new [] { conditionProperty };
    
    SetShapeState(labelDesignation, g);
    SetShapeState(labelDescription, g);
    SetShapeState(labelSignal, g);
    SetShapeState(labelCondition, g);
    SetShapeState(frame, g);
    SetShapeState(separator, g);
    SetConnectorAsInput(ci, g);
    SetConnectorAsOutput(co, g);
    
    g.Shapes.Add(labelDesignation);
    g.Shapes.Add(labelDescription);
    g.Shapes.Add(labelSignal);
    g.Shapes.Add(labelCondition);
    g.Shapes.Add(frame);
    g.Shapes.Add(separator);
    g.Connectors.Add(ci);
    g.Connectors.Add(co);
    
    return g;
}

// AND
XGroup CreateAndGate()
{
    var g = XGroup.Create("AND");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var labelProperty = ShapeProperty.Create("&");
    g.AddProperty("Label", labelProperty);
    label.Properties = new [] { labelProperty };
    
    SetShapeState(label, g);
    SetShapeState(frame, g);
    SetConnectorAsNone(cl, g);
    SetConnectorAsNone(cr, g);
    SetConnectorAsNone(ct, g); 
    SetConnectorAsNone(cb, g);
                
    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(cl);
    g.Connectors.Add(cr);
    g.Connectors.Add(ct);
    g.Connectors.Add(cb);
   
    return g;
}

// OR
XGroup CreateOrGate()
{
    var g = XGroup.Create("OR");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}{1}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var prefixProperty = ShapeProperty.Create("≥");
    var counterProperty = ShapeProperty.Create("1");
    g.AddProperty("Prefix", prefixProperty);
    g.AddProperty("Counter", counterProperty);
    label.Properties = new [] { prefixProperty, counterProperty };

    SetShapeState(label, g);
    SetShapeState(frame, g);
    SetConnectorAsNone(cl, g);
    SetConnectorAsNone(cr, g);
    SetConnectorAsNone(ct, g);
    SetConnectorAsNone(cb, g);
        
    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(cl);
    g.Connectors.Add(cr);
    g.Connectors.Add(ct);
    g.Connectors.Add(cb);

    return g;
}

// XOR
XGroup CreateXorGate()
{
    var g = XGroup.Create("XOR");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var labelProperty = ShapeProperty.Create("=1");
    g.AddProperty("Label", labelProperty);
    label.Properties = new [] { labelProperty };
    
    SetShapeState(label, g);
    SetShapeState(frame, g);
    SetConnectorAsNone(cl, g);
    SetConnectorAsNone(cr, g);
    SetConnectorAsNone(ct, g);
    SetConnectorAsNone(cb, g);
            
    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(cl);
    g.Connectors.Add(cr);
    g.Connectors.Add(ct);
    g.Connectors.Add(cb);
 
    return g;
}

// INVERTER
XGroup CreateInverterGate()
{
    var g = XGroup.Create("INVERTER");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var labelProperty = ShapeProperty.Create("1");
    g.AddProperty("Label", labelProperty);
    label.Properties = new [] { labelProperty };
    
    SetShapeState(label, g);
    SetShapeState(frame, g);
    SetConnectorAsNone(cl, g);
    SetConnectorAsNone(cr, g);
    SetConnectorAsNone(ct, g);
    SetConnectorAsNone(cb, g);
            
    g.Shapes.Add(label);
    g.Shapes.Add(frame);
    g.Connectors.Add(cl);
    g.Connectors.Add(cr);
    g.Connectors.Add(ct);
    g.Connectors.Add(cb);
 
    return g;
}

// TIMER-ON
XGroup CreateTimerOn()
{
    var g = XGroup.Create("TIMER-ON");

    var label = XText.Create(-15, -15, 45, 0, styleTextMediumCC, ps, "{0}{1}{2}", false, "");
    var t0 = XText.Create(0, 3, 15, 18, styleTextMediumCC, ps, "{0}", false, "");
    var t1 = XText.Create(15, 3, 30, 18, styleTextMediumCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var l0 = XLine.Create(7, 18, 7, 22, styleLineThick, ps);
    var l1 = XLine.Create(23, 18, 23, 22, styleLineThick, ps);
    var l2 = XLine.Create(23, 20, 7, 20, styleLineThick, ps);
    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var prefixProperty = ShapeProperty.Create("T=");
    var delayProperty = ShapeProperty.Create("1");
    var unitProperty = ShapeProperty.Create("s");
    g.AddProperty("Prefix", prefixProperty);
    g.AddProperty("Delay", delayProperty);
    g.AddProperty("Unit", unitProperty);
    label.Properties = new [] { prefixProperty, delayProperty, unitProperty };

    var t0Property = ShapeProperty.Create("T");
    g.AddProperty("T0", t0Property);
    t0.Properties = new [] { t0Property };
    
    var t1Property = ShapeProperty.Create("0");
    g.AddProperty("T1", t1Property);
    t1.Properties = new [] { t1Property };

    SetShapeState(label, g);
    SetShapeState(t0, g);
    SetShapeState(t1, g);
    SetShapeState(frame, g);
    SetShapeState(l0, g);
    SetShapeState(l1, g);
    SetShapeState(l2, g);
    SetConnectorAsNone(cl, g);
    SetConnectorAsNone(cr, g);
    SetConnectorAsNone(ct, g);
    SetConnectorAsNone(cb, g);
        
    g.Shapes.Add(label);
    g.Shapes.Add(t0);
    g.Shapes.Add(t1);
    g.Shapes.Add(frame);
    g.Shapes.Add(l0);
    g.Shapes.Add(l1);
    g.Shapes.Add(l2);
    g.Connectors.Add(cl);
    g.Connectors.Add(cr);
    g.Connectors.Add(ct);
    g.Connectors.Add(cb);

    return g;
}

c.Groups.Add(CreateInputSignal());
c.Groups.Add(CreateOutputSignal());
c.Groups.Add(CreateSignal());
c.Groups.Add(CreateAndGate());
c.Groups.Add(CreateOrGate());
c.Groups.Add(CreateXorGate());
c.Groups.Add(CreateInverterGate());
c.Groups.Add(CreateTimerOn());
