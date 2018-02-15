public class TreeData
{
    public TreeNode BaseNode; // stores the base of the tree
    System.Random prng; // random generator for the tree
    public TreeData(float bChance, int baseLength, System.Random prng)
    {
        this.prng = prng; // uses this because i'm not creative with variable names
        BaseNode = new TreeNode(); // creates new tree node
        GenerateBranch(BaseNode, baseLength, bChance, false); // generates a branch with the given statistics
    }
    void GenerateBranch(TreeNode parent, int length, float branchChance, bool isBranch) // recursive node generator
    {
        if (length < 1) // if the length is zero (hopefully i didn't actually mess up and allow for negative branch lengths)
        {
            return; // don't continue generation (exit case)
        }
        TreeNode currentParent = parent; // create parent
        bool justBranched = false; // mark just branched as false
        bool shouldBranch = isBranch; // keep branching if it's a branch
        for (int i = 0; i < length; i++) // iterates through tree length
        {
            TreeNode newNode = new TreeNode(); // create a new node
            if (shouldBranch) // if it's a branch
            {
                currentParent.Branch = newNode; // mark the parent's branch with the new node
            }
            else
            {
                currentParent.Up = newNode; // go up
            }
            shouldBranch = false; // stop branching
            currentParent = newNode; // mark the current parent as the newly generated node
            if (!justBranched && i != 0 && i != length - 1 && prng.NextFloat(0, 1) <= branchChance) // dont generate a branch on the first or last segment
            {
                justBranched = true; // mark as just branched
                GenerateBranch(newNode, length - (i + 2), branchChance * 0.66f, true); // create branch
            }
            else
            {
                justBranched = false; // reset flag
            }
        }
    }
}
public class TreeNode // node to hold branch info
{
    public TreeNode Up; // node that is directly above
    public TreeNode Branch; // node that is adjacent
    public TreeNode() // constructor marks both as null
    {
        Up = null;
        Branch = null;
    }
    public int ChildrenCount() // number of children
    {
        int c = 0; // initial count
        foreach (TreeNode t in new TreeNode[] { Up, Branch }) // more elegant solution in case i wanted to add more branches
        {
            if (t != null) // if it exists
            {
                c++; // increment child count by 1
            }
        }
        return c; // return the count
    }
}
public class BranchExtrusion // branch extrusion data
{
    public BuilderPolygon Polygon; // holds the polygon
    public TreeNode Node; // holds the corresponding node
    public float Scale; // holds the scale
    public BranchExtrusion(BuilderPolygon p, TreeNode t, float s) // simple constructor
    {
        Polygon = p;
        Node = t;
        Scale = s;
    }
}