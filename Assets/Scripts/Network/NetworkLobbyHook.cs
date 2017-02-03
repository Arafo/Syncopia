using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona el paso de datos entre el menú multijugador y la partida
/// </summary>
public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        if (lobbyPlayer == null)
            return;

        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();

        if (lp != null)
            NetworkGameManager.AddShip(gamePlayer, lp.slot, lp.playerColor, lp.nameInput.text, lp.playerControllerId);
    }
}
