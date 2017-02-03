using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona el registro, la reproducción y la destrucción
/// de todos los efectos de sonido que se usan en los menús
/// y durante las carreras
/// </summary>
public class AudioManager : MonoBehaviour {

    private List<AudioSource> clips = new List<AudioSource>();
    private List<float> clipsDuration = new List<float>();
    private List<float> clipsTimers = new List<float>();

    /// <summary>
    /// Destruye kis clips de la escena cuando se acaba su duración
    /// </summary>
    void Update() {
        if (clips.Count > 0) {
            for (int i = 0; i < clips.Count; i++) {
                clipsTimers[i] += (!GameSettings.isPaused ? Time.deltaTime : Time.unscaledDeltaTime);
                if (clipsTimers[i] > clipsDuration[i]) {
                    // Borrar clip de audio
                    Destroy(clips[i].gameObject);

                    // Borrar los items de las listas referentes al clip de audio 
                    clips.Remove(clips[i]);
                    clipsDuration.Remove(clipsDuration[i]);
                    clipsTimers.Remove(clipsTimers[i]);
                }
            }
        }
    }

    /// <summary>
    /// Registra un clip para reproducirlo
    /// </summary>
    /// <param name="source"></param>
    public void RegisterClip(AudioSource source) {
        // Registrar el clip
        clips.Add(source);

        // Configurar los contadores
        clipsTimers.Add(0.0f);
        clipsDuration.Add(source.clip.length);
    }
}