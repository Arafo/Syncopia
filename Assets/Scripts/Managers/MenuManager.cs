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
}
