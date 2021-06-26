using System;
using System.Drawing;

namespace Utility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property)]
    public class RectAttribute : Attribute
    {
        public RectAttribute(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Rectangle Rectangle => new(X, Y, Width, Height);
    }
}