using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Define la estructura que heredan el resto de clases que componen una nave
/// </summary>
public class ShipCore : NetworkBehaviour {

    public ShipReferer ship;

    // Use this for initialization
    public virtual void OnStart() {
    }

    // Update is called once per frame
    public virtual void OnUpdate() {
    }
}

