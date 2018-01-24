using System.Collections.Generic;
using System.Linq;
using System;

public class MazeGenerator
{
    static int border = 2;

    public Node[,] Weight;
    public float[,] Map;
    public List<int[]> MapPoints;

    int nodeWidth; // maximum number of nodes
    int nodeHeight; // same as above
    int meshScale = 5; // scale of the mesh (we are exporting to a mesh builder after the graph is converted into a heightmap)
    List<int[]> dead = new List<int[]>(); // mark a node as dead if it can no longer form any arcs (either due to its location, or all of its adjacent nodes are used)
    List<int[]> active = new List<int[]>(); // the currently active nodes (which we want to check)
    float high; // the export values
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
                MapPoints.Add(new int[] { x, y }); // add point to map
            }
        }
        if (transitionEdge) // if we should have a smooth transition
        {
            for (int e = 0; e < border; e++) // we want to iterate through each part of the point in layers (from the inside out)
            {
                for (int y = (yPoint * meshScale) - (e + 1); y < (yPoint * meshScale) + meshScale + (e + 1); y++) // iterate through each y point under the above constraints
                {
                    for (int x = (xPoint * meshScale) - (e + 1); x < (xPoint * meshScale) + meshScale + (e + 1); x++) // same for x
                    {
                        if (Map[x, y] != low) // if the point has already been overwritten, then ignore it
                        {
                            continue;
                        }
                        else
                        {
                            Map[x, y] = value / (float)Math.Pow(2, e + 1); // otherwise map the point's height with an exponential curve
                        }
                    }
                }
            }
        }
    }
    List<int[]> getSurrouding(int[] currentPoint) // returns a list of surrounding nodes
    {
        List<int[]> possiblePoints = new List<int[]>(); // list of possible nodes
        Direction[] possibleDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right }; // array of directions
        List<Direction> skip = new List<Direction>(); // directions to ignore
        // the following checks are to determine if the node exists on the edge of the tree (we still have the tree topologically represented as a rectangle with each node connected to its adjacent)
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
        foreach (Direction singleDirection in possibleDirections) // iterate through each direction
        {
            if (skip.Contains(singleDirection)) // if we have already specified to skip the direction
            {
                continue; // ignore and go to next direction
            }
            int[] newPoint = currentPoint.Move(singleDirection); // move to the point
            if (active.Any(newPoint.SequenceEqual)) // if the point is already active, ignore it (as it will be part of the graph)
            {
                continue;
            }
            possiblePoints.Add(newPoint); // add new points
        }
        if (possiblePoints.Count == 0) // if there are no new points
        {
            dead.Add(currentPoint); // mark the current point as dead (and skip checking it in the next iteration)
        }
        return possiblePoints; // return all possible points
    }
    int[] getExportPoint(int[] c)
    {
        return new int[] { (2 * c[0]) + border, (2 * c[1]) + border }; // remaps the point to the export point
    }
    public class Node
    {
        public int X;
        public int Y;
        public Node(int x, int y) // really simple constructor
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
    public class Arc // automated arc creation
    {
        public int[] Start; // coordinates for start node
        public int[] End; // same as above for end
        public int Distance; // the square distance between the start and end nodes
        public Direction Direction; // direction (fed into terrain generation)
        public Arc() { } // empty constructor used to generate placeholder variable
        public Arc(MazeGenerator parentMaze, int[] startNode, int[] endNode) // constructor used by algorithm 
        {
            Start = startNode; // take the start param
            End = endNode; // take the end param
            Distance = parentMaze.Weight[startNode[0], startNode[1]].Distance(parentMaze.Weight[endNode[0], endNode[1]]); // calculate the topographical (not topological) square distance between the two points (no square root as it brings in decimals and a square root which is resource intensive when done many times per graph)
            if (startNode[0] == endNode[0]) // if the start node has the same x coordinate
            {
                if (startNode[1] > endNode[1]) // and the start node's y coordinate is higher than the end's
                {
                    Direction = Direction.Up; // then the arc is travelling up
                }
                else
                {
                    Direction = Direction.Down; // same as above
                }
            }
            else // otherwise if x is different
            {
                if (startNode[0] > endNode[0]) // do the same checks for the x coordinate
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