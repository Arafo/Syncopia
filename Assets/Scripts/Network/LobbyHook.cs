using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Subclass this and redefine the function you want
// then add it to the lobby prefab
public abstract class LobbyHook : MonoBehaviour {
    public virtual void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) { }
}