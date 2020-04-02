#r "System.Linq"
using System;
using System.Linq;
using System.Threading.Tasks;

int frames = 10;
int delay = 250;

Task.Factory.StartNew(async () => 
{
    var shapes = PageState.SelectedShapes.ToList();
    if (shapes.Count <= 0) return;
    for (int i = 0; i < frames; i++)
    {
        foreach (var shape in shapes) shape.IsFilled = !shape.IsFilled;
        await Task.Delay(delay);
    }
    foreach (var shape in shapes) shape.IsFilled = true;
});
