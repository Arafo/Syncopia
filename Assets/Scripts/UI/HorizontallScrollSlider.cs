using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HorizontallScrollSlider : MonoBehaviour {

    public Button leftButton;
    public Button rigthButton;
    public List<string> listContent;

    [Range(1, 99)]
    public int rangeContent;
    public bool isRange;

    public Text actualText;
    public int firstIndex;
    public int index = 0;

    // Use this for initialization
    void Start () {
        if (isRange) {
            for (int i = 1; i <= rangeContent; i++) {
                listContent.Add(i.ToString());
            }
        }
        actualText.text = listContent[firstIndex];
        index = firstIndex;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void setNextText() {
        index++;
        if (index >= listContent.Count)
            index = 0;

        actualText.text = listContent[index];
    }

    public void setLastText() {
        index--;
        if (index < 0)
            index = listContent.Count - 1;

        actualText.text = listContent[index];
    }
}
