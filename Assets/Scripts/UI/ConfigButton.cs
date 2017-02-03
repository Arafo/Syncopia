using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;
using System;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona las animaciones y los colores de los botones
/// </summary>
public class ConfigButton : MonoBehaviour {

    [Header("[ REFERENCIAS ]")]
    public HorizontallScrollSlider currentSelectorSelected;
    public Button leftArrowButton;
    public Button rightArrowButton;


    private float delayAnimationTimer;
    private float currentAnimationTimer;

    void Start() {
        delayAnimationTimer = 0.2f;
        currentAnimationTimer = 0f;
    }

    /// <summary>
    /// Contrala las animaciones de los botones
    /// </summary>
    void FixedUpdate() {
        float horizontalInput = ReInput.players.GetPlayer(0).GetAxis("MenuHorizontal");
        if (currentSelectorSelected!= null && horizontalInput != 0f && currentAnimationTimer <= Time.time) {
            currentAnimationTimer = Time.time + delayAnimationTimer;
            if (horizontalInput > 0) {
                ChangeButtonNormalColor(leftArrowButton, new Color(1f, 1f, 1f, 0.588f));
                ChangeButtonNormalColor(rightArrowButton, Color.white);
                //currentSelectorSelected.rigthButton.onClick.Invoke();
                //currentSelectorSelected.setNextText();
            }
            else {
                ChangeButtonNormalColor(rightArrowButton, new Color(1f, 1f, 1f, 0.588f));
                ChangeButtonNormalColor(leftArrowButton, Color.white);
                //currentSelectorSelected.leftButton.onClick.Invoke();
                //currentSelectorSelected.setLastText();
            }
        }
    }

    /// <summary>
    /// Controla el selector y el color de los botones
    /// </summary>
    /// <param name="selector"></param>
    public void SetCurrentSelector(GameObject selector) {
        currentSelectorSelected = selector.GetComponent<HorizontallScrollSlider>();

        if (leftArrowButton != null)
            ChangeButtonNormalColor(leftArrowButton, new Color(1f, 1f, 1f, 0.588f));


        if (rightArrowButton != null)
            ChangeButtonNormalColor(rightArrowButton, new Color(1f, 1f, 1f, 0.588f));


        if (currentSelectorSelected != null) {
            Button[] buttons = currentSelectorSelected.GetComponentsInChildren<Button>();
            leftArrowButton = buttons[0];
            rightArrowButton = buttons[1];
        }
    }

    /// <summary>
    /// Cambia el color de botón
    /// </summary>
    /// <param name="button"></param>
    /// <param name="color"></param>
    private void ChangeButtonNormalColor(Button button, Color color) {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }
}
