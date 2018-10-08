// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor
{
    public static class Log
    {
        public static bool Enabled { get; set; }

        public static void Info(string value)
        {
            if (Enabled)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ForegroundColor = color;
            }
        }

        public static void Warning(string value)
        {
            if (Enabled)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(value);
                Console.ForegroundColor = color;
            }
        }

        public static void Error(string value)
        {
            if (Enabled)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(value);
                Console.ForegroundColor = color;
            }
        }
    }
}
