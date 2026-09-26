using System.Globalization;
using Core2D.Model.Expressions;
using Xunit;

namespace Core2D.UI.Tests;

public class NumericExpressionTests
{
    [Theory]
    [InlineData("(24+8)/2", "100", "16")]
    [InlineData("2+3*4", "0", "14")]
    [InlineData("-12", "100", "-12")]
    [InlineData("+=8", "100", "108")]
    [InlineData("-=8", "100", "92")]
    [InlineData("*=2", "100", "200")]
    [InlineData("/=2", "100", "50")]
    [InlineData("+8", "100", "108")]
    [InlineData("/2", "100", "50")]
    [InlineData("50%", "160", "80")]
    [InlineData("+=10%", "160", "176")]
    [InlineData("-=10%", "160", "144")]
    [InlineData("*=50%", "160", "80")]
    [InlineData("/=50%", "160", "320")]
    [InlineData("200*50%", "0", "100")]
    [InlineData("0.1+0.2", "0", "0.3")]
    [InlineData("1.5e2 / 3", "0", "50")]
    [InlineData("1e-2", "0", "0.01")]
    public void EvaluatesBoundedDecimalExpressions(string expression, string basis, string expected)
    {
        var culture = CultureInfo.InvariantCulture;
        Assert.True(NumericExpression.TryEvaluate(expression, decimal.Parse(basis, culture), culture, out var value));
        Assert.Equal(decimal.Parse(expected, culture), value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1/0")]
    [InlineData("/=0")]
    [InlineData("1e100")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("(1+2")]
    [InlineData("2px")]
    [InlineData("1..2")]
    [InlineData("2;System.Exit()")]
    [InlineData("+=")]
    [InlineData("99999999999999999999999999999")]
    public void RejectsInvalidInputWithoutChangingTheBasis(string expression)
    {
        Assert.False(NumericExpression.TryEvaluate(expression, 42, CultureInfo.InvariantCulture, out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void HandlesCultureAndBoundsParserWork()
    {
        Assert.True(NumericExpression.TryEvaluate("(1,5+2,5)*2", 0, CultureInfo.GetCultureInfo("de-DE"), out var value));
        Assert.Equal(8, value);
        Assert.False(NumericExpression.TryEvaluate(new string('(', 120) + "1" + new string(')', 120), 0, CultureInfo.InvariantCulture, out _));
        Assert.False(NumericExpression.TryEvaluate(new string('1', 257), 0, CultureInfo.InvariantCulture, out _));
        Assert.False(NumericExpression.TryEvaluate("+=1", decimal.MaxValue, CultureInfo.InvariantCulture, out _));
    }
}
