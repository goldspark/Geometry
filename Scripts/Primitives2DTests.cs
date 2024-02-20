using System;
using System.Diagnostics;
using System.Dynamic;
using Godot;
using static Primitives2D;

public class Primitives2DTests
{
    //Projeciton on separating axis result
    public struct Interval
    {
        public float min;
        public float max;
    }


    //Using slope-intercept form
    public static bool PointOnLine(Vector2 point, MyLine line)
    {
        float dy = (line.end.y - line.start.y);
        float dx = (line.end.x - line.start.x);
        float m = dy / dx;
        float b = line.start.y - m * line.start.x;
        return Primitives2D.CompareFloats(point.y, m * point.x + b);
    }

    public static bool PointInCircle(Vector2 point, MyCircle c)
    {
        MyLine line = new MyLine(point, c.position);

        if(line.LengthSquared() < c.radius * c.radius)
        {
            return true;
        }

        return false;
    }

    public static bool PointInRectangle(Vector2 p, Rectangle rectangle)
    {
        Vector2 min = rectangle.GetMin();
        Vector2 max = rectangle.GetMax();

        return min.x <= p.x &&
             min.y <= p.y &&
             max.x >= p.x &&
             max.y >= p.y;
    }

    public static bool PointInOrientedRectangle(Vector2 p, OrientedRectangle rectangle)
    {
        Vector2 rotVector = p - rectangle.position;

        float theta = -rectangle.rotation;

        var t = new Transform2D();
        t.x.x = t.y.y = Mathf.Cos(theta);
        t.x.y = t.y.x = -Mathf.Sin(theta);
        

        rotVector = t * rotVector;

        Rectangle localRect = new Rectangle(new Vector2(0, 0), rectangle.halfExtents * 2.0f);
        Vector2 localPoint = rotVector + rectangle.halfExtents;

        return PointInRectangle(localPoint, localRect);

    }

    public static bool LineCircle(MyLine l, MyCircle c)
    {
        Vector2 d = l.end - l.start;
        float t = (c.position - l.start).Dot(d) / d.Dot(d);

        t = Mathf.Max(t, 0.0f);
        t = Mathf.Min(t, 1.0f);

        Vector2 closestPoint = l.start + t * d;
        MyLine circleToClosest = new MyLine(c.position, closestPoint);

        return circleToClosest.LengthSquared() < c.radius * c.radius;
   
    }

    public static Vector2 ClosestPoint(MyLine l, MyCircle c)
    {
        Vector2 d = l.end - l.start;
        float t = (c.position - l.start).Dot(d) / d.Dot(d);

        t = Mathf.Max(t, 0.0f);
        t = Mathf.Min(t, 1.0f);

        Vector2 closestPoint = l.start + d * t;

        return closestPoint;
    }

    public static bool LineLine(MyLine l1, MyLine l2, ref Vector2 pointOfIntersection)
    {
        Vector2 a = l1.start;
        Vector2 b = l1.end;
        Vector2 c = l2.start;
        Vector2 d = l2.end;

        Vector2 ab = b - a;
        Vector2 cd = d - c;

        float aTop = (d.x - c.x) * (c.y - a.y) - (d.y - c.y) * (c.x - a.x);
        float bottom = (d.x - c.x) * (b.y - a.y) - (d.y - c.y) * (b.x - a.x);
        float bTop = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    

        if (bottom == 0)
            return false;


        float alpha = aTop / bottom;
        float beta = bTop / bottom;

        if(alpha > 0 && alpha <= 1f && beta > 0 && beta <= 1f)
        {
            pointOfIntersection = a + alpha * ab;
            return true;
        }

        return false;

    }

