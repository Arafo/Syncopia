using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

//Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
public class LobbyMenu : MonoBehaviour {

    public LobbyManager lobbyManager;

    public RectTransform lobbyServerList;
    public RectTransform lobbyPanel;

    private int i = 0;

    public void OnEnable() {
        lobbyManager.topPanel.ToggleVisibility(true);
    }

    /// <summary>
    /// Evento del click al pulsar sobre el botón de crear host
    /// </summary>
    public void OnClickHost() {
        lobbyManager.StartHost();
    }

    /// <summary>
    /// Evento del click al pulsar sobre el botón JOIN
    /// 
    /// </summary>
    public void OnClickJoin() {
        lobbyManager.ChangeTo(lobbyPanel);
        lobbyManager.StartClient();

        lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        lobbyManager.DisplayIsConnecting();

        lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClickCreateMatchmakingGame() {
        lobbyManager.StartMatchMaker();
        lobbyManager.matchMaker.ListMatches(0, int.MaxValue, "", true, 0, 0, OnMatchCount);

        /*lobbyManager.matchMaker.CreateMatch(
                "Syncopia-" + i,
                (uint)lobbyManager.maxPlayers,
                true,
                "", "", "", 0, 0,
                lobbyManager.OnMatchCreate);

        lobbyManager.backDelegate = lobbyManager.StopHost;
        lobbyManager._isMatchmaking = true;
        lobbyManager.DisplayIsConnecting();

        lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);*/
    }

    /// <summary>
    /// Callback de la petición de la lista de partidas al crear el matchmaking
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matches"></param>
    public void OnMatchCount(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {

        // Busca el número de partida más bajo entre las que ya estan creadas
        foreach (MatchInfoSnapshot info in matches) {
            int number = int.Parse(info.name.Split('-')[1]);
            if (number == i)
                i++;
            else
                break;
        }

        lobbyManager.matchMaker.CreateMatch(
            "Syncopia-" + i,
            (uint)lobbyManager.maxPlayers,
            true,
            "", "", "", 0, 0,
            lobbyManager.OnMatchCreate);

        lobbyManager.backDelegate = lobbyManager.StopHost;
        lobbyManager._isMatchmaking = true;
        lobbyManager.DisplayIsConnecting();

        lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClickOpenServerList() {
        lobbyManager.StartMatchMaker();
        lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
        lobbyManager.ChangeTo(lobbyServerList);
    }
}
