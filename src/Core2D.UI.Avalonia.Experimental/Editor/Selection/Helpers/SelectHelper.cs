// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Spatial;

namespace Core2D.Editor.Selection.Helpers
{
    public static class SelectHelper
    {
        public static BaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, target, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, target, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, target, radius, null) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, target, radius) : null;

            if (shapePoint != null || shape != null || guide != null)
            {
                if (shapePoint != null)
                {
                    Log.Info($"Hover Shape Point: {shapePoint.GetType()}");
                    return shapePoint;
                }
                else if (shape != null)
                {
                    Log.Info($"Hover Shape: {shape.GetType()}");
                    return shape;
                }
                else if (guidePoint != null)
                {
                    Log.Info($"Hover Guide Point: {guidePoint.GetType()}");
                    return guidePoint;
                }
                else if (guide != null)
                {
                    Log.Info($"Hover Guide: {guide.GetType()}");
                    return guide;
                }
            }

            Log.Info("No Hover");
            return null;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, Modifier modifier)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, point, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, point, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, point, radius, null) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, point, radius) : null;

            if (shapePoint != null || shape != null || guidePoint != null || guide != null)
            {
                bool haveNewSelection =
                    (shapePoint != null && !context.Renderer.Selected.Contains(shapePoint))
                    || (shape != null && !context.Renderer.Selected.Contains(shape))
                    || (guidePoint != null && !context.Renderer.Selected.Contains(guidePoint))
                    || (guide != null && !context.Renderer.Selected.Contains(guide));

                if (context.Renderer.Selected.Count >= 1
                    && !haveNewSelection
                    && !modifier.HasFlag(selectionModifier))
                {
                    return true;
                }
                else
                {
                    if (shapePoint != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(shapePoint))
                            {
                                Log.Info($"Deselected Shape Point: {shapePoint.GetType()}");
                                shapePoint.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Shape Point: {shapePoint.GetType()}");
                                shapePoint.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            Log.Info($"Selected Shape Point: {shapePoint.GetType()}");
                            shapePoint.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (shape != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(shape))
                            {
                                Log.Info($"Deselected Shape: {shape.GetType()}");
                                shape.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Shape: {shape.GetType()}");
                                shape.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            Log.Info($"Selected Shape: {shape.GetType()}");
                            shape.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (guidePoint != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(guidePoint))
                            {
                                Log.Info($"Deselected Guide Point: {guidePoint.GetType()}");
                                guidePoint.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Guide Point: {guidePoint.GetType()}");
                                guidePoint.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            Log.Info($"Selected Guide Point: {guidePoint.GetType()}");
                            guidePoint.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (guide != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(guide))
                            {
                                Log.Info($"Deselected Guide: {guide.GetType()}");
                                guide.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Guide: {guide.GetType()}");
                                guide.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            Log.Info($"Selected Guide: {guide.GetType()}");
                            guide.Select(context.Renderer);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Rect2 rect, double radius, Modifier modifier)
        {
            var shapes =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Shapes, rect, radius) : null;

            var guides =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Guides, rect, radius) : null;

            if (shapes != null || guides != null)
            {
                if (shapes != null)
                {
                    if (modifier.HasFlag(selectionModifier))
                    {
                        foreach (var shape in shapes)
                        {
                            if (context.Renderer.Selected.Contains(shape))
                            {
                                Log.Info($"Deselected Shape: {shape.GetType()}");
                                shape.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Shape: {shape.GetType()}");
                                shape.Select(context.Renderer);
                            }
                        }
                        return context.Renderer.Selected.Count > 0;
                    }
                    else
                    {
                        Log.Info($"Selected Shapes: {shapes?.Count ?? 0}");
                        context.Renderer.Selected.Clear();
                        foreach (var shape in shapes)
                        {
                            shape.Select(context.Renderer);
                        }
                        return true;
                    }
                }
                else if (guides != null)
                {
                    if (modifier.HasFlag(selectionModifier))
                    {
                        foreach (var guide in guides)
                        {
                            if (context.Renderer.Selected.Contains(guide))
                            {
                                Log.Info($"Deselected Guide: {guide.GetType()}");
                                guide.Deselect(context.Renderer);
                            }
                            else
                            {
                                Log.Info($"Selected Guide: {guide.GetType()}");
                                guide.Select(context.Renderer);
                            }
                        }
                        return context.Renderer.Selected.Count > 0;
                    }
                    else
                    {
                        Log.Info($"Selected Guides: {guides?.Count ?? 0}");
                        context.Renderer.Selected.Clear();
                        foreach (var guide in guides)
                        {
                            guide.Select(context.Renderer);
                        }
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
