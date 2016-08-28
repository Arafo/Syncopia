using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerSettings {

    // server-side
    public static int maxPlayers;
    public static int hostPort;
    public static List<NetworkedPlayer> players = new List<NetworkedPlayer>();
    public static bool isServer;
    public static bool isNetworked;

    // race specific
    public static bool playerFinished;
    public static float raceCountdown;
}
