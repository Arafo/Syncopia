﻿using UnityEngine;
using System.Collections.Generic;

public class RaceSettings {

    // Referencias
    public static TrackData trackData;
    public static Camera currentCamera;
    public static List<ShipReferer> ships = new List<ShipReferer>();
    public static RaceManager raceManager;

    // Opciones
    public static int racers = 5;
    public static int laps = 1;
    public static string trackToLoad;

    public static bool countdownReady;
    public static bool countdownFinished = true;

}
