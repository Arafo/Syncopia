using UnityEngine;
using System.Collections;

public class ClipManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private static AudioSource CreateSource() {
        GameObject newSound = new GameObject();
        return newSound.AddComponent<AudioSource>();
    }

    public static AudioSource CreateOneShot(AudioClip clip, float volume) {
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1.0f;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;
        source.Play();

        return source;
    }
}
