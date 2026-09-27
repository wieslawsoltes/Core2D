using System.Globalization;
using System.Linq;
using Core2D.Model.Style;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDashPatternTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("8,4;1 3", "8 4 1 3")]
    [InlineData(" 1.25 2.5 ", "1.25 2.5")]
    [InlineData("0 4", "0 4")]
    [InlineData("1e2 2e-1", "100 0.2")]
    public void ValidPatternsNormalizeToRendererCompatibleInvariantText(string? text, string expected)
    {
        Assert.True(DashPattern.TryNormalize(text, out var normalized, out var values));
        Assert.Equal(expected, normalized);
        Assert.Equal(string.IsNullOrEmpty(expected) ? 0 : expected.Split(' ').Length, values.Length);
    }

    [Theory]
    [InlineData("-1 4")]
    [InlineData("0 0")]
    [InlineData("NaN 1")]
    [InlineData("Infinity 1")]
    [InlineData("100001 1")]
    [InlineData("1 / 4")]
    [InlineData(",;")]
    public void InvalidPatternsDoNotExposePartialResults(string text)
    {
        Assert.False(DashPattern.TryNormalize(text, out var normalized, out var values));
        Assert.Empty(normalized);
        Assert.Empty(values);
    }

    [Fact]
    public void ParsingIsBoundedAndIndependentOfCurrentCulture()
    {
        Assert.False(DashPattern.TryNormalize(string.Join(" ", Enumerable.Repeat("1", 33)), out _, out _));
        Assert.False(DashPattern.TryNormalize(new string('1', 513), out _, out _));
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
            Assert.True(DashPattern.TryNormalize("1.25,2.5", out var text, out var values));
            Assert.Equal("1.25 2.5", text);
            Assert.Equal(new[] { 1.25, 2.5 }, values);
        }
        finally { CultureInfo.CurrentCulture = old; }
    }
}
