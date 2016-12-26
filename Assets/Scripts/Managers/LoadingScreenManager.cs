using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour {

    private AsyncOperation level;

    [Multiline]
    public string[] hints;
    public Text shipDesc;
    public Image loadingBarMask;

    // Use this for initialization
    void Start() {

        // Consejos
        int rand = Random.Range(0, hints.Length);
        shipDesc.text = hints[rand];

        // Si no hay ningun circuito se carga el circuito de Test
        if (RaceSettings.trackToLoad == null || RaceSettings.trackToLoad == "")
            RaceSettings.trackToLoad = "Blood Dragon";

        LoadLevel();
    }

    private void LoadLevel() {
        level = SceneManager.LoadSceneAsync(SceneIndexManager.SceneIndexFromName(RaceSettings.trackToLoad));
        level.allowSceneActivation = false;
        StartCoroutine(LevelCoroutine());
    }

    IEnumerator LevelCoroutine() {

        while (!level.isDone) {
            loadingBarMask.fillAmount = level.progress;

            // El progreso siempre para en 0.9
            if (level.progress >= 0.9f) {
                break;
            }

            yield return null;
        }

        loadingBarMask.fillAmount = 1.0f;
        yield return new WaitForSeconds(1); // Para probar que funciona, quitarlo para niveles más complejos
        level.allowSceneActivation = true;
    }
}
