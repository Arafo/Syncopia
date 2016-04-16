using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralLayer {

    public int totalNeurons;
    public int totalInputs;
    public List<Neuron> neurons = new List<Neuron>();

    public NeuralLayer(int neurons, int inputs) {
        totalInputs = inputs;
        totalNeurons = neurons;

        if (this.neurons.Count < neurons) {
            for (int i = 0; i < neurons; i++) {
                this.neurons.Add(new Neuron(inputs));
            }
        }
    }

    public NeuralLayer(List<Neuron> input) {
        neurons = input;
        totalNeurons = input.Count;
    }

    public NeuralLayer() {
    }

    public void Populate(int neurons, int inputs) {
        totalInputs = inputs;
        totalNeurons = neurons;

        if (this.neurons.Count < neurons) {
            for (int i = 0; i < neurons; i++) {
                this.neurons.Add(new Neuron(inputs));
            }
        }
    }

    public List<float> Evaluate(List<float> input) {
        int inputIndex = 0;
        List<float> output = new List<float>();

        for (int i = 0; i < totalNeurons; i++) {
            float activation = 0.0f;
            for (int j = 0; j < neurons[i].inputs - 1; j++) {
                activation += input[inputIndex] * neurons[i].weights[j];
                inputIndex++;
            }

            // bias
            activation += neurons[i].weights[neurons[i].inputs] * (-1.0f);//BIAS == -1.0f
            output.Add(Sigmoid(activation, 1.0f));
            inputIndex = 0;
        }
        return output;
    }

    public float Sigmoid(float a, float p) {
        float ap = (-a) / p;
        return (1 / (1 + Mathf.Exp(ap)));
    }

    //public void SetLayer(Neuron[] input) {
      //  neurons = input;
        //totalNeurons = input.Length;
    //}

    public void SetNeurons(List<Neuron> neurons, int numNeurons, int numInputs) {
        totalInputs = numInputs;
        totalNeurons = numNeurons;
        this.neurons = neurons;
    }

    public List<Neuron> GetNeurons() {
        return neurons;
    }

    public void SetWeights(List<float> weights, int numNeurons, int numInputs) {
        int index = 0;
        totalInputs = numInputs;
        totalNeurons = numNeurons;

        if (neurons.Count < numNeurons) {
            for (int i = 0; i < numNeurons - neurons.Count; i++) {
                neurons.Add(new Neuron(numInputs));
            }
        }

        for (int i = 0; i < numNeurons; i++) {
            if (neurons[i].weights.Count < numInputs) {
                for (int k = 0; k < numInputs - neurons[i].weights.Count; k++) {
                    neurons[i].weights.Add(0.0f);
                }
            }
            for (int j = 0; j < numInputs; j++) {
                neurons[i].weights[j] = weights[index];
                index++;
            }
        }
    }

    public List<float> GetWeights() {
        List<float> weights = new List<float>();

        for (int i = 0; i < totalNeurons; i++) {
            for (int j = 0; j < neurons[i].weights.Count; j++) {
                weights[totalNeurons * i + j] = neurons[i].weights[j];
            }
        }
        return weights;
    }
}
