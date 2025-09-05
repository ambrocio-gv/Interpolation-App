using System.Globalization;
using Interpolation.Core;

namespace Interpolation.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        modePicker.SelectedIndex = 0; // default: Single
    }

    // ===== Mode switching =====
    void OnModeChanged(object sender, EventArgs e)
    {
        var isSingle = modePicker.SelectedIndex == 0;
        singleFrame.IsVisible = isSingle;
        doubleFrame.IsVisible = !isSingle;
    }

    // ===== Single mode =====
    static bool Num(string? s, out double v)
        => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);

    void ClearSingleErrorUI()
    {
        s_err.Text = string.Empty;
        foreach (var e in new[] { s_x1, s_x, s_x2, s_y1, s_y2 })
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
        if (!Num(s_x1.Text, out var x1)) { await ErrorSingle(s_x1, "x1 must be a number."); return; }
        if (!Num(s_x.Text,  out var x))  { await ErrorSingle(s_x,  "x (target) must be a number."); return; }
        if (!Num(s_x2.Text, out var x2)) { await ErrorSingle(s_x2, "x2 must be a number."); return; }
        if (!Num(s_y1.Text, out var y1)) { await ErrorSingle(s_y1, "y at x1 must be a number."); return; }
        if (!Num(s_y2.Text, out var y2)) { await ErrorSingle(s_y2, "y at x2 must be a number."); return; }

        if (Math.Abs(x2 - x1) < double.Epsilon)
        {
            await ErrorSingle(s_x2, "x1 and x2 cannot be equal (division by zero)." );
            return;
        }

        var minX = Math.Min(x1, x2);
        var maxX = Math.Max(x1, x2);
        if (x < minX)
        {
            await ErrorSingle(s_x, $"x is too low; should be ≥ {minX}.");
            return;
        }
        if (x > maxX)
        {
            await ErrorSingle(s_x, $"x is too high; should be ≤ {maxX}.");
            return;
        }

        var y = InterpolationMath.Single(x, x1, x2, y1, y2);
        s_out.Text = y?.ToString("0.#####", CultureInfo.InvariantCulture) ?? "—";
    }

    void OnSingleClear(object sender, EventArgs e)
    {
        foreach (var t in new[] { s_x, s_x1, s_x2, s_y1, s_y2 })
            t.Text = string.Empty;
        s_out.Text = string.Empty;
        ClearSingleErrorUI();
    }

    // ===== Double mode (bilinear grid) =====
    async void OnDoubleCompute(object sender, EventArgs e)
    {
        // X values
        if (!Num(xL.Text, out var xLeft))    { await A("X Left"); return; }
        if (!Num(xT.Text, out var xTarget))  { await A("X Target"); return; }
        if (!Num(xR.Text, out var xRight))   { await A("X Right"); return; }

        // Y values
        if (!Num(yL.Text, out var yLow))     { await A("Y Low"); return; }
        if (!Num(yT.Text, out var yTarget))  { await A("Y Target"); return; }
        if (!Num(yH.Text, out var yHigh))    { await A("Y High"); return; }

        // Corner Z values
        if (!Num(z11.Text, out var Z11))     { await A("Z at (Left, Low)"); return; }
        if (!Num(z13.Text, out var Z13))     { await A("Z at (Right, Low)"); return; }
        if (!Num(z31.Text, out var Z31))     { await A("Z at (Left, High)"); return; }
        if (!Num(z33.Text, out var Z33))     { await A("Z at (Right, High)"); return; }

        var (V11, V12, V13, V21, V22, V23, V31, V32, V33) =
            InterpolationMath.Bilinear3x3(xLeft, xTarget, xRight, yLow, yTarget, yHigh, Z11, Z13, Z31, Z33);

        // Guard invalid spans
        if (!V11.HasValue)
        {
            await DisplayAlert("Invalid Input", "X Left/Right or Y Low/High cannot be equal.", "OK");
            return;
        }

        // Fill grid (only computed cells; corners are inputs)
        v_12.Text = T(V12);
        v_21.Text = T(V21);
        v_22.Text = T(V22);
        v_23.Text = T(V23);
        v_32.Text = T(V32);
    }

    void OnDoubleClear(object sender, EventArgs e)
    {
        foreach (var t in new[] { xL, xT, xR, yL, yT, yH, z11, z13, z31, z33 })
            t.Text = string.Empty;
        foreach (var l in new[] { v_12, v_21, v_22, v_23, v_32 })
            l.Text = string.Empty;
    }

    // ===== helpers =====
    Task A(string f) => DisplayAlert("Invalid Input", $"Please enter a valid number for {f}.", "OK");
    static string T(double? v) => v?.ToString("0.#####", CultureInfo.InvariantCulture) ?? "—";
}
