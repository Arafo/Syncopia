using UnityEngine;
using System.Collections;

public class ShipInput : MonoBehaviour {

    public int m_PlayerNumber = 1;
    public const string m_verticalAxis = "Vertical";
    public const string m_HorizontalAxis = "Horizontal";
    private string m_MovementAxisName;
    private string m_TurnAxisName;

    // Inputs
    public float m_SteerAxis;
    public float m_PitchAxis;
    public bool m_AccelerationButton;
    public bool m_BoostButton;
    public bool m_CameraButton;
    public float m_LeftAirBrakeAxis;
    public float m_RightAirBrakeAixs;

    // Use this for initialization
    void Start () {
        m_MovementAxisName = m_verticalAxis + m_PlayerNumber;
        m_TurnAxisName = m_HorizontalAxis + m_PlayerNumber;
    }

    // Update is called once per frame
    void Update () {
        m_AccelerationButton = Input.GetButton("Acceleration");
        m_SteerAxis = Input.GetAxis("Steer");
        m_PitchAxis = Input.GetAxis("Pitch");
        m_LeftAirBrakeAxis = Input.GetAxis("Left brake");
        m_RightAirBrakeAixs = Input.GetAxis("Right brake"); 
        m_BoostButton = Input.GetButton("Boost");
        m_CameraButton = Input.GetButtonDown("Camera");
    }
}
