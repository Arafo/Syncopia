using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Rewired;
using UnityEngine.EventSystems;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona el funcionamiento de la opción seleccionada en un botón horizontal
/// </summary>
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
            m_InputSteep += (Time.timeScale != 0 ? Time.deltaTime : Time.unscaledDeltaTime);
            if (m_HorizontalInput > 0 && m_InputSteep > m_MaxInputSteep) {
                //button.setNextText();
                Debug.Log("DER");
                m_InputSteep = 0f;
                button.rigthButton.onClick.Invoke();
            }
            else if (m_HorizontalInput < 0 && m_InputSteep > m_MaxInputSteep) {
                //button.setLastText();
                Debug.Log("IZQ");
                m_InputSteep = 0f;
                button.leftButton.onClick.Invoke();
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
    /// Establece la dificultad
    /// </summary>
    public void SetDifficulty() {
        menuManager.SetDifficulty(button.index);
    }

    /// <summary>
    /// Establece el número de vueltas
    /// </summary>
    public void SetLaps() {
        menuManager.SetLaps(Convert.ToInt32(button.listContent[button.index]));
    }

    /// <summary>
    /// Establece el número de jugadores
    /// </summary>
    public void SetPlayers() {
        // Se suma una unidad al numero de jugadores porque el propio jugador tambien cuenta
        menuManager.SetPlayers(Convert.ToInt32(button.listContent[button.index]) + 1);
    }

    public void SetOverrideRaceSettings() {
        menuManager.SetOverrideRaceSettings(true);
    }
}
