using Godot;
using System;
using System.Xml.Schema;

public class Primitives2D
{

    public static bool CompareFloats(float x, float y)
    {
        const float epsilon = 1.192092896e-07f;

        return Math.Abs(x - y) <= epsilon * Math.Max(1.0f, Math.Max(Math.Abs(x), Math.Abs(y)));
    }

    public class MyLine
    {
        public Vector2 start, end;

        public MyLine()
        {
            start = new Vector2();
            end = new Vector2();
        }

        public MyLine(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public float Length()
        {
            return (this.end - this.start).Length();
        }

        public float LengthSquared()
        {
            return (this.end - this.start).LengthSquared();
        }
    }
    public class MyCircle
    {
        public Vector2 position;
        public float radius;

        public MyCircle()
        {
            radius = 1.0f;
        }

        public MyCircle(Vector2 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
    public class Rectangle
    {
        public Vector2 origin;
        public Vector2 size;

        public Rectangle()
        {
            size = new Vector2(1f, 1f);
        }

        public Rectangle(Vector2 origin, Vector2 size)
        {
            this.origin = origin;
            this.size = size;
        }

        public Vector2 GetMin()
        {
            Vector2 p1 = origin;
            Vector2 p2 = origin + size;

            return new Vector2(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y));
        }

        public Vector2 GetMax()
        {
            Vector2 p1 = origin;
            Vector2 p2 = origin + size;

            return new Vector2(Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y));
        }

        public static Rectangle FromMinMax(Vector2 min, Vector2 max)
        {
            return new Rectangle(min, max - min);
        }
       
    }
    public class OrientedRectangle
    {
        public Vector2 position;
        public Vector2 halfExtents;
        public float rotation;

        public OrientedRectangle()
        {
            halfExtents = new Vector2(1f, 1f);
            rotation = 0f;
        }
        public OrientedRectangle(Vector2 position, Vector2 halfExtents)
        {
            this.position = position;
            this.halfExtents = halfExtents;
            this.rotation = 0;
        }
        public OrientedRectangle(Vector2 position, Vector2 halfExtents, float rotation)
        {
            this.position = position;
            this.halfExtents = halfExtents;
            this.rotation = rotation;
        }
    }



}
