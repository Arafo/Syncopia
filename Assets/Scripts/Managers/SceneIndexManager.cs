using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Proporciona una estructura para obtener el índice 
/// de las escenas por su nombre
/// </summary>
public class SceneIndexManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Devuelve el indice de la scena scene
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public static int SceneIndexFromName(string scene) {
        switch(scene) {
            case "Test" :
                return 2;
            case "Blood Dragon" :
                return 3;
            case "Volcano" :
                return 5;
            case "Menu" :
                return 0;
            case "Online" :
                return 4;
            default :
                return 0;                    
        }
    }
}
