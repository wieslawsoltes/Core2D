// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Editor;

namespace Core2D.Avalonia.Behaviors
{
    public sealed class DropBehavior : Behavior<Control>
    {
        public static readonly AvaloniaProperty EditorProperty =
            AvaloniaProperty.Register<DropBehavior, ProjectEditor>(nameof(Editor));

        public ProjectEditor Editor
        {
            get => (ProjectEditor)GetValue(EditorProperty);
            set => SetValue(EditorProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private void Debug(object sender)
        {
            var root = sender as IControl;
            var point = (root.VisualRoot as IInputRoot)?.MouseDevice?.GetPosition(root) ?? default(Point);
            var control = root.GetVisualsAt(point, x => x.IsVisible).FirstOrDefault();
            Console.WriteLine($"[{control}] : {point}");
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);

            //if (!e.Data.Contains(DataFormats.Text) && !e.Data.Contains(DataFormats.FileNames))
            //    e.DragEffects = DragDropEffects.None;

            Console.WriteLine($"DragOver sender: {sender}, source: {e.Source}");
            Debug(sender);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);

            //if (!e.Data.Contains(DataFormats.Text) && !e.Data.Contains(DataFormats.FileNames))
            //    e.DragEffects = DragDropEffects.None;

            Console.WriteLine($"DragEnter sender: {sender}, source: {e.Source}");
            Debug(sender);
        }

        private void Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine($"Drop sender: {sender}, source: {e.Source}");
            Debug(sender);

            foreach (var format in e.Data.GetDataFormats())
            {
                Console.WriteLine($"[{format}] : {e.Data.Get(format)}");
            }

            if (e.Data.Contains(DataFormats.Text))
            {
                var text = e.Data.GetText();
                Console.WriteLine($"[{DataFormats.Text}] : {text}");
                Console.WriteLine(text);
            }

            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames().ToArray();

                foreach (var file in files)
                {
                    Console.WriteLine($"[{DataFormats.FileNames}] : {file}");
                }

                Editor?.OnDropFiles(files);
            }

            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}
