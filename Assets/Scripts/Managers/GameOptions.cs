using UnityEngine;
using System.IO;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la lectura y escritura de los ajustes de las 
/// opciones en el fichero de configuración
/// </summary>
public class GameOptions : MonoBehaviour {

    /// <summary>
    /// Guarda los ajustes de las opciones
    /// </summary>
    public static void SaveGameSettings() {
        INIParser ini = new INIParser();
        ini.Open(GameSettings.GetDirectory() + "settings.ini"); {
            ini.WriteValue("Display", "Screen Width", GameSettings.GS_RESOLUTION.x);
            ini.WriteValue("Display", "Screen Height", GameSettings.GS_RESOLUTION.y);
            ini.WriteValue("Display", "Fullscreen", GameSettings.GS_FULLSCREEN);
            ini.WriteValue("Display", "Framecap", GameSettings.GS_FRAMECAP);

            ini.WriteValue("Rendering", "Draw Distance", GameSettings.GS_DRAWDISTANCE);
            ini.WriteValue("Rendering", "Bloom", GameSettings.GS_BLOOM);
            ini.WriteValue("Rendering", "Dynamic Resolution", GameSettings.GS_DYNAMICRESOLUTION);
            ini.WriteValue("Rendering", "AA", GameSettings.GS_AA);
            ini.WriteValue("Rendering", "AO", GameSettings.GS_AO);
            ini.WriteValue("Rendering", "CAMDMG", GameSettings.GS_CAMERADAMAGE);
            ini.WriteValue("Rendering", "TONEMAP", GameSettings.GS_TONEMAPPING);

            ini.WriteValue("Audio", "Master Volume", AudioSettings.VOLUME_MAIN);
            ini.WriteValue("Audio", "SFX Volume", AudioSettings.VOLUME_SFX);
            ini.WriteValue("Audio", "Voices Volume", AudioSettings.VOLUME_VOICES);
            ini.WriteValue("Audio", "Music Volume", AudioSettings.VOLUME_MUSIC);
            ini.WriteValue("Audio", "Custom Music", AudioSettings.customMusicEnabled);

            ini.WriteValue("Gameplay", "Default Camera", GameSettings.G_DEFAULTCAMERA);
            ini.WriteValue("Gameplay", "Countdown Mode", GameSettings.G_COUNTDOWNTYPE);
            ini.WriteValue("Gameplay", "Intro Voices", GameSettings.G_TRACKINTROVOICES);
            ini.WriteValue("Gameplay", "Mirror", GameSettings.G_MIRROR);
            ini.WriteValue("Gameplay", "Language", (int)GameSettings.G_LANGUAGE);
            ini.WriteValue("Gameplay", "Custom HUD Color R", GameSettings.G_CUSTOMHUDCOLOR.r);
            ini.WriteValue("Gameplay", "Custom HUD Color B", GameSettings.G_CUSTOMHUDCOLOR.b);
            ini.WriteValue("Gameplay", "Custom HUD Color G", GameSettings.G_CUSTOMHUDCOLOR.g);
            ini.WriteValue("Gameplay", "Custom HUD Color A", GameSettings.G_CUSTOMHUDCOLOR.a);

            ini.WriteValue("Control Settings", "Vibration", GameSettings.IN_VIBRATION);
            ini.WriteValue("Control Settings", "Analog Steer Deadzone", GameSettings.steerDeadZone);
            ini.WriteValue("Control Settings", "Analog Pitch Deadzone", GameSettings.pitchDeadZone);

            ini.WriteValue("Controls", "Thrust 1", (int)GameSettings.IN_KB_THRUST);
            ini.WriteValue("Controls", "Thrust 2", (int)GameSettings.IN_GP_THRUST);
            ini.WriteValue("Controls", "Camera 1", (int)GameSettings.IN_KB_CAMERA);
            ini.WriteValue("Controls", "Camera 2", (int)GameSettings.IN_GP_CAMERA);
            ini.WriteValue("Controls", "Look Behind 1", (int)GameSettings.IN_KB_LOOKBEHIND);
            ini.WriteValue("Controls", "Look Behind 2", (int)GameSettings.IN_GP_LOOKBEHIND);
            ini.WriteValue("Controls", "Afterburner 1", (int)GameSettings.IN_KB_AFTERBURN);
            ini.WriteValue("Controls", "Afterburner 2", (int)GameSettings.IN_GP_AFTERBURN);
            ini.WriteValue("Controls", "Airbrake Left 1", (int)GameSettings.IN_KB_AIRBRAKE_LEFT);
            ini.WriteValue("Controls", "Airbrake Left 2", (int)GameSettings.IN_GP_AIRBRAKE_LEFT);
            ini.WriteValue("Controls", "Airbrake Right 1", (int)GameSettings.IN_KB_AIRBRAKE_RIGHT);
            ini.WriteValue("Controls", "Airbrake Right 2", (int)GameSettings.IN_GP_AIRBRAKE_RIGHT);
            ini.WriteValue("Controls", "Pause 1", (int)GameSettings.IN_KB_PAUSE);
            ini.WriteValue("Controls", "Pause 2", (int)GameSettings.IN_GP_PAUSE);
            ini.WriteValue("Controls", "Steer -1", (int)GameSettings.IN_KB_STEER_LEFT);
            ini.WriteValue("Controls", "Steer 1", (int)GameSettings.IN_KB_STEER_RIGHT);
            ini.WriteValue("Controls", "Pitch -1", (int)GameSettings.IN_KB_PITCH_UP);
            ini.WriteValue("Controls", "Pitch 1", (int)GameSettings.IN_KB_PITCH_DOWN);
        }
        ini.Close();
    }

