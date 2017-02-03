using UnityEngine;
using System.Collections;
using System;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Crea todas las fuentes de sonido que se utilizan
/// durante las partidas.
/// </summary>
public class ClipManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Crea una nueva fuente de audio
    /// </summary>
    /// <returns></returns>
    private static AudioSource CreateSource() {
        GameObject newSound = new GameObject();
        return newSound.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Crea un nuevo clip con un volumen dado
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Crea un nuevo clip con un volumen y tono dados
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public static AudioSource CreateClip(AudioClip clip, float volume, float pitch) {
        if (GameObject.Find(clip.name) != null)
            return null;

        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;

        //Debug.Log((GameObject.Find(clip.ToString()) == null) + " " + clip.ToString() );
        source.Play();

        // Registrar la fuente de sonido
        AudioSettings.RegisterClip(source);

        return source;
    }

    /// <summary>
    /// Crea un nuevo clip con un volumen, tono, ditancia minima y 
    /// distancia máxima dados
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="parent"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
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
