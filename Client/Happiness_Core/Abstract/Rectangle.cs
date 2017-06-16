using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public struct Rectangle
    {
        int _x, _y, _width, _height;
        public Rectangle(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public bool Contains(int x, int y)
        {
            return x >= _x && x <= (_x + _width) && y >= _y && y <= (_y + _height);
        }

        public void Offset(int x, int y)
        {
            _x += x;
            _y += y;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }

        public int Top { get { return _y; } }
        public int Bottom { get { return _y + _height; } }

        public int Left { get { return _x; } }
        public int Right { get { return _x + _width; } }

        public Vector2 Center { get { return new Vector2(_x + ((Right - Left) * 0.5f), _y + ((Bottom - Top) * 0.5f)); } }

        public static Rectangle Empty { get { return new Rectangle(0, 0, 0, 0); } }
    }
}
