using UnityEngine;
using System.Collections;
using System;

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

    public static AudioSource CreateClip(AudioClip clip, float volume) {
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1.0f;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;
        source.Play();

        // Registrar la fuente de sonido
        AudioSettings.RegisterClip(source);

        return source;
    }

    public static AudioSource CreateClip(AudioClip clip, float volume, float pitch) {
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;
        source.Play();

        // Registrar la fuente de sonido
        AudioSettings.RegisterClip(source);

        return source;
    }

    public static AudioSource CreateClip(AudioClip clip, Transform parent, float volume, float pitch, float minDistance, float maxDistance) {
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.maxDistance = maxDistance;
        source.minDistance = minDistance;
        source.gameObject.name = clip.name;
        source.spatialBlend = 1.0f;

        source.transform.parent = parent;
        source.transform.localPosition = Vector3.zero;
        source.Play();

        // Registrar la fuente de sonido
        AudioSettings.RegisterClip(source);

        return source;
    }
}
