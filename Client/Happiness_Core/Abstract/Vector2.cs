using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public struct Vector2
    {
        float _x, _y;
        public Vector2(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }

        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            return new Vector2(a._x + b._x, a._y + b._y);
        }

        public static Vector2 Sub(Vector2 a, Vector2 b)
        {
            return new Vector2(a._x - b._x, a._y - b._y);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {            
            return new Vector2(a._x + ((b._x - a._x) * t), a._y + ((b._y - a._y) * t));
        }

        public static Vector2 Negate(Vector2 n)
        {
            return new Vector2(-n._x, -n._y);
        }

        public static Vector2 Multiply(Vector2 v, float s)
        {
            return new Vector2(v._x * s, v._y * s);
        }

        public static Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return new Vector2(a._x * b._x, a._y * b._y);
        }

        static Vector2 _zero = new Vector2(0, 0);
        public static Vector2 Zero { get { return _zero; } }
    }
}
