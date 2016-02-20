using UnityEngine;
using System.Collections;

public class ShipLoad : MonoBehaviour {

    public Rigidbody m_body;
    public GameObject m_Axis;
    public ShipConfig m_Config;

    // Use this for initialization
    void Start () {

        // Crear rigidbody
        m_body = gameObject.AddComponent<Rigidbody>();
        m_body.useGravity = false;
        m_body.drag = 0;
        m_body.angularDrag = 20;
        m_body.constraints = RigidbodyConstraints.FreezeRotationY;

        // Crear el eje de la nave
        m_Axis = new GameObject("Axis");
        m_Axis.transform.parent = transform;
        m_Axis.transform.localPosition = Vector3.zero;

        // Cargar el prefab de la nave (estatico de momento)
        GameObject ShipObject = Instantiate(Resources.Load("Ship1") as GameObject) as GameObject;
        ShipObject.transform.parent = m_Axis.transform;
        ShipObject.transform.localPosition = Vector3.zero;

        // Cargar la configuracion de la nave
        m_Config = ShipObject.GetComponent<ShipConfig>();

        //
        gameObject.AddComponent<ShipTrailEffects>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
