using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Proporciona una función para gestionar el paso 
/// de datos entre el menú multijugador y la partida.
/// </summary>
public abstract class LobbyHook : MonoBehaviour {
    public virtual void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) { }
}