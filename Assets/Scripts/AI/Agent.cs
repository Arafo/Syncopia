using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

    // Red Neuronal
    public bool hasFailed;
    public NeuralNetwork net;

    private int collidedCorner;
    private float headingAngle; // Grados
    private float heading;

    // RayCasting
    private float RayCast_Length;
    private RaycastHit hit_l, hit_fl, hit_f, hit_fr, hit_r;
    private Vector3 origin, left, frontleft, front, frontright, right;

    public float Rotation; //max rotate speed
    public float Speed;

    public float leftForce;
    public float rightForce;
    public float leftTheta;
    public float rightTheta;
    public float dist;

    private CheckpointHit hit;

    // Use this for initialization
    void Start() {
        hasFailed = false;
        net = new NeuralNetwork(1, 5, 8, 2);

        RayCast_Length = 10.0f;

        origin = transform.position + Vector3.up * 0.2f;
        heading = transform.rotation.eulerAngles.y;
        float angle = heading / 180 * Mathf.PI;

        left = new Vector3(origin.x - RayCast_Length * Mathf.Cos(angle), origin.y, origin.z + RayCast_Length * Mathf.Sin(angle));
        frontleft = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle - Mathf.PI / 4), origin.y, origin.z - RayCast_Length * Mathf.Cos(angle - Mathf.PI / 4));
        front = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle), origin.y, origin.z - RayCast_Length * Mathf.Cos(angle));
        frontright = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle + Mathf.PI / 4), origin.y, origin.z - RayCast_Length * Mathf.Cos(angle + Mathf.PI / 4));
        right = origin + origin - left;

        hit = gameObject.GetComponent<CheckpointHit>();
    }

    // Update is called once per frame
    void Update() {

        hasFailed = hit.crash;

        if (!hasFailed) {
            dist += Time.deltaTime;

            RayCasting();

            float[] inputs = new float[5];
            inputs[0] = Normalise(hit_l.distance);
            inputs[1] = Normalise(hit_fl.distance);
            inputs[2] = Normalise(hit_f.distance);
            inputs[3] = Normalise(hit_fr.distance);
            inputs[4] = Normalise(hit_r.distance);

            net.SetInput(inputs);
            net.Update();

            leftForce = net.GetOutput(0);
            rightForce = net.GetOutput(1);

            leftTheta = Rotation * leftForce;
            rightTheta = Rotation * rightForce;

            headingAngle += (leftTheta - rightTheta) * Time.deltaTime;

            float speed = (Mathf.Abs(leftForce + rightForce)) / 2;
            speed *= Speed;

            speed = Utils.Clamp(speed, -Speed, Speed);
        }
        else {
            dist = 0.0f;
        }
    }

    private void RayCasting() {
        origin = transform.position + Vector3.up * 0.2f;

        heading = transform.rotation.eulerAngles.y;
        float angle = heading / 180 * Mathf.PI;

        left = new Vector3(origin.x - RayCast_Length * Mathf.Cos(angle), origin.y, origin.z - RayCast_Length * Mathf.Sin(angle));
        frontleft = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle - Mathf.PI / 4), origin.y, origin.z + RayCast_Length * Mathf.Cos(angle - Mathf.PI / 4));
        front = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle), origin.y, origin.z + RayCast_Length * Mathf.Cos(angle));
        frontright = new Vector3(origin.x - RayCast_Length * Mathf.Sin(angle + Mathf.PI / 4), origin.y, origin.z + RayCast_Length * Mathf.Cos(angle + Mathf.PI / 4));
        right = origin + origin - left;

        // Izquierda
        Physics.Linecast(origin, left, out hit_l);
        Debug.DrawLine(origin, left, Color.yellow);
        // Izquierda - Frente
        Physics.Linecast(origin, frontleft, out hit_fl);
        Debug.DrawLine(origin, frontleft, Color.red);
        // Frente
        Physics.Linecast(origin, front, out hit_f);
        Debug.DrawLine(origin, front, Color.white);
        // Derecha - Frente
        Physics.Linecast(origin, frontright, out hit_fr);
        Debug.DrawLine(origin, frontright, Color.green);
        // Derecha
        Physics.Linecast(origin, right, out hit_r);
        Debug.DrawLine(origin, right, Color.blue);
    }

    public void ClearFailure() {
        hasFailed = false;
        hit.crash = false;
        hit.checkpoints = 0;
        dist = 0.0f;
        collidedCorner = -1;
    }

    public void Attach(NeuralNetwork net) {
        this.net = net;
    }

    public float Normalise(float i) {
        float depth = i / RayCast_Length;
        return 1 - depth;
    }
}
