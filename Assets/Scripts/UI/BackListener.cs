using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class BackListener : MonoBehaviour {

    [Header("[ REFERENCIAS ]")]
    public MenuAnimationManager animManager;
    public GameObject toPanel;
    public bool usePrevious;
    public Selectable lastSelected;

    public bool useNext;
    public GameObject nextPanel;
    public GameObject fromPanel;
    public bool isRoot;

    public GameObject[] onBackDisable;
    public GameObject[] onBackEnable;

    public void SetNextPanel(GameObject next) {
        nextPanel = next;
        useNext = true;
    }

    public void GotoNextPanel() {
        if (useNext && nextPanel != null)
            animManager.StartAnimation(nextPanel);
    }

    public void GotoPreviousPanel() {
        if (fromPanel != null)
            animManager.StartAnimationBackPedal(fromPanel);
    }

    void Update() {
        /*if (animManager == null) {
            animManager = GameObject.Find("AnimationManager").GetComponent<MenuAnimationManager>();
        }*/

        /*if (ReInput.players.GetPlayer(0).GetButtonDown("Cancel")) {
            if (isRoot)
                return;

            for (int i = 0; i < onBackDisable.Length; ++i)
                onBackDisable[i].SetActive(false);

            for (int i = 0; i < onBackEnable.Length; ++i)
                onBackEnable[i].SetActive(true);

            if (!animManager.isAnimating) {
                if (usePrevious)
                    animManager.StartAnimationBackPedal(fromPanel);
                else
                    animManager.StartAnimation(toPanel);
            }
        }*/
    }
}
