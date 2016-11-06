using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
//Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
public class LobbyPlayer : NetworkLobbyPlayer {

    static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
    //used on server to avoid assigning the same color to two player
    static List<int> _colorInUse = new List<int>();

    public Button trackButton;
    public Button trackDisplayButton;
    public Button shipButton;
    public Button shipDisplayButton;
    public Button colorButton;
    public Button colorDisplayButton;
    public InputField nameInput;
    public Button readyButton;
    public Button readyDisplayButton;
    public Button waitingPlayerButton;
    public Button removePlayerButton;

    public GameObject localIcone;
    public GameObject remoteIcone;

    //OnMyName function will be invoked on clients when server change the value of playerName
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";
    [SyncVar(hook = "OnMyColor")]
    public Color playerColor = Color.white;
    [SyncVar(hook = "OnMyShip")]
    public Enumerations.E_SHIPS playerShip = Enumerations.E_SHIPS.FLYER;
    [SyncVar(hook = "OnTrack")]
    public string track = "Test";


    public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
    public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

    static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
    static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
    static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
    static Color TransparentColor = new Color(0, 0, 0, 0);

    //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
    //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

    public void Awake() {
        //DontDestroyOnLoad(transform.gameObject);
    }

    public override void OnClientEnterLobby() {
        base.OnClientEnterLobby();

        if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

        LobbyPlayerList._instance.AddPlayer(this);
        LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

        // El circuito solo se muestra en el prefab del servidor
        trackDisplayButton.gameObject.SetActive(this == LobbyPlayerList._instance._players[0]);

        if (isLocalPlayer) {
            SetupLocalPlayer();
        }
        else {
            SetupOtherPlayer();
        }

        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        OnMyName(playerName);
        OnMyColor(playerColor);
        OnMyShip(playerShip);
        OnTrack(track);
    }

    public override void OnStartAuthority() {
        base.OnStartAuthority();

        //if we return from a game, color of text can still be the one for "Ready"
        //readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        readyDisplayButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

        SetupLocalPlayer();
    }

    void ChangeReadyButtonColor(Color c) {
        ColorBlock b = readyDisplayButton.colors;
        b.normalColor = c;
        b.pressedColor = c;
        b.highlightedColor = c;
        b.disabledColor = c;
        readyDisplayButton.colors = b;
    }

    void SetupOtherPlayer() {
        nameInput.interactable = false;
        removePlayerButton.interactable = NetworkServer.active;

        ChangeReadyButtonColor(NotReadyColor);

        readyDisplayButton.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyDisplayButton.interactable = false;

        OnClientReady(false);
    }

    void SetupLocalPlayer() {
        nameInput.interactable = true;
        remoteIcone.gameObject.SetActive(false);
        localIcone.gameObject.SetActive(true);

        readyButton.gameObject.SetActive(true);
        shipButton.gameObject.SetActive(true);
        trackButton.gameObject.SetActive(true);
        colorButton.gameObject.SetActive(true);

        // Color de la fola del jugador local
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.7f);

        int index = 0;
        for (int i = 0; i < LobbyPlayerList._instance._players.Count; i++) {
            if (LobbyPlayerList._instance._players[i] == this) {
                break;
            }
            index++;
        }

        float diff = LobbyPlayerList._instance._players[0].gameObject.GetComponent<RectTransform>().anchoredPosition.y - gameObject.GetComponent<RectTransform>().anchoredPosition.y;
        Debug.Log(LobbyPlayerList._instance._players[0].GetComponent<RectTransform>().localPosition);
        Debug.Log(GetComponent<RectTransform>().localPosition);

        readyButton.gameObject.transform.position = new Vector3(readyButton.gameObject.transform.position.x,
            readyButton.gameObject.transform.position.y + 50 * index,
            readyButton.gameObject.transform.position.z);
        shipButton.gameObject.transform.position = new Vector3(shipButton.gameObject.transform.position.x,
            shipButton.gameObject.transform.position.y + 50 * index,
            shipButton.gameObject.transform.position.z);
        trackButton.gameObject.transform.position = new Vector3(trackButton.gameObject.transform.position.x,
            trackButton.gameObject.transform.position.y + 50 * index,
            trackButton.gameObject.transform.position.z);
        colorButton.gameObject.transform.position = new Vector3(colorButton.gameObject.transform.position.x,
            colorButton.gameObject.transform.position.y + 50 * index,
            colorButton.gameObject.transform.position.z); CheckRemoveButton();

        CheckRemoveButton();

        if (playerColor == Color.white)
            CmdColorChange();

        ChangeReadyButtonColor(JoinColor);

        readyDisplayButton.transform.GetChild(0).GetComponent<Text>().text = "NOT READY";
        readyButton.interactable = true;

