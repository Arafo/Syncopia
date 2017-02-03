using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Almacena la información de un cliente en una partida multijugador
/// </summary>
public class NetworkShipSetup : NetworkBehaviour {
    [Header("UI")]
    public Text m_NameText;

    [Header("Network")]
    [Space]
    [SyncVar]
    public Color m_Color;

    [SyncVar]
    public string m_PlayerName;

    //this is the player number in all of the players
    [SyncVar]
    public int m_PlayerNumber;

    //This is the local ID when more than 1 player per client
    [SyncVar]
    public int m_LocalID;

    [SyncVar]
    public bool m_IsReady = false;


    public override void OnStartClient() {
        base.OnStartClient();

        if (!isServer) //if not hosting, we had the ship to the gamemanger for easy access!
            NetworkGameManager.AddShip(gameObject, m_PlayerNumber, m_Color, m_PlayerName, m_LocalID);
    }

    [ClientCallback]
    public void Update() {
        if (!isLocalPlayer) {
            return;
        }
    }

    public override void OnNetworkDestroy() {
        NetworkGameManager.s_Instance.RemoveShip(gameObject);
    }
}
