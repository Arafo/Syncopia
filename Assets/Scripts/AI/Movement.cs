﻿using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    Agent agent;
    ShipInput input;
    public float leftsteer, rightsteer, lefttheta, righttheta;
    public float rotate, speed;

    public float heading;

    // Use this for initialization
    void Start() {
        agent = gameObject.GetComponent<Agent>();
        input = gameObject.GetComponent<ShipInput>();

        rotate = agent.Rotation;
        speed = agent.Speed;

        heading = transform.rotation.eulerAngles.y - 90;
        leftsteer = 0.0f;
        rightsteer = 0.0f;
        lefttheta = 0.0f;
        righttheta = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        heading = -transform.rotation.eulerAngles.y - 90;

        leftsteer = agent.leftForce;
        rightsteer = agent.rightForce;
        lefttheta = agent.leftTheta;
        righttheta = agent.rightTheta;

        float angle = (lefttheta - righttheta);

        transform.Rotate(new Vector3(0, angle, 0));

        float dir = -heading / 180 * Mathf.PI;

        float nx = -speed * Mathf.Cos(dir);
        float nz = speed * Mathf.Sin(dir);

        Vector3 newsp = new Vector3(nx, 0, nz);
        //Vector3 newsp = new Vector3(0, 0, nz);

        Vector3 newpos = transform.position + newsp;
        transform.position = newpos;

        //input.m_AccelerationButton = true;
        //input.m_SteerAxis = Mathf.Clamp(angle, -1.0f, 1.0f);
        //Debug.Log(nz);

    }
}
