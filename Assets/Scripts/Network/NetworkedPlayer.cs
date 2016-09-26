using System;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class NetworkedPlayer {

    [Header("Network")]
    [Space]

    [SyncVar]
    public Color m_PlayerColor;               // This is the color this tank will be tinted.
    //public Transform m_SpawnPoint;            // The position and direction the tank will have when it spawns.

    [HideInInspector]
    [SyncVar]
    public int m_PlayerNumber;                // This specifies which player this the manager for.

    [HideInInspector]
    public GameObject m_Instance;             // A reference to the instance of the tank when it is created.

    [HideInInspector]
    [SyncVar]
    public string m_PlayerName;                    // The player name set in the lobby

    [HideInInspector]
    [SyncVar]
    public int m_LocalPlayerID;                    // The player localID (if there is more than 1 player on the same machine)

    [SyncVar]
    public bool m_HasSpawned = false;

    [SyncVar]
    public bool m_IsReady = false;


    // in-game
    //public bool hasSpawned;
    //public bool gameReady;
    //public ShipReferer ship;
    //public Vector3 spawnPos;
    //public Quaternion spawnRot;

    public NetworkShipSetup m_Setup;


    public void Config(int i) {
        if (ServerSettings.players[i].m_Instance.GetComponent<NetworkIdentity>().isLocalPlayer) {

            Enumerations.E_SHIPS team = Enumerations.E_SHIPS.FLYER;

            GameObject newShip = new GameObject("PLAYER SHIP" + i);
            ShipLoader loader = newShip.AddComponent<ShipLoader>();

            float rot = newShip.transform.eulerAngles.y;
            newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

            // position ship at spawn
            newShip.transform.position = RaceSettings.trackData.spawnPositions[i];
            newShip.transform.rotation = RaceSettings.trackData.spawnRotations[i];

            //NetworkTransform nt = newShip.AddComponent<NetworkTransform>();

            Renderer rend = ServerSettings.players[i].m_Instance.transform.GetChild(0).GetComponent<Renderer>();
            if (rend != null) {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.SetPixel(0, 0, Color.grey);
                texture.SetPixel(1, 0, Color.white);
                texture.SetPixel(0, 1, Color.black);
                texture.SetPixel(1, 1, m_PlayerColor);
                texture.Apply();

                // connect texture to material of GameObject this script is attached to
                rend.material.SetTexture("_DetailAlbedoMap", texture);
            }

            // spawn
            loader.SpawnShip(team, 2, ServerSettings.players[i].m_Instance);
        }
        /*else {
            loader.SpawnClientShip(team, 3, ServerSettings.players[i].m_Instance);
        }*/
    }


    public void ConfigRemote(int i) {
        if (!ServerSettings.players[i].m_Instance.GetComponent<NetworkIdentity>().isLocalPlayer) {

            Enumerations.E_SHIPS team = Enumerations.E_SHIPS.FLYER;

            GameObject newShip = new GameObject("PLAYER SHIP" + i);
            ShipLoader loader = newShip.AddComponent<ShipLoader>();

            float rot = newShip.transform.eulerAngles.y;
            newShip.transform.rotation = Quaternion.Euler(0.0f, rot, 0.0f);

            // position ship at spawn
            newShip.transform.position = RaceSettings.trackData.spawnPositions[i];
            newShip.transform.rotation = RaceSettings.trackData.spawnRotations[i];

            //NetworkTransform nt = newShip.AddComponent<NetworkTransform>();

            Renderer rend = ServerSettings.players[i].m_Instance.transform.GetChild(0).GetComponent<Renderer>();
            if (rend != null) {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.SetPixel(0, 0, Color.grey);
                texture.SetPixel(1, 0, Color.white);
                texture.SetPixel(0, 1, Color.black);
                texture.SetPixel(1, 1, m_PlayerColor);
                texture.Apply();

                // connect texture to material of GameObject this script is attached to
                rend.material.SetTexture("_DetailAlbedoMap", texture);
            }

            // spawn
            loader.SpawnClientShip(team, 2, ServerSettings.players[i].m_Instance);
        }
        /*else {
            loader.SpawnClientShip(team, 3, ServerSettings.players[i].m_Instance);
        }*/
    }


    public void Setup() {

        m_Setup = m_Instance.GetComponent<NetworkShipSetup>();

        //setup is use for diverse Network Related sync
        m_Setup.m_Color = m_PlayerColor;
        m_Setup.m_PlayerName = m_PlayerName;
        m_Setup.m_PlayerNumber = m_PlayerNumber;
        m_Setup.m_LocalID = m_LocalPlayerID;

    }

    public void Test() {
        Debug.Log(m_Instance.ToString());

        if (RaceSettings.ships[0].finished && !ServerSettings.playerFinished || ServerSettings.raceCountdown < 0 && !ServerSettings.playerFinished) {
            ServerSettings.playerFinished = true;

            // Habilitar los resultados
            RaceSettings.raceManager.results.gameObject.SetActive(true);
            RaceSettings.raceManager.results.Results();

            // Desactivar HUD
            RaceSettings.raceManager.ui.gameObject.SetActive(false);

            RaceSettings.ships[0].finished = true;
            RaceSettings.ships[0].isAI = true;
        }
    }
}