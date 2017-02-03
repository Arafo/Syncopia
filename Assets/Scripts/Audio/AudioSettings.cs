using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la lectura y el almacenamiento de la lista de canciones que se
/// reproducen durante las carreras. También proporciona acceso al gestor 
/// que se encarga de reproducir la música en las carreras.
/// </summary>
public class AudioSettings {
    // Configuracion del audio
    public static float VOLUME_MAIN = 1.0f;
    public static float VOLUME_MUSIC = 0.7f;
    public static float VOLUME_SFX = 1.0f;
    public static float VOLUME_VOICES = 1.0f;
    public static string[] musicLocations;

    public static bool customMusicEnabled = false;
    public static AudioManager manager;

    /// <summary>
    /// Almacena en un array de strings la ruta donde se encuentra
    /// la música que se va a reproducir durante las carreras
    /// </summary>
    public static void LoadMusic() {
        TextAsset musicList = Resources.Load("music") as TextAsset;
        List<string> musicLocs = new List<string>();
        using (StringReader sr = new StringReader(musicList.ToString())) {
            string newLine = sr.ReadLine();

            while (newLine != null) {
                if (!newLine.StartsWith("#"))
                    musicLocs.Add(newLine);
                newLine = sr.ReadLine();
            }
        }

        musicLocations = musicLocs.ToArray();
    }

    /// <summary>
    /// Registra una fuente de audio en el manager
    /// </summary>
    /// <param name="source"></param>
    public static void RegisterClip(AudioSource source) {
        if (manager == null) {
            // Crea el manager del sonido si no existe
            GameObject audioManager = new GameObject("MANAGER_AUDIO");
            manager = audioManager.AddComponent<AudioManager>();
        }

        manager.RegisterClip(source);
    }
}