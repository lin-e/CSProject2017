public class Genes
{
    public float[] DNA;
    public Genes(float[] dna) // overloaded constructor to allow for optional parameters
    {
        DNA = dna; // set the dna to the parameter
    }
    public Genes() // the case when the constructor gets no parameters
    {
        DNA = new float[4]; // declare the new array
        DNA[0] = AgentManager.Generator.NextFloat(AgentManager.AttractionMin, AgentManager.AttractionMax); // attraction multiplier for the player
        DNA[1] = AgentManager.Generator.NextFloat(AgentManager.ViewMin, AgentManager.ViewMax); // the view range for the target
        DNA[2] = AgentManager.Generator.NextFloat(AgentManager.AttractionMin, AgentManager.AttractionMax); // attraction multiplier for targets
        DNA[3] = AgentManager.Generator.NextFloat(AgentManager.ViewMin, AgentManager.ViewMax); // view range for targets
    }
    public Genes Mix(Genes other)
    {
        float[] newDNA = new float[4]; // create a new array for the DNA
        for (int i = 0; i < 4; i++) // iterate 4 times
        {
            if (AgentManager.Generator.NextFloat(0, 1) < AgentManager.MutationChance) // if this should be a mutation
            {
                newDNA[i] = new Genes().DNA[i]; // create a new gene and take the DNA value
            }
            else // otherwise if we're crossing genes
            {
                newDNA[i] = (AgentManager.Generator.NextFloat(0, 1) < 0.5) ? DNA[i] : other.DNA[i]; // if the roll is less than 0.5, use this gene, otherwise use the other parent
            }
        }
        return new Genes(newDNA);
    }
}
