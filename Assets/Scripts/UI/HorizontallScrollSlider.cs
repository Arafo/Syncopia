using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona el comportamiento de las flechas de un botón horizontal
/// </summary>
public class HorizontallScrollSlider : MonoBehaviour {

    public Button leftButton;
    public Button rigthButton;
    public List<string> listContent;
    public List<string> hideContent;

    [Range(1, 99)]
    public int rangeContent;
    public bool isRange;
    public bool isText;

    public Text actualText;
    public int firstIndex;
    public int index = 0;

    // Use this for initialization
    void Start () {
        if (listContent != null) {
            if (isRange) {
                for (int i = 1; i <= rangeContent; i++) {
                    listContent.Add(i.ToString());
                }
            }
            if (firstIndex < listContent.Count) {
                if (isText)
                    actualText.text = LanguageSingleton._instance.getString(listContent[firstIndex]);
                else
                    actualText.text = listContent[firstIndex];
            }
            index = firstIndex;
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    /// <summary>
    /// Flecha siguiente
    /// </summary>
    public void setNextText() {
        index++;

        if (index >= listContent.Count)
            index = 0;

        if (hideContent != null && hideContent.Contains(listContent[index])) {
            setNextText();
        }

        actualText.text = isText ? LanguageSingleton._instance.getString(listContent[index]) : listContent[index];
    }

    /// <summary>
    /// Flecha anterior
    /// </summary>
    public void setLastText() {
        index--;

        if (index < 0)
            index = listContent.Count - 1;

        if (hideContent != null && hideContent.Contains(listContent[index])) {
            setLastText();
        }

        actualText.text = isText ? LanguageSingleton._instance.getString(listContent[index]) : listContent[index];
    }
}
