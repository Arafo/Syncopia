using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class MusicManager : MonoBehaviour {

    public AudioSource source;
    public AudioClip clip;

    public ShipReferer ship;

	// Use this for initialization
	void Start () {
        source = gameObject.AddComponent<AudioSource>();
        AudioSettings.LoadMusic();
    }

    // Update is called once per frame
    void Update () {
        if (GameSettings.isPaused && source.isPlaying)
            source.Pause();
        else if (!GameSettings.isPaused && !source.isPlaying)
            source.UnPause();

        if ((!source.isPlaying && !GameSettings.isPaused) || Input.GetKeyDown(KeyCode.Period)) {
            // Cargar cancion
            if (RaceSettings.countdownFinished)
                StartCoroutine(LoadSongCoroutine());
        }

        source.volume = AudioSettings.VOLUME_MUSIC;
    }

    IEnumerator LoadSongCoroutine() {
        if (AudioSettings.customMusicEnabled) {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory + "\\UserData\\Music\\", "*.mp3");
            int random = UnityEngine.Random.Range(0, files.Length);

            WWW www = new WWW("file:///" + files[random]);
            while (!www.isDone) {

            }
            AudioClip clip = www.GetAudioClip(false, false, AudioType.MPEG);
            source.clip = clip;
            source.Play();
        }

        {
            int rand = UnityEngine.Random.Range(0, AudioSettings.musicLocations.Length);
            ResourceRequest request = Resources.LoadAsync(AudioSettings.musicLocations[rand]);
            yield return request;

            source.clip = request.asset as AudioClip;
            source.Play();
        }
    }
}
