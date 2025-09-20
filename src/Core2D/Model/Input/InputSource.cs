// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.Model.Input;

public abstract class InputSource
{
    public IObservable<InputArgs>? BeginDown { get; set; }

    public IObservable<InputArgs>? BeginUp { get; set; }

    public IObservable<InputArgs>? EndDown { get; set; }

    public IObservable<InputArgs>? EndUp { get; set; }

    public IObservable<InputArgs>? Move { get; set; }
}