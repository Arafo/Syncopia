﻿using UnityEngine;
using System.Collections;

public class SceneIndexManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
