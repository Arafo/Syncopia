using UnityEngine;
using System.Collections;

public class AIData {
    public ShipReferer ship;

    public Vector3 localOffset;
    public int segmentOffset;

    public bool shipBehind;
    public bool shipInFront;
    public bool shipLeft;
    public bool shipRight;

    public AIData(ShipReferer targetShip) {
        ship = targetShip;
    }
}
