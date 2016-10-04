using UnityEngine;
using System.Collections;

public class ShipCamera : ShipCore {

    private int cameraMode;
    private Vector3 cameraOffset;
    private Vector3 shipHoverCameraAnimOffset;
    private float cameraAccelLag;
    private float cameraFallLag;
    private float cameraInputAmount;
    private float cameraMoveGain;
    private float cameraMoveFalloff;
    private float cameraFallingOffset;
    private float cameraTurningOffset;
    private float cameraTurningHeight;
    private float cameraFallHelper;
    private float cameraTurnAmount;
    private float cameraPitchMod;
    private float cameraWantedPitch;
    private float cameraFoV;
    private float cameraWallSurfFOV;
    private float cameraBoostFoV;
    private float cameraBoostLength;
    private float cameraMovementLength;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Camera>() != null) {
            UpdateCamera();
        }
    }

    public void UpdateCameraMode()
    {
        cameraMode++;
        if (cameraMode > 1) {
            cameraMode = 0;
        }
    }

    private void UpdateCamera()
    {
        // Modo 0
        if (cameraMode == 0) {
            ship.axis.SetActive(true);
            cameraInputAmount = Mathf.Lerp(cameraInputAmount, -ship.transform.InverseTransformDirection(ship.body.velocity).x, Time.deltaTime * ship.config.cameraSpeed);
            //
            if (ship.config.cameraTurnDamp == 0f) {
                cameraTurnAmount = ship.sim.turnAmount;
            }
            else {
                cameraTurnAmount = Mathf.Lerp(cameraTurnAmount, ship.sim.turnAmount, Time.deltaTime * ship.config.cameraTurnDamp);
            }

            cameraOffset.x = Mathf.Lerp(cameraOffset.x, cameraInputAmount * 0.012f + (cameraTurnAmount * ship.config.cameraTurnSensitivity + ship.sim.airbrakeAmount), Time.deltaTime * (ship.config.cameraSpeed * 0.8f));
            cameraFallHelper = Mathf.Lerp(cameraFallHelper, ship.config.cameraClosePos.y - cameraTurningHeight - transform.localPosition.y, Time.deltaTime * (ship.config.cameraSpeed * 0.1f));
            cameraFallHelper = Mathf.Clamp(cameraFallHelper, 0f, 0.03f);
            cameraFallLag = Mathf.Lerp(cameraFallLag, ship.transform.InverseTransformDirection(ship.body.velocity).y * (0.05f - cameraFallHelper), Time.deltaTime * ship.config.cameraSpeed);
            cameraTurningHeight = Mathf.Lerp(cameraTurningHeight, Mathf.Abs(cameraOffset.x) * 0.3f, Time.deltaTime * (ship.config.cameraSpeed * 0.5f));
            cameraOffset.y = ship.config.cameraClosePos.y - cameraFallLag + cameraBoostLength * 0.7f;
            
            //
            if (ship.sim.isGrounded) {
                cameraFallingOffset = Mathf.Lerp(cameraFallingOffset, 0f, Time.deltaTime * 2f);
            }
            else  {
                cameraFallingOffset = Mathf.Lerp(cameraFallingOffset, -2f, Time.deltaTime * 0.05f);
            }
            cameraAccelLag = Mathf.Lerp(cameraAccelLag, -(ship.transform.InverseTransformDirection(ship.body.velocity).z * Time.deltaTime) * 0.25f, Time.deltaTime * ship.config.cameraSpeed);
            cameraTurningOffset = Mathf.Lerp(cameraTurningOffset, Mathf.Abs(cameraOffset.x) * 0.2f, Time.deltaTime * (ship.config.cameraSpeed * 0.2f));
            cameraOffset.z = ship.config.cameraClosePos.z + cameraAccelLag + cameraTurningOffset - cameraBoostLength;

            //
            if (ship.control.isThrusting) {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, 6.5f, Time.deltaTime * 3f);
            }
            else if (ship.input.m_LeftAirBrakeAxis != 0f && ship.input.m_RightAirBrakeAixs != 0f && ship.transform.InverseTransformDirection(ship.body.velocity).z > 10f) {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, -7.5f, Time.deltaTime * 3f);
            }
            else {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, 0f, Time.deltaTime * 3f);
            }

            //transform.parent = transform;
            transform.parent = ship.transform;
            transform.localPosition = cameraOffset + -shipHoverCameraAnimOffset * 0.4f;
            float value = Mathf.Abs(ship.transform.InverseTransformDirection(ship.body.angularVelocity).x) * 0.5f;
            value = Mathf.Clamp(value, 0.1f, float.PositiveInfinity);
            //float num = ship.transform.InverseTransformDirection(ship.body.velocity).z * 0.04f;
            //num = Mathf.Clamp(num, 0f, 16f);
            //cameraPitchMod = Mathf.Lerp(cameraPitchMod, ship.transform.InverseTransformDirection(ship.body.angularVelocity).x * num, Time.deltaTime * ship.config.cameraSpeed);
            cameraPitchMod = Mathf.Lerp(cameraPitchMod, ship.transform.InverseTransformDirection(ship.body.angularVelocity).x * 16f, Time.deltaTime * ship.config.cameraSpeed);
            float x = cameraOffset.x * 0.1f;
            Vector3 a = ship.transform.TransformPoint(x, -4f, 25f + cameraPitchMod + cameraMovementLength);
            transform.rotation = Quaternion.LookRotation(a - transform.position, ship.transform.up);
        }

        // Modo 1
        else if (cameraMode == 1) {
            ship.axis.SetActive(false);
            transform.parent = transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 0f, -ship.sim.bankAmount * 1.3f);
        }

        float num3 = Mathf.Abs(transform.InverseTransformDirection(ship.body.velocity).z) * Time.deltaTime * 10f;
        cameraFoV = Mathf.Lerp(cameraFoV, ship.config.cameraFoV + num3, Time.deltaTime * ship.config.cameraSpeed);

        //
        if (cameraFoV < 60f) {
            cameraFoV = 60f;
        }

        //
        if (ship.sim.isBoosting) {
            cameraBoostFoV = Mathf.Lerp(cameraBoostFoV, 10f, Time.deltaTime * 16f);
            cameraBoostLength = Mathf.Lerp(cameraBoostLength, 0.1f, Time.deltaTime * 16f);
        }
        else {
            cameraBoostFoV = Mathf.Lerp(cameraBoostFoV, 0f, Time.deltaTime * 3f);
            cameraBoostLength = Mathf.Lerp(cameraBoostLength, 0f, Time.deltaTime * 4f);
        }
        GetComponent<Camera>().fieldOfView = cameraFoV + cameraBoostFoV;
    }
}
