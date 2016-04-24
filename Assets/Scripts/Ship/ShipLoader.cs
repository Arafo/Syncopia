using UnityEngine;
using System.Collections;

public class ShipLoader : MonoBehaviour {


    public void SpawnShip(string Ship, bool AI) {
        GameObject ShipObject = Instantiate(Resources.Load(Ship) as GameObject) as GameObject;

        // Cargar la configuracion de la nave
        ShipConfig config = ShipObject.GetComponent<ShipConfig>();

        // Crear clases
        ShipReferer referer = gameObject.AddComponent<ShipReferer>();
        referer.sim = gameObject.AddComponent<ShipSimulation>();
        referer.control = gameObject.AddComponent<ShipController>();
        referer.input = gameObject.AddComponent<ShipInput>();
        referer.ai = gameObject.AddComponent<ShipAI>();
        referer.effects = gameObject.AddComponent<ShipTrailEffects>();

        referer.config = config;
        referer.effects.ship = referer;
        referer.input.ship = referer;
        referer.sim.ship = referer;
        referer.ai.ship = referer;
        referer.control.ship = referer;

        // Crear el eje de la nave
        GameObject axis = new GameObject("Axis");
        axis.transform.localPosition = Vector3.zero;
        axis.transform.localRotation = Quaternion.identity;
        axis.transform.parent = transform;

        // 
        ShipObject.transform.parent = axis.transform;
        ShipObject.transform.localPosition = Vector3.zero;
        ShipObject.transform.localRotation = Quaternion.identity;


        // Crear rigidbody
        Rigidbody body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.drag = 1.0f;
        body.angularDrag = 35f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Añadimos la colision
        //MeshCollider mc = referer.mesh.AddComponent<MeshCollider>();
        //mc.convex = true;
        //gameObject.tag = "Ship";
        //mc.gameObject.layer = LayerMask.NameToLayer("Ship");

        GameObject collider = new GameObject("Collider");
        collider.tag = "Player";
        collider.transform.parent = transform;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        collider.AddComponent<BoxCollider>();
        collider.GetComponent<BoxCollider>().size = config.size;
        //collider.GetComponent<BoxCollider>().isTrigger = true; // CUIDADO CON ESTO
        collider.GetComponent<BoxCollider>().transform.localScale = Vector3.one;

        referer.body = body;
        referer.mesh = collider;
        referer.axis = axis;
        referer.isAI = AI;
        referer.cam = SetCamera(referer, false);

        // Destruimos la instancia
        RaceSettings.ships.Add(referer);
        Destroy(this);
    }

    private Camera SetCamera(ShipReferer referer, bool isAi) {
        GameObject newCamera = new GameObject("Ship Camera");
        newCamera.transform.parent = transform;
        newCamera.transform.localPosition = new Vector3(0, 4, -15);
        newCamera.transform.rotation = new Quaternion(0.1f, 0, 0, 1);


        // Añadir todos los componentes de una camara
        Camera camera = newCamera.AddComponent<Camera>();
        camera.backgroundColor = Color.black;
        newCamera.AddComponent<GUILayer>();
        newCamera.AddComponent<FlareLayer>();
        newCamera.AddComponent<AudioListener>();

        camera.nearClipPlane = 0.05f;
        camera.farClipPlane = 1500.0f;

        if (isAi) {
            newCamera.SetActive(false);
        }
        else {
            RaceSettings.currentCamera = camera;
            ShipCamera sc = newCamera.AddComponent<ShipCamera>();
            sc.ship = referer;
        }
        return camera;
    }
}
