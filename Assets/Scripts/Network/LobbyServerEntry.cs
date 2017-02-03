﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la información que muestra cada partida multijugador en la lista de partidas
/// </summary>
public class LobbyServerEntry : MonoBehaviour {
    public Text serverInfoText;
    public Text slotInfo;
    public Button joinButton;

    public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager, Color c) {
        serverInfoText.text = match.name;

        slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

        NetworkID networkID = match.networkId;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager); });

        GetComponent<Image>().color = c;
    }

    void JoinMatch(NetworkID networkID, LobbyManager lobbyManager) {
        lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
        lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        lobbyManager._isMatchmaking = true;
        lobbyManager.DisplayIsConnecting();
        lobbyManager.serverListPanel.gameObject.SetActive(false);
    }
}