using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace WPFPerspectiveEffect.Effects;

public sealed class PerspectiveEffect : ShaderEffect
{
    private readonly PixelShader _pixelShader = new()
    {
        UriSource = new Uri(
        "/WPFPerspectiveEffect;component/Shaders/Perspective.ps",
        UriKind.Relative)
    };

    public PerspectiveEffect()
    {
        PixelShader = _pixelShader;

        DdxUvDdyUvRegisterIndex = 9;

        UpdateShaderValue(InputProperty);

        UpdateShaderValue(M11Property);
        UpdateShaderValue(M12Property);
        UpdateShaderValue(M13Property);

        UpdateShaderValue(M21Property);
        UpdateShaderValue(M22Property);
        UpdateShaderValue(M23Property);

        UpdateShaderValue(M31Property);
        UpdateShaderValue(M32Property);
        UpdateShaderValue(M33Property);

        UpdateMatrix();
    }

    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(PerspectiveEffect),
            0,
            SamplingMode.Bilinear);

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public static readonly DependencyProperty TopLeftProperty =
        DependencyProperty.Register(
            nameof(TopLeft),
            typeof(Point),
            typeof(PerspectiveEffect),
            new UIPropertyMetadata(
                new Point(0, 0),
                OnCornerChanged));

    public Point TopLeft
    {
        get => (Point)GetValue(TopLeftProperty);
        set => SetValue(TopLeftProperty, value);
    }

    public static readonly DependencyProperty TopRightProperty =
        DependencyProperty.Register(
            nameof(TopRight),
            typeof(Point),
            typeof(PerspectiveEffect),
            new UIPropertyMetadata(
                new Point(1, 0),
                OnCornerChanged));

    public Point TopRight
    {
        get => (Point)GetValue(TopRightProperty);
        set => SetValue(TopRightProperty, value);
    }

    public static readonly DependencyProperty BottomRightProperty =
        DependencyProperty.Register(
            nameof(BottomRight),
            typeof(Point),
            typeof(PerspectiveEffect),
            new UIPropertyMetadata(
                new Point(1, 1),
                OnCornerChanged));

    public Point BottomRight
    {
        get => (Point)GetValue(BottomRightProperty);
        set => SetValue(BottomRightProperty, value);
    }

    public static readonly DependencyProperty BottomLeftProperty =
        DependencyProperty.Register(
            nameof(BottomLeft),
            typeof(Point),
            typeof(PerspectiveEffect),
            new UIPropertyMetadata(
                new Point(0, 1),
                OnCornerChanged));

    public Point BottomLeft
    {
        get => (Point)GetValue(BottomLeftProperty);
        set => SetValue(BottomLeftProperty, value);
    }

    private static void OnCornerChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ((PerspectiveEffect)d).UpdateMatrix();
    }

    private static readonly DependencyProperty M11Property =
        RegisterConstant(nameof(M11), 0);

    private static readonly DependencyProperty M12Property =
        RegisterConstant(nameof(M12), 1);

    private static readonly DependencyProperty M13Property =
        RegisterConstant(nameof(M13), 2);

    private static readonly DependencyProperty M21Property =
        RegisterConstant(nameof(M21), 3);

    private static readonly DependencyProperty M22Property =
        RegisterConstant(nameof(M22), 4);

    private static readonly DependencyProperty M23Property =
        RegisterConstant(nameof(M23), 5);

    private static readonly DependencyProperty M31Property =
        RegisterConstant(nameof(M31), 6);

    private static readonly DependencyProperty M32Property =
        RegisterConstant(nameof(M32), 7);

    private static readonly DependencyProperty M33Property =
        RegisterConstant(nameof(M33), 8);

    private double M11
    {
        get => (double)GetValue(M11Property);
        set => SetValue(M11Property, value);
    }

    private double M12
    {
        get => (double)GetValue(M12Property);
        set => SetValue(M12Property, value);
    }

    private double M13
    {
        get => (double)GetValue(M13Property);
        set => SetValue(M13Property, value);
    }

    private double M21
    {
        get => (double)GetValue(M21Property);
        set => SetValue(M21Property, value);
    }

    private double M22
    {
        get => (double)GetValue(M22Property);
        set => SetValue(M22Property, value);
    }

    private double M23
    {
        get => (double)GetValue(M23Property);
        set => SetValue(M23Property, value);
    }

    private double M31
    {
        get => (double)GetValue(M31Property);
        set => SetValue(M31Property, value);
    }

    private double M32
    {
        get => (double)GetValue(M32Property);
        set => SetValue(M32Property, value);
    }

    private double M33
    {
        get => (double)GetValue(M33Property);
        set => SetValue(M33Property, value);
    }

    private static DependencyProperty RegisterConstant(string name, int register)
        => DependencyProperty.Register(
            name,
            typeof(double),
            typeof(PerspectiveEffect),
            new UIPropertyMetadata(
                0.0,
                PixelShaderConstantCallback(register)));

    private void UpdateMatrix()
    {
        Matrix3x3 matrix = Homography.CreateInverse(
            TopLeft, TopRight, BottomRight, BottomLeft);

        M11 = matrix.M11;
        M12 = matrix.M12;
        M13 = matrix.M13;

        M21 = matrix.M21;
        M22 = matrix.M22;
        M23 = matrix.M23;

        M31 = matrix.M31;
        M32 = matrix.M32;
        M33 = matrix.M33;
    }
}

