using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la información del servidor al que se conecta un cliente
/// </summary>
public class LobbyTopPanel : MonoBehaviour {

    public bool isInGame = false;
    protected bool isDisplayed = true;

    void Start() {
    }

    void Update() {
        if (!isInGame)
            return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleVisibility(!isDisplayed);
        }

    }

    public void ToggleVisibility(bool visible) {
        isDisplayed = visible;
        foreach (Transform t in transform) {
            t.gameObject.SetActive(isDisplayed);
        }
    }
}
