using UnityEngine;
using System.Collections;

public class Genome {

    private float[] genes;
    private float fitness;

    public Genome(int numGenes)
    {
        genes = new float[numGenes];
        for (int i = 0; i < genes.Length; i++)
            genes[i] = 0.0f;
        fitness = 0;
    }

    public float[] GetGenes()
    {
        return genes;
    }

    public void SetGenes(float[] genes)
    {
        this.genes = genes;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public void SetFitness(float fitness)
    {
        this.fitness = fitness;
    }
}
