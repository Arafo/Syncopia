using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
//using TeamUtility.IO;
using Rewired;
using UnityEngine.EventSystems;

public class HorizontallScrollSliderButton : Button {

    public MenuEventManager menuManager;
    public HorizontallScrollSlider button;
    private float m_HorizontalInput;
    private bool m_Highlighted = false;
    private float m_InputSteep;
    private float m_MaxInputSteep = 0.25f;


    // Update is called once per frame
    void Update () {
        m_HorizontalInput = ReInput.players.GetPlayer(0).GetAxis("MenuHorizontal");
        if (m_Highlighted) {
            m_InputSteep += Time.deltaTime;
            if (m_HorizontalInput > 0 && m_InputSteep > m_MaxInputSteep) {
                button.setNextText();
                m_InputSteep = 0f;
            }
            else if (m_HorizontalInput < 0 && m_InputSteep > m_MaxInputSteep) {
                button.setLastText();
                m_InputSteep = 0f;
            }
        }
    }

    public override void OnSelect(BaseEventData data) {
        base.OnSelect(data);
        m_Highlighted = true;
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        m_Highlighted = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetDifficulty() {
        menuManager.SetDifficulty(button.index);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetLaps() {
        menuManager.SetLaps(Convert.ToInt32(button.listContent[button.index]));
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetPlayers() {
        // Se suma una unidad al numero de jugadores porque el propio jugador tambien cuenta
        menuManager.SetPlayers(Convert.ToInt32(button.listContent[button.index]) + 1);
    }

    public void SetOverrideRaceSettings() {
        menuManager.SetOverrideRaceSettings(true);
    }
}
