using UnityEngine;
using System.Collections;

public class ShipLoad : MonoBehaviour {

    public Rigidbody m_body;
    public GameObject m_Axis;
    public ShipConfig m_Config;

    // Use this for initialization
    void Awake () {

        //
        gameObject.AddComponent<ShipSimulation>();
        gameObject.AddComponent<ShipController>();
        gameObject.AddComponent<ShipInput>();

        // Crear rigidbody
        m_body = gameObject.AddComponent<Rigidbody>();
        m_body.useGravity = false;
        m_body.drag = 1f;
        m_body.angularDrag = 35f;
        m_body.constraints = RigidbodyConstraints.FreezeRotationY;
        m_body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

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

        // Añadimos la colision
        GameObject collider = new GameObject("Collider");
        collider.tag = "Player";
        collider.transform.parent = transform;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        collider.AddComponent<BoxCollider>();
        collider.GetComponent<BoxCollider>().size = m_Config.size;
        //collider.GetComponent<BoxCollider>().isTrigger = true;
        collider.GetComponent<BoxCollider>().transform.localScale = Vector3.one;
        GetComponent<ShipSimulation>().m_collider = collider;

        // Añadimos la instancia de los efectos de la estela
        gameObject.AddComponent<ShipTrailEffects>();

        // Destruimos la instancia
        //UnityEngine.Object.Destroy(this);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
