public class TreeData
{
    public TreeNode BaseNode;
    System.Random prng;
    public TreeData(float bChance, int baseLength, System.Random prng)
    {
        this.prng = prng;
        BaseNode = new TreeNode();
        GenerateBranch(BaseNode, baseLength, bChance, false);
    }
    void GenerateBranch(TreeNode parent, int length, float branchChance, bool isBranch)
    {
        if (length < 1)
        {
            return;
        }
        TreeNode currentParent = parent;
        bool justBranched = false;
        bool shouldBranch = isBranch;
        for (int i = 0; i < length; i++)
        {
            TreeNode newNode = new TreeNode();
            if (shouldBranch)
            {
                currentParent.Branch = newNode;
            }
            else
            {
                currentParent.Up = newNode;
            }
            shouldBranch = false;
            currentParent = newNode;
            if (!justBranched && i != 0 && i != length - 1 && prng.NextFloat(0, 1) <= branchChance) // dont generate a branch on the first segment because thats stupid, and the last one?
            {
                justBranched = true;
                GenerateBranch(newNode, length - (i + 2), branchChance * 0.66f, true);
            }
            else
            {
                justBranched = false;
            }
        }
    }
}
public class TreeNode
{
    public TreeNode Up;
    public TreeNode Branch;
    public TreeNode()
    {
        Up = null;
        Branch = null;
    }
    public int ChildrenCount()
    {
        int c = 0;
        if (Up != null)
        {
            c++;
        }
        if (Branch != null)
        {
            c++;
        }
        return c;
    }
}
public class BranchExtrusion
{
    public BuilderPolygon Polygon;
    public TreeNode Node;
    public float Scale;
    public BranchExtrusion(BuilderPolygon p, TreeNode t, float s)
    {
        Polygon = p;
        Node = t;
        Scale = s;
    }
}