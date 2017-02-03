using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la reproducción de la música en los menús.
/// </summary>
public class MusicSingleton : MonoBehaviour {

    public static MusicSingleton _instance;
    public AudioClip musicMainMenu;

    /// <summary>
    /// Singleton 
    /// </summary>
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

    /// <summary>
    /// Limita la reproduccioón del clio musicMainMenu a
    /// las escenas Menu y Online
    /// </summary>
    /// <param name="level"></param>
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