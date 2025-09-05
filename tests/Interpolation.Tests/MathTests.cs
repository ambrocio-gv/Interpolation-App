using Xunit;
using Interpolation.Core;

public class MathTests
{
    [Fact]
    public void Single_Works()
    {
        var y = InterpolationMath.Single(5, 0, 10, 0, 10);
        Assert.Equal(5, y);
    }

    [Fact]
    public void DoubleVbStyle_DoesNotCrash()
    {
        var res = InterpolationMath.DoubleVbStyle(
            A1: 10, a2: 8, a3: 12,
            b1: 100, b3: 200,
            c1: 110, c3: 210,
            D1: 5, d2: 3, d3: 7
        );
        Assert.NotNull(res);
    }

    [Fact]
    public void Bilinear3x3_MatchesSample()
    {
        var (V11, V12, V13, V21, V22, V23, V31, V32, V33) =
            InterpolationMath.Bilinear3x3(
                xLeft: 9.2, xTarget: 9.25, xRight: 9.4,
                yLow: 20000, yTarget: 20480, yHigh: 21000,
                z11: 55267,  z13: 54382,
                z31: 55727,  z33: 54798,
                round: 2);

        // Corners (should match inputs)
        Assert.Equal(55267.00, V11);
        Assert.Equal(54382.00, V13);
        Assert.Equal(55727.00, V31);
        Assert.Equal(54798.00, V33);

        // Edges & center from the screenshot
        Assert.Equal(55045.75, V12);
        Assert.Equal(55487.80, V21);
        Assert.Equal(55261.27, V22);
        Assert.Equal(54581.68, V23);
        Assert.Equal(55494.75, V32);
    }
}
