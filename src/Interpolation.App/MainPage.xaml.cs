using System.Globalization;
using Interpolation.Core;

namespace Interpolation.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        // Default to Single mode
        rbSingle.IsChecked = true;
        singleFrame.IsVisible = true;
        doubleFrame.IsVisible = false;
        
        // Wire up the unified buttons
        btnInterpolate.Clicked += OnUnifiedInterpolate;
        btnClear.Clicked += OnUnifiedClear;
    }

    // ===== Unified Button Handlers =====
    void OnUnifiedInterpolate(object? sender, EventArgs e)
    {
        if (rbSingle.IsChecked)
            OnSingleCompute(sender!, e);
        else
            OnDoubleCompute(sender!, e);
    }

    void OnUnifiedClear(object? sender, EventArgs e)
    {
        if (rbSingle.IsChecked)
            OnSingleClear(sender!, e);
        else
            OnDoubleClear(sender!, e);
    }

    // ===== Mode switching (RadioButtons) =====
    void OnModeRadioChanged(object sender, CheckedChangedEventArgs e)
    {
        // Only update when the change is to checked state
        if (!e.Value) return;
        var isSingle = rbSingle.IsChecked;
        singleFrame.IsVisible = isSingle;
        doubleFrame.IsVisible = !isSingle;
    }

    // ===== Single mode =====
    static bool Num(string? s, out double v)
        => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);

    void ClearSingleErrorUI()
    {
        s_err.Text = string.Empty;
        foreach (var e in new[] { s_a1, s_a2, s_a3, s_b1, s_b3 })
            e.BackgroundColor = Colors.White;
    }

    async Task ErrorSingle(Entry target, string message)
    {
        ClearSingleErrorUI();
        target.BackgroundColor = new Color(1.0f, 0.88f, 0.88f); // MistyRose-like
        s_err.Text = message;
        await Task.Delay(10);
        target.Focus();
    }

    async void OnSingleCompute(object sender, EventArgs e)
    {
        ClearSingleErrorUI();
        if (!Num(s_a1.Text, out var x1)) { await ErrorSingle(s_a1, "x1 must be a number."); return; }
        if (!Num(s_a2.Text, out var x))  { await ErrorSingle(s_a2, "x (target) must be a number."); return; }
        if (!Num(s_a3.Text, out var x2)) { await ErrorSingle(s_a3, "x2 must be a number."); return; }
        if (!Num(s_b1.Text, out var y1)) { await ErrorSingle(s_b1, "y at x1 must be a number."); return; }
        if (!Num(s_b3.Text, out var y2)) { await ErrorSingle(s_b3, "y at x2 must be a number."); return; }

        if (Math.Abs(x2 - x1) < double.Epsilon)
        {
            await ErrorSingle(s_a3, "x1 and x2 cannot be equal (division by zero)." );
            return;
        }

        var minX = Math.Min(x1, x2);
        var maxX = Math.Max(x1, x2);
        if (x < minX)
        {
            await ErrorSingle(s_a2, $"x is too low; should be ≥ {minX}.");
            return;
        }
        if (x > maxX)
        {
            await ErrorSingle(s_a2, $"x is too high; should be ≤ {maxX}.");
            return;
        }

        var y = InterpolationMath.Single(x, x1, x2, y1, y2);
        s_b2.Text = y?.ToString("0.#####", CultureInfo.InvariantCulture) ?? "—";
        // Hide soft keyboard after a successful solve
        DismissKeyboard();
    }

    void OnSingleClear(object sender, EventArgs e)
    {
        foreach (var t in new[] { s_a2, s_a1, s_a3, s_b1, s_b3 })
            t.Text = string.Empty;
        s_b2.Text = string.Empty;
        ClearSingleErrorUI();
    }

    // ===== Double mode (bilinear grid) =====
    void OnDoubleCompute(object sender, EventArgs e)
    {
        // Try parse all inputs but don't fail fast; compute what we can.
        var hasXLeft   = Num(d1.Text, out var xLeft);
        var hasXTarget = Num(d2.Text, out var xTarget);
        var hasXRight  = Num(d3.Text, out var xRight);
        var hasYLow    = Num(a1.Text, out var yLow);
        var hasYTarget = Num(a2.Text, out var yTarget);
        var hasYHigh   = Num(a3.Text, out var yHigh);

        var hasZ11 = Num(b1.Text, out var Z11);
        var hasZ13 = Num(c1.Text, out var Z13);
        var hasZ31 = Num(b3.Text, out var Z31);
        var hasZ33 = Num(c3.Text, out var Z33);

        var hasX = hasXLeft && hasXTarget && hasXRight;
        var hasY = hasYLow && hasYTarget && hasYHigh;

        var canInterpX = hasX && Math.Abs(xRight - xLeft) > double.Epsilon;
        var canInterpY = hasY && Math.Abs(yHigh  - yLow)  > double.Epsilon;

        double? V12 = null, V21 = null, V22 = null, V23 = null, V32 = null;

        // Horizontal (top and bottom rows)
        if (canInterpX && hasZ11 && hasZ13)
            V12 = InterpolationMath.Single(xTarget, xLeft, xRight, Z11, Z13);
        if (canInterpX && hasZ31 && hasZ33)
            V32 = InterpolationMath.Single(xTarget, xLeft, xRight, Z31, Z33);

        // Vertical (left and right columns)
        if (canInterpY && hasZ11 && hasZ31)
            V21 = InterpolationMath.Single(yTarget, yLow, yHigh, Z11, Z31);
        if (canInterpY && hasZ13 && hasZ33)
            V23 = InterpolationMath.Single(yTarget, yLow, yHigh, Z13, Z33);

        // Center: bilinear if both axes valid; otherwise try 1-D from computed edges
        if (canInterpX && canInterpY && hasZ11 && hasZ13 && hasZ31 && hasZ33)
        {
            var grid = InterpolationMath.Bilinear3x3(
                xLeft, xTarget, xRight,
                yLow,  yTarget, yHigh,
                Z11,   Z13,     Z31,   Z33);
            V22 = grid.V22;
        }
        else if (canInterpX && V21.HasValue && V23.HasValue)
        {
            V22 = InterpolationMath.Single(xTarget, xLeft, xRight, V21.Value, V23.Value);
        }
        else if (canInterpY && V12.HasValue && V32.HasValue)
        {
            V22 = InterpolationMath.Single(yTarget, yLow, yHigh, V12.Value, V32.Value);
        }
        else if (V21.HasValue)
        {
            V22 = V21; // Degenerate: center equals vertical interpolation result
        }
        else if (V12.HasValue)
        {
            V22 = V12; // Degenerate: center equals horizontal interpolation result
        }

        // Only show center result (a2d2) if we have all core inputs for proper bilinear interpolation
        bool hasAllCoreInputs = hasX && hasY && hasZ11 && hasZ13 && hasZ31 && hasZ33;

        // Update UI (labels show "—" for nulls)
        a1d2.Text = T(V12);
        b2.Text   = T(V21);
        a2d2.Text = hasAllCoreInputs ? T(V22) : "";  // Only show result when all inputs complete
        c2.Text   = T(V23);
        a3d3.Text = T(V32);

        // Hide soft keyboard only when full bilinear result is produced
        if (hasAllCoreInputs && V22.HasValue)
            DismissKeyboard();
    }

    void OnDoubleClear(object sender, EventArgs e)
    {
        foreach (var t in new[] { d1, d2, d3, a1, a2, a3, b1, c1, b3, c3 })
            t.Text = string.Empty;
        foreach (var l in new[] { a1d2, b2, a2d2, c2, a3d3 })
            l.Text = string.Empty;
    }

    // ===== helpers =====
    Task A(string f) => DisplayAlert("Invalid Input", $"Please enter a valid number for {f}.", "OK");
    static string T(double? v) => v?.ToString("0.#####", CultureInfo.InvariantCulture) ?? "—";

    // Dismiss soft keyboard by removing focus from any Entry fields
    void DismissKeyboard()
    {
        try
        {
            foreach (var entry in new Entry?[]
            {
                s_a1, s_a2, s_a3, s_b1, s_b3,
                d1, d2, d3, a1, a2, a3, b1, c1, b3, c3
            })
            {
                entry?.Unfocus();
            }
        }
        catch { /* no-op: best-effort */ }
    }

    // Handle Enter/Done navigation between inputs
    void OnEntryCompleted(object sender, EventArgs e)
    {
        if (sender is not Entry entry)
            return;

        Entry? next = null;

        if (singleFrame.IsVisible)
        {
            // Column-major order with wrap
            if (ReferenceEquals(entry, s_a1)) next = s_a2;
            else if (ReferenceEquals(entry, s_a2)) next = s_a3;
            else if (ReferenceEquals(entry, s_a3)) next = s_b1;
            else if (ReferenceEquals(entry, s_b1)) next = s_b3;
            else if (ReferenceEquals(entry, s_b3)) next = s_a1; // wrap to top-left
        }
        else if (doubleFrame.IsVisible)
        {
            // Top X inputs (exception): move right, stop at rightmost
            if (ReferenceEquals(entry, d1)) next = d2;
            else if (ReferenceEquals(entry, d2)) next = d3;
            else if (ReferenceEquals(entry, d3)) next = a1; // D3 loops to A1

            // Lower grid: column-major, then wrap to d1
            else if (ReferenceEquals(entry, a1)) next = a2;
            else if (ReferenceEquals(entry, a2)) next = a3;
            else if (ReferenceEquals(entry, a3)) next = b1;
            else if (ReferenceEquals(entry, b1)) next = b3;
            else if (ReferenceEquals(entry, b3)) next = c1;
            else if (ReferenceEquals(entry, c1)) next = c3;
            else if (ReferenceEquals(entry, c3)) next = d1; // wrap to top X
        }

        next?.Focus();
    }
}
