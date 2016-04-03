using UnityEngine;
using System.Collections;

public class GeneticAlgorithm {

    public float MUTATION_RATE;
    public float MAX_PERBETUATION;

    public int generation;
    public int currentGenome;
    public Genome[] population;
    public int totalPopulation;
    public int id;

    public GeneticAlgorithm() {
        this.currentGenome = -1;
        this.totalPopulation = 0;
        generation = 1;
        id = 0;
        MUTATION_RATE = 0.15f;
        MAX_PERBETUATION = 0.3f;
    }


    public Genome CreateNewGenome(int totalWeights) {
        Genome genome = new Genome();
        genome.SetId(id);
        genome.SetFitness(0.0f);
        float[] genes = new float[totalWeights];

        for (int i = 0; i < totalWeights; i++) {
            genes[i] = RandomClamped();
        }
        genome.SetGenes(genes);
        id++;

        return genome;
    }

    public Genome GetNextGenome() {
        currentGenome++;
        if (currentGenome > population.Length)
            return null;
        return population[currentGenome];
    }

    public Genome GetBestGenome() {
        int bestGenome = -1;
        float fitness = 0;
        for (int i = 0; i < population.Length; i++) {
            if (population[i].GetFitness() > fitness) {
                fitness = population[i].GetFitness();
                bestGenome = i;
            }
        }
        return population[bestGenome];
    }

    public Genome[] GetBestCases(int totalGenomes) {
        Genome[] output = new Genome[totalGenomes];
        int genomeCount = 0;
        int runCount = 0;

        while (genomeCount < totalGenomes) {
            if (runCount > 10)
                return null;
            runCount++;

            float bestFitness = 0;
            int bestIndex = -1;
            for (int i = 0; i < this.totalPopulation; i++) {
                if (population[i].GetFitness() > bestFitness) {
                    bool isUsed = false;

                    for (int j = 0; j < output.Length; j++) {
                        if (output[j] != null && output[j].GetId() == population[i].id) {
                            isUsed = true;
                        }
                    }

                    if (isUsed == false) {
                        bestIndex = i;
                        bestFitness = population[bestIndex].GetFitness();
                    }
                }
            }

            if (bestIndex != -1) {
                output[genomeCount] = population[bestIndex];
                genomeCount++;
            }
        }
        return output;
    }

    public Genome GetWorstGenome() {
        int worstGenome = -1;
        float fitness = int.MaxValue;
        for (int i = 0; i < population.Length; i++) {
            if (population[i].GetFitness() < fitness) {
                fitness = population[i].GetFitness();
                worstGenome = i;
            }
        }
        return population[worstGenome];
    }

    public Genome GetGenome(int index) {
        if (index >= totalPopulation)
            return null;
        return population[index];
    }

    public int GetCurrentGenomeIndex() {
        return currentGenome;
    }

    public int GetCurrentGeneration() {
        return generation;
    }

    public int GetTotalPopulation() {
        return totalPopulation;
    }

    public void Breed() {
        Genome[] bestGenomes = GetBestCases(4);
        Genome[] children = new Genome[totalPopulation];
        Genome best = new Genome();
        best.SetFitness(0.0f);
        best.SetId(bestGenomes[0].GetId());
        best.SetGenes(bestGenomes[0].GetGenes());
        //Mutate(best);
        children[0] = best;

        Genome [] babies = CrossOver(bestGenomes[0], bestGenomes[1]);
        Mutate(babies[0]);
        Mutate(babies[1]);
        children[1] = babies[0];
        children[2] = babies[1];
        babies = CrossOver(bestGenomes[0], bestGenomes[2]);
        Mutate(babies[0]);
        Mutate(babies[1]);
        children[3] = babies[0];
        children[4] = babies[1];
        babies = CrossOver(bestGenomes[0], bestGenomes[3]);
        Mutate(babies[0]);
        Mutate(babies[1]);
        children[5] = babies[0];
        children[6] = babies[1];

        babies = CrossOver(bestGenomes[1], bestGenomes[2]);
        Mutate(babies[0]);
        Mutate(babies[1]);
        children[7] = babies[0];
        children[8] = babies[1];
        babies = CrossOver(bestGenomes[1], bestGenomes[3]);
        Mutate(babies[0]);
        Mutate(babies[1]);
        children[9] = babies[0];
        children[10] = babies[1];

        int remainingChildren = (totalPopulation - 11);
        for (int i = 11; i < 11 + remainingChildren; i++) {
            children[i] = CreateNewGenome(bestGenomes[0].GetGenes().Length);
        }

        ClearPopulation();
        population = children;

        currentGenome = 0;
        generation++;
    }


    public Genome[] CrossOver(Genome g1, Genome g2) {
        Genome[] babies = new Genome[2];
        int totalWeights = g1.GetGenes().Length;
        int crossover = (int)Random.Range(0, totalWeights - 1);

        //
        babies[0] = new Genome();
        babies[0].SetId(id);
        float[] genes = new float[totalWeights];
        for (int i = 0; i < totalWeights; i++) {
            genes[i] = 0.0f;
        }
        babies[0].SetGenes(genes);
        id++;

        //
        babies[1] = new Genome();
        babies[1].SetId(id);
        babies[1].SetGenes(genes);
        id++;

        // CrossOver
        for (int i = 0; i < crossover; i++) {
            babies[0].SetGen(i, g1.GetGen(i));
            babies[1].SetGen(i, g2.GetGen(i));
        }

        for (int i = crossover; i < totalWeights; i++) {
            babies[0].SetGen(i, g2.GetGen(i));
            babies[1].SetGen(i, g1.GetGen(i));
        }

        return babies;
    }

    public Genome RouletteWheel() {
        return null;
    }

    public void Mutate(Genome genome) {
        for (int i = 0; i < genome.GetGenes().Length; i++) {
            if (RandomClamped() < MUTATION_RATE) {
                genome.SetGen(i, genome.GetGen(i) + (RandomClamped() * MAX_PERBETUATION));
            }
        }
    }

    public void GeneratePopulation(int totalPopulation, int totalWeights) {
        generation = 1;
        ClearPopulation();
        currentGenome = -1;
        this.totalPopulation = totalPopulation;

        //resize
        if (population == null || population.Length < totalPopulation) {
            Genome[] auxGenome = new Genome[totalPopulation];
            int i = 0;
            if (population != null)
                for (i = 0; i < population.Length; i++)
                    auxGenome[i] = population[i];
            if (population != null)
                i = population.Length;
            for (; i < totalPopulation; i++)
                auxGenome[i] = new Genome();
            population = auxGenome;
        }

        for (int i = 0; i < population.Length; i++) {
            Genome genome = new Genome();
            genome.SetId(id);
            genome.SetFitness(0.0f);
            float[] genes = new float[totalWeights];
            //resize
            for (int k = 0; k < totalWeights; k++) {
                genes[k] = RandomClamped();
            }
            genome.SetGenes(genes);

            id++;
            population[i] = genome;
        }
    }

    public void ClearPopulation() {
        if (population != null)
            for (int i = 0; i < population.Length; i++) {
                if (population[i] != null) {
                    population[i] = null;
                }
            }
    }

    public void SetGenomeFitness(float fitness, int index) {
        if (index >= population.Length)
            return;
        else
            population[index].SetFitness(fitness);
    }

    public float RandomFloat() {
        float rand = (float)Random.Range(0.0f, int.MaxValue);
        return rand / int.MaxValue + 1.0f;
    }

    public float RandomClamped() {
        return RandomFloat() - RandomFloat();
    }
}
