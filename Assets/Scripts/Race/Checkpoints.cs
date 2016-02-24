using UnityEngine;
using System.Collections;

public class Checkpoints : MonoBehaviour {

    public Transform[] checkPointArray;
    private ShipController m_control;

    // Use this for initialization
    void Start () {
        for (int i = 1; i < checkPointArray.Length; i++) {
            checkPointArray[i].GetComponent<Renderer>().material.SetFloat("_Mode", 4f);
        }
        checkPointArray[0].GetComponent<Renderer>().material.SetColor("_SpecColor", Color.red);


    }
	
	// Update is called once per frame
	void Update () {

    }
}
