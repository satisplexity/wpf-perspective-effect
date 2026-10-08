using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace WPFPerspectiveEffect;

public partial class MainWindow : Window
{
    private const double SMOOTH_SPEED = 12.0;

    private readonly Stopwatch _clock = Stopwatch.StartNew();

    private double _lastTime;

    private Point _targetTopLeft = new(0, 0);
    private Point _targetTopRight = new(1, 0);
    private Point _targetBottomLeft = new(0, 1);
    private Point _targetBottomRight = new(1, 1);

    public MainWindow()
    {
        InitializeComponent();

        _lastTime = _clock.Elapsed.TotalSeconds;

        CompositionTarget.Rendering += OnRendering;
    }

    protected override void OnClosed(EventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;

        base.OnClosed(e);
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        double currentTime = _clock.Elapsed.TotalSeconds;

        double delta = currentTime - _lastTime;
        _lastTime = currentTime;

        double factor = 1.0 - Math.Exp(-SMOOTH_SPEED * delta);

        PART_Perspective.TopLeft = Smooth(PART_Perspective.TopLeft, _targetTopLeft, factor);
        PART_Perspective.TopRight = Smooth(PART_Perspective.TopRight, _targetTopRight, factor);
        PART_Perspective.BottomLeft = Smooth(PART_Perspective.BottomLeft, _targetBottomLeft, factor);
        PART_Perspective.BottomRight = Smooth(PART_Perspective.BottomRight, _targetBottomRight, factor);
    }

    private Point Smooth(Point current, Point target, double factor)
    {
        if ((target - current).LengthSquared < 0.00000001)
            return target;

        return current + (target - current) * factor;
    }

    private void TopLeftJoystick_OffsetChanged(object sender, Point offset)
        => _targetTopLeft = offset;

    private void TopRigthJoystick_OffsetChanged(object sender, Point offset)
        => _targetTopRight = offset;

    private void BottomLeftJoystick_OffsetChanged(object sender, Point offset)
        => _targetBottomLeft = offset;

    private void BottomRightJoystick_OffsetChanged(object sender, Point offset)
        => _targetBottomRight = offset;
}