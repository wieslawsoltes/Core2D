var p = Context.Editor.Project;
var c = Context.Editor.Project.CurrentContainer;

var sg = p.StyleGroups.Where(x => x.Name == "Logic").FirstOrDefault();
if (sg == null)
{
    sg = ShapeStyleGroup.Create("Logic");
    p.StyleGroups.Add(sg);
}
var styles = sg.Styles;

var gl = p.GroupLibraries.Where(x => x.Name == "Logic").FirstOrDefault();
if (gl == null)
{
    gl = GroupLibrary.Create("Logic");
    p.GroupLibraries.Add(gl);
}

p.CurrentStyleGroup = sg;
p.CurrentGroupLibrary = gl;

var groups = gl.Groups;
var ps = p.PointShape;

var styleTextMediumLC = ShapeStyle.Create(
    "Logic-Text-Medium-LC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", "consola.ttf",
    11.0, 
    FontStyle.Regular,
    TextHAlignment.Left, TextVAlignment.Center);
styles.Add(styleTextMediumLC);

var styleTextMediumCC = ShapeStyle.Create(
    "Logic-Text-Medium-CC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", "consola.ttf",
    11.0, 
    FontStyle.Regular,
    TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextMediumCC);

var styleTextBigCC = ShapeStyle.Create(
    "Logic-Text-Big-CC", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null, 
    "Consolas", "consola.ttf",
    14.0, 
    FontStyle.Regular,
    TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleTextBigCC);

var styleLineThick = ShapeStyle.Create(
    "Logic-Line-Thick", 
    255, 0, 0, 0, 
    255, 0, 0, 0, 
    2.0, 
    null,
    "Consolas", "consola.ttf",
    12.0, 
    FontStyle.Regular,
    TextHAlignment.Center, TextVAlignment.Center);
styles.Add(styleLineThick);

sg.CurrentStyle = sg.Styles.FirstOrDefault();