    public static bool LineRectangle(MyLine l, Rectangle r)
    {
        if(PointInRectangle(l.start, r) || PointInRectangle(l.end, r))
        {
            return true;
        }

        Vector2 norm = (l.end - l.start).Normalized();
        norm.x = (norm.x != 0) ? 1.0f / norm.x : 0;
        norm.y = (norm.y != 0) ? 1.0f / norm.y : 0;

        Vector2 min = (r.GetMin() - l.start) * norm;
        Vector2 max = (r.GetMax() - l.start) * norm;

        float tmin = Mathf.Max(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y));
        float tmax = Mathf.Min(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));

        if (tmax < 0 || tmin > tmax)
        {
            return false;
        }

        float t = (tmin < 0.0f) ? tmax : tmin;
        return t > 0.0f && t * t < l.LengthSquared();

    }

    public static bool LineOrientedRecrangle(MyLine line, OrientedRectangle rectangle)
    {
        float theta = -rectangle.rotation;

        var t = Transform2D.Identity;
        t.x.x = t.y.y = Mathf.Cos(theta);
        t.x.y = Mathf.Sin(theta);
        t.y.x = Mathf.Sin(theta);
        t.y.x *= -1;

        MyLine localLine = new MyLine();

        Vector2 rotVector = line.start - rectangle.position;
        rotVector = t * rotVector;
        localLine.start = rotVector + rectangle.halfExtents;
        rotVector = line.end - rectangle.position;
        rotVector = t * rotVector;
        localLine.end = rotVector + rectangle.halfExtents;

        Rectangle localRect = new Rectangle(new Vector2(0, 0), rectangle.halfExtents * 2.0f);

        return LineRectangle(localLine, localRect);
    }


    public static bool CircleRectangle(MyCircle circle, Rectangle rectangle)
    {
        Vector2 min = rectangle.GetMin();
        Vector2 max = rectangle.GetMax();

        //now we finding the closest point on the rect to the pos of the circle
        Vector2 closestPoint = circle.position;
        if (closestPoint.x < min.x)
            closestPoint.x = min.x;
        else if (closestPoint.x > max.x)
            closestPoint.x = max.x;

        if (closestPoint.y < min.y)
            closestPoint.y = min.y;
        else if (closestPoint.y > max.y)
            closestPoint.y = max.y;

        MyLine line = new MyLine(circle.position, closestPoint);
        return line.LengthSquared() <= circle.radius * circle.radius;


    }

    public static bool CircleOrientedRectangle(MyCircle circle, OrientedRectangle rectangle)
    {
        Vector2 r = circle.position - rectangle.position;

        float theta = -rectangle.rotation;

        var t = new Transform2D();
        t.x.x = t.y.y = Mathf.Cos(theta);
        t.x.y = t.y.x = Mathf.Sin(theta);
        t.y.x *= -1;

        r = t * r;

        MyCircle c = new MyCircle(r + rectangle.halfExtents, circle.radius);

        Rectangle rect = new Rectangle(new Vector2(0, 0), rectangle.halfExtents * 2.0f);

        return CircleRectangle(c, rect);

    }

    public static bool RectangleRectangle(Rectangle rect1, Rectangle rect2)
    {
        Vector2 aMin = rect1.GetMin();
        Vector2 aMax = rect1.GetMax();
        Vector2 bMin = rect2.GetMin();
        Vector2 bMax = rect2.GetMax();

        bool overX = ((bMin.x <= aMax.x) && (aMin.x <= bMax.x));
        bool overY = ((bMin.y <= aMax.y) && (aMin.y <= bMax.y));

        return overX && overY;
    }

    

    public static Interval GetInterval(Rectangle rect, Vector2 axis)
    {
        Interval result = new Interval();

        Vector2 min = rect.GetMin();
        Vector2 max = rect.GetMax();

        Vector2[] verts = { //all vertices of rect
            new Vector2(min.x, min.y), new Vector2(min.x, max.y),
            new Vector2(max.x, max.y), new Vector2(max.x, min.y),
        };

        result.min = result.max = axis.Dot(verts[0]);

        for(int i = 0; i < 4; i++)
        {
            float proj = axis.Dot(verts[i]);
            if(proj < result.min) 
                result.min = proj;
            if (proj > result.max)
                result.max = proj;
        }

        return result;
    }


    public static Interval GetInterval(OrientedRectangle rect, Vector2 axis)
    {
        Rectangle nonOriented = new Rectangle(rect.position - rect.halfExtents, rect.halfExtents * 2.0f);

        Vector2 min = nonOriented.GetMin();
        Vector2 max = nonOriented.GetMax();

        Vector2[] nonOrientedVerts =
        {
            min, max,
            new Vector2(min.x, max.y), new Vector2(max.x, min.y)
        };

        float theta = Mathf.Deg2Rad(rect.rotation);
        var t = new Transform2D();
        t.x.x = t.y.y = Mathf.Cos(theta);
        t.x.y = t.y.x = Mathf.Sin(theta);
        t.y.x *= -1;


        for(int i = 0; i < nonOrientedVerts.Length; i++)
        {
            Vector2 p = nonOrientedVerts[i] - rect.position;
            p *= t;
            nonOrientedVerts[i] = rect.position + p;
        }

        Interval res = new Interval();
        res.min = res.max = axis.Dot(nonOrientedVerts[0]);
        for(int i = 0; i < 4; i++)
        {
            float proj = axis.Dot(nonOrientedVerts[i]);

            if (proj < res.min)
                res.min = proj;
            if(proj > res.max)
                res.max = proj;
        }
        return res;
    }

    public static bool OverlapOnAxis(Rectangle rect1, Rectangle rect2, Vector2 axis)
    {
        Interval a = GetInterval(rect1, axis);
        Interval b = GetInterval(rect2, axis);

        return ((b.min <= a.max) && (a.min <= b.max));
    }

    public static bool RectRectSAT(Rectangle rect1, Rectangle rect2)
    {
        Vector2[] axisToTest = { new Vector2(1, 0), new Vector2(0, 1) };

        for(int i = 0; i < 2; i++)
        {
            if(!OverlapOnAxis(rect1, rect2, axisToTest[i]))
            {
                return false;
            }
        }

        return true;
    }

    /*
     * bool GenericSAT(Shape shape1, Shape shape2) {
   // 1) Test the face normals of object 1 as the separating axis
   std::vector<mathVector>normals = GetFaceNormals(shape1);
   for (int i = 0; i<normals.size(); ++i) {
      if (!OverlapOnAxis(shape1, shape2, normals[i])) {
         return true; // Seperating axis found, early out
      }
   }
   // 2) Test the face normals of object 2 as the separating axis
   normals = GetFaceNormals(shape2);
   for (int i = 0; i<normals.size(); ++i) {
      if (!OverlapOnAxis(shape1, shape2, normals[i])) {
         return true; // Seperating axis found, early out
      }
   }
   //3) Check the normalized cross product of each shapes edges.
   std::vector<mathVector> edges1 = GetEdges(shape1);
   std::vector<mathVector> edges2 = GetEdges(shape2);
   for (int i = 0; i< edges1.size(); ++i) {
      for (int j = 0; j < edges2.size(); ++j) {
         mathVector testAxis = Cross(edges1[i], edges2[j]);
         if (!OverlapOnAxis(shape1, shape2, testAxis)) {
            return true; // Separating axis found, early out
         }
      }
   }
   // No separating axis found, the objects do not intersect
   return false;
}
     * 
     */

}
