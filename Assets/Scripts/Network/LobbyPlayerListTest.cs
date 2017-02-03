using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// </summary>
public class LobbyPlayerListTest : MonoBehaviour {

    public static LobbyPlayerListTest _instance = null;

    public RectTransform playerListContentTransform;
    public GameObject warningDirectPlayServer;
    public Transform addButtonRow;

    public MenuAnimationManager animManager;
    public GameObject trackMenu;
    public GameObject shipMenu;
    public GameObject optionsMenu;

    protected VerticalLayoutGroup _layout;
    public List<LobbyPlayerTest> _players = new List<LobbyPlayerTest>();

    public void Start() {
        _instance = this;
        _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();

        // Evita que se destruya el objeto del menu que contiene la lista de jugadores 
        // derivados de la clase NetworkLobbyPlayer
        DontDestroyOnLoad(gameObject);
    }

    public void OnEnable() {
        //_instance = this;
        //_layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();

        // Evita que se destruya el objeto del menu que contiene la lista de jugadores 
        // derivados de la clase NetworkLobbyPlayer
        //DontDestroyOnLoad(gameObject);
    }

    public void DisplayDirectServerWarning(bool enabled) {
        if (warningDirectPlayServer != null)
            warningDirectPlayServer.SetActive(enabled);
    }

    void Update() {
        //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
        //sometime to child being assigned before layout was enabled/init, leading to broken layouting)

        if (_layout)
            _layout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    public void AddPlayer(LobbyPlayerTest player) {
        if (_players.Contains(player))
            return;

        _players.Add(player);

        player.transform.SetParent(playerListContentTransform, false);
        addButtonRow.transform.SetAsLastSibling();

        PlayerListModified();
    }

    public void RemovePlayer(LobbyPlayerTest player) {
        _players.Remove(player);
        PlayerListModified();
    }

    public void PlayerListModified() {
        int i = 0;
        foreach (LobbyPlayerTest p in _players) {
            p.OnPlayerListChanged(i);
            ++i;
        }
    }
}