internal static class Homography
{
    public static Matrix3x3 CreateInverse(
        Point topLeft, Point topRight,
        Point bottomRight, Point bottomLeft)
    {
        var target = new[]
        {
            topLeft,
            topRight,
            bottomRight,
            bottomLeft
        };

        var source = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1)
        };

        /*
         * x' = (a*x + b*y + c) / (g*x + h*y + 1)
         * y' = (d*x + e*y + f) / (g*x + h*y + 1)
         */

        double[,] matrix = new double[8, 9];

        for (var i = 0; i < 4; i++)
        {
            double x = target[i].X;
            double y = target[i].Y;

            double u = source[i].X;
            double v = source[i].Y;

            int row = i * 2;

            matrix[row, 0] = x;
            matrix[row, 1] = y;
            matrix[row, 2] = 1;
            matrix[row, 3] = 0;
            matrix[row, 4] = 0;
            matrix[row, 5] = 0;
            matrix[row, 6] = -u * x;
            matrix[row, 7] = -u * y;
            matrix[row, 8] = u;

            matrix[row + 1, 0] = 0;
            matrix[row + 1, 1] = 0;
            matrix[row + 1, 2] = 0;
            matrix[row + 1, 3] = x;
            matrix[row + 1, 4] = y;
            matrix[row + 1, 5] = 1;
            matrix[row + 1, 6] = -v * x;
            matrix[row + 1, 7] = -v * y;
            matrix[row + 1, 8] = v;
        }

        double[] result = Solve(matrix);

        return new Matrix3x3(
            result[0], result[1], result[2],
            result[3], result[4], result[5],
            result[6], result[7], 1);
    }

    private static double[] Solve(double[,] matrix)
    {
        const int size = 8;

        for (int column = 0; column < size; column++)
        {
            int pivot = column;

            for (int row = column + 1; row < size; row++)
                if (Math.Abs(matrix[row, column]) > Math.Abs(matrix[pivot, column]))
                    pivot = row;

            if (Math.Abs(matrix[pivot, column]) < 1e-10)
                throw new InvalidOperationException("The perspective quadrilateral is degenerate.");

            if (pivot != column)
                for (var j = column; j <= size; j++)
                    (matrix[column, j], matrix[pivot, j]) = (matrix[pivot, j], matrix[column, j]);

            double divisor = matrix[column, column];

            for (int j = column; j <= size; j++)
                matrix[column, j] /= divisor;

            for (int row = 0; row < size; row++)
            {
                if (row == column)
                    continue;

                var factor = matrix[row, column];

                for (var j = column; j <= size; j++)
                    matrix[row, j] -=
                        factor * matrix[column, j];
            }
        }

        double[] result = new double[size];

        for (int i = 0; i < size; i++)
            result[i] = matrix[i, size];

        return result;
    }
}

internal readonly struct Matrix3x3
{
    public Matrix3x3(
        double m11, double m12, double m13,
        double m21, double m22, double m23,
        double m31, double m32, double m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;

        M21 = m21;
        M22 = m22;
        M23 = m23;

        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    public double M11 { get; }
    public double M12 { get; }
    public double M13 { get; }

    public double M21 { get; }
    public double M22 { get; }
    public double M23 { get; }

    public double M31 { get; }
    public double M32 { get; }
    public double M33 { get; }
}