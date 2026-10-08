using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace WPFPerspectiveEffect.Controls;

public sealed class Joystick : Button
{
    public event EventHandler<Point>? OffsetChanged;

    #region Title property

    public static readonly DependencyProperty TitleProperty
        = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Joystick),
            new PropertyMetadata("Joystick"));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion

    #region Offset property

    public static readonly DependencyProperty OffsetProperty
        = DependencyProperty.Register(
            nameof(Offset),
            typeof(Point),
            typeof(Joystick),
            new PropertyMetadata(new Point(0, 0)));

    public Point Offset
    {
        get => (Point)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    #endregion

    public static readonly DependencyProperty MaxXProperty
        = DependencyProperty.Register(
            nameof(MaxX),
            typeof(double),
            typeof(Joystick),
            new PropertyMetadata(1.0, OnMaxXUpdated));

    private static void OnMaxXUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Joystick joystick = (Joystick)d;

        double value = (double)e.NewValue;

        joystick.MaxXText = value.ToString();
    }

    public double MaxX
    {
        get => (double)GetValue(MaxXProperty);
        set => SetValue(MaxXProperty, value);
    }

    public static readonly DependencyProperty MaxXTextProperty
        = DependencyProperty.Register(
            nameof(MaxXText),
            typeof(string),
            typeof(Joystick),
            new PropertyMetadata("1"));

    public string MaxXText
    {
        get => (string)GetValue(MaxXTextProperty);
        set => SetValue(MaxXTextProperty, value);
    }

    public static readonly DependencyProperty MinXProperty
        = DependencyProperty.Register(
            nameof(MinX),
            typeof(double),
            typeof(Joystick),
            new PropertyMetadata(-1.0, OnMinXUpdated));

    private static void OnMinXUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Joystick joystick = (Joystick)d;

        double value = (double)e.NewValue;

        joystick.MinXText = value.ToString();
    }

    public double MinX
    {
        get => (double)GetValue(MinXProperty);
        set => SetValue(MinXProperty, value);
    }

    public static readonly DependencyProperty MinXTextProperty
        = DependencyProperty.Register(
            nameof(MinXText),
            typeof(string),
            typeof(Joystick),
            new PropertyMetadata("-1"));

    public string MinXText
    {
        get => (string)GetValue(MinXTextProperty);
        set => SetValue(MinXTextProperty, value);
    }

    public static readonly DependencyProperty MaxYProperty
    = DependencyProperty.Register(
        nameof(MaxY),
        typeof(double),
        typeof(Joystick),
        new PropertyMetadata(1.0, OnMaxYUpdated));

    private static void OnMaxYUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Joystick joystick = (Joystick)d;

        double value = (double)e.NewValue;

        joystick.MaxYText = value.ToString();
    }

    public double MaxY
    {
        get => (double)GetValue(MaxYProperty);
        set => SetValue(MaxYProperty, value);
    }

    public static readonly DependencyProperty MaxYTextProperty
        = DependencyProperty.Register(
            nameof(MaxYText),
            typeof(string),
            typeof(Joystick),
            new PropertyMetadata("1"));

    public string MaxYText
    {
        get => (string)GetValue(MaxYTextProperty);
        set => SetValue(MaxYTextProperty, value);
    }

    public static readonly DependencyProperty MinYProperty
    = DependencyProperty.Register(
        nameof(MinY),
        typeof(double),
        typeof(Joystick),
        new PropertyMetadata(-1.0, OnMinYUpdated));

    private static void OnMinYUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Joystick joystick = (Joystick)d;

        double value = (double)e.NewValue;

        joystick.MinYText = value.ToString();
    }

    public double MinY
    {
        get => (double)GetValue(MinYProperty);
        set => SetValue(MinYProperty, value);
    }

    public static readonly DependencyProperty MinYTextProperty
        = DependencyProperty.Register(
            nameof(MinYText),
            typeof(string),
            typeof(Joystick),
            new PropertyMetadata("-1"));

    public string MinYText
    {
        get => (string)GetValue(MinYTextProperty);
        set => SetValue(MinYTextProperty, value);
    }

    private Border? _area;
    private TextBlock? _offsetInfo;
    private TranslateTransform? _stickPosition;

    public Joystick()
    {
        Loaded += OnFirstLoaded;
    }

    private void OnFirstLoaded(object sender, RoutedEventArgs e)
    {
        // Инициализируем только один раз.
        Loaded -= OnFirstLoaded;

        Point center = new(
            (MinX + MaxX) / 2,
            (MinY + MaxY) / 2);

        SetCurrentValue(OffsetProperty, center);

        UpdateStickPosition();

        if (_offsetInfo is not null)
            _offsetInfo.Text = $"[ {center.X:F2} ; {center.Y:F2} ]";

        OffsetChanged?.Invoke(this, center);
    }

    public override void OnApplyTemplate()
    {
        if (_area is not null)
            _area.SizeChanged -= OnAreaSizeChanged;

        base.OnApplyTemplate();

        _area = GetTemplateChild("PART_Area") as Border;
        _offsetInfo = GetTemplateChild("PART_OffsetInfo") as TextBlock;
        _stickPosition = GetTemplateChild("PART_StickPosition") as TranslateTransform;

        if (_area is not null)
            _area.SizeChanged += OnAreaSizeChanged;

        UpdateStickPosition();
    }

    private void OnAreaSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateStickPosition();
    }

    protected override void OnMouseMove(MouseEventArgs mouse)
    {
        base.OnMouseMove(mouse);

        if (_area is null ||
            mouse.LeftButton != MouseButtonState.Pressed)
            return;

        double width = _area.ActualWidth;
        double height = _area.ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        Point position = mouse.GetPosition(_area);

        double percentX = Math.Clamp(position.X / width, 0, 1);
        double percentY = Math.Clamp(position.Y / height, 0, 1);

        Point offset = new(
            MinX + (MaxX - MinX) * percentX,
            MinY + (MaxY - MinY) * percentY);

        SetCurrentValue(OffsetProperty, offset);

        UpdateStickPosition();

        if (_offsetInfo is not null)
            _offsetInfo.Text = $"[ {offset.X:F2} ; {offset.Y:F2} ]";

        OffsetChanged?.Invoke(this, offset);
    }

    private void UpdateStickPosition()
    {
        if (_stickPosition is null || _area is null)
            return;

        double rangeX = MaxX - MinX;
        double rangeY = MaxY - MinY;

        double percentX = rangeX > 0
            ? Math.Clamp((Offset.X - MinX) / rangeX, 0, 1)
            : 0.5;

        double percentY = rangeY > 0
            ? Math.Clamp((Offset.Y - MinY) / rangeY, 0, 1)
            : 0.5;

        _stickPosition.X = (percentX - 0.5) * _area.ActualWidth;
        _stickPosition.Y = (percentY - 0.5) * _area.ActualHeight;
    }
}