using UnityEngine;
using System.Collections;

public class NeuralLayer {

    public int totalNeurons;
    private int currentNeurons = 0;
    public int totalInputs;
    public Neuron[] neurons;

    public NeuralLayer(int neurons, int inputs) {
        totalInputs = inputs;
        totalNeurons = neurons;
        this.neurons = new Neuron[neurons];

        if (currentNeurons < neurons) {
            for (int i = 0; i < neurons; i++) {
                this.neurons[i] = new Neuron(inputs);
                currentNeurons++;
            }
        }
    }

    public NeuralLayer(Neuron[] input) {
        neurons = input;
        totalNeurons = input.Length;
    }

    public void Populate(int neurons, int inputs) {
        totalInputs = inputs;
        totalNeurons = neurons;

        if (this.neurons.Length < neurons) {
            for (int i = 0; i < neurons; i++) {
                this.neurons[i] = new Neuron(inputs);
            }
        }
    }

    public float[] Evaluate(float[] input) {
        int inputIndex = 0;
        float[] output = new float[totalNeurons];

        for (int i = 0; i < totalNeurons; i++) {
            float activation = 0.0f;
            for (int j = 0; j < neurons[i].inputs - 1; j++) {
                activation += input[inputIndex] * neurons[i].weights[j];
                inputIndex++;
            }

            // bias
            activation += neurons[i].weights[neurons[i].inputs] * (-1.0f);//BIAS == -1.0f

            output[i] = Sigmoid(activation, 1.0f);
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

    public void SetNeurons(Neuron[] neurons, int numNeurons, int numInputs) {
        totalInputs = numInputs;
        totalNeurons = numNeurons;
        this.neurons = neurons;
    }

    public Neuron[] GetNeurons() {
        return neurons;
    }

    public void SetWeights(float[] weights, int numNeurons, int numInputs) {
        int index = 0;
        totalInputs = numInputs;
        totalNeurons = numNeurons;

        if (neurons.Length < numNeurons) {
            for (int i = 0; i < numNeurons - neurons.Length; i++) {
                neurons[i] = new Neuron(numInputs);
            }
        }

        for (int i = 0; i < numNeurons; i++) {
            if (neurons[i].weights.Length < numInputs) {
                for (int k = 0; k < numInputs - neurons[i].weights.Length; k++) {
                    neurons[i].weights[k] = 0.0f;
                }
            }
            for (int j = 0; j < numInputs; j++) {
                neurons[i].weights[j] = weights[index];
                index++;
            }
        }
    }

    public float[] GetWeights() {
        float[] weights = new float[totalNeurons]; // TODO CORREGIR

        for (int i = 0; i < totalNeurons; i++) {
            for (int j = 0; j < neurons[i].weights.Length; j++) {
                weights[totalNeurons * i + j] = neurons[i].weights[j];
            }
        }
        return weights;
    }
}
