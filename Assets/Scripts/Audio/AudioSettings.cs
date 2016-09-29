using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AudioSettings {
    // Configuracion del audio
    public static float VOLUME_MAIN = 1.0f;
    public static float VOLUME_MUSIC = 0.7f;
    public static float VOLUME_SFX = 1.0f;
    public static float VOLUME_VOICES = 1.0f;
    public static string[] musicLocations;

    public static bool customMusicEnabled = false;
    public static AudioManager manager;

    public static void LoadMusic() {
        TextAsset musicList = Resources.Load("music") as TextAsset;
        List<string> musicLocs = new List<string>();
        using (StringReader sr = new StringReader(musicList.ToString())) {
            string newLine = sr.ReadLine();

            while (newLine != null) {
                musicLocs.Add(newLine);
                newLine = sr.ReadLine();
            }
        }

        musicLocations = musicLocs.ToArray();
    }

    public static void RegisterClip(AudioSource source) {
        if (manager == null) {
        }

        manager.RegisterClip(source);
    }
}