using UnityEngine.UI;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Componente de texto que se actualiza en función del idioma seleccionado
/// </summary>
public class TextLang : Text {

    public string key;
    private Enumerations.E_LANGUAGE language;

    void Start() {
        Refresh();
    }

    void Update() {
        // Comprobacion cuando cambia el idioma
        if (language != GameSettings.G_LANGUAGE)
            Refresh();
    }

    /// <summary>
    /// Actualiza el texto
    /// </summary>
    public void Refresh() {
        language = GameSettings.G_LANGUAGE;
        if (LanguageSingleton._instance != null && key != null)
            text = LanguageSingleton._instance.getString(key);
    }

    /// <summary>
    /// Cambia la clave del texto
    /// </summary>
    /// <param name="value"></param>
    public void SetKey(string value) {
        key = value;
        Refresh();
    }
}