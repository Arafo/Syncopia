using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : ShipCore {

    public int currentCheckpoint = 0;
    public int currentLap = 0;
    public bool isAI;
    public bool isThrusting;
    public float boostPower;
    public float boostTimer;
    private bool startBoost;
    public float powerBoost = 100f;
    public bool powerBoosting;
    public bool isRespawning;
    public Vector3 respawnPoint;
    public Quaternion respawnRotation;
    private float respawnTimer;
    private float respawnTimerMax = 2f;
    public int performedBarrelRolls;

    public bool finishedRace = false;
    public float totalS;
    public float bestLap = float.MaxValue;
    public List<float> laps = new List<float>();

    public override void OnStart() {
        respawnPoint = new Vector3(transform.position.x, 10, transform.position.z);
        respawnRotation = transform.rotation;
    }

    public override void OnUpdate() {

        powerBoosting = false;
        isThrusting = ship.input.m_AccelerationButton;
        totalS += Time.deltaTime;

        //
        if (ship.input.m_CameraButton) {
            //GetComponent<ShipCamera>().UpdateCameraMode();
        }

        //
        if (ship.input.m_BoostButton && powerBoost > 0f) {
            powerBoosting = true;
            powerBoost -= Time.deltaTime * 65f;
            //
            if (powerBoost < 0f) {
                powerBoost = 0f;
            }
            GetComponent<ShipSimulation>().boostingOverride = true;
            boostTimer = 0.2f;
            boostPower = 800f;
        }

        //
        if (isRespawning) {

        }

        //
        if (!isAI) {
            //
            if (ship.input.m_AccelerationButton && !startBoost) {
                boostTimer = 0.8f;
                boostPower = 1000f;
                startBoost = true;
            }

            //
            if (GetComponent<ShipSimulation>().BRBoostTimer > 0f) {
                boostTimer = 0.2f;
                boostPower = 650f;
            }
        }
    }

    private void FixedUpdate() {
        Boost();
        CheckRespawn();
    }

    private void Boost() {
        //
        if (boostTimer > 0f) {
            boostTimer -= Time.deltaTime;
            GetComponent<ShipSimulation>().boostingOverride = true;
            GetComponent<Rigidbody>().AddForce(transform.forward * boostPower);
        }
    }

    private void CheckRespawn() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit)) {
            if (hit.collider.gameObject.tag == "Track") {
                respawnTimer = 0f;
            }
            else {
                respawnTimer += Time.deltaTime;
            }
        }
        else {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > respawnTimerMax) {
                isRespawning = true;
                respawnTimer = 0f;
            }
        }
    }
}
