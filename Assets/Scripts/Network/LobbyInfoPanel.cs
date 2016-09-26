using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyInfoPanel : MonoBehaviour {
    public Text infoText;
    public Text buttonText;
    public Button singleButton;

    public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk) {
        infoText.text = info;

        if (singleButton != null) {

            buttonText.text = buttonInfo;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null) {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { gameObject.SetActive(false); });
        }

        gameObject.SetActive(true);
    }
}
