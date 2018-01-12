using UnityEngine;
using System.Collections.Generic;
using System;

public class PolyhedronBuilder
{
    public List<int> Triangles;
    public List<Vector3> Vertices;
    public List<BuilderPolygon> Polygons;
    public PolyhedronBuilder()
    {
        Triangles = new List<int>();
        Vertices = new List<Vector3>();
        Polygons = new List<BuilderPolygon>();
    }
    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) // clockwise; top left is a, top right is b, bottom right is c, bottom left is d
    {
        AddPolygon(a, b, c, d);
    }
    public void AddPolygon(params Vector3[] v)
    {
        Vertices.AddRange(v);
        for (int i = 0; i < v.Length - 2; i++)
        {
            Triangles.Add(Vertices.Count - v.Length);
            Triangles.Add(Vertices.Count - (i + 1));
            Triangles.Add(Vertices.Count - (i + 2));
        }
        Polygons.Add(new BuilderPolygon(this, v));
    }
    public Mesh Generate()
    {
        Mesh m = new Mesh();
        m.vertices = Vertices.ToArray();
        m.triangles = Triangles.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }
    public BuilderPolygon LastPolygon()
    {
        return Polygons[Polygons.Count - 1];
    }
    public BuilderPolygon Polygon(int i)
    {
        return Polygons[i];
    }
}
public class BuilderPolygon
{
    public Vector3[] Vertices;
    public Vector3 Normal;
    PolyhedronBuilder parent;
    public int Index;
    public BuilderPolygon(PolyhedronBuilder p, params Vector3[] v)
    {
        parent = p;
        Vertices = v;
        Index = parent.Triangles.Count - getTriangleCount();
        Normal = v.Normal();
    }
    public void Remove()
    {
        int tCount = getTriangleCount();
        parent.Triangles.RemoveRange(Index, tCount);
        foreach (BuilderPolygon p in parent.Polygons)
        {
            if (p.Index > Index)
            {
                p.Index -= tCount;
            }
        }
    }
    public void Extrude(Vector3 offset)
    {
        Vector3[] newVertices = ExtrudeRaw(offset);
        Extrude(newVertices);
    }
    public void Extrude(Vector3[] newPoints)
    {
        if (newPoints.Length != Vertices.Length)
        {
            return;
        }
        Remove(); //breaks a lot when theres more than one removal.
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector3 a = newPoints[i];
            Vector3 b = Vertices[i];
            int nextPoint = i + 1;
            if (nextPoint == Vertices.Length)
            {
                nextPoint = 0;
            }
            Vector3 c = Vertices[nextPoint];
            Vector3 d = newPoints[nextPoint];
            parent.AddQuad(a, b, c, d);
        }
        parent.AddPolygon(newPoints);
    }
    public Vector3[] ExtrudeRaw(Vector3 offset)
    {
        List<Vector3> newVerts = new List<Vector3>();
        foreach (Vector3 v in Vertices)
        {
            newVerts.Add(v + offset);
        }
        return newVerts.ToArray();
    }
    int getTriangleCount()
    {
        return (Vertices.Length - 2) * 3;
    }
}
public static class PolyhedronExtensions
{
    public static bool useQuaternion = false;
    public static Vector3 Normal(this Vector3[] v)
    {
        return (Vector3.Cross((v[3] - v[0]), (v[1] - v[0]))).normalized;
    }
    public static Vector3[] QuadScale(this Vector3[] v, float factor, Vector3 centre)
    {
        if (v.Length != 4)
        {
            return v;
        }
        List<Vector3> ret = new List<Vector3>();
        foreach (Vector3 vert in v)
        {
            float dX = vert.x - centre.x;
            float dY = vert.y - centre.y;
            float dZ = vert.z - centre.z;
            ret.Add(new Vector3(centre.x + (dX * factor), centre.y + (dY * factor), centre.z + (dZ * factor)));
        }
        return ret.ToArray();
    }
    public static Vector3[] QuadScale(this Vector3[] v, float factor)
    {
        return v.QuadScale(factor, v.QuadMidpoint());
    }
    public static Vector3 Rotate(this Vector3 v, float x, float y, float z, Vector3 centre)
    {
        float rX = x * Mathf.Deg2Rad;
        float rY = y * Mathf.Deg2Rad;
        float rZ = z * Mathf.Deg2Rad;
        Vector3 translated = v - centre;
        float[] temp = new float[3];

        float[] xTrig = { Mathf.Sin(rX), Mathf.Cos(rX) };
        temp[0] = translated.x;
        temp[1] = (xTrig[1] * translated.y) - (xTrig[0] * translated.z);
        temp[2] = (xTrig[0] * translated.y) + (xTrig[1] * translated.z);
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        float[] yTrig = { Mathf.Sin(rY), Mathf.Cos(rY) };
        temp[0] = (yTrig[1] * translated.x) + (yTrig[0] * translated.z);
        temp[1] = translated.y;
        temp[2] = (yTrig[1] * translated.z) - (yTrig[0] * translated.x);
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        float[] zTrig = { Mathf.Sin(rZ), Mathf.Cos(rZ) };
        temp[0] = (zTrig[1] * translated.x) - (zTrig[0] * translated.y);
        temp[1] = (zTrig[0] * translated.x) + (zTrig[1] * translated.y);
        temp[2] = translated.z;
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        translated = translated + centre;

        return translated;
    }
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z, Vector3 centre)
    {
        if (v.Length != 4)
        {
            return v;
        }
        Vector3[] rotated = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            rotated[i] = v[i].Rotate(x, y, z, centre);
        }
        return rotated;
    }
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z) // do this all in degrees because its easier
    {
        return v.QuadRotate(x, y, z, v.QuadMidpoint());
    }
    public static float NextFloat(this System.Random rng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
    public static Vector3 QuadMidpoint(this Vector3[] v)
    {
        if (v.Length != 4)
        {
            return new Vector3(0, 0, 0);
        }
        float mX = 0;
        float mY = 0;
        float mZ = 0;
        foreach (Vector3 vert in v)
        {
            mX += vert.x;
            mY += vert.y;
            mZ += vert.z;
        }
        mX = mX / 4f;
        mY = mY / 4f;
        mZ = mZ / 4f;
        return new Vector3(mX, mY, mZ);
    }
    public static Vector3 RotateTo(this Vector3 initialVector, Vector3 desiredVector)
    {
        if (useQuaternion)
        {
            return initialVector.RotateToQuaternion(desiredVector);
        }
        else
        {
            return initialVector.RotateToMatrix(desiredVector);
        }
    }
    static Vector3 RotateToQuaternion(this Vector3 initialVector, Vector3 desiredVector)
    {
        Vector3 u = initialVector.normalized;
        Vector3 v = desiredVector.normalized;
        Quaternion q;
        Vector3 a = Vector3.Cross(u, v);
        q.x = a.x;
        q.y = a.y;
        q.z = a.z;
        q.w = Mathf.Sqrt(u.sqrMagnitude * v.sqrMagnitude) + Vector3.Dot(u, v);
        return q.eulerAngles;
    }
    static int GetSign(this float k)
    {
        if (k == 0)
        {
            return 1;
        }
        return Convert.ToInt16((Mathf.Abs(k) / k));
    }
    // my implementation based on solving matrices as sim equations
    static Vector3 RotateToMatrix(this Vector3 initialVector, Vector3 desiredVector)
    {
        Vector3 u = initialVector;
        Vector3 v = desiredVector;
        if (u.x == v.x && u.y == v.y && u.z == v.z)
        {
            return new Vector3(0, 0, 0);
        }
        float x = 0;
        float xCos01 = Mathf.Acos(((v.y * u.y) + (v.z * u.z)) / (Mathf.Pow(u.y, 2) + Mathf.Pow(u.z, 2)));
        float xCos02 = (Mathf.PI * 2) - xCos01;
        float xSin01 = Mathf.Asin(((v.z * u.y) - (v.y * u.z)) / (Mathf.Pow(u.y, 2) + Mathf.Pow(u.z, 2)));
        float xSin02 = Mathf.PI - xSin01;
        x = TrigSolutionOverlap(xSin01, xSin02, xCos01, xCos02, u, v);
        if (u.x == -v.x && u.x != 0)
        {
            x = -Mathf.PI;
        }

        float y = 0;
        float yCos01 = Mathf.Acos(((v.x * u.x) + (v.z * u.z)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.z, 2)));
        float yCos02 = (Mathf.PI * 2) - yCos01;
        float ySin01 = Mathf.Asin(((v.x * u.z) - (v.z * u.x)) / (Mathf.Pow(u.z, 2) + Mathf.Pow(u.x, 2)));
        float ySin02 = Mathf.PI - ySin01;
        y = TrigSolutionOverlap(ySin01, ySin02, yCos01, yCos02, u, v);
        if (u.y == -v.y && u.y != 0)
        {
            y = -Mathf.PI;
        }

        float z = 0;
        float zCos01 = Mathf.Acos(((v.x * u.x) + (v.y * u.y)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.y, 2)));
        float zCos02 = (Mathf.PI * 2) - zCos01;
        float zSin01 = Mathf.Asin(((v.y * u.x) - (v.x * u.y)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.y, 2)));
        float zSin02 = Mathf.PI - zSin01;
        z = TrigSolutionOverlap(zSin01, zSin02, zCos01, zCos02, u, v);
        if (u.z == -v.z && u.z != 0)
        {
            z = -Mathf.PI;
        }

        return new Vector3(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
    }
    public static float TrigSolutionOverlap(float s1, float s2, float c1, float c2, Vector3 u, Vector3 v, int accuracy = 4)
    {
        float[] s = { s1, s2 };
        if (s[0] < 0)
        {
            s[0] += Mathf.PI * 2;
        }
        if (s[1] < 0)
        {
            s[1] += Mathf.PI * 2;
        }
        float[] c = { c1, c2 };
        if (c[0] < 0)
        {
            c[0] += Mathf.PI * 2;
        }
        if (c[1] < 0)
        {
            c[1] += Mathf.PI * 2;
        }
        foreach (float sin in s)
        {
            foreach (float cos in c)
            {
                if (Math.Round(sin, accuracy) == Math.Round(cos, accuracy))
                {
                    return sin;
                }
            }
        }
        return 0;
    }
}
/*
 using UnityEngine;
using System.Collections.Generic;
using System;

public class PolyhedronBuilder
{
    public List<int> Triangles;
    public List<Vector3> Vertices;
    public List<BuilderPolygon> Polygons;
    public PolyhedronBuilder()
    {
        Triangles = new List<int>();
        Vertices = new List<Vector3>();
        Polygons = new List<BuilderPolygon>();
    }
    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) // clockwise; top left is a, top right is b, bottom right is c, bottom left is d
    {
        AddPolygon(a, b, c, d);
    }
    public void AddPolygon(params Vector3[] v)
    {
        Vertices.AddRange(v);
        for (int i = 0; i < v.Length - 2; i++)
        {
            Triangles.Add(Vertices.Count - v.Length);
            Triangles.Add(Vertices.Count - (i + 1));
            Triangles.Add(Vertices.Count - (i + 2));
        }
        Polygons.Add(new BuilderPolygon(this, v));
    }
    public Mesh Generate()
    {
        Mesh m = new Mesh();
        m.vertices = Vertices.ToArray();
        m.triangles = Triangles.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }
    public BuilderPolygon LastPolygon()
    {
        return Polygons[Polygons.Count - 1];
    }
    public BuilderPolygon Polygon(int i)
    {
        return Polygons[i];
    }
}
public class BuilderPolygon
{
    public Vector3[] Vertices;
    public Vector3 Normal;
    PolyhedronBuilder parent;
    public int Index;
    public BuilderPolygon(PolyhedronBuilder p, params Vector3[] v)
    {
        parent = p;
        Vertices = v;
        Index = parent.Triangles.Count - getTriangleCount();
        Normal = v.Normal();
    }
    public void Remove()
    {
        int tCount = getTriangleCount();
        parent.Triangles.RemoveRange(Index, tCount);
        foreach (BuilderPolygon p in parent.Polygons)
        {
            if (p.Index > Index)
            {
                p.Index -= tCount;
            }
        }
    }
    public void Extrude(Vector3 offset)
    {
        Vector3[] newVertices = ExtrudeRaw(offset);
        Extrude(newVertices);
    }
    public void Extrude(Vector3[] newPoints)
    {
        if (newPoints.Length != Vertices.Length)
        {
            return;
        }
        Remove(); //breaks a lot when theres more than one removal.
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector3 a = newPoints[i];
            Vector3 b = Vertices[i];
            int nextPoint = i + 1;
            if (nextPoint == Vertices.Length)
            {
                nextPoint = 0;
            }
            Vector3 c = Vertices[nextPoint];
            Vector3 d = newPoints[nextPoint];
            parent.AddQuad(a, b, c, d);
        }
        parent.AddPolygon(newPoints);
    }
    public Vector3[] ExtrudeRaw(Vector3 offset)
    {
        List<Vector3> newVerts = new List<Vector3>();
        foreach (Vector3 v in Vertices)
        {
            newVerts.Add(v + offset);
        }
        return newVerts.ToArray();
    }
    int getTriangleCount()
    {
        return (Vertices.Length - 2) * 3;
    }
}
public static class PolyhedronExtensions
{
    public static bool useQuaternion = false;
    public static Vector3 Normal(this Vector3[] v)
    {
        return (Vector3.Cross((v[2] - v[0]), (v[1] - v[0]))).normalized;
    }
    public static Vector3[] QuadScale(this Vector3[] v, float factor, Vector3 centre)
    {
        if (v.Length != 4)
        {
            return v;
        }
        List<Vector3> ret = new List<Vector3>();
        foreach (Vector3 vert in v)
        {
            float dX = vert.x - centre.x;
            float dY = vert.y - centre.y;
            float dZ = vert.z - centre.z;
            ret.Add(new Vector3(centre.x + (dX * factor), centre.y + (dY * factor), centre.z + (dZ * factor)));
        }
        return ret.ToArray();
    }
    public static Vector3[] QuadScale(this Vector3[] v, float factor)
    {
        return v.QuadScale(factor, v.QuadMidpoint());
    }
    public static Vector3 Rotate(this Vector3 v, float x, float y, float z, Vector3 centre)
    {
        float rX = x * Mathf.Deg2Rad;
        float rY = y * Mathf.Deg2Rad;
        float rZ = z * Mathf.Deg2Rad;
        Vector3 translated = v - centre;
        float[] temp = new float[3];

        float[] xTrig = { Mathf.Sin(rX), Mathf.Cos(rX) };
        temp[0] = translated.x;
        temp[1] = (xTrig[1] * translated.y) - (xTrig[0] * translated.z);
        temp[2] = (xTrig[0] * translated.y) + (xTrig[1] * translated.z);
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        float[] yTrig = { Mathf.Sin(rY), Mathf.Cos(rY) };
        temp[0] = (yTrig[1] * translated.x) + (yTrig[0] * translated.z);
        temp[1] = translated.y;
        temp[2] = (yTrig[1] * translated.z) - (yTrig[0] * translated.x);
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        float[] zTrig = { Mathf.Sin(rZ), Mathf.Cos(rZ) };
        temp[0] = (zTrig[1] * translated.x) - (zTrig[0] * translated.y);
        temp[1] = (zTrig[0] * translated.x) + (zTrig[1] * translated.y);
        temp[2] = translated.z;
        translated.x = temp[0];
        translated.y = temp[1];
        translated.z = temp[2];

        translated = translated + centre;

        return translated;
    }
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z, Vector3 centre)
    {
        if (v.Length != 4)
        {
            return v;
        }
        Vector3[] rotated = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            rotated[i] = v[i].Rotate(x, y, z, centre);
        }
        return rotated;
    }
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z) // do this all in degrees because its easier
    {
        return v.QuadRotate(x, y, z, v.QuadMidpoint());
    }
    public static float NextFloat(this System.Random rng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
    public static Vector3 QuadMidpoint(this Vector3[] v)
    {
        if (v.Length != 4)
        {
            return new Vector3(0, 0, 0);
        }
        float mX = 0;
        float mY = 0;
        float mZ = 0;
        foreach (Vector3 vert in v)
        {
            mX += vert.x;
            mY += vert.y;
            mZ += vert.z;
        }
        mX = mX / 4f;
        mY = mY / 4f;
        mZ = mZ / 4f;
        return new Vector3(mX, mY, mZ);
    }
    public static Vector3 RotateTo(this Vector3 initialVector, Vector3 desiredVector)
    {
        if (useQuaternion)
        {
            return initialVector.RotateToQuaternion(desiredVector);
        }
        else
        {
            return initialVector.RotateToMatrix(desiredVector);
        }
    }
    static Vector3 RotateToQuaternion(this Vector3 initialVector, Vector3 desiredVector)
    {
        Vector3 u = initialVector.normalized;
        Vector3 v = desiredVector.normalized;
        Quaternion q;
        Vector3 a = Vector3.Cross(u, v);
        q.x = a.x;
        q.y = a.y;
        q.z = a.z;
        q.w = Mathf.Sqrt(u.sqrMagnitude * v.sqrMagnitude) + Vector3.Dot(u, v);
        return q.eulerAngles;
    }
    static Vector3 RotateToMatrix(this Vector3 initialVector, Vector3 desiredVector)
    {
        Vector3 u = initialVector.normalized;
        Vector3 v = desiredVector.normalized;
        float x = 0;
        float xCos01 = Mathf.Acos(((v.y * u.y) + (v.z * u.z)) / (Mathf.Pow(u.y, 2) + Mathf.Pow(u.z, 2)));
        float xCos02 = -xCos01;
        float xSin01 = Mathf.Asin(((v.z * u.y) - (v.y * u.z)) / (Mathf.Pow(u.y, 2) + Mathf.Pow(u.z, 2)));
        float xSin02 = Mathf.PI - xSin01;
        x = TrigSolutionOverlap(xSin01, xSin02, xCos01, xCos02);

        float y = 0;
        float yCos01 = Mathf.Acos(((v.x * u.x) + (v.z * u.z)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.z, 2)));
        float yCos02 = -yCos01;
        float ySin01 = Mathf.Asin(((v.x * u.z) - (v.z * u.x)) / (Mathf.Pow(u.z, 2) + Mathf.Pow(u.x, 2)));
        float ySin02 = Mathf.PI - ySin01;
        y = TrigSolutionOverlap(ySin01, ySin02, yCos01, yCos02);

        float z = 0;
        float zCos01 = Mathf.Acos(((v.x * u.x) + (v.y * u.y)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.y, 2)));
        float zCos02 = -zCos01;
        float zSin01 = Mathf.Asin(((v.y * u.x) - (v.x * u.y)) / (Mathf.Pow(u.x, 2) + Mathf.Pow(u.y, 2)));
        float zSin02 = Mathf.PI - zSin01;
        z = TrigSolutionOverlap(zSin01, zSin02, zCos01, zCos02);

        return new Vector3(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
    }
    public static float TrigSolutionOverlap(float s1, float s2, float c1, float c2, int accuracy = 4)
    {
        float[] s = { s1, s2 };
        float[] c = { c1, c2 };
        s = s.IntervalFix();
        c = c.IntervalFix();
        Debug.Log(string.Format("Sin {0}, Sin {1}, Cos {2}, Cos {3}", s[0], s[1], c[0], c[1]));
        foreach (float sin in s)
        {
            foreach (float cos in c)
            {
                if (Math.Round(sin, accuracy) == Math.Round(cos, accuracy))
                {
                    return sin;
                }
            }
        }
        return 0;
    }
    public static float[] IntervalFix(this float[] s)
    {
        float[] ret = new float[s.Length];
        for (int t = 0; t < s.Length; t++)
        {
            float n = s[t];
            if (float.IsNaN(n))
            {
                n = 0;
            }
            if (n <= 0)
            {
                n += Mathf.PI * 2;
            }
            ret[t] = n;
        }
        return ret;
    }
}*/
