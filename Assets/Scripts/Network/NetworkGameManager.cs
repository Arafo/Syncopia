using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkGameManager : NetworkBehaviour {

    static public NetworkGameManager s_Instance;

    public RaceManager manager;
    public bool allPlayersSpawned;

    void Awake() {
        s_Instance = this;
    }

    [ServerCallback]
    void Start() {
        StartCoroutine(GameLoop());
    }


    /// <summary>
    /// Add a ship from the lobby hook
    /// </summary>
    /// <param name="ship">The actual GameObject instantiated by the lobby, which is a NetworkBehaviour</param>
    /// <param name="playerNum">The number of the player (based on their slot position in the lobby)</param>
    /// <param name="c">The color of the player, choosen in the lobby</param>
    /// <param name="name">The name of the Player, choosen in the lobby</param>
    /// <param name="localID">The localID. e.g. if 2 player are on the same machine this will be 1 & 2</param>
    static public void AddShip(GameObject ship, int playerNum, Color c, string name, int localID) {
        NetworkedPlayer tmp = new NetworkedPlayer();
        tmp.m_Instance = ship;
        tmp.m_PlayerNumber = playerNum;
        tmp.m_PlayerColor = c;
        tmp.m_PlayerName = name;
        tmp.m_LocalPlayerID = localID;
        tmp.Setup();

        ServerSettings.players.Add(tmp);
        //m_Ships.Add(tmp);
    }

    public void RemoveShip(GameObject ship) {
        NetworkedPlayer toRemove = null;
        foreach (var tmp in ServerSettings.players) {
            if (tmp.m_Instance == ship) {
                toRemove = tmp;
                break;
            }
        }

        if (toRemove != null) {
            ServerSettings.players.Remove(toRemove);
            // Se destruye el onjeto del  jugador si se desconecta
            Destroy(ship.transform.parent.gameObject.transform.parent.gameObject);
        }
    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop() {
        while (ServerSettings.players.Count < 1)
            yield return null;

        //wait to be sure that all are ready to start
        yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(Setup());

        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        //yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine.
        yield return StartCoroutine(RoundEnding());

        yield return StartCoroutine(NextRound());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if there is a winner of the game.
        /*if (m_GameWinner != null) {// If there is a game winner, wait for certain amount or all player confirmed to start a game again
            m_GameIsFinished = true;
            float leftWaitTime = 15.0f;
            bool allAreReady = false;
            int flooredWaitTime = 15;

            while (leftWaitTime > 0.0f && !allAreReady) {
                yield return null;

                allAreReady = true;
                foreach (var tmp in m_Tanks) {
                    allAreReady &= tmp.IsReady();
                }

                leftWaitTime -= Time.deltaTime;

                int newFlooredWaitTime = Mathf.FloorToInt(leftWaitTime);

                if (newFlooredWaitTime != flooredWaitTime) {
                    flooredWaitTime = newFlooredWaitTime;
                    string message = EndMessage(flooredWaitTime);
                    RpcUpdateMessage(message);
                }
            }

            LobbyManager.s_Singleton.ServerReturnToLobby();
        }
        else {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }*/

        // countdown if server
        //if (ServerSettings.playerFinished) {
        //ServerSettings.raceCountdown -= Time.deltaTime;
        //RpcFinishCountdown(ServerSettings.raceCountdown);
        //}
    }

    private IEnumerator Setup() {
        //we notify all clients that the round is starting
        RpcSetup();

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return 3;
    }

    [ClientRpc]
    void RpcSetup() {
        // Se configura el jugador local
        for (int i = 0; i < ServerSettings.players.Count; ++i) {
            ServerSettings.players[i].Config(i);
        }

        // Se configuran los jugadores remotos
        for (int i = 0; i < ServerSettings.players.Count; ++i) {
            ServerSettings.players[i].ConfigRemote(i);
        }
        allPlayersSpawned = true;
    }

    private IEnumerator RoundPlaying() {
        // Se notifica a los cliente que la carrera ha empezado, ya se pueden mover
        RpcRoundPlaying();

        // Mientras no haya un ganador...
        while (!OneShipFinished()) {
            yield return null;
        }
    }

    /// <summary>
    /// Devuelve true si hay una o más naves que hayan finalizado, false en 
    /// caso contrario.
    /// </summary>
    /// <returns></returns>
    private bool OneShipFinished() {
        int numShipsFinished = 0;

        for (int i = 0; i < RaceSettings.ships.Count; i++) {
            if (RaceSettings.ships[i].finished)
                numShipsFinished++;
        }

        return numShipsFinished > 0;
    }

    private IEnumerator RoundEnding() {
        //notify client they should disable tank control
        ServerSettings.raceCountdown = 30;

        while (ServerSettings.raceCountdown > 0) {
            yield return new WaitForSeconds(1f);
            ServerSettings.raceCountdown -= 1f;
            RpcFinishCountdown(ServerSettings.raceCountdown);
            RpcRoundEnding();
        }

        RpcFinishCountdown(-1f);
        RpcRoundEnding();

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator NextRound() {
        ServerSettings.raceCountdown = 10;

        while (ServerSettings.raceCountdown > 0) {
            yield return new WaitForSeconds(1f);
            ServerSettings.raceCountdown -= 1f;
            RpcFinishCountdown(ServerSettings.raceCountdown);
        }
        RpcFinishCountdown(-1f);

        LobbyManager.s_Singleton.ServerReturnToLobby();
        LobbyManager.s_Singleton.menuManager.StartAnimation(LobbyManager.s_Singleton.lobbyPanel.gameObject);
        //NetworkManager.singleton.ServerChangeScene("Blood Dragon");

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return new WaitForSeconds(1f);
    }

    [ClientRpc]
    private void RpcRoundEnding() {
        for (int i = 0; i < ServerSettings.players.Count; i++) {
            ServerSettings.players[i].Test();
        }
    }

    [ClientRpc]
    void RpcRoundPlaying() {

    }

    [ClientRpc]
    public void RpcFinishCountdown(float value) {
        ServerSettings.raceCountdown = value;
        if (value < 0) {
            RaceSettings.raceManager.UpdateCounterResults("");
        }
        else {
            RaceSettings.raceManager.UpdateCounterResults(value.ToString());
        }
    }

    [ClientRpc]
    public void RpcSetCountdown(string value) {
        if (isServer)
            return;

        manager.UpdateCounter(value);
        switch (value) {
            case "3":
                ClipManager.CreateClip(manager.clipThree, AudioSettings.VOLUME_VOICES, 1.0f);
                break;
            case "2":
                ClipManager.CreateClip(manager.clipTwo, AudioSettings.VOLUME_VOICES, 1.0f);
                break;
            case "1":
                ClipManager.CreateClip(manager.clipOne, AudioSettings.VOLUME_VOICES, 1.0f);
                break;
            case "GO!":
                ClipManager.CreateClip(manager.clipGo, AudioSettings.VOLUME_VOICES, 1.0f);
                RaceSettings.countdownFinished = true;
                RaceSettings.shipsRestrained = false;
                break;
            default:
                break;
        }
    }
}