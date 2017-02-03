using UnityEngine;
using System.Collections;
using System.IO;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Asegura que solo existe una instancia de la clase
/// LanguageManager e inicia el cambio de idiomas
/// </summary>
public static class LanguageSingleton {

    public static LanguageManager _instance;
    private static string file = "language";

    /// <summary>
    /// Gestiona la instancia de la clase LanguageManager
    /// y el cambio del lenguaje
    /// </summary>
    /// <param name="newLang"></param>
    public static void InstanceLanguage(bool newLang = false) {
        if (_instance == null) {
            _instance = new LanguageManager(file, GameSettings.G_LANGUAGE.ToDescription());
        }

        // Cambio de idioma
        if (newLang && _instance != null)
            _instance.setLanguage(file, GameSettings.G_LANGUAGE.ToDescription());
    }
}