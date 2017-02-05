using System;

namespace Utils
{
    internal static class ConvertM
    {
        static public double mmToFeet(double x)
        {
            return x / 304.8;
        }
        static public double feetTomm(double x)
        {
            return x / 0.00328084;
        }
        static public double cmToFeet(double x)
        {
            return x / 30.48;
        }
        static public double feetToCm(double x)
        {
            return x / 0.0328084;
        }
        static public double feetToM(double x)
        {
            return x / 3.28084;
        }
        static public double degreesToRadians(double x)
        {
            return (x * (Math.PI / 180));
        }
        static public double radiansToDegrees(double x)
        {
            return (x * (180 / Math.PI));
        }
    }
}
