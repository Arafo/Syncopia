using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Asegura que solo existe una instancia de la clase
/// EventSystem de Unity que se encarga de gestionar 
/// la entrada de los controladores en el modo multijugador.
/// </summary>
public class EventSystemChecker : MonoBehaviour {
    //public GameObject eventSystem;

    // Use this for initialization
    void Awake() {
        if (!FindObjectOfType<EventSystem>()) {
            //Instantiate(eventSystem);
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
        }
    }
}