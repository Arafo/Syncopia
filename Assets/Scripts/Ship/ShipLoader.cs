using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la carga de todos los componentes que definen a una nave
/// </summary>
public class ShipLoader : MonoBehaviour {

    public void SpawnShip(Enumerations.E_SHIPS Ship, int type, GameObject player = null) {
        GameObject ShipObject = null;
        if (type != 2)
            ShipObject = Instantiate(Resources.Load("Ships/" + Ship.ToString()) as GameObject) as GameObject;
        else
            ShipObject = player;

        // Crear rigidbody
        Rigidbody body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotationY;
        body.drag = 1f;
        body.angularDrag = 35f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Cargar la configuracion de la nave
        ShipConfig config = ShipObject.GetComponent<ShipConfig>();

        // Crear clases
        ShipReferer referer = gameObject.AddComponent<ShipReferer>();
        referer.sim = gameObject.AddComponent<ShipSimulation>();
        referer.control = gameObject.AddComponent<ShipController>();
        referer.input = gameObject.AddComponent<ShipInput>();

        if (type != 2)
            referer.position = gameObject.AddComponent<ShipPosition>();
        else
            referer.position = ShipObject.AddComponent<ShipPosition>();

        referer.ai = gameObject.AddComponent<ShipAI>();
        referer.effects = gameObject.AddComponent<ShipTrailEffects>();
        referer.music = gameObject.AddComponent<ShipAudio>();
        referer.mesh = config.mesh;

        // Icono Minimap
        //MiniMapObject mmo = gameObject.AddComponent<MiniMapObject>();
        //Image icon = (Instantiate(Resources.Load("UI/AIMinimapIcon") as GameObject) as GameObject).GetComponent<Image>();
        //mmo.image = icon;
        //Destroy(icon);

        referer.config = config;
        referer.effects.ship = referer;
        referer.input.ship = referer;
        referer.sim.ship = referer;
        referer.position.ship = referer;
        referer.ai.ship = referer;
        referer.control.ship = referer;
        referer.music.ship = referer;

        // Crear el eje de la nave
        GameObject axis = new GameObject("Axis");
        axis.transform.localPosition = Vector3.zero;
        axis.transform.localRotation = Quaternion.identity;
        axis.transform.parent = transform;
    
        // 
        ShipObject.transform.parent = axis.transform;
        ShipObject.transform.localPosition = Vector3.zero;
        ShipObject.transform.localRotation = Quaternion.identity;


        // Interpolación del movimiento solo en el multiplayer
        ShipObject.GetComponent<NetworkPlayerController>().enabled = type == 2;

        // Añadimos la colision
        MeshCollider mc = referer.mesh.AddComponent<MeshCollider>();
        mc.convex = true;
        gameObject.tag = "Ship";
        mc.gameObject.tag = "Ship";
        mc.gameObject.layer = LayerMask.NameToLayer("Ship");

        PhysicMaterial physicMaterial = new PhysicMaterial();
        physicMaterial.bounciness = 0f;
        physicMaterial.dynamicFriction = 0f;
        physicMaterial.staticFriction = 0f;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;

        GameObject collider = new GameObject("Collider");
        collider.tag = "Ship";
        collider.layer = LayerMask.NameToLayer("Ship");
        collider.transform.parent = transform;
        collider.transform.localPosition = new Vector3(0, 0f, 0); //Vector3.zero;
        collider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        collider.AddComponent<BoxCollider>();
        collider.GetComponent<BoxCollider>().size = config.size;
        //collider.GetComponent<BoxCollider>().isTrigger = true; // CUIDADO CON ESTO
        collider.GetComponent<BoxCollider>().material = physicMaterial;
        //collider.GetComponent<BoxCollider>().transform.localScale = Vector3.one;
        //collider.GetComponent<BoxCollider>().gameObject.SetActive(false);
        mc.material = physicMaterial;

        // Esfera para representar la nave en el minimap
        GameObject minimap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        minimap.name = "Minimap Sphere";
        minimap.layer = LayerMask.NameToLayer("Minimap");
        minimap.transform.parent = transform;
        minimap.transform.localPosition = new Vector3(0, 20, 0);
        minimap.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        minimap.transform.localScale = new Vector3(100, 100, 100);
        minimap.GetComponent<SphereCollider>().enabled = false;
        Renderer rend = minimap.GetComponent<Renderer>();
        rend.material.color = type != 1 && type != 2 ? Color.yellow : Color.grey;
        rend.material.SetColor("_EmissionColor", type != 1 && type != 2 ? Color.yellow : Color.grey);

        referer.body = body;
        referer.mesh = collider;
        referer.axis = axis;
        referer.isAI = (type == 1);
        referer.cam = SetCamera(referer, (type == 1 /*|| type == 2*/));

        //referer.isAI = (type == 1);
        //referer.isNetworked = (type == 2);

        // Destruimos la instancia
        RaceSettings.ships.Add(referer);
        Destroy(this);
    }

