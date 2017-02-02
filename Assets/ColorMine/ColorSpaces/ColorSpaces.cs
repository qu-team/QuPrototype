using ColorMine.ColorSpaces.Conversions;
using Color = UnityEngine.Color;

namespace ColorMine.ColorSpaces {

/// <summary>
/// Defines the public methods for all color spaces
/// </summary>
public interface IColorSpace {
    /// <summary>
    /// Initialize settings from an Rgb object
    /// </summary>
    /// <param name="color"></param>
    void Initialize(IRgb color);

    /// <summary>
    /// Convert the color space to Rgb, you should probably using the "To" method instead. Need to figure out a way to "hide" or otherwise remove this method from the public interface.
    /// </summary>
    /// <returns></returns>
    IRgb ToRgb();

    /// <summary>
    /// Convert any IColorSpace to any other IColorSpace.
    /// </summary>
    /// <typeparam name="T">IColorSpace type to convert to</typeparam>
    /// <returns></returns>
    T To<T>() where T : IColorSpace, new();

    /// <summary>
    /// Determine how close two IColorSpaces are to each other using a passed in algorithm
    /// </summary>
    /// <param name="compareToValue">Other IColorSpace to compare to</param>
    /// <param name="comparer">Algorithm to use for comparison</param>
    /// <returns>Distance in 3d space as double</returns>
    double Compare(IColorSpace compareToValue, IColorSpaceComparison comparer);
}

/// <summary>
/// Abstract ColorSpace class, defines the To method that converts between any IColorSpace.
/// </summary>
public abstract class ColorSpace : IColorSpace
{
    public abstract void Initialize(IRgb color);
    public abstract IRgb ToRgb();

    /// <summary>
    /// Convienience method for comparing any IColorSpace
    /// </summary>
    /// <param name="compareToValue"></param>
    /// <param name="comparer"></param>
    /// <returns>Single number representing the difference between two colors</returns>
    public double Compare(IColorSpace compareToValue, IColorSpaceComparison comparer)
    {
        return comparer.Compare(this, compareToValue);
    }

    /// <summary>
    /// Convert any IColorSpace to any other IColorSpace
    /// </summary>
    /// <typeparam name="T">Must implement IColorSpace, new()</typeparam>
    /// <returns></returns>
    public T To<T>() where T : IColorSpace, new()
    {
        if (typeof(T) == GetType())
        {
            return (T)MemberwiseClone();
        }

        var newColorSpace = new T();
        newColorSpace.Initialize(ToRgb());

        return newColorSpace;
    }
}

public interface IRgb : IColorSpace {
    double R { get; set; }
    double G { get; set; }
    double B { get; set; }
}

public class Rgb : ColorSpace, IRgb {

    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }

    public override void Initialize(IRgb color) {
        RgbConverter.ToColorSpace(color,this);
    }

    public override IRgb ToRgb() {
        return RgbConverter.ToColor(this);
    }

    public static implicit operator Color(Rgb rgb) {
        return new Color((float)rgb.R / 256f, (float)rgb.G / 256f, (float)rgb.B / 256f);
    }
}

public interface IXyz : IColorSpace {
    double X { get; set; }
    double Y { get; set; }
    double Z { get; set; }
}

public class Xyz : ColorSpace, IXyz {

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public override void Initialize(IRgb color) {
        XyzConverter.ToColorSpace(color,this);
    }

    public override IRgb ToRgb() {
        return XyzConverter.ToColor(this);
    }
}

public interface ILab : IColorSpace {
    double L { get; set; }
    double A { get; set; }
    double B { get; set; }
}

public class Lab : ColorSpace, ILab {

    public double L { get; set; }
    public double A { get; set; }
    public double B { get; set; }

    public override void Initialize(IRgb color) {
        LabConverter.ToColorSpace(color,this);
    }

    public override IRgb ToRgb() {
        return LabConverter.ToColor(this);
    }
}

}
