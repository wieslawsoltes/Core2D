// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.Model;

public interface ILog : IDisposable
{
    string? LastMessage { get; }

    void Initialize(string path);

    void Close();

    void LogInformation(string message);

    void LogInformation(string format, params object[] args);

    void LogWarning(string message);

    void LogWarning(string format, params object[] args);

    void LogError(string message);

    void LogError(string format, params object[] args);

    void LogException(Exception ex);
}