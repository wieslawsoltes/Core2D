// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.IO;

namespace Core2D.Model;

public interface IFileSystem
{
    string? GetBaseDirectory();

    bool Exists(string path);

    Stream? Open(string path);

    Stream? Create(string path);

    byte[]? ReadBinary(Stream stream);

    void WriteBinary(Stream stream, byte[]? bytes);

    string? ReadUtf8Text(Stream stream);

    void WriteUtf8Text(Stream stream, string? text);

    string? ReadUtf8Text(string path);

    void WriteUtf8Text(string path, string? text);
}
