using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HorizontallScrollSliderButton : Button {

    public MenuEventManager menuManager;
    public HorizontallScrollSlider button;

    // Update is called once per frame
    void Update () {
	
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
