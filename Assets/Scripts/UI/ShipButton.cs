using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipButton : Button {

    public MenuEventManager menuManager;
    public Enumerations.E_SHIPS ship;

    /// <summary>
    /// 
    /// </summary>
    public void SetShip() {
        menuManager.SetShip(ship, true);
    }
}