    /// <summary>
    /// Carga los ajustes de las opciones
    /// </summary>
    public static void LoadGameSettings() {
        if (!File.Exists(GameSettings.GetDirectory() + "settings.ini"))
            SaveGameSettings();

        INIParser ini = new INIParser();
        ini.Open(GameSettings.GetDirectory() + "settings.ini"); {
            GameSettings.GS_RESOLUTION.x = ini.ReadValue("Display", "Screen Width", Screen.width);
            GameSettings.GS_RESOLUTION.y = ini.ReadValue("Display", "Screen Height", Screen.height);
            GameSettings.GS_FULLSCREEN = ini.ReadValue("Display", "Fullscreen", GameSettings.GS_FULLSCREEN);
            GameSettings.GS_FRAMECAP = ini.ReadValue("Display", "Framecap", GameSettings.GS_FRAMECAP);

            GameSettings.GS_DRAWDISTANCE = ini.ReadValue("Rendering", "Draw Distance", GameSettings.GS_DRAWDISTANCE);
            GameSettings.GS_BLOOM = ini.ReadValue("Rendering", "Bloom", GameSettings.GS_BLOOM);
            GameSettings.GS_DYNAMICRESOLUTION = ini.ReadValue("Rendering", "Dynamic Resolution", GameSettings.GS_DYNAMICRESOLUTION);
            GameSettings.GS_AA = ini.ReadValue("Rendering", "AA", GameSettings.GS_AA);
            GameSettings.GS_AO = ini.ReadValue("Rendering", "AO", GameSettings.GS_AO);
            GameSettings.GS_CAMERADAMAGE = ini.ReadValue("Rendering", "CAMDMG", GameSettings.GS_CAMERADAMAGE);
            GameSettings.GS_TONEMAPPING = ini.ReadValue("Rendering", "TONEMAP", GameSettings.GS_TONEMAPPING);

            AudioSettings.VOLUME_MAIN = (float)ini.ReadValue("Audio", "Master Volume", AudioSettings.VOLUME_MAIN);
            AudioSettings.VOLUME_SFX = (float)ini.ReadValue("Audio", "SFX Volume", AudioSettings.VOLUME_SFX);
            AudioSettings.VOLUME_VOICES = (float)ini.ReadValue("Audio", "Voices Volume", AudioSettings.VOLUME_VOICES);
            AudioSettings.VOLUME_MUSIC = (float)ini.ReadValue("Audio", "Music Volume", AudioSettings.VOLUME_MUSIC);

            AudioSettings.customMusicEnabled = ini.ReadValue("Audio", "Custom Music", AudioSettings.customMusicEnabled);

            GameSettings.IN_VIBRATION = ini.ReadValue("Control Settings", "Vibration", GameSettings.IN_VIBRATION);

            GameSettings.G_DEFAULTCAMERA = ini.ReadValue("Gameplay", "Default Camera", GameSettings.G_DEFAULTCAMERA);
            GameSettings.G_COUNTDOWNTYPE = ini.ReadValue("Gameplay", "Countdown Mode", GameSettings.G_COUNTDOWNTYPE);
            GameSettings.G_TRACKINTROVOICES = ini.ReadValue("Gameplay", "Intro Voices", GameSettings.G_TRACKINTROVOICES);
            GameSettings.G_MIRROR = ini.ReadValue("Gameplay", "Mirror", GameSettings.G_MIRROR);
            GameSettings.G_LANGUAGE = (Enumerations.E_LANGUAGE)ini.ReadValue("Gameplay", "Language", (int)GameSettings.G_LANGUAGE);

            GameSettings.G_CUSTOMHUDCOLOR.r = (float)ini.ReadValue("Gameplay", "Custom HUD Color R", GameSettings.G_CUSTOMHUDCOLOR.r);
            GameSettings.G_CUSTOMHUDCOLOR.g = (float)ini.ReadValue("Gameplay", "Custom HUD Color G", GameSettings.G_CUSTOMHUDCOLOR.g);
            GameSettings.G_CUSTOMHUDCOLOR.b = (float)ini.ReadValue("Gameplay", "Custom HUD Color B", GameSettings.G_CUSTOMHUDCOLOR.b);
            GameSettings.G_CUSTOMHUDCOLOR.a = (float)ini.ReadValue("Gameplay", "Custom HUD Color A", GameSettings.G_CUSTOMHUDCOLOR.a);
        }
        ini.Close();
    }
}
