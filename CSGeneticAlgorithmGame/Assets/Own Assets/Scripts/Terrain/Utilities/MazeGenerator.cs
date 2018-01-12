using System.Collections.Generic;
using System.Linq;
using System;

public class MazeGenerator
{
    static int border = 2;

    public Node[,] Weight;
    public float[,] Map;
    public List<int[]> MapPoints;

    int nodeWidth;
    int nodeHeight;
    int meshScale = 5;
    List<int[]> dead = new List<int[]>();
    List<int[]> active = new List<int[]>();
    float high;
    float low;

    public MazeGenerator(int width, int height, float low, float high, int scale)
    {
        MapPoints = new List<int[]>(); // create a new list of points to be passed into the mesh generator
        meshScale = scale; // set the scale of the map
        this.high = high; // corresponding high value (as we either take a point as 1 or 0)
        this.low = low; // same as above
        nodeWidth = (int)Math.Ceiling(width / 2f); // get the max number of nodes
        nodeHeight = (int)Math.Ceiling(height / 2f); // same as above but in the other direction
        Random generator = new Random(); // create a new generator
        Weight = new Node[nodeWidth, nodeHeight]; // create an empty array of nodes
        for (int y = 0; y < nodeHeight; y++) // iterate through all nodes
        {
            for (int x = 0; x < nodeWidth; x++)
            {
                Weight[x, y] = new Node(generator.Next(1, 1000), generator.Next(1, 1000)); // create random weighted nodes (such that prim's algorithm can be applied)
            }
        }
        Map = new float[(width + (border * 2)) * meshScale, (height + (border * 2)) * meshScale]; // create new float array to hold mapped points)
        for (int y = 0; y < Map.GetLength(1); y++) // iterate through each row
        {
            for (int x = 0; x < Map.GetLength(0); x++) // iterate each column in said row
            {
                Map[x, y] = low; // set it all to the low value
            }
        }
    }
    public void Generate()
    {
        active.Add(new int[] { 0, 0 }); // initialise the top-left node
        while (active.Count() < Weight.Length) // do this while the list of active nodes is less than the total nodes
        {
            List<Arc> arcs = new List<Arc>(); // create a new list of arcs
            foreach (int[] singleNode in active.Except(dead)) // iterate through each active node, but ignoring dead (where there are no more connections)
            {
                foreach (int[] newNode in getSurrouding(singleNode)) // create a list of surrouding arcs
                {
                    arcs.Add(new Arc(this, singleNode, newNode)); // add the new arc
                }
            }
            Arc currentArc = new Arc(); // create an empty arc
            int min = int.MaxValue; // take the minimum value as the max value of an integer (to make sure that all numbers will be lower than it)
            foreach (Arc arc in arcs) // goes through each arc in the possible arcs
            {
                if (arc.Distance < min) // if this is lower than the current minimum
                {
                    min = arc.Distance; // set the minimum to the current distance
                    currentArc = arc; // set the arc to the current arc
                }
            }
            active.Add(currentArc.End); // add the end node to the list of active nodes (such that it will be used later on)
            int[] pathPos = getExportPoint(currentArc.Start); // get the position it will take on the map
            List<int[]> toSet = new List<int[]>(); // create a list of co-ordinates to set
            toSet.Add(getExportPoint(currentArc.Start)); // map start point
            toSet.Add(pathPos.Move(currentArc.Direction)); // take the position in the arc direction
            toSet.Add(getExportPoint(currentArc.End)); // map end point
            foreach (int[] point in toSet) // remap each co-ordinates in the list
            {
                setMap(point, high); // set it with a high value (which is then configured)
            }
        }
    }
    void setMap(int[] point, float value, bool transitionEdge = true)
    {
        int xPoint = point[0]; // take the point's x value
        int yPoint = point[1]; // take the y value
        for (int y = (yPoint * meshScale); y < (yPoint * meshScale) + meshScale; y++) // iterate through each mesh node (remember that one node in the maze might correspond to 16 actual mesh points)
        {
            for (int x = (xPoint * meshScale); x < (xPoint * meshScale) + meshScale; x++)
            {
                Map[x, y] = value; // sets the value 
                MapPoints.Add(new int[] { x, y }); 
            }
        }
        if (transitionEdge)
        {
            for (int e = 0; e < border; e++)
            {
                for (int y = (yPoint * meshScale) - (e + 1); y < (yPoint * meshScale) + meshScale + (e + 1); y++)
                {
                    for (int x = (xPoint * meshScale) - (e + 1); x < (xPoint * meshScale) + meshScale + (e + 1); x++)
                    {
                        if (Map[x, y] != low)
                        {
                            continue;
                        }
                        else
                        {
                            Map[x, y] = value / (float)Math.Pow(2, e + 1);
                        }
                    }
                }
            }
        }
    }
    List<int[]> getSurrouding(int[] currentPoint)
    {
        List<int[]> possiblePoints = new List<int[]>();
        Direction[] possibleDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        List<Direction> skip = new List<Direction>();
        if (currentPoint[0] == 0)
        {
            skip.Add(Direction.Left);
        }
        if (currentPoint[0] == nodeWidth - 1)
        {
            skip.Add(Direction.Right);
        }
        if (currentPoint[1] == 0)
        {
            skip.Add(Direction.Up);
        }
        if (currentPoint[1] == nodeHeight - 1)
        {
            skip.Add(Direction.Down);
        }
        foreach (Direction singleDirection in possibleDirections)
        {
            if (skip.Contains(singleDirection))
            {
                continue;
            }
            int[] newPoint = currentPoint.Move(singleDirection);
            if (active.Any(newPoint.SequenceEqual))
            {
                continue;
            }
            possiblePoints.Add(newPoint);
        }
        if (possiblePoints.Count == 0)
        {
            dead.Add(currentPoint);
        }
        return possiblePoints;
    }
    int[] getExportPoint(int[] c)
    {
        return new int[] { (2 * c[0]) + border, (2 * c[1]) + border };
    }
    public class Node
    {
        public int X;
        public int Y;
        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int Distance(Node other) // calculate distance relative to other nodes 
        {
            int deltaXSquared = (int)Math.Pow(X - other.X, 2);
            int deltaYSquared = (int)Math.Pow(Y - other.Y, 2);
            return deltaXSquared + deltaYSquared; // notice i'm not returning a square root, as we can expect the following; if x > y (x, y are real and positive), then sqrt(x) > sqrt(y)
        }
    }
    public class Arc
    {
        public int[] Start;
        public int[] End;
        public int Distance;
        public Direction Direction;
        public Arc() { }
        public Arc(MazeGenerator parentMaze, int[] startNode, int[] endNode)
        {
            Start = startNode;
            End = endNode;
            Distance = parentMaze.Weight[startNode[0], startNode[1]].Distance(parentMaze.Weight[endNode[0], endNode[1]]);
            if (startNode[0] == endNode[0])
            {
                if (startNode[1] > endNode[1])
                {
                    Direction = Direction.Up;
                }
                else
                {
                    Direction = Direction.Down;
                }
            }
            else
            {
                if (startNode[0] > endNode[0])
                {
                    Direction = Direction.Left;
                }
                else
                {
                    Direction = Direction.Right;
                }
            }
        }
    }
}
public enum Direction // easier for me to read
{
    Up,
    Down,
    Left,
    Right
}
public static class MazeGeneratorExtensions // movements in 2d, since we're only doing meshes in 2 dimensions
{
    public static int[] Move(this int[] origin, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new int[] { origin[0], origin[1] - 1 };
            case Direction.Down:
                return new int[] { origin[0], origin[1] + 1 };
            case Direction.Left:
                return new int[] { origin[0] - 1, origin[1] };
            case Direction.Right:
                return new int[] { origin[0] + 1, origin[1] };
        }
        return null;
    }
}