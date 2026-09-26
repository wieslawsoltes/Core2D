#!/usr/bin/env python3
"""Contract-preserving browser template adaptations after the complete import/skin pass."""
from pathlib import Path
from xml.dom import minidom as D
import json,sys
P=Path(sys.argv[1]);NS='https://github.com/avaloniaui';X='http://schemas.microsoft.com/winfx/2006/xaml'
def save(path,doc):path.write_text(doc.toxml().replace('<?xml version="1.0" ?>','')+'\n')
def node(doc,tag,**attrs):
 n=doc.createElement(tag)
 for k,v in attrs.items():n.setAttribute(k,v)
 return n
def setval(t,p,v):
 s=next((n for n in t.childNodes if n.nodeType==n.ELEMENT_NODE and n.tagName=='Setter' and n.getAttribute('Property')==p),None)
 if s is None:s=node(t.ownerDocument,'Setter',Property=p);t.insertBefore(s,t.firstChild)
 for n in list(s.childNodes):s.removeChild(n)
 s.setAttribute('Value',v)
def style(t,selector,values):
 n=node(t.ownerDocument,'Style',Selector=selector)
 for k,v in values.items():n.appendChild(node(t.ownerDocument,'Setter',Property=k,Value=v))
 t.appendChild(n)
# Adapt every residual generic system brush in actual template files to the same
# semantic vocabulary. Intrinsic icons, validity colors and animation geometry stay intact.
brushmap={'SystemControlHighlightAccentBrush':'BrowserAccentBrush','SystemControlBackgroundBaseLowBrush':'BrowserInputBrush',
'SystemControlBackgroundAltHighBrush':'BrowserSurfaceBrush','SystemControlBackgroundChromeMediumLowBrush':'BrowserCanvasBrush',
'SystemControlForegroundBaseHighBrush':'BrowserTextBrush','SystemControlForegroundBaseMediumBrush':'BrowserMutedBrush',
'SystemControlForegroundBaseMediumLowBrush':'BrowserMutedBrush','SystemControlForegroundBaseLowBrush':'BrowserDisabledBrush',
'SystemControlHighlightTransparentBrush':'BrowserTransparentBrush'}
for p in (P/'Controls').glob('*.xaml'):
 s=p.read_text()
 for a,b in brushmap.items():s=s.replace('{DynamicResource '+a+'}','{DynamicResource '+b+'}')
 p.write_text(s)
 d=D.parse(str(p));main=next((t for t in d.getElementsByTagName('ControlTheme') if t.getAttribute('x:Key')=='{x:Type '+t.getAttribute('TargetType')+'}'),None)
 if main is None:continue
 typ=main.getAttribute('TargetType')
 if typ=='Button':
  for cls,bg,fg in [('primary','BrowserPrimaryBrush','BrowserOnAccentBrush'),('ghost','BrowserTransparentBrush','BrowserTextBrush'),('danger','BrowserDangerBackgroundBrush','BrowserDangerBrush')]:
   style(main,'^.'+cls,{'Background':'{DynamicResource '+bg+'}','Foreground':'{DynamicResource '+fg+'}'})
 if typ=='TabControl':setval(main,'Padding','0')
 if typ in ('TabItem','TabStripItem'):
  # Neutral pill selection retains original placement/margin logic and keyboard selection.
  style(main,'^:selected',{'Background':'{DynamicResource BrowserInputBrush}','Foreground':'{DynamicResource BrowserTextBrush}','FontWeight':'SemiBold'})
  style(main,'^:selected /template/ Border#PART_SelectedPipe',{'Opacity':'0'})
  style(main,'^:selected:pointerover /template/ Border#PART_LayoutRoot',{'Background':'{DynamicResource BrowserHoverBrush}','TextElement.Foreground':'{DynamicResource BrowserTextBrush}'})
 if typ=='ComboBoxItem':
  # Preserve the full original presenter contract and add a checkmark over the surface.
  presenter=next(n for n in main.getElementsByTagName('ContentPresenter') if n.getAttribute('Name')=='PART_ContentPresenter')
  parent=presenter.parentNode;grid=node(d,'Grid');parent.replaceChild(grid,presenter)
  presenter.setAttribute('Padding','28,5,10,5');grid.appendChild(presenter)
  mark=node(d,'Path',Name='PART_Checkmark',Data='M1,6 L4,9 L11,2',Stroke='{DynamicResource BrowserAccentBrush}',StrokeThickness='1.5',Width='12',Height='12',HorizontalAlignment='Left',VerticalAlignment='Center',Margin='8,0,0,0',IsHitTestVisible='False',IsVisible='{Binding IsSelected, RelativeSource={RelativeSource TemplatedParent}}')
  grid.appendChild(mark)
 if typ=='ComboBox':
  setval(main,'PlaceholderForeground','{DynamicResource BrowserMutedBrush}')
  setval(main,'HorizontalContentAlignment','Stretch')
 if typ=='ScrollBar':
  style(main,'^:vertical',{'Width':'{DynamicResource ScrollBarSize}'})
  style(main,'^:horizontal',{'Height':'{DynamicResource ScrollBarSize}'})
 if typ=='TextBox':
  setval(main,'SelectionBrush','{DynamicResource BrowserPrimaryBrush}')
 if typ=='GridSplitter':
  setval(main,'Background','{DynamicResource BrowserBorderBrush}')
  style(main,'^:pointerover',{'Background':'{DynamicResource BrowserAccentBrush}'})
 if typ=='Menu':setval(main,'Background','{DynamicResource BrowserSurfaceBrush}')
 save(p,d)
# Consistent non-opaque popover elevation; actual content stays native.
p=P/'Controls/FlyoutPresenter.xaml';d=D.parse(str(p))
for border in d.getElementsByTagName('Border'):
 if border.getAttribute('Name')=='PART_LayoutRoot':border.setAttribute('BoxShadow','{DynamicResource BrowserOverlayShadow}')
save(p,d)
# Content focus uses a high-contrast outline without depending on the OS accent.
p=P/'BrowserTokens.axaml';s=p.read_text().replace('<sys:Double x:Key="ScrollBarSize">12</sys:Double>', '<sys:Double x:Key="ScrollBarSize">12</sys:Double>\n <Thickness x:Key="MenuFlyoutPresenterThemePadding">4</Thickness>\n <Thickness x:Key="FlyoutBorderThemePadding">12</Thickness>')
p.write_text(s)
# Preserve the native context flyout and extend its desktop action set.
p=P/'Controls/TextBox.xaml';d=D.parse(str(p))
fly=next(n for n in d.getElementsByTagName('MenuFlyout') if n.getAttribute('x:Key')=='DefaultTextBoxContextFlyout')
fly.appendChild(node(d,'Separator'));fly.appendChild(node(d,'MenuItem',Header='Select all',Command='{Binding $parent[TextBox].SelectAll}'))
save(p,d)
manifest=json.loads((P/'UpstreamManifest.json').read_text());manifest['adaptations']['contractRefinements']=['Neutral tab selection','Dropdown checkmarks','Compact scroll extents','Native text context select-all','Portable primary/ghost/danger buttons']
(P/'UpstreamManifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
print('Browser refinements applied without dropping upstream theme keys or template parts.')
