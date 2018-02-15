using UnityEngine;
using System.Collections.Generic;
using System;

public class PolyhedronBuilder
{
    public List<int> Triangles; // list of all triangles
    public List<Vector3> Vertices; // list of all vertices
    public List<BuilderPolygon> Polygons; // list of all polygons
    public PolyhedronBuilder() // constructor initialises all lists
    {
        Triangles = new List<int>();
        Vertices = new List<Vector3>();
        Polygons = new List<BuilderPolygon>();
    }
    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) // clockwise; top left is a, top right is b, bottom right is c, bottom left is d
    {
        AddPolygon(a, b, c, d); // adds a polygon with the 4 points
    }
    public void AddPolygon(params Vector3[] v) // creates a polygon with n verts
    {
        Vertices.AddRange(v); // adds the params to the list
        for (int i = 0; i < v.Length - 2; i++) // iterates through the polygons to create the verticies
        {
            Triangles.Add(Vertices.Count - v.Length);
            Triangles.Add(Vertices.Count - (i + 1));
            Triangles.Add(Vertices.Count - (i + 2));
        }
        Polygons.Add(new BuilderPolygon(this, v)); // adds polygon to list
    }
    public Mesh Generate() // generates the actual mesh
    {
        Mesh m = new Mesh(); // the code below is quite self-explanatory, converts from lists to array, and calculates render normals and bounds
        m.vertices = Vertices.ToArray();
        m.triangles = Triangles.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        return m;
    }
    public BuilderPolygon LastPolygon() // literally just gets the last polygon from the list
    {
        return Polygons[Polygons.Count - 1];
    }
    public BuilderPolygon Polygon(int i) // gets the (i + 1)th polygon from the list
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
    public BuilderPolygon(PolyhedronBuilder p, params Vector3[] v) // creates the polygon with given params
    {
        parent = p;
        Vertices = v;
        Index = parent.Triangles.Count - getTriangleCount(); // calculate the index (for when something needs to be removed)
        Normal = v.Normal(); // get normal of a vector array
    }
    public void Remove() // remove the polygon from the builder
    {
        int tCount = getTriangleCount(); // get the triangle count
        parent.Triangles.RemoveRange(Index, tCount); // remove the given triangles from the builder's list
        foreach (BuilderPolygon p in parent.Polygons) // iterates through each polygon in the parent
        {
            if (p.Index > Index) // if the index is after this polygon's index
            {
                p.Index -= tCount; // reduce the index by the triangle count
            }
        }
    }
    public void Extrude(Vector3 offset) // extrude the polygon by the given offset
    {
        Vector3[] newVertices = ExtrudeRaw(offset); // calculate the extrusion
        Extrude(newVertices); // extrude by the offset
    }
    public void Extrude(Vector3[] newPoints) // extrude when given raw points
    {
        if (newPoints.Length != Vertices.Length) // if the points are mismatched
        {
            return;
        }
        Remove(); //breaks a lot when theres more than one removal.
        for (int i = 0; i < Vertices.Length; i++) // iterates through each vertex
        {
            Vector3 a = newPoints[i]; // takes the point from the extrusion
            Vector3 b = Vertices[i]; // with the corresponding point on the base shape
            int nextPoint = (i + 1) % Vertices.Length; // takes the next two points (using modulo to wrap around)
            Vector3 c = Vertices[nextPoint]; // does the same as above
            Vector3 d = newPoints[nextPoint];
            parent.AddQuad(a, b, c, d); // creates a new quad with the given points
        }
        parent.AddPolygon(newPoints); // create a polygon with the raw extruded points
    }
    public Vector3[] ExtrudeRaw(Vector3 offset) // calculate the extrusion points
    {
        List<Vector3> newVerts = new List<Vector3>(); // create a list of new vertices
        foreach (Vector3 v in Vertices) // iterates through each existing vertex
        {
            newVerts.Add(v + offset); // add the offset to the existing vertex
        }
        return newVerts.ToArray(); // returns the new points as an array
    }
    int getTriangleCount()
    {
        return (Vertices.Length - 2) * 3; // simple formula for calculating the number of triangles
    }
}
public static class PolyhedronExtensions
{
    public static bool useQuaternion = false; // toggle for whether to use the quaternion mode or to use my matrix solving mode
    public static Vector3 Normal(this Vector3[] v) // commonly used formula for calculating the normal of a triangle based on cross product
    {
        return (Vector3.Cross((v[3] - v[0]), (v[1] - v[0]))).normalized;
    }
    public static Vector3[] Scale(this Vector3[] v, float factor, Vector3 centre) // scaling method for quads
    {
        List<Vector3> ret = new List<Vector3>(); // the return value
        foreach (Vector3 vert in v) // iterates through each point
        {
            float dX = vert.x - centre.x; // translate to be relative to the centre
            float dY = vert.y - centre.y;
            float dZ = vert.z - centre.z;
            ret.Add(new Vector3(centre.x + (dX * factor), centre.y + (dY * factor), centre.z + (dZ * factor))); // does the scaling, then translates back
        }
        return ret.ToArray(); // converts to array and returns
    }
    public static Vector3[] QuadScale(this Vector3[] v, float factor) // simpler method
    {
        return v.Scale(factor, v.QuadMidpoint()); // scale the quad by the factor and automatically finds the midpoint
    }
    public static Vector3 Rotate(this Vector3 v, float x, float y, float z, Vector3 centre) // rotate a point around a given origin
    {
        // this uses the standard rotation matrices taught in further mathematics, it was quicker to manually implement than to use a maths library
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
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z, Vector3 centre) // rotates a quad around a given centre
    {
        if (v.Length != 4) // only does the operation if it is a quad
        {
            return v;
        }
        Vector3[] rotated = new Vector3[4]; // creates an array to hold the rotated vectors
        for (int i = 0; i < 4; i++) // iterates through each point
        {
            rotated[i] = v[i].Rotate(x, y, z, centre); // rotates by the given amount
        }
        return rotated; // return the rotated points
    }
    public static Vector3[] QuadRotate(this Vector3[] v, float x, float y, float z) // do this all in degrees because its easier
    {
        return v.QuadRotate(x, y, z, v.QuadMidpoint()); // does what the function name states, but uses the calculated point
    }
    public static float NextFloat(this System.Random rng, float min, float max) // calculates a random float with min max
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble()); // uses lerp to go between the range
    }
    public static Vector3 QuadMidpoint(this Vector3[] v) // calculates the midpoint of a given vector set
    {
        if (v.Length != 4) // only used for quads
        {
            return new Vector3(0, 0, 0);
        }
        // really simple implementation finding the mean of all the points, sort of like finding the centre of mass with the vertices being masses on a plane
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
    public static Vector3 RotateTo(this Vector3 initialVector, Vector3 desiredVector) // calculate the rotation between two vectors
    {
        // has two seperate methods
        if (useQuaternion)
        {
            return initialVector.RotateToQuaternion(desiredVector);
        }
        else
        {
            return initialVector.RotateToMatrix(desiredVector);
        }
    }
    static Vector3 RotateToQuaternion(this Vector3 initialVector, Vector3 desiredVector) // quaternion rotation method
    {
        Vector3 u = initialVector.normalized; // normalise both u and v
        Vector3 v = desiredVector.normalized;
        Quaternion q; // create the quaternion
        Vector3 a = Vector3.Cross(u, v); // take the cross product (normal) of the two vectors
        q.x = a.x; // set all values
        q.y = a.y;
        q.z = a.z;
        q.w = Mathf.Sqrt(u.sqrMagnitude * v.sqrMagnitude) + Vector3.Dot(u, v); // set w component
        return q.eulerAngles; // return the euler angles for the quaternion
    }
    static int GetSign(this float k) // literally gets the sign of the number
    {
        if (k == 0) // if it's 0, it's positive (really doesn't matter)
        {
            return 1;
        }
        return Convert.ToInt16((Mathf.Abs(k) / k)); // divide the absolute by the actual and round
    }
    static Vector3 RotateToMatrix(this Vector3 initialVector, Vector3 desiredVector)
    {
        // my implementation based on solving matrices as simultaneous equations (check documentation for full explanation)
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
        // the code below just forces everything into the [0, 2pi] range
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
        // check for solution overlaps, with some tolerance as floating point math can be slightly inaccurate
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
