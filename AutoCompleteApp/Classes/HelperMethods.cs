
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoCompleteApp.Classes
{
    public static class HelperMethods
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData,
          int dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        public static IntPtr handle;
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public static Cursor Current { get; set; }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        public static bool getpixelsFireFox(bool isBing)
        {
            Thread.Sleep(5000);
            System.Drawing.Bitmap bmpScreenshot = Screenshot();
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.firefoxBing, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.firefoxclose, bmpScreenshot, out location);
        }

        public static Bitmap Screenshot()
        {
            Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);


            Graphics g = Graphics.FromImage(bmpScreenshot);

            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);

            return bmpScreenshot;
        }

        public static bool FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack, out Point location)
        {
            for (int outerX = 0; outerX < bmpHaystack.Width - bmpNeedle.Width; outerX++)
            {
                for (int outerY = 0; outerY < bmpHaystack.Height - bmpNeedle.Height; outerY++)
                {
                    for (int innerX = 0; innerX < bmpNeedle.Width; innerX++)
                    {
                        for (int innerY = 0; innerY < bmpNeedle.Height; innerY++)
                        {
                            Color cNeedle = bmpNeedle.GetPixel(innerX, innerY);
                            Color cHaystack = bmpHaystack.GetPixel(innerX + outerX, innerY + outerY);

                            if (cNeedle.R != cHaystack.R || cNeedle.G != cHaystack.G || cNeedle.B != cHaystack.B)
                            {
                                goto notFound;
                            }
                        }
                    }
                    location = new Point(outerX, outerY);
                    return true;
                    notFound:
                    continue;
                }
            }
            location = Point.Empty;
            return false;
        }

        public static bool getpixelsFireFoxSearchDone(bool isBing)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.bingFireFoxSearchDone, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.firefoxSearchDone, bmpScreenshot, out location);
        }
           
        public static void BrowserSecondStep(string browserName)
        {
           
            ScrollingTheMouse();
            Thread.Sleep(3000);
            MoveMouseAround(); 
        }

        public static void MouseClick()
        {
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep((new Random()).Next(20, 30));
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        private static Point GetCursorPosition()
        {
            var cursor = new Cursor(Cursor.Current.Handle);            
            Point p = new Point(Cursor.Position.X, Cursor.Position.Y);
            return p;
        }

        public static void LinearSmoothMove(Point newPosition, int steps)
        {
            Point start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;
            Random rnd = new Random();
            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                Cursor.Position = Point.Round(iterPoint); 
                Thread.Sleep(rnd.Next(3, 10));
            } 
        }

        public static void MoveMouseAround()
        {
            Random rnd = new Random();
            RECT rect = new RECT();
            GetWindowRect(GetForegroundWindow(), out rect);
            int centerRight = rect.Right / 2;
            int centerBottom = rect.Bottom / 2;
            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);

            Point start = GetCursorPosition();
            LinearSmoothMove(new Point(start.X + rnd.Next(100, 200), start.Y + rnd.Next(150, 300)), 50);
            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(start.X - rnd.Next(150, 225), start.Y - rnd.Next(100, 150)), 40);

            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);
            ScrollingTheMouse();
        }

        public static void LinearSmoothMoveReverse(Point newPosition, int steps)
        {
            Point start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X + start.X, newPosition.Y + start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;
            Random rnd = new Random();
            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X - slope.X, iterPoint.Y - slope.Y);
                Cursor.Position = Point.Round(iterPoint);
                //SetCursorPosition(Point.Round(iterPoint));
                Thread.Sleep(rnd.Next(5, 20));
            } 
        }
                 
        private static void ScrollingTheMouse()
        {
            for (int a = 0; a <= 1; a++)
            {
                for (int i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.WHEEL, 0, 0, -50 + (new Random()).Next(10, 30), 0);
                    Thread.Sleep((new Random()).Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep((new Random()).Next(400, 500));
                for (int i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.WHEEL, 0, 0, 50 + (new Random()).Next(15, 30), 0);
                    Thread.Sleep((new Random()).Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep((new Random()).Next(400, 700));
            }
        }
         
    }
}
