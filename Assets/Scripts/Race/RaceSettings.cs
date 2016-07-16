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
    public static Enumerations.E_DIFFICULTY difficulty = Enumerations.E_DIFFICULTY.MEDIUM;
    public static bool overrideRaceSettings = false;
    public static Enumerations.E_SHIPS playerShip = Enumerations.E_SHIPS.FLYER;
    public static string trackToLoad;

    public static bool countdownReady;
    public static bool countdownFinished = true;

    public static bool shipsRestrained;

}
