using UnityEngine;
using System.Collections;
using System.IO;

public static class LanguageSingleton {

    public static LanguageManager _instance;
    private static string file = "Resources/language.xml";

    public static void InstanceLanguage() {
        if (_instance == null) {
            _instance = new LanguageManager(Path.Combine(Application.dataPath, file), GameSettings.G_LANGUAGE.ToDescription());
        }
    }

}
