using UnityEngine;
using System.Collections;
using System.IO;

public static class LanguageSingleton {

    public static LanguageManager _instance;
    private static string file = "language";

    public static void InstanceLanguage(bool newLang = false) {
        if (_instance == null) {
            _instance = new LanguageManager(file, GameSettings.G_LANGUAGE.ToDescription());
        }

        // Cambio de idioma
        if (newLang && _instance != null)
            _instance.setLanguage(file, GameSettings.G_LANGUAGE.ToDescription());
    }
}