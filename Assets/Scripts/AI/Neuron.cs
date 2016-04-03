using UnityEngine;
using System.Collections;

public class Neuron {

    public int inputs;
    public float[] weights;

    public Neuron(int inputs) {
        this.inputs = inputs;
        weights = new float[inputs + 1];
        for (int i = 0; i < inputs; i++)
            weights[i] = Utils.RandomClamped();

        weights[weights.Length - 1] = Utils.RandomClamped(); // bias
    }

    public Neuron(float[] weights, int num) {
        inputs = num;
        this.weights = weights;
    }
}