        //have to use child count of player prefab already setup as "this.slot" is not set yet
        if (playerName == "")
            CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));

        //we switch from simple name display to name input
        colorButton.interactable = true;
        nameInput.interactable = true;
        shipButton.interactable = true;

        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener(OnNameChanged);

        colorButton.onClick.RemoveAllListeners();
        colorButton.onClick.AddListener(OnColorClicked);

        shipButton.onClick.RemoveAllListeners();
        shipButton.onClick.AddListener(OnShipClicked);

        trackButton.onClick.RemoveAllListeners();
        trackButton.onClick.AddListener(OnTrackClicked);

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);

        //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
        //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
        if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
    }

    //This enable/disable the remove button depending on if that is the only local player or not
    public void CheckRemoveButton() {
        if (!isLocalPlayer)
            return;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

        removePlayerButton.interactable = localPlayerCount > 1;
    }

    public override void OnClientReady(bool readyState) {
        if (readyState) {
            ChangeReadyButtonColor(TransparentColor);

            Text textComponent = readyDisplayButton.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = "READY";
            Text textComponent1 = readyButton.transform.GetChild(0).GetComponent<Text>();
            textComponent1.text = "NOT READY";
            textComponent.color = ReadyColor;
            //readyButton.interactable = false;
            colorButton.interactable = false;
            shipButton.interactable = false;
            trackButton.interactable = false;
            nameInput.interactable = false;
            LobbyPlayerList._instance.addButtonRow.gameObject.GetComponent<Button>().interactable = false;
        }
        else {
            ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

            Text textComponent = readyDisplayButton.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = isLocalPlayer ? "JOIN" : "...";
            Text textComponent1 = readyButton.transform.GetChild(0).GetComponent<Text>();
            textComponent1.text = "READY";
            textComponent.color = Color.white;
            readyButton.interactable = isLocalPlayer;
            colorButton.interactable = isLocalPlayer;
            shipButton.interactable = isLocalPlayer;
            trackButton.interactable = isServer;
            nameInput.interactable = isLocalPlayer;
            LobbyPlayerList._instance.addButtonRow.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void OnPlayerListChanged(int idx) {
        //GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
    }

    ///===== callback from sync var

    public void OnMyName(string newName) {
        playerName = newName;
        nameInput.text = playerName;
    }

    public void OnMyColor(Color newColor) {
        playerColor = newColor;
        colorDisplayButton.GetComponent<Image>().color = newColor;
    }

    public void OnMyShip(Enumerations.E_SHIPS newShip) {
        playerShip = newShip;
        shipDisplayButton.transform.GetChild(0).GetComponent<Text>().text = newShip.ToString();
    }

    public void OnTrack(string newTrack) {
        track = newTrack;
        trackDisplayButton.transform.GetChild(0).GetComponent<Text>().text = newTrack.ToString();
    }

    //===== UI Handler

    //Note that those handler use Command function, as we need to change the value on the server not locally
    //so that all client get the new value throught syncvar
    public void OnColorClicked() {
        CmdColorChange();
    }

    public void OnShipClicked() {
        CmdShìpChange();
    }

    public void OnTrackClicked() {
        CmdTrackChange();
        //LobbyPlayerList._instance.animManager.StartAnimation(LobbyPlayerList._instance.trackMenu);
        //CmdTrackChange();
    }

    public void OnReadyClicked() {
        if (!readyToBegin)
            SendReadyToBeginMessage();
        else
            SendNotReadyToBeginMessage();
    }

    public void OnNameChanged(string str) {
        CmdNameChanged(str);
    }

    public void OnRemovePlayerClick() {
        if (isLocalPlayer) {
            RemovePlayer();
        }
        else if (isServer)
            LobbyManager.s_Singleton.KickPlayer(connectionToClient);

    }

    public void ToggleJoinButton(bool enabled) {
        //readyButton.gameObject.SetActive(enabled);
        waitingPlayerButton.gameObject.SetActive(!enabled);
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int countdown) {
        LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
        LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
    }

    [ClientRpc]
    public void RpcUpdateRemoveButton() {
        CheckRemoveButton();
    }

    //====== Server Command

    [Command]
    public void CmdColorChange() {
        int idx = System.Array.IndexOf(Colors, playerColor);

        int inUseIdx = _colorInUse.IndexOf(idx);

        if (idx < 0) idx = 0;

        idx = (idx + 1) % Colors.Length;

        bool alreadyInUse = false;

        do {
            alreadyInUse = false;
            for (int i = 0; i < _colorInUse.Count; ++i) {
                if (_colorInUse[i] == idx) {//that color is already in use
                    alreadyInUse = true;
                    idx = (idx + 1) % Colors.Length;
                }
            }
        }
        while (alreadyInUse);

        if (inUseIdx >= 0) {//if we already add an entry in the colorTabs, we change it
            _colorInUse[inUseIdx] = idx;
        }
        else {//else we add it
            _colorInUse.Add(idx);
        }

        playerColor = Colors[idx];
    }

    [Command]
    public void CmdShìpChange() {
        int shipCount = System.Enum.GetNames(typeof(Enumerations.E_SHIPS)).Length;
        if ((int)playerShip + 1 >= shipCount) {
            playerShip = 0;
        }
        else {
            playerShip += 1;
        }
    }

    [Command]
    public void CmdTrackChange() {
        switch(track) {
            case "Test":
                track = "Blood Dragon";
                break;
            case "Blood Dragon":
                track = "Test";
                break;
        }
    }

    [Command]
    public void CmdNameChanged(string name) {
        playerName = name;
    }

    //Cleanup thing when get destroy (which happen when client kick or disconnect)
    public void OnDestroy() {
        LobbyPlayerList._instance.RemovePlayer(this);
        if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

        int idx = System.Array.IndexOf(Colors, playerColor);

        if (idx < 0)
            return;

        for (int i = 0; i < _colorInUse.Count; ++i) {
            if (_colorInUse[i] == idx) {//that color is already in use
                _colorInUse.RemoveAt(i);
                break;
            }
        }
    }
}