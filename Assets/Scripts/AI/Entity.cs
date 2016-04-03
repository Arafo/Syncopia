using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

    Agent testAgent;
    private List<Agent> agents;
    public float currentFitness;
    public float bestFitness;
    private float currentTimer;
    private int checkPointsHit;

    public NeuralNetwork net;

    public GeneticAlgorithm ga;
    public int checkpointsNum;
    public GameObject[] checkpoints;
    public Material normal;

    private Vector3 defaultpos;
    private Quaternion defaultrot;

    CheckpointHit hit;

    // Use this for initialization
    void Start() {

        ga = new GeneticAlgorithm();
        int totalWeights = 5 * 8 + 8 * 2 + 8 + 2;
        ga.GeneratePopulation(15, totalWeights);
        currentFitness = 0.0f;
        bestFitness = 0.0f;

        net = new NeuralNetwork(1, 5, 8, 2);
        Genome genome = ga.GetNextGenome();
        net.FromGenome(genome, 5, 8, 2);

        testAgent = gameObject.GetComponent<Agent>();
        testAgent.Attach(net);

        hit = gameObject.GetComponent<CheckpointHit>();
        checkpointsNum = hit.checkpoints;
        defaultpos = transform.position;
        defaultrot = transform.rotation;
    }

    // Update is called once per frame
    void Update() {
        if (hit != null)
            checkpointsNum = hit.checkpoints;
        if (testAgent.hasFailed) {
            if (ga.GetCurrentGenomeIndex() == 15 - 1) {
                EvolveGenomes();
                return;
            }
            NextTestSubject();
        }
        currentFitness = testAgent.dist;
        if (currentFitness > bestFitness) {
            bestFitness = currentFitness;
        }
    }

    public void NextTestSubject() {
        ga.SetGenomeFitness(currentFitness, ga.GetCurrentGenomeIndex());
        currentFitness = 0.0f;
        Genome genome = ga.GetNextGenome();

        net.FromGenome(genome, 5, 8, 2);

        transform.position = defaultpos;
        transform.rotation = defaultrot;

        testAgent.Attach(net);
        testAgent.ClearFailure();

        // Resetear las checkpointa
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (GameObject c in checkpoints) {
            Renderer tmp = c.gameObject.GetComponent<Renderer>();
            tmp.material = normal;
            Checkpoint p = c.gameObject.GetComponent<Checkpoint>();
            p.passed = false;
        }
    }

    public void BreedNewPopulation() {
        ga.ClearPopulation();
        int totalweights = 5 * 8 + 8 * 2 + 8 + 2;
        ga.GeneratePopulation(15, totalweights);
    }

    public void EvolveGenomes() {
        ga.Breed();
        NextTestSubject();
    }

    public int GetCurrentMemberOfPopulation() {
        return ga.GetCurrentGenomeIndex();
    }
}
