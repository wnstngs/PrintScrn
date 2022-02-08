using System;
using System.Runtime.InteropServices;

namespace PrintScrn.Native
{
    public static partial class Win32Type
    {
        private const int CCHDEVICENAME = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFOEX
        {
            public int Size;

            public RECT Monitor;

            public RECT WorkArea;

            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;

            public void Init()
            { 
                Size = 40 + 2 * CCHDEVICENAME;
                DeviceName = string.Empty;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }

            public int X
            {
                get => Left;
                set
                {
                    Right -= (Left - value);
                    Left = value;
                }
            }

            public int Y
            {
                get => Top;
                set
                {
                    Bottom -= (Top - value);
                    Top = value;
                }
            }

            public int Height
            {
                get => Bottom - Top;
                set => Bottom = value + Top;
            }

            public int Width
            {
                get => Right - Left;
                set => Right = value + Left;
            }

            public System.Drawing.Point Location
            {
                get => new(Left, Top);
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public System.Drawing.Size Size
            {
                get => new(Width, Height);
                set
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object? obj)
            {
                return obj switch
                {
                    RECT rect => Equals(rect),
                    System.Drawing.Rectangle rectangle => Equals(new RECT(rectangle)),
                    _ => false
                };
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle) this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    "{{Left={0},Top={1},Right={2},Bottom={3}}}",
                    Left, 
                    Top, 
                    Right, 
                    Bottom
                );
            }
        }

        [Flags]
        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            CAPTUREBLT = 0x40000000
        }
    }
}