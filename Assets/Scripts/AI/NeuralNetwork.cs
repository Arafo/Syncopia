using UnityEngine;
using System.Collections;

public class NeuralNetwork {

    private int inputAmount;
    private int outputAmount;

    float[] inputs;
    float[] outputs;

    NeuralLayer inputLayer;
    NeuralLayer outputLayer;
    NeuralLayer[] hiddenLayers;

    public NeuralNetwork(int inputs, int outputs, int hidden, int hiddenNeurons) {
        inputAmount = inputs;
        outputAmount = outputs;
        hiddenLayers = new NeuralLayer[hidden];
        this.outputs = new float[outputs];

        for (int i = 0; i < hidden; i++) {
            NeuralLayer layer = new NeuralLayer(hiddenNeurons, inputs);
            hiddenLayers[i] = layer;
        }
        outputLayer = new NeuralLayer(outputs, hiddenNeurons);
    }

    public Genome ToGenome() {
        Genome genome = new Genome(hiddenLayers.Length);

        for (int i = 0; i < hiddenLayers.Length; i++) {
            float[] weights = hiddenLayers[i].GetWeights();
            for (int j = 0; j < weights.Length; j++) {
                genome.SetGen(j, weights[j]);
            }
        }

        float[] outweights = outputLayer.GetWeights();
        for (int i = 0; i < outweights.Length; i++) {
            genome.SetGen(i, outweights[i]);
        }

        return genome;
    }

    public void FromGenome(Genome genome, int inputs, int outputs, int hiddenNeurons) {
        Release();
        outputAmount = outputs;
        inputAmount = inputs;
        //int weightsForHidden = inputs * hiddenNeurons;
        Neuron[] neurons = new Neuron[hiddenNeurons];

        for (int i = 0; i < hiddenNeurons; i++) {
            float[] weights = new float[inputs + 1];
            for (int j = 0; j < inputs + 1; j++) {
                weights[j] = genome.GetGen(i * hiddenNeurons + j);
            }
            neurons[i] = new Neuron(weights, inputs);
        }

        NeuralLayer hidden = new NeuralLayer(neurons);
        Debug.Log ("fromgenome, hiddenlayer neruons#: " + neurons.Length);
        Debug.Log ("fromgenome, hiddenlayer numInput: " + neurons[0].inputs);
        this.hiddenLayers[0] = hidden;
        //int weightsForOutput = hiddenNeurons * outputs;
        Neuron[] outneurons = new Neuron[outputs];

        for (int i = 0; i < outputs; i++) {
            float[] weights = new float[hiddenNeurons + 1];
            for (int j = 0; j < hiddenNeurons + 1; j++) {
                weights[j] = genome.GetGen(i * hiddenNeurons + j);
            }
            outneurons[i] = new Neuron(weights, hiddenNeurons);
        }
        this.outputLayer = new NeuralLayer(outneurons);
        Debug.Log ("fromgenome, outputlayer neruons#: " + outneurons.Length);
        Debug.Log ("fromgenome, outputlayer numInput: " + outneurons[0].inputs);
    }

    public void Update() {

        for (int i = 0; i < hiddenLayers.Length; i++) {
            if (i > 0) {
                inputs = outputs;
            }
            if (hiddenLayers[i] != null)
                outputs = hiddenLayers[i].Evaluate(inputs);
        }
        inputs = outputs;
        if (outputLayer == null)
            outputs = outputLayer.Evaluate(inputs);
        outputs = outputLayer.Evaluate(inputs);

    }

    public void Release() {
        if (inputLayer != null) {
            inputLayer = null;
        }

        if (outputLayer != null) {
            outputLayer = null;
        }

        for (int i = 0; i < hiddenLayers.Length; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        }
    }

    public void SetInput(float[] input) {
        inputs = input;
    }

    public float[] GetInput() {
        return inputs;
    }

    public int GetTotalInputs() {
        return inputAmount;
    }

    public float GetOutput(int output) {
        if (output >= outputAmount)
            return 0.0f;
        return outputs[output];
    }

    public int GetTotalOutputs() {
        return outputAmount;
    }

    public int GetTotalHiddenLayers() {
        return hiddenLayers.Length;
    }
}
