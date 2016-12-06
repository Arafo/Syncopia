using UnityEngine.UI;

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

    public void Refresh() {
        language = GameSettings.G_LANGUAGE;
        if (LanguageSingleton._instance != null)
            text = LanguageSingleton._instance.getString(key);
    }
}