// INPUT
XGroup CreateInputSignal()
{
    var g = XGroup.Create("INPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var co = XPoint.Create(30, 15, ps, "O");

    var labelProperty = ShapeProperty.Create("Label", "IN");
    g.Database.AddProperty(labelProperty);
    label.Properties = new [] { labelProperty };

    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsOutput(co);

    return g;
}

// OUTPUT
XGroup CreateOutputSignal()
{
    var g = XGroup.Create("OUTPUT");

    var label = XText.Create(0, 0, 30, 30, styleTextBigCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var ci = XPoint.Create(0, 15, ps, "I");
    
    var labelProperty = ShapeProperty.Create("Label", "OUT");
    g.Database.AddProperty(labelProperty);
    label.Properties = new [] { labelProperty };

    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsInput(ci);

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
   
    var designationProperty = ShapeProperty.Create("Designation", "Designation");
    g.Database.AddProperty(designationProperty);
    labelDesignation.Properties = new [] { designationProperty };

    var descriptionProperty = ShapeProperty.Create("Description", "Description");
    g.Database.AddProperty(descriptionProperty);
    labelDescription.Properties = new [] { descriptionProperty };
    
    var signalProperty = ShapeProperty.Create("Signal", "Signal");
    g.Database.AddProperty(signalProperty);
    labelSignal.Properties = new [] { signalProperty };
    
    var conditionProperty = ShapeProperty.Create("Condition", "Condition");
    g.Database.AddProperty(conditionProperty);
    labelCondition.Properties = new [] { conditionProperty };
    
    g.AddShape(labelDesignation);
    g.AddShape(labelDescription);
    g.AddShape(labelSignal);
    g.AddShape(labelCondition);
    g.AddShape(frame);
    g.AddShape(separator);
    g.AddConnectorAsInput(ci);
    g.AddConnectorAsOutput(co);

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

    var labelProperty = ShapeProperty.Create("Label", "&");
    g.Database.AddProperty(labelProperty);
    label.Properties = new [] { labelProperty };
    
    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct); 
    g.AddConnectorAsNone(cb);

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

    var prefixProperty = ShapeProperty.Create("Prefix", "≥");
    var counterProperty = ShapeProperty.Create("Counter", "1");
    g.Database.AddProperty(prefixProperty);
    g.Database.AddProperty(counterProperty);
    label.Properties = new [] { prefixProperty, counterProperty };

    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

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

    var labelProperty = ShapeProperty.Create("Label", "=1");
    g.Database.AddProperty(labelProperty);
    label.Properties = new [] { labelProperty };
    
    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

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

    var labelProperty = ShapeProperty.Create("Label", "1");
    g.Database.AddProperty(labelProperty);
    label.Properties = new [] { labelProperty };
    
    g.AddShape(label);
    g.AddShape(frame);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

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

    var prefixProperty = ShapeProperty.Create("Prefix", "T=");
    var delayProperty = ShapeProperty.Create("Delay", "1");
    var unitProperty = ShapeProperty.Create("Unit", "s");
    g.Database.AddProperty(prefixProperty);
    g.Database.AddProperty(delayProperty);
    g.Database.AddProperty(unitProperty);
    label.Properties = new [] { prefixProperty, delayProperty, unitProperty };

    var t0Property = ShapeProperty.Create("T0", "T");
    g.Database.AddProperty(t0Property);
    t0.Properties = new [] { t0Property };
    
    var t1Property = ShapeProperty.Create("T1", "0");
    g.Database.AddProperty(t1Property);
    t1.Properties = new [] { t1Property };

    g.AddShape(label);
    g.AddShape(t0);
    g.AddShape(t1);
    g.AddShape(frame);
    g.AddShape(l0);
    g.AddShape(l1);
    g.AddShape(l2);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

    return g;
}

// TIMER-OFF
XGroup CreateTimerOff()
{
    var g = XGroup.Create("TIMER-OFF");

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

    var prefixProperty = ShapeProperty.Create("Prefix", "T=");
    var delayProperty = ShapeProperty.Create("Delay", "1");
    var unitProperty = ShapeProperty.Create("Unit", "s");
    g.Database.AddProperty(prefixProperty);
    g.Database.AddProperty(delayProperty);
    g.Database.AddProperty(unitProperty);
    label.Properties = new [] { prefixProperty, delayProperty, unitProperty };

    var t0Property = ShapeProperty.Create("T0", "0");
    g.Database.AddProperty(t0Property);
    t0.Properties = new [] { t0Property };
    
    var t1Property = ShapeProperty.Create("T1", "T");
    g.Database.AddProperty(t1Property);
    t1.Properties = new [] { t1Property };

    g.AddShape(label);
    g.AddShape(t0);
    g.AddShape(t1);
    g.AddShape(frame);
    g.AddShape(l0);
    g.AddShape(l1);
    g.AddShape(l2);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

    return g;
}

// TIMER-PULSE
XGroup CreateTimerPulse()
{
    var g = XGroup.Create("TIMER-PULSE");

    var label = XText.Create(-15, -15, 45, 0, styleTextMediumCC, ps, "{0}{1}{2}", false, "");
    var t = XText.Create(7, 2, 23, 15, styleTextMediumCC, ps, "{0}", false, "");
    var frame = XRectangle.Create(0, 0, 30, 30, styleLineThick, ps, false, "");
    var l0 = XLine.Create(7, 24, 11, 24, styleLineThick, ps);
    var l1 = XLine.Create(19, 24, 23, 24, styleLineThick, ps);
    var l2 = XLine.Create(11, 16, 19, 16, styleLineThick, ps);
    var l3 = XLine.Create(11, 16, 11, 24, styleLineThick, ps);
    var l4 = XLine.Create(19, 16, 19, 24, styleLineThick, ps);

    var cl = XPoint.Create(0, 15, ps, "L");
    var cr = XPoint.Create(30, 15, ps, "R");
    var ct = XPoint.Create(15, 0, ps, "T");
    var cb = XPoint.Create(15, 30, ps, "B");

    var prefixProperty = ShapeProperty.Create("Prefix", "T=");
    var delayProperty = ShapeProperty.Create("Delay", "1");
    var unitProperty = ShapeProperty.Create("Unit", "s");
    g.Database.AddProperty(prefixProperty);
    g.Database.AddProperty(delayProperty);
    g.Database.AddProperty(unitProperty);
    label.Properties = new [] { prefixProperty, delayProperty, unitProperty };

    var tProperty = ShapeProperty.Create("T", "T");
    g.Database.AddProperty(tProperty);
    t.Properties = new [] { tProperty };

    g.AddShape(label);
    g.AddShape(t);
    g.AddShape(frame);
    g.AddShape(l0);
    g.AddShape(l1);
    g.AddShape(l2);
    g.AddShape(l3);
    g.AddShape(l4);
    g.AddConnectorAsNone(cl);
    g.AddConnectorAsNone(cr);
    g.AddConnectorAsNone(ct);
    g.AddConnectorAsNone(cb);

    return g;
}

groups.Add(CreateInputSignal());
groups.Add(CreateOutputSignal());
groups.Add(CreateSignal());
groups.Add(CreateAndGate());
groups.Add(CreateOrGate());
groups.Add(CreateXorGate());
groups.Add(CreateInverterGate());
groups.Add(CreateTimerOn());
groups.Add(CreateTimerOff());
groups.Add(CreateTimerPulse());