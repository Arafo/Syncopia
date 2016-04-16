using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralNetwork {

    private int inputAmount;
    private int outputAmount;

    List<float> inputs = new List<float>();
    List<float> outputs = new List<float>();

    NeuralLayer inputLayer;
    NeuralLayer outputLayer;
    List<NeuralLayer> hiddenLayers = new List<NeuralLayer>();

    public NeuralNetwork(int inputs, int outputs, int hiddenLayers, int hiddenNeurons) {
        inputAmount = inputs;
        outputAmount = outputs;
        outputLayer = new NeuralLayer(outputs, hiddenNeurons);

        for (int i = 0; i < hiddenLayers; i++) {
            NeuralLayer layer = new NeuralLayer(hiddenNeurons, inputs);
            this.hiddenLayers.Add(layer);
        }
        outputLayer = new NeuralLayer(outputs, hiddenNeurons);
    }

    public void Update() {
        outputs.Clear();

        for (int i = 0; i < hiddenLayers.Count; i++) {
            if (i > 0) {
                inputs = outputs;
            }
            if (hiddenLayers[i] != null)
                outputs = hiddenLayers[i].Evaluate(inputs);
        }
        inputs = outputs;
        if (outputLayer == null)
            outputs = outputLayer.Evaluate(inputs);
        //outputs = outputLayer.Evaluate(inputs);

    }

    public void Release() {
        if (inputLayer != null) {
            inputLayer = new NeuralLayer();
        }

        if (outputLayer != null) {
            outputLayer = new NeuralLayer();
        }

        for (int i = 0; i < hiddenLayers.Count; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        }
        hiddenLayers.Clear();
    }

    public Genome ToGenome() {
        Genome genome = new Genome();

        for (int i = 0; i < hiddenLayers.Count; i++) {
            List<float> weights = hiddenLayers[i].GetWeights();
            for (int j = 0; j < weights.Count; j++) {
                genome.GetGenes().Add(weights[j]);
            }
        }

        List<float> outweights = outputLayer.GetWeights();
        for (int i = 0; i < outweights.Count; i++) {
            genome.GetGenes().Add(outweights[i]);
        }

        return genome;
    }

    public void FromGenome(Genome genome, int inputs, int outputs, int hiddenNeurons) {
        Release();
        outputAmount = outputs;
        inputAmount = inputs;
        //int weightsForHidden = inputs * hiddenNeurons;
        List<Neuron> neurons = new List<Neuron>();

        for (int i = 0; i < hiddenNeurons; i++) {
            List<float> weights = new List<float>();
            for (int j = 0; j < inputs + 1; j++) {
                weights.Add(0.0f);
                weights[j] = genome.GetGen(i * hiddenNeurons + j);
            }
            neurons.Add(new Neuron(weights, inputs));
        }

        NeuralLayer hidden = new NeuralLayer(neurons);
        //Debug.Log ("fromgenome, hiddenlayer neruons#: " + neurons.Count);
        //Debug.Log ("fromgenome, hiddenlayer numInput: " + neurons[0].inputs);
        this.hiddenLayers.Add(hidden);

        //int weightsForOutput = hiddenNeurons * outputs;
        List<Neuron> outneurons = new List<Neuron>();

        for (int i = 0; i < outputs; i++) {
            List<float> weights = new List<float>();
            for (int j = 0; j < hiddenNeurons + 1; j++) {
                weights.Add(0.0f);
                weights[j] = genome.GetGen(i * hiddenNeurons + j);
            }
            outneurons.Add(new Neuron(weights, hiddenNeurons));
        }
        this.outputLayer = new NeuralLayer(outneurons);
        //Debug.Log ("fromgenome, outputlayer neruons#: " + outneurons.Count);
        //Debug.Log ("fromgenome, outputlayer numInput: " + outneurons[0].inputs);
    }

    public void SetInput(List<float> input) {
        inputs = input;
    }

    public List<float> GetInput() {
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
        return hiddenLayers.Count;
    }
}