    public void SpawnClientShip(Enumerations.E_SHIPS Ship, int type, GameObject player = null) {

        GameObject ShipObject = player;
        // Cargar la configuracion de la nave
        ShipConfig config = ShipObject.GetComponent<ShipConfig>();

        // Crear clases
        ShipReferer referer = gameObject.AddComponent<ShipReferer>();
        referer.sim = gameObject.AddComponent<ShipSimulation>();
        //referer.control = gameObject.AddComponent<ShipController>();
        //referer.input = gameObject.AddComponent<ShipInput>();
        if (type != 2)
            referer.position = gameObject.AddComponent<ShipPosition>();
        else
            referer.position = ShipObject.AddComponent<ShipPosition>();
        //referer.ai = gameObject.AddComponent<ShipAI>();
        //referer.effects = gameObject.AddComponent<ShipTrailEffects>();

        referer.config = config;
        //referer.effects.ship = referer;
        //referer.input.ship = referer;
        referer.sim.ship = referer;
        referer.position.ship = referer;
        //referer.ai.ship = referer;
        //referer.control.ship = referer;

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
                    
        // Interpolación del movimiento solo en el multiplayer
        ShipObject.GetComponent<NetworkPlayerController>().enabled = type == 2;

        // Añadimos la colision
        //MeshCollider mc = referer.mesh.AddComponent<MeshCollider>();
        //mc.convex = true;
        //gameObject.tag = "Ship";
        //mc.gameObject.layer = LayerMask.NameToLayer("Ship");

        GameObject collider = new GameObject("Collider");
        collider.tag = "Ship";
        collider.layer = LayerMask.NameToLayer("Ship");
        // En el caso del cliente la colision tiene que estar en el objeto instanciado
        // para que se sincronize con el servidor
        collider.transform.parent = ShipObject.transform;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        collider.AddComponent<BoxCollider>();
        collider.GetComponent<BoxCollider>().size = config.size;
        //collider.GetComponent<BoxCollider>().isTrigger = true; // CUIDADO CON ESTO
        collider.GetComponent<BoxCollider>().transform.localScale = Vector3.one;

        referer.body = body;
        referer.mesh = collider;
        referer.axis = axis;
        referer.isNetworked = true;
        referer.isAI = (type == 1);
        //referer.cam = SetCamera(referer, (type == 1 /*|| type == 2*/));

        //referer.isAI = (type == 1);
        //referer.isNetworked = (type == 2);

        // Destruimos la instancia
        RaceSettings.ships.Add(referer);
        Destroy(this);
    }

    private Camera SetCamera(ShipReferer referer, bool isAi) {
        GameObject newCamera = new GameObject("Ship Camera");
        newCamera.transform.parent = transform;
        //newCamera.transform.localPosition = new Vector3(0, 4, 15);
        newCamera.transform.rotation = Quaternion.Euler(10, referer.body.rotation.eulerAngles.y, 0);


        // Añadir todos los componentes de una camara
        Camera camera = newCamera.AddComponent<Camera>();
        camera.backgroundColor = Color.black;
        newCamera.AddComponent<GUILayer>();
        newCamera.AddComponent<FlareLayer>();
        //newCamera.AddComponent<AudioListener>();
        //newCamera.AddComponent<DynamicResolution>();

        UnityStandardAssets.ImageEffects.Blur pauseBlur = newCamera.AddComponent<UnityStandardAssets.ImageEffects.Blur> ();
        pauseBlur.blurShader = Shader.Find("Hidden/BlurEffectConeTap");
        pauseBlur.enabled = false;

        camera.nearClipPlane = 0.05f;
        camera.farClipPlane = 5000.0f;
        camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Minimap")) & ~(1 << LayerMask.NameToLayer("HUD"));
        //camera.targetTexture = Instantiate(Resources.Load("DynamicResolution") as RenderTexture) as RenderTexture;
        //GameObject.Find("Canvas/RawImage").GetComponent<RawImage>().texture = camera.targetTexture;

        if (isAi) {
            newCamera.SetActive(false);
        }
        else {
            if (GameObject.Find("Directional light").GetComponent<VolumetricLight>() != null) {
                VolumetricLightRenderer vlr = newCamera.AddComponent<VolumetricLightRenderer>();
                vlr.DefaultSpotCookie = Resources.Load("spot") as Texture2D;
            }
            RaceSettings.currentCamera = camera;
            ShipCamera sc = newCamera.AddComponent<ShipCamera>();
            sc.ship = referer;
        }
        return camera;
    }
}
