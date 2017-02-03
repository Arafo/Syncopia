using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la carga de la partida y la pantalla 
/// que se muestra mientras se carga
/// </summary>
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

    /// <summary>
    /// Carga el nivel seleccionado de forma asincrona
    /// </summary>
    private void LoadLevel() {
        level = SceneManager.LoadSceneAsync(SceneIndexManager.SceneIndexFromName(RaceSettings.trackToLoad));
        level.allowSceneActivation = false;
        StartCoroutine(LevelCoroutine());
    }

    /// <summary>
    /// Coroutine para gestionar el proceso de carga
    /// </summary>
    /// <returns></returns>
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
        yield return new WaitForSeconds(1);
        level.allowSceneActivation = true;
    }
}
