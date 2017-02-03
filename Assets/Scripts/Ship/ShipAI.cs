using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona el comportamiento de la inteligencia artificial que utilizan las naves
/// </summary>
public class ShipAI : ShipCore {

    private AIChecks checks;
    private AIData[] data;

    // Configuracion de la IA
    private float xStableRatio = 1.0f;
    private float aiRacingLineRand;
    private float aiSteerSpeedRand;

    private float deadTimer;

    private Quaternion aiSteer;
    private float aiDrag;
    private float aiSteerTilt;
    public float rotDelta;
    private float prevRot;
    private float aiResistance;
    private float steerSpeedup;
    private float steerSlowdown;

    // Configuracion de los adelantamientos
    private ShipReferer overtakeTarget;
    private Vector3 overTakeOffset;
    public float overTakeTimer;
    private float overtakeSide;

    private Vector3 offset;
    private Vector3 sectionPos;
    private Vector3 flatPos;

    private bool closeToShip;

    public override void OnStart() {
        xStableRatio = Random.Range(0.6f, 0.8f);
        aiRacingLineRand = Random.Range(-0.5f, 0.5f);
        aiSteerSpeedRand = Random.Range(6, 10);
        steerSpeedup = Random.Range(0.2f, 0.5f);

        if (ship.isAI) {
            ship.config.engineAmount = Mathf.Lerp(ship.config.engineAmount, RaceSettings.ships[0].config.engineAmount, 0.9f);
        }

        checks = new AIChecks(ship);
        data = new AIData[0];
    }

    public override void OnUpdate() {
        data = checks.GetAwarenessData(data);
        AIUpdate();
    }

