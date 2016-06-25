using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HorizontallScrollSlider : MonoBehaviour {

    public Button leftButton;
    public Button rigthButton;
    public List<string> listContent;
    public Text actualText;
    public int firstIndex;
    private int index = 0;

    // Use this for initialization
    void Start () {
        actualText.text = listContent[firstIndex];
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
