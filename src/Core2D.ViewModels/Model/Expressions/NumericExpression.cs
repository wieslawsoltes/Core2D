// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;

namespace Core2D.Model.Expressions;

/// <summary>Evaluates bounded, culture-aware decimal expressions without scripting or reflection.</summary>
public static class NumericExpression
{
    /// <summary>
    /// Evaluates arithmetic, parentheses, percentages, and relative +=, -=, *= and /= edits.
    /// A lone percentage is relative to <paramref name="basis"/>; percentages inside arithmetic
    /// are fractions. A relative percentage increment is relative to the basis. A leading minus is an absolute negative number, not relative subtraction.
    /// </summary>
    public static bool TryEvaluate(string? text, decimal basis, CultureInfo culture, out decimal value)
    {
        value = basis;
        if (string.IsNullOrWhiteSpace(text) || text.Length > 256)
        {
            return false;
        }

        ReadOnlySpan<char> input = text.AsSpan().Trim();
        char operation = '\0';
        if (input.Length >= 2 && input[1] == '=' && input[0] is '+' or '-' or '*' or '/')
        {
            operation = input[0];
            input = input[2..].Trim();
        }
        else if (input[0] is '+' or '*' or '/')
        {
            operation = input[0];
            input = input[1..].Trim();
        }

        try
        {
            decimal result;
            if (operation is '\0' or '+' or '-' && input.Length > 1 && input[^1] == '%'
                && decimal.TryParse(input[..^1], NumberStyles.Float, culture, out decimal percent))
            {
                result = checked(basis * (percent / 100m));
            }
            else
            {
                var parser = new Parser(input, culture);
                result = parser.ReadExpression(0);
                parser.EnsureEnd();
            }

            value = operation switch
            {
                '+' => checked(basis + result),
                '-' => checked(basis - result),
                '*' => checked(basis * result),
                '/' => checked(basis / result),
                _ => result
            };
            return true;
        }
        catch (Exception exception) when (exception is FormatException or OverflowException or DivideByZeroException)
        {
            return false;
        }
    }

    private ref struct Parser
    {
        private readonly ReadOnlySpan<char> _input;
        private readonly CultureInfo _culture;
        private readonly ReadOnlySpan<char> _decimalSeparator;
        private int _position;

        public Parser(ReadOnlySpan<char> input, CultureInfo culture)
        {
            _input = input;
            _culture = culture;
            _decimalSeparator = culture.NumberFormat.NumberDecimalSeparator.AsSpan();
            _position = 0;
        }

        public decimal ReadExpression(int depth)
        {
            GuardDepth(depth);
            decimal result = ReadProduct(depth + 1);
            while (true)
            {
                if (Take('+')) result = checked(result + ReadProduct(depth + 1));
                else if (Take('-')) result = checked(result - ReadProduct(depth + 1));
                else return result;
            }
        }

        private decimal ReadProduct(int depth)
        {
            decimal result = ReadUnary(depth + 1);
            while (true)
            {
                if (Take('*')) result = checked(result * ReadUnary(depth + 1));
                else if (Take('/')) result = checked(result / ReadUnary(depth + 1));
                else return result;
            }
        }

        private decimal ReadUnary(int depth)
        {
            GuardDepth(depth);
            if (Take('+')) return ReadUnary(depth + 1);
            if (Take('-')) return checked(-ReadUnary(depth + 1));
            decimal result;
            if (Take('('))
            {
                result = ReadExpression(depth + 1);
                if (!Take(')')) throw new FormatException("Missing closing parenthesis.");
            }
            else
            {
                result = ReadNumber();
            }
            if (Take('%')) result /= 100m;
            return result;
        }

        private decimal ReadNumber()
        {
            SkipWhiteSpace();
            int start = _position;
            while (_position < _input.Length)
            {
                char character = _input[_position];
                if (character is >= '0' and <= '9') _position++;
                else if (!_decimalSeparator.IsEmpty && _input[_position..].StartsWith(_decimalSeparator, StringComparison.Ordinal))
                    _position += _decimalSeparator.Length;
                else break;
            }
            if (_position < _input.Length && _input[_position] is 'e' or 'E')
            {
                _position++;
                if (_position < _input.Length && _input[_position] is '+' or '-') _position++;
                while (_position < _input.Length && _input[_position] is >= '0' and <= '9') _position++;
            }
            if (start == _position || !decimal.TryParse(_input[start.._position], NumberStyles.Float, _culture, out decimal result))
                throw new FormatException("Expected a finite decimal number.");
            return result;
        }

        private bool Take(char character)
        {
            SkipWhiteSpace();
            if (_position >= _input.Length || _input[_position] != character) return false;
            _position++;
            return true;
        }

        private void SkipWhiteSpace()
        {
            while (_position < _input.Length && char.IsWhiteSpace(_input[_position])) _position++;
        }

        private static void GuardDepth(int depth)
        {
            if (depth > 32) throw new FormatException("Expression nesting is too deep.");
        }

        public void EnsureEnd()
        {
            SkipWhiteSpace();
            if (_position != _input.Length) throw new FormatException("Unexpected input.");
        }
    }
}
