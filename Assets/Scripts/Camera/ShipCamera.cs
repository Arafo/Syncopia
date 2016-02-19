using UnityEngine;
using System.Collections;

public class ShipCamera : MonoBehaviour {

    public GameObject Camera;

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

    // Referencias
    private ShipSimulation m_Sim;
    private ShipInput m_input;
    private ShipController m_controller;
    private ShipLoad m_Ship;

    // Use this for initialization
    void Start () {
        m_input = GetComponent<ShipInput>();
        m_controller = GetComponent<ShipController>();
        m_Sim = GetComponent<ShipSimulation>();
        m_Ship = GetComponent<ShipLoad>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Camera != null) {
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
            m_Ship.m_Axis.SetActive(true);
            cameraInputAmount = Mathf.Lerp(cameraInputAmount, -transform.InverseTransformDirection(m_Ship.m_body.velocity).x, Time.deltaTime * m_Ship.m_Config.cameraSpeed);
            //
            if (m_Ship.m_Config.cameraTurnDamp == 0f) {
                cameraTurnAmount = m_Sim.turnAmount;
            }
            else {
                cameraTurnAmount = Mathf.Lerp(cameraTurnAmount, m_Sim.turnAmount, Time.deltaTime * m_Ship.m_Config.cameraTurnDamp);
            }

            cameraOffset.x = Mathf.Lerp(cameraOffset.x, cameraInputAmount * 0.012f + (cameraTurnAmount * m_Ship.m_Config.cameraTurnSensitivity + m_Sim.airbrakeAmount), Time.deltaTime * (m_Ship.m_Config.cameraSpeed * 0.8f));
            cameraFallHelper = Mathf.Lerp(cameraFallHelper, m_Ship.m_Config.cameraClosePos.y - cameraTurningHeight - Camera.transform.localPosition.y, Time.deltaTime * (m_Ship.m_Config.cameraSpeed * 0.1f));
            cameraFallHelper = Mathf.Clamp(cameraFallHelper, 0f, 0.03f);
            cameraFallLag = Mathf.Lerp(cameraFallLag, transform.InverseTransformDirection(m_Ship.m_body.velocity).y * (0.05f - cameraFallHelper), Time.deltaTime * m_Ship.m_Config.cameraSpeed);
            cameraTurningHeight = Mathf.Lerp(cameraTurningHeight, Mathf.Abs(cameraOffset.x) * 0.3f, Time.deltaTime * (m_Ship.m_Config.cameraSpeed * 0.5f));
            cameraOffset.y = m_Ship.m_Config.cameraClosePos.y - cameraFallLag + cameraBoostLength * 0.7f;
            
            //
            if (m_Sim.isGrounded) {
                cameraFallingOffset = Mathf.Lerp(cameraFallingOffset, 0f, Time.deltaTime * 2f);
            }
            else  {
                cameraFallingOffset = Mathf.Lerp(cameraFallingOffset, -2f, Time.deltaTime * 0.05f);
            }
            cameraAccelLag = Mathf.Lerp(cameraAccelLag, -(transform.InverseTransformDirection(m_Ship.m_body.velocity).z * Time.deltaTime) * 0.25f, Time.deltaTime * m_Ship.m_Config.cameraSpeed);
            cameraTurningOffset = Mathf.Lerp(cameraTurningOffset, Mathf.Abs(cameraOffset.x) * 0.2f, Time.deltaTime * (m_Ship.m_Config.cameraSpeed * 0.2f));
            cameraOffset.z = m_Ship.m_Config.cameraClosePos.z + cameraAccelLag + cameraTurningOffset - cameraBoostLength;

            //
            if (m_controller.isThrusting) {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, 6.5f, Time.deltaTime * 3f);
            }
            else if (m_input.m_LeftAirBrakeAxis != 0f && m_input.m_RightAirBrakeAixs != 0f && transform.InverseTransformDirection(m_Ship.m_body.velocity).z > 10f) {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, -7.5f, Time.deltaTime * 3f);
            }
            else {
                cameraMovementLength = Mathf.Lerp(cameraMovementLength, 0f, Time.deltaTime * 3f);
            }

            Camera.transform.parent = transform;
            Camera.transform.localPosition = cameraOffset + -shipHoverCameraAnimOffset * 0.4f;
            float value = Mathf.Abs(transform.InverseTransformDirection(m_Ship.m_body.angularVelocity).x) * 0.5f;
            value = Mathf.Clamp(value, 0.1f, float.PositiveInfinity);
            float num = transform.InverseTransformDirection(m_Ship.m_body.velocity).z * 0.04f;
            num = Mathf.Clamp(num, 0f, 16f);
            cameraPitchMod = Mathf.Lerp(cameraPitchMod, transform.InverseTransformDirection(m_Ship.m_body.angularVelocity).x * num, Time.deltaTime * m_Ship.m_Config.cameraSpeed);
            float x = cameraOffset.x * 0.1f;
            Vector3 a = transform.TransformPoint(x, -4f, 25f + cameraPitchMod + cameraMovementLength);
            Camera.transform.rotation = Quaternion.LookRotation(a - Camera.transform.position, transform.up);
        }

        // Modo 1
        else if (cameraMode == 1) {
            m_Ship.m_Axis.SetActive(false);
            Camera.transform.parent = transform;
            Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localRotation = Quaternion.Euler(0f, 0f, -m_Sim.bankAmount * 1.3f);
        }

        float num3 = Mathf.Abs(transform.InverseTransformDirection(m_Ship.m_body.velocity).z) * Time.deltaTime * 10f;
        cameraFoV = Mathf.Lerp(cameraFoV, m_Ship.m_Config.cameraFoV + num3, Time.deltaTime * m_Ship.m_Config.cameraSpeed);

        //
        if (cameraFoV < 60f) {
            cameraFoV = 60f;
        }

        //
        if (m_Sim.isBoosting) {
            cameraBoostFoV = Mathf.Lerp(cameraBoostFoV, 10f, Time.deltaTime * 16f);
            cameraBoostLength = Mathf.Lerp(cameraBoostLength, 0.1f, Time.deltaTime * 16f);
        }
        else {
            cameraBoostFoV = Mathf.Lerp(cameraBoostFoV, 0f, Time.deltaTime * 3f);
            cameraBoostLength = Mathf.Lerp(cameraBoostLength, 0f, Time.deltaTime * 4f);
        }
        Camera.GetComponent<Camera>().fieldOfView = cameraFoV + cameraBoostFoV;
    }
}
