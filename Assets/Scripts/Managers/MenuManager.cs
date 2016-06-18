using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartRace() {
        SceneManager.LoadScene("Test");
    }

    public void Quit() {
        Application.Quit();
    }

    public float sec = 0.1f;
    public void WaitForActivate(GameObject gameObject) {
        StartCoroutine(LateCall(gameObject));
    }

    IEnumerator LateCall(GameObject gameObject) {

        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }



}
