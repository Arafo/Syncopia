using UnityEngine.UI;

public class TextLang : Text {

    public string key;

    void Start() {
        Refresh();
    }

    public void Refresh() {
        if (LanguageSingleton._instance != null)
            text = LanguageSingleton._instance.getString(key);
    }
}