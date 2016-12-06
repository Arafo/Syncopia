using UnityEngine;
using System.Collections;
using System.IO;

public static class LanguageSingleton {

    public static LanguageManager _instance;
    private static string file = "Resources/language.xml";

    public static void InstanceLanguage(bool newLang = false) {
        if (_instance == null) {
            _instance = new LanguageManager(Path.Combine(Application.dataPath, file), GameSettings.G_LANGUAGE.ToDescription());
        }

        // Cambio de idioma
        if (newLang && _instance != null)
            _instance.setLanguage(Path.Combine(Application.dataPath, file), GameSettings.G_LANGUAGE.ToDescription());
    }
}