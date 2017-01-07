using UnityEngine;
using System.IO;
using System;

public class GameSettings : MonoBehaviour {

    // Configuracion grafica
    public static Vector2 GS_RESOLUTION = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    public static int GS_FRAMECAP = 60;
    public static int GS_DRAWDISTANCE = 60;
    public static bool GS_FULLSCREEN = false;
    public static int GS_BLOOM = 1;
    public static int GS_DYNAMICRESOLUTION = 0;
    public static int GS_AA = 0;
    public static bool GS_AO = false;
    public static bool GS_CAMERADAMAGE = false;
    public static bool GS_TONEMAPPING = false;

    // Configuracion de los controles
    public static bool IN_VIBRATION = true;
    public static KeyCode IN_KB_THRUST = KeyCode.Space;
    public static KeyCode IN_GP_THRUST;
    public static KeyCode IN_KB_CAMERA = KeyCode.C;
    public static KeyCode IN_GP_CAMERA;
    public static KeyCode IN_KB_LOOKBEHIND = KeyCode.Q;
    public static KeyCode IN_GP_LOOKBEHIND;
    public static KeyCode IN_KB_AFTERBURN = KeyCode.E;
    public static KeyCode IN_GP_AFTERBURN;
    public static KeyCode IN_KB_AIRBRAKE_LEFT = KeyCode.A;
    public static KeyCode IN_KB_AIRBRAKE_RIGHT = KeyCode.D;
    public static KeyCode IN_GP_AIRBRAKE_LEFT;
    public static KeyCode IN_GP_AIRBRAKE_RIGHT;
    public static KeyCode IN_KB_PAUSE = KeyCode.Escape;
    public static KeyCode IN_GP_PAUSE;

    public static KeyCode IN_KB_STEER_LEFT = KeyCode.LeftArrow;
    public static KeyCode IN_KB_STEER_RIGHT = KeyCode.RightArrow;
    public static KeyCode IN_KB_PITCH_UP = KeyCode.UpArrow;
    public static KeyCode IN_KB_PITCH_DOWN = KeyCode.DownArrow;

    public static float steerDeadZone;
    public static float pitchDeadZone;
    public static float leftAirbrakeDeadZone;
    public static float rightAirbrakeDeadZone;

    // Configuracion del juego
    public static Color G_CUSTOMHUDCOLOR; // HUD color
    public static int G_DEFAULTCAMERA = 0;
    public static int G_COUNTDOWNTYPE = 0; 
    public static bool G_TRACKINTROVOICES = true;
    public static bool G_MIRROR = false;
    public static Enumerations.E_LANGUAGE G_LANGUAGE = Enumerations.E_LANGUAGE.ENGLISH;

    // Trucos (disables progressing)
    public static bool C_SUPERTHRUST = false; // Boost x4
    public static bool C_GODTHRUST = false; // Boost x4 y siempre activo
    public static bool C_GRIP = false; // Todas las naves tienen 8 de grip
    public static bool C_GLIDE = false;

    public static bool optionsClose = false;
    public static string profileName = "Developer";

    // Pausa
    public static bool isPaused;

    public static void PauseToggle() {
        if (isPaused) {
            isPaused = false;
            Time.timeScale = 1.0f;
        }
        else if (!isPaused) {
            isPaused = true;
            Time.timeScale = !ServerSettings.isNetworked ? 0.0f : 1f;
        }
    }

    public static void PauseToggle(bool state) {
        isPaused = state;
        if (isPaused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public static string GetDirectory() {
        string path = Environment.CurrentDirectory + "/UserData/";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!Directory.Exists(path + "/Music/"))
            Directory.CreateDirectory(path + "/Music/");

        return path;
    }


    public static void CapFPS(int cap) {
        Application.targetFrameRate = cap;
    }

    public static void SetScreen() {
        Screen.SetResolution(Mathf.RoundToInt(GS_RESOLUTION.x), Mathf.RoundToInt(GS_RESOLUTION.y), GS_FULLSCREEN, GS_FRAMECAP);
    }

    public static int AutoQualitySettings() {
        int shaderLevel = SystemInfo.graphicsShaderLevel;
        //int fillrate = SystemInfo.graphicsPixelFillrate;
        int vram = SystemInfo.graphicsMemorySize;
        int cpus = SystemInfo.processorCount;

        int score = 0;

        // Shaders
        if (shaderLevel < 10)
            score = 1000;
        else if (shaderLevel < 20)
            score = 1300;
        else if (shaderLevel < 30)
            score = 2000;
        else
            score = 3000;

        // CPU
        if (cpus >= 8)
            score *= 3;
        else if (cpus <= 4)
            score *= 2;
        else if (cpus <= 2)
            score *= 1;

        // VRAM
        if (vram >= 2000)
            score *= 4;
        if (vram <= 1024)
            score *= 2;
        if (vram <= 512)
            score *= 1;
        else if (vram <= 128)
            score /= 2;

        Debug.Log(score);
        Console.print(score);
        // Min Score -> 500
        // Max Score -> 36000
        // Nivel 1 -> 12000, Nivel 2 -> 24000, Nivel 3 -> 32000
        return score;

    }
}
