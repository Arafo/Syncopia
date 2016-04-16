using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

    [Header("[ TRACK DATA ]")]
    public TrackData trackData;
    // Use this for initialization
    void Start () {
        RaceSettings.trackData = trackData;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
