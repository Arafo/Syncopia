using UnityEngine;
using System.Collections;

public class MusicSingleton : MonoBehaviour {

    public static MusicSingleton _instance;

    public AudioClip musicMainMenu;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        else {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded(int level) {
        if (SceneIndexManager.SceneIndexFromName("Menu") == level 
            || SceneIndexManager.SceneIndexFromName("Online") == level) {
            AudioSource audio = GetComponent<AudioSource>();
            if (!audio.isPlaying) {
                audio.Stop();
                //GetComponent<AudioSource>().clip = musicMainMenu;
                audio.Play();
            }
        }
        else {
            GetComponent<AudioSource>().Stop();
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}