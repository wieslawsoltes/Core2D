using System;
using System.Threading.Tasks;

int frames = 10;
int delay = 250;

Task.Factory.StartNew(async () => 
{
    var shapes = Editor.PageState.SelectedShapes;
    for (int i = 0; i < frames; i++)
    {
        foreach (var shape in shapes) shape.IsFilled = !shape.IsFilled;
        await Task.Delay(delay);
    }
    foreach (var shape in shapes) shape.IsFilled = true;
});
