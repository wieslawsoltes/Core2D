// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Input;
using Dock.Avalonia;

namespace Core2D.UI.Avalonia.Dock.Handlers
{
    /// <summary>
    /// Tree view drop handler.
    /// </summary>
    public class TreeViewDropHandler : DefaultDropHandler
    {
        public static IDropHandler Instance = new TreeViewDropHandler();

        // FIXME:
        /*
        private bool ValidateTreeView(ProjectEditor editor, DragEventArgs e, bool bExecute, TreeView tree)
        {
            var sourceItem = e.Data.Get(DragDataFormats.Parent);
            var targetItem = (e.Source as IControl)?.Parent?.Parent;

            if (sourceItem is TreeViewItem source && targetItem is TreeViewItem target)
            {
                var sourceData = source.DataContext;
                var targetData = target.DataContext;

                switch (sourceData)
                {
                    case LayerContainer sourceLayer:
                        {
                            switch (targetData)
                            {
                                case LayerContainer targetLayer:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (e.DragEffects == DragDropEffects.Copy)
                                        {
                                            if (bExecute)
                                            {
                                                var layer = editor?.Clone(sourceLayer);
                                                editor?.Project.AddLayer(targetPage, layer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project?.RemoveLayer(sourceLayer);
                                                editor?.Project.AddLayer(targetPage, sourceLayer);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project.AddLayer(targetPage, sourceLayer);
                                                e.DragEffects = DragDropEffects.None;
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                            }

                            return false;
                        }
                    case PageContainer sourcePage:
                        {
                            switch (targetData)
                            {
                                case LayerContainer targetLayer:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (e.DragEffects == DragDropEffects.Copy)
                                        {
                                            if (bExecute)
                                            {
                                                var page = editor?.Clone(sourcePage);
                                                editor?.Project.AddPage(targetDocument, page);
                                                editor?.Project?.SetCurrentContainer(page);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Move)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project?.RemovePage(sourcePage);
                                                editor?.Project.AddPage(targetDocument, sourcePage);
                                                editor?.Project?.SetCurrentContainer(sourcePage);
                                            }
                                            return true;
                                        }
                                        else if (e.DragEffects == DragDropEffects.Link)
                                        {
                                            if (bExecute)
                                            {
                                                editor?.Project.AddPage(targetDocument, sourcePage);
                                                editor?.Project?.SetCurrentContainer(sourcePage);
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                            }

                            return false;
                        }
                    case DocumentContainer sourceDocument:
                        {
                            switch (targetData)
                            {
                                case LayerContainer targetLayer:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case PageContainer targetPage:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                                case DocumentContainer targetDocument:
                                    {
                                        if (bExecute)
                                        {
                                            // TODO:
                                        }
                                        return true;
                                    }
                            }

                            return false;
                        }
                }
            }

            return false;
        }

        private bool Validate(ProjectEditor editor, object sender, DragEventArgs e, bool bExecute)
        {
            var point = DropHelper.GetPosition(sender, e);

            switch (sender)
            {
                case TreeView tree:
                    return ValidateTreeView(editor, e, bExecute, tree);
            }

            return false;
        }
        */

        /// <inheritdoc/>
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            // FIXME:
            //if (context is ProjectEditor editor)
            //{
            //    return Validate(editor, sender, e, false);
            //}
            return false;
        }

        /// <inheritdoc/>
        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            // FIXME:
            //if (context is ProjectEditor editor)
            //{
            //    return Validate(editor, sender, e, true);
            //}
            return false;
        }
    }
}
