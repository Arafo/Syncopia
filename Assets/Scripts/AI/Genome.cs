using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Genome {

    public int id;
    private List<float> genes;
    private float fitness;


    public Genome() {
        fitness = 0;
    }

    public Genome(int numGenes) {
        genes = new List<float>();
        for (int i = 0; i < genes.Count; i++)
            genes[i] = 0.0f;
        fitness = 0;
    }

    public int GetId() {
        return id;
    }

    public void SetId(int id) {
        this.id = id;
    }

    public List<float> GetGenes() {
        return genes;
    }

    public void SetGenes(List<float> genes) {
        this.genes = genes;
    }

    public void SetGen(int index, float value) {
        this.genes[index] = value;
    }

    public float GetGen(int index) {
        return genes[index];
    }

    public float GetFitness() {
        return fitness;
    }

    public void SetFitness(float fitness) {
        this.fitness = fitness;
    }
}
