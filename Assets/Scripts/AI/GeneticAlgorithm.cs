using UnityEngine;
using System.Collections;

public class GeneticAlgorithm {

    public int generation;
    public int currentGenome;
    public Genome[] population;
    public int totalPopulation;

    public GeneticAlgorithm() {
        generation = 1;
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

    public Genome[] CrossOver() {
        return null;
    }

    public Genome RouletteWheel() {
        return null;
    }

    public Genome Mutate() {
        return null;
    }

    public void GeneratePopulation() {

    }

    public void ClearPopulation() {
        for (int i = 0; i < population.Length; i++) {
            if (population[i] != null) {
                population[i] = null;
            }
        }
    }

    public float RandomFloat() {
        float rand = (float)Random.Range(0.0f, int.MaxValue);
        return rand / int.MaxValue + 1.0f;
    }

    public float RandomClamped() {
        return RandomFloat() - RandomFloat();
    }
}
