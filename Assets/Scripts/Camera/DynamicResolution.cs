using UnityEngine;
using System.Collections;

public class DynamicResolution : MonoBehaviour {

    private float timer = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(Screen.currentResolution);
        timer += Time.deltaTime;

        if (timer > 3f) {
            Screen.SetResolution(1440, 900, true);
            timer = 0f;
        }
        else
            Screen.SetResolution(2880, 1800, true);

    }
}