    /// <summary>
    /// Inteligencia artificial
    /// </summary>
    void AIUpdate() {

        if (RaceSettings.shipsRestrained) {
            ship.input.m_AccelerationButton = false;
            return;
        }

        // AI siempre acelera
        ship.input.m_AccelerationButton = true;

        // Cantidad de segmentos a comprobar
        int lookAheadAmount = 4;

        offset = transform.InverseTransformPoint(ship.currentSection.position);

        sectionPos = AILookAhead(lookAheadAmount);
        sectionPos += (Helpers.SectionGetRotation(ship.currentSection) * Vector3.right) * aiRacingLineRand;
        sectionPos.y = 0;

        flatPos = transform.position;
        flatPos.y = 0;

        if (RaceSettings.countdownFinished) {
            // Autopilot
            if (!ship.isAI && ship.autopilot) {
                steerSpeedup = 0.8f;
                ship.sim.engineThrust *= 0.85f;
            }
            else if (ship.isAI) {
                switch (ship.currentPosition) {
                    case 1:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.7f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 0.8f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 0.95f;
                                break;
                        }
                        break;
                    case 2:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.75f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                        }
                        break;
                    case 3:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.8f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                        }
                        break;
                    case 4:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.85f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 0.95f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                        }
                        break;
                    case 5:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.95f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 1.0f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                        }
                        break;
                    case 6:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 0.95f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 1.0f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                        }
                        break;
                    case 7:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 1f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.1f;
                                break;
                        }
                        break;
                    case 8:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineThrust *= 1f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineThrust *= 1.05f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineThrust *= 1.1f;
                                break;
                        }
                        break;
                }
            }

            float num = (!ship.autopilot) ? 0.9f : 0.4f;
            if (Mathf.Abs(offset.x) > ship.currentSection.width * num && RaceSettings.countdownFinished) {
                //ship.body.AddForce(offset.x * (20f + steerSpeedup * 5f) * transform.right, ForceMode.Acceleration);
            }

            for (int j = 0; j < data.Length; j++) {
                if (data[j].shipInFront && overTakeTimer < 0f) {
                    //Debug.Log(ship.name + "Nave delante " + data[j].ship.name + " " + data[j].localOffset.z);
                    overtakeTarget = data[j].ship;
                    overtakeSide = -Mathf.Sign(data[j].localOffset.x);
                    if (overtakeSide == 0f) {
                        overtakeSide = 1f;
                    }
                    overTakeTimer = 3.5f;
                }

                if (data[j].shipLeft) {
                    //Debug.Log(ship.name + "Nave izquierda " + data[j].ship.name + " " + data[j].localOffset.z);
                    ship.body.AddForce(transform.right * 50f);
                }

                if (data[j].shipRight) {
                    //Debug.Log(ship.name + "Nave derecha " + data[j].ship.name + " " + data[j].localOffset.z);
                    ship.body.AddForce(-transform.right * 50f);
                }
            }

            if (overTakeTimer > 0f && overtakeTarget) {
                //Debug.Log(ship.name + " adelanta a " + overtakeTarget.name);
                overTakeOffset = sectionPos + Helpers.SectionGetRotation(ship.currentSection) * Vector3.right * (overtakeSide * 10.3f);
                overTakeOffset.y = 0f;
                //Debug.Log(Vector3.Distance(transform.position, overtakeTarget.transform.position));
                if (Vector3.Distance(transform.position, overtakeTarget.transform.position) > 12f) {
                    //Debug.Log("NO OVERTAKE " + ship.name + " TO " + overtakeTarget.name);
                    overTakeTimer = -1f;
                }
                if (overtakeTarget.transform.InverseTransformPoint(transform.position).z > 6f) {
                    //Debug.Log("NO OVERTAKE " + ship.name + " TO " + overtakeTarget.name);
                    overTakeTimer = -1f;
                }
            }

            if (overTakeTimer > 0f) {
                Vector3 force = overtakeTarget.transform.right * (overtakeSide * Mathf.Abs(overtakeTarget.transform.InverseTransformPoint(transform.position).x));
                //Debug.Log(ship.name + " PAALANTE " + force);
                ship.body.AddForce(overtakeTarget.transform.right * (overtakeSide * Mathf.Abs(overtakeTarget.transform.InverseTransformPoint(transform.position).x)) * 40f);
            }

            overTakeTimer -= Time.deltaTime;
            Vector3 basePos = ((overTakeTimer <= 0f) ? sectionPos : overTakeOffset);
            Vector3 lookPos = basePos - flatPos;
            aiSteer = Quaternion.Lerp(aiSteer, Quaternion.LookRotation(lookPos), Time.deltaTime * 40f);
            Quaternion lookRot = aiSteer;

            // Rotar nave
            Vector3 tempRot = transform.eulerAngles;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 4f);
            transform.rotation = Quaternion.Euler(tempRot.x, transform.eulerAngles.y, tempRot.z);

            Debug.DrawLine(transform.position, new Vector3(basePos.x, transform.position.y, basePos.z), Color.blue);

            rotDelta = Mathf.DeltaAngle(transform.eulerAngles.y, prevRot);
            prevRot = transform.eulerAngles.y;

            aiSteerTilt = Mathf.Lerp(aiSteerTilt, rotDelta * 45f, Time.deltaTime * 24f);
            aiSteerTilt = Mathf.Clamp(aiSteerTilt, -55f, 55f);
            ship.sim.bankVelocity = aiSteerTilt;

            // Drag
            aiResistance = Mathf.Lerp(aiResistance, Mathf.Clamp(Mathf.Abs(rotDelta * 0.001f), 0f, 1f), Time.deltaTime * 5f);
        }
        else {
            ship.sim.engineThrust *= 0.65f;
        }


        steerSlowdown = 0.05f;
        Vector3 direction = transform.InverseTransformDirection(ship.body.velocity);
        if (!ship.autopilot) {
            direction.z *= 1f - Mathf.Clamp(Mathf.Abs(rotDelta), 0f, 0.1f) * steerSlowdown;
        }
        direction.x *= 0.9f;
        Vector3 velocity = transform.TransformDirection(direction);
        ship.body.velocity = velocity;

        //if (!ship.autopilot && RaceSettings.countdownFinished) {
            //ship.sim.engineThrust *= 0.80f;
        //}
    }

    /// <summary>
    /// Obtiene la posicion del segmento que esta a amount
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private Vector3 AILookAhead(int amount) {
        TrackSegment start = ship.currentSection;
        TrackSegment next = start;
        for (int i = 0; i < amount; ++i) {
            next = next.next;
        }

        return next.position;
    }
}
