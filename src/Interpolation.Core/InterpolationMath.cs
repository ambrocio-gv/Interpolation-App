using System;

namespace Interpolation.Core;

public static class InterpolationMath
{
    /// <summary>
    /// Single (1-D) linear interpolation: given x between x1 and x2, return y.
    /// Returns null if x1 == x2.
    /// </summary>
    public static double? Single(double x, double x1, double x2, double y1, double y2, int round = 5)
    {
        var denom = x2 - x1;
        if (Math.Abs(denom) < double.Epsilon) return null;
        var y = y1 + (x - x1) * (y2 - y1) / denom;
        return Math.Round(y, round);
    }

    /// <summary>
    /// Double interpolation (VB6-style chain you provided).
    /// Returns (B2, C2, A1D1, A2D2, A3D3), rounded to 'round' decimals.
    /// </summary>
    public static (double? B2, double? C2, double? A1D1, double? A2D2, double? A3D3)
        DoubleVbStyle(double A1, double a2, double a3,
                      double b1, double b3,
                      double? c1, double? c3,
                      double? D1, double? d2, double? d3,
                      int round = 5)
    {
        double? B2 = null, C2 = null, A1D1 = null, A2D2 = null, A3D3 = null;

        if (Math.Abs(A1 - a3) > double.Epsilon)
        {
            B2 = b1 - (A1 - a2) * (b1 - b3) / (A1 - a3);
            if (c1.HasValue && c3.HasValue)
                C2 = c1.Value - (A1 - a2) * (c1.Value - c3.Value) / (A1 - a3);
        }

        if (D1.HasValue && d2.HasValue && d3.HasValue && Math.Abs(D1.Value - d3.Value) > double.Epsilon)
        {
            A1D1 = b1 - (D1 - d2) * (b1 - (c1 ?? b1)) / (D1 - d3);
            if (B2.HasValue && C2.HasValue)
                A2D2 = B2.Value - (D1 - d2) * (B2.Value - C2.Value) / (D1 - d3);
            A3D3 = b3 - (D1 - d2) * (b3 - (c3 ?? b3)) / (D1 - d3);
        }

        static double? R(double? v, int r) => v.HasValue ? Math.Round(v.Value, r) : null;
        return (R(B2, round), R(C2, round), R(A1D1, round), R(A2D2, round), R(A3D3, round));
    }

    /// <summary>
    /// Bilinear interpolation on a 2D grid using four corner values.
    /// Inputs are three X values (left, target, right) and three Y values (low, target, high),
    /// plus the four corner Z values: (xLeft,yLow), (xRight,yLow), (xLeft,yHigh), (xRight,yHigh).
    /// Returns the full 3x3 grid: rows are Y=[low,target,high], columns are X=[left,target,right].
    /// If the X or Y span is zero, returns nulls.
    /// </summary>
    public static (
        double? V11, double? V12, double? V13,
        double? V21, double? V22, double? V23,
        double? V31, double? V32, double? V33)
        Bilinear3x3(
            double xLeft, double xTarget, double xRight,
            double yLow,  double yTarget, double yHigh,
            double z11,   double z13,     double z31,   double z33,
            int round = 5)
    {
        var dx = xRight - xLeft;
        var dy = yHigh  - yLow;
        if (Math.Abs(dx) < double.Epsilon || Math.Abs(dy) < double.Epsilon)
        {
            return (null, null, null, null, null, null, null, null, null);
        }

        double Lerp(double a, double b, double t) => a + t * (b - a);

        var tx = (xTarget - xLeft) / dx;
        var ty = (yTarget - yLow)  / dy;

        var v11 = z11;                         // (xL, yL)
        var v13 = z13;                         // (xR, yL)
        var v31 = z31;                         // (xL, yH)
        var v33 = z33;                         // (xR, yH)

        var v12 = Lerp(z11, z13, tx);          // top middle
        var v32 = Lerp(z31, z33, tx);          // bottom middle
        var v21 = Lerp(z11, z31, ty);          // middle left
        var v23 = Lerp(z13, z33, ty);          // middle right
        var v22 = Lerp(v21, v23, tx);          // center

        static double? R(double v, int r) => Math.Round(v, r);
        return (
            R(v11, round), R(v12, round), R(v13, round),
            R(v21, round), R(v22, round), R(v23, round),
            R(v31, round), R(v32, round), R(v33, round)
        );
    }
}
