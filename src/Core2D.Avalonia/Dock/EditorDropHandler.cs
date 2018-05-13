// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Dock.Avalonia;
using Dock.Model;

namespace Core2D.Avalonia.Dock
{
    public class EditorDropHandler : IDropHandler
    {
        public static IDropHandler Instance = new EditorDropHandler();

        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();

                Console.WriteLine($"[{DataFormats.Text}] : {text}");

                if (bExecute)
                {
                    editor?.OnTryPaste(text);
                }

                return true;
            }

            foreach (var format in e.Data.GetDataFormats())
            {
                var data = e.Data.Get(format);

                Console.WriteLine($"[{format}] : {data}");

                switch (data)
                {
                    case BaseShape shape:
                        return editor?.OnDropShape(shape, point.X, point.Y, bExecute) == true;
                    case Record record:
                        return editor?.OnDropRecord(record, point.X, point.Y, bExecute) == true;
                    case ShapeStyle style:
                        return editor?.OnDropStyle(style, point.X, point.Y, bExecute) == true;
                    case PageContainer page:
                        return editor?.OnDropTemplate(page, point.X, point.Y, bExecute) == true;
                    default:
                        Console.WriteLine($"Drop type was not handled: {data}");
                        break;
                }
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();

                foreach (var file in files)
                {
                    Console.WriteLine($"[{DataFormats.FileNames}] : {file}");
                }

                if (bExecute)
                {
                    editor?.OnDropFiles(files);
                }

                return true;
            }

            Console.WriteLine($"DragEffects: {e.DragEffects}");

            return false;
        }

        public bool Validate(object context, object sender, DragEventArgs e)
        {
            if (context is ProjectEditor editor)
            {
                return Validate(editor, sender, e, false);
            }
            return false;
        }

        public bool Execute(object context, object sender, DragEventArgs e)
        {
            if (context is ProjectEditor editor)
            {
                return Validate(editor, sender, e, true);
            }
            return false;
        }
    }
}
