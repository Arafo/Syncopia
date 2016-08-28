using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        if (lobbyPlayer == null)
            return;

        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();

        if (lp != null)
            NetworkGameManager.AddShip(gamePlayer, lp.slot, lp.playerColor, lp.nameInput.text, lp.playerControllerId);
    }
}
