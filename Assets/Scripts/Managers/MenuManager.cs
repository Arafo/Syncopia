using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// </summary>
public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Inicia la escena Test
    /// </summary>
    public void StartRace() {
        SceneManager.LoadScene("Test");
    }

    /// <summary>
    ///  Cierra la aplicación
    /// </summary>
    public void Quit() {
        Application.Quit();
    }

    public float sec = 0.1f;
    /// <summary>
    /// Espera sec para activar el objeto gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    public void WaitForActivate(GameObject gameObject) {
        StartCoroutine(LateCall(gameObject));
    }

    /// <summary>
    /// Coroutine para activar el objeto gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    IEnumerator LateCall(GameObject gameObject) {

        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }
}
