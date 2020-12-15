#r "Core2D"
#r "System.Drawing"
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Core2D.Containers;
using Core2D.Data;
using Core2D.FileWriter.Emf;
using Core2D.Renderer;

OnCopyAsEmf();

void OnCopyAsEmf()
{
    var imageChache = Project as IImageCache;
    var page = Project.CurrentContainer;
    var shapes = Project.SelectedShapes;
    var writer = FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter)) as EmfWriter;

    var db = (object)page.Data.Properties;
    var record = (object)page.Data.Record;
    DataFlow.Bind(page.Template, db, record);
    DataFlow.Bind(page, db, record);

    using var bitmap = new Bitmap((int)page.Width, (int)page.Height);

    if (shapes != null && shapes.Count > 0)
    {
        using var ms = writer.MakeMetafileStream(bitmap, shapes, imageChache);
        ms.Position = 0;
        SetClipboard(ms);
    }
    else
    {
        using var ms = writer.MakeMetafileStream(bitmap, page, imageChache);
        ms.Position = 0;
        SetClipboard(ms);
    }
}

void SetClipboard(MemoryStream ms)
{
    using var metafile = new Metafile(ms);
    var emfHandle = metafile.GetHenhmetafile();
    if (emfHandle.Equals(IntPtr.Zero))
    {
        return;
    }

    var emfCloneHandle = CopyEnhMetaFile(emfHandle, IntPtr.Zero);
    if (emfCloneHandle.Equals(IntPtr.Zero))
    {
        return;
    }

    try
    {
        if (OpenClipboard(IntPtr.Zero))
        {
            if (EmptyClipboard())
            {
                SetClipboardData(CF_ENHMETAFILE, emfCloneHandle);
                CloseClipboard();
            }
        }
    }
    finally
    {
        DeleteEnhMetaFile(emfHandle);
    }
}

private const uint CF_ENHMETAFILE = 14;

[DllImport("user32.dll", SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool OpenClipboard(IntPtr hWndNewOwner);

[DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
internal static extern bool EmptyClipboard();

[DllImport("user32.dll", SetLastError = true)]
private static extern bool CloseClipboard();

[DllImport("user32.dll")]
private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

[DllImport("gdi32.dll")]
private static extern IntPtr CopyEnhMetaFile(IntPtr hemetafileSrc, IntPtr hNULL);

[DllImport("gdi32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool DeleteEnhMetaFile(IntPtr hemetafile);
