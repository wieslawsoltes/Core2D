// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model.Input;

public readonly struct InputArgs
{
    public double X { get; }

    public double Y { get; }

    public ModifierFlags Modifier { get; }

    public InputArgs(double x, double y, ModifierFlags modifier)
    {
        X = x;
        Y = y;
        Modifier = modifier;
    }

    public void Deconstruct(out double x, out double y)
    {
        x = X;
        y = Y;
    }

    public void Deconstruct(out double x, out double y, out ModifierFlags modifier)
    {
        x = X;
        y = Y;
        modifier = Modifier;
    }
}