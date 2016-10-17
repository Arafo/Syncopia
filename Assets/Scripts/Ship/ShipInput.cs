using UnityEngine;
using System.Collections;
using Rewired;

public class ShipInput : ShipCore {

    public int m_PlayerNumber = 1;
    public const string m_verticalAxis = "Vertical";
    public const string m_HorizontalAxis = "Horizontal";
    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Player player;

    // Inputs
    public float m_SteerAxis;
    public float m_PitchAxis;
    public bool m_AccelerationButton;
    public bool m_BoostButton;
    public bool m_CameraButton;
    public float m_LeftAirBrakeAxis;
    public float m_RightAirBrakeAixs;
    public float m_AirBrakesAxis;

    // Use this for initialization
    public override void OnStart () {
        m_MovementAxisName = m_verticalAxis + m_PlayerNumber;
        m_TurnAxisName = m_HorizontalAxis + m_PlayerNumber;
        player = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    public override void OnUpdate () {
        if (ship.isAI || ship.autopilot) {
            m_SteerAxis = 0.0f;
            m_PitchAxis = 0.0f;
            m_LeftAirBrakeAxis = 0.0f;
            m_RightAirBrakeAixs = 0.0f;
            //ship.ai.OnUpdate();
        }
        else {
            if (!RaceSettings.shipsRestrained) {
                m_AccelerationButton = player.GetButton("Acceleration");
                m_SteerAxis = player.GetAxis("Steer");
                m_PitchAxis = player.GetAxis("Pitch");
                m_LeftAirBrakeAxis = player.GetAxis("Left brake");
                m_RightAirBrakeAixs = player.GetAxis("Right brake");
                m_BoostButton = player.GetButton("Boost");
                m_CameraButton = player.GetButtonDown("Camera");
                m_AirBrakesAxis = m_LeftAirBrakeAxis + m_RightAirBrakeAixs;
            }
            else {
                m_AccelerationButton = false;
                m_SteerAxis = 0.0f;
                m_PitchAxis = 0.0f;
                m_LeftAirBrakeAxis = 0.0f;
                m_RightAirBrakeAixs = 0.0f;
            } 
        }
    }
}
