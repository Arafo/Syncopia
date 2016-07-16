using UnityEngine;
using System.Collections;

public class ShipAI : ShipCore {

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

    // Configuracion de los adelantamientos
    private ShipReferer overtakeTarget;
    private Vector3 overTakeOffset;
    public float overTakeTimer;
    private float overtakeSide;

    private bool closeToShip;

    public override void OnStart() {
        xStableRatio = Random.Range(0.6f, 0.8f);
        aiRacingLineRand = Random.Range(0.8f, 1.5f);
        aiSteerSpeedRand = Random.Range(6, 10);
        steerSpeedup = Random.Range(0.5f, 1.2f);

        if (ship.isAI) {
            ship.config.engineAmount = Mathf.Lerp(ship.config.engineAmount, RaceSettings.ships[0].config.engineAmount, 0.9f);
        }
    }

    public override void OnUpdate() {
        AIUpdate();
    }

    void AIUpdate() {

        if (RaceSettings.shipsRestrained) {
            ship.input.m_AccelerationButton = false;
            return;
        }

        // AI siempre acelera
        ship.input.m_AccelerationButton = true;

        // Cantidad de segmentos a comprobar
        int lookAheadAmount = 4;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0f, 1 << LayerMask.NameToLayer("Track"))) {
            TrackTile t = Helpers.TileFromTriangleIndex(hit.triangleIndex, RaceSettings.trackData.trackData.mappedTiles);
            TrackSegment newSection = Helpers.TileGetSection(t);

            if (newSection.index > ship.currentSection.index - 15 && newSection.index < ship.currentSection.index + 15)
                ship.currentSection = newSection;

        }

        Vector3 offset = transform.InverseTransformPoint(ship.currentSection.position);

        Vector3 sectionPos = AILookAhead(lookAheadAmount);
        sectionPos += (Helpers.SectionGetRotation(ship.currentSection) * Vector3.right) * aiRacingLineRand;
        sectionPos.y = 0;

        Vector3 flatPos = transform.position;
        flatPos.y = 0;

        if (RaceSettings.countdownFinished) {
            // Autopilot
            if (!ship.isAI && ship.autopilot) {
                steerSpeedup = 0.8f;
                //ship.sim.enginePower = 0.85f;
            }
            else if (ship.isAI) {
                switch (ship.place) {
                    case 1:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.75f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.75f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                        }
                        break;
                    case 2:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.75f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.75f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                        }
                        break;
                    case 3:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.95f;
                                break;
                        }
                        break;
                    case 4:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.95f;
                                break;
                        }
                        break;
                    case 5:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.95f;
                                break;
                        }
                        break;
                    case 6:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.85f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 0.95f;
                                break;
                        }
                        break;
                    case 7:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 1.0f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 1.1f;
                                break;
                        }
                        break;
                    case 8:
                        switch (RaceSettings.difficulty) {
                            case Enumerations.E_DIFFICULTY.EASY:
                                ship.sim.engineSpeed *= 0.9f;
                                break;
                            case Enumerations.E_DIFFICULTY.MEDIUM:
                                ship.sim.engineSpeed *= 1.0f;
                                break;
                            case Enumerations.E_DIFFICULTY.HARD:
                                ship.sim.engineSpeed *= 1.1f;
                                break;
                        }
                        break;
                }
            } 

            if (overTakeTimer > 0) {
                ship.sim.engineSpeed = overtakeTarget.sim.engineSpeed * 1.3f;
                overTakeOffset = overtakeTarget.transform.TransformPoint(Mathf.Sin(-offset.x) * 0.8f, 0.0f, 0.5f);

                if (Vector3.Distance(transform.position, overtakeTarget.transform.position) > 4) {
                    overTakeTimer = -1;
                }

                if (overtakeTarget.transform.InverseTransformPoint(transform.position).z > 0f) {
                    overTakeTimer = -1;
                }
            }

            // Frenar si se esta detras de otra nave
            if (closeToShip)
                ship.sim.engineSpeed *= 1.5f;

            // Empujar al centro del circuito
            if (Mathf.Abs(offset.x) > ship.currentSection.width * 0.1f && RaceSettings.countdownFinished) {
                Vector3 pushDir = Helpers.SectionGetRotation(ship.currentSection) * Vector3.right;
                pushDir.y = 0;
                ship.body.AddForce((offset.x * (400 + (steerSpeedup * 85))) * pushDir, ForceMode.Acceleration);

                Vector3 ignoreZ = transform.InverseTransformDirection(pushDir);
                ignoreZ.x = 0.0f;
                ignoreZ.y = 0.0f;
                ship.body.AddForce(transform.forward * Mathf.Abs(ignoreZ.z * steerSpeedup));
            }

            closeToShip = false;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5, 1 << LayerMask.NameToLayer("Ship"))) {
                Debug.Log("Ship " + ship.place + " overtake 2");
                if (hit.distance < 0.5f)
                    closeToShip = true;
            }

            // Comprobacion de adelantamiento
            RaycastHit outSphere;
            if (Physics.Raycast(transform.position, transform.forward, out outSphere, 5.0f, 1 << LayerMask.NameToLayer("Ship"))) {
                Debug.Log("Ship " + ship.place + " overtake 2");
                if (overTakeTimer < 0) {
                    overtakeTarget = outSphere.transform.gameObject.GetComponent<ShipReferer>();

                    overtakeSide = -Mathf.Sign(offset.x);
                    if (overtakeSide == 0) overtakeSide = 1;

                    overTakeOffset = overtakeTarget.transform.TransformPoint(overtakeSide * 4, 0.0f, 0.5f);
                    overTakeTimer = 3.5f;
                }

                float o = overtakeTarget.transform.InverseTransformPoint(transform.position).x * 1000;
                offset += (Helpers.SectionGetRotation(ship.currentSection) * Vector3.right) * -o;
            }

            // Adelantar
            if (closeToShip || overTakeTimer > 0) {
                Debug.Log("Ship " + ship.place + " overtake 2");
                ship.body.AddForce(overtakeTarget.transform.right * (overtakeSide * Mathf.Abs(overtakeTarget.transform.InverseTransformPoint(transform.position).x)));
            }

            overTakeTimer -= Time.deltaTime;
            Vector3 basePos = (overTakeTimer > 0) ? overTakeOffset : sectionPos;
            Vector3 lookPos = basePos - flatPos;
            aiSteer = Quaternion.Lerp(aiSteer, Quaternion.LookRotation(lookPos), Time.deltaTime * 10);
            Quaternion lookRot = aiSteer;

            // Rotar nave
            Vector3 tempRot = transform.eulerAngles;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 4f);
            transform.rotation = Quaternion.Euler(tempRot.x, transform.eulerAngles.y, tempRot.z);

            Debug.DrawLine(transform.position, new Vector3(basePos.x, transform.position.y, basePos.z), Color.blue);

            rotDelta = Mathf.DeltaAngle(transform.eulerAngles.y, prevRot);
            prevRot = transform.eulerAngles.y;

            aiSteerTilt = Mathf.Lerp(aiSteerTilt, rotDelta * 35, Time.deltaTime * 2);
            aiSteerTilt = Mathf.Clamp(aiSteerTilt, -55.0f, 55.0f);
            ship.sim.bankVelocity = aiSteerTilt;

            // Drag
            aiResistance = Mathf.Lerp(aiResistance, Mathf.Clamp(Mathf.Abs(rotDelta * 0.001f), 0.0f, 1.0f), Time.deltaTime * 5);
        }
        else {
            ship.sim.engineSpeed *= 0.65f;
        }

        Vector3 lv = transform.InverseTransformDirection(ship.body.velocity);
        lv.x *= 1 - 0.9f;
        Vector3 wv = transform.TransformDirection(lv);
        ship.body.velocity = wv;

        if (!ship.autopilot && RaceSettings.countdownFinished) {
            ship.sim.engineSpeed *= 0.80f;
        }
    }

    private Vector3 AILookAhead(int amount) {
        TrackSegment start = ship.currentSection;
        TrackSegment next = start;
        for (int i = 0; i < amount; ++i) {
            next = next.next;
        }

        return next.position;
    }
}
