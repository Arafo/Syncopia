using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neuron {

    public int inputs;
    public List<float> weights = new List<float>();

    public Neuron(int inputs) {
        this.inputs = inputs;
        for (int i = 0; i < inputs; i++)
            weights.Add(Utils.RandomClamped());

        weights.Add(Utils.RandomClamped()); // bias
    }

    public Neuron(List<float> weights, int num) {
        inputs = num;
        this.weights = weights;
    }
}
