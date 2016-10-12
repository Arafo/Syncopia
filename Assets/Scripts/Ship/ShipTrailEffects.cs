using UnityEngine;
using System.Collections;
using Kvant;

public class ShipTrailEffects : ShipCore {

    private bool isColliding;

    private float visualLightItensity;
    private float idleLightFlickerSpeed = 100f;
    private float idleLightFlickerAmount = 2f;
    private float idleLightFlickerTimer;

    private float trailOpacity;
    private float trailStartSize;
    private float trailEndSize;
    private float trailMaxStartSize;
    private float trailMaxEndSize;
    private float trailBrightness;
    public Light lightObject;
    public TrailRenderer trailObject;

    private float boosterRemainTimer;
    private float boosterFalloff;
    private float boosterLength;
    private Color boosterColor;
    private float boosterOpacity;
    private Renderer boosterLeft;
    private Renderer boosterRight;

    private float windOpacity;
    private TrailRenderer windLeft;
    private TrailRenderer windRight;

    private float sprayThrottle;
    private Spray spray;

    // Use this for initialization
    public override void OnStart () {
        // Cargamos todos los efectos
        lightObject = ship.config.engineLight.GetComponent<Light>();
        trailObject = ship.config.engineTrail.GetComponent<TrailRenderer>();
        boosterColor = ship.config.boosterLeft.GetComponent<Renderer>().material.GetColor("_TintColor");
        boosterLeft = ship.config.boosterLeft.GetComponent<Renderer>();
        boosterRight = ship.config.boosterRight.GetComponent<Renderer>();
        windLeft = ship.config.windTrailLeft.GetComponent<TrailRenderer>();
        windRight = ship.config.windTrailRight.GetComponent<TrailRenderer>();
        spray = ship.config.engineSpray.GetComponent<Spray>();

        trailMaxStartSize = trailObject.startWidth;
        trailMaxEndSize = trailObject.endWidth;
}
	
	// Update is called once per frame
	public override void OnUpdate () {
        UpdateTrailEffects();
    }

    private void UpdateTrailEffects() {

        // Calibrar la longitud, opacidad e intensidad de la luz de la cola
        if (ship.input.m_AccelerationButton) {
            idleLightFlickerTimer = 0f;
            visualLightItensity = Mathf.Lerp(visualLightItensity, 3f, Time.deltaTime);
            if (visualLightItensity < 1.5f) {
                visualLightItensity = 1.5f;
            }

            if (transform.InverseTransformDirection(ship.body.velocity).z > 50f) {
                trailStartSize = Mathf.Lerp(trailStartSize, trailMaxStartSize, Time.deltaTime * 6f);
                trailEndSize = Mathf.Lerp(trailEndSize, trailMaxEndSize, Time.deltaTime * 6f);
                trailOpacity = Mathf.Lerp(trailOpacity, 0.5f, Time.deltaTime * 5f);
                sprayThrottle = Mathf.Lerp(trailOpacity, 1f, Time.deltaTime * 5f);
            }
            else {
                trailStartSize = Mathf.Lerp(trailStartSize, 0f, Time.deltaTime * 6f);
                trailEndSize = Mathf.Lerp(trailEndSize, 0f, Time.deltaTime * 6f);
                trailOpacity = Mathf.Lerp(trailOpacity, 0f, Time.deltaTime * 2f);
                sprayThrottle = Mathf.Lerp(trailEndSize, 0f, Time.deltaTime * 3f);
            }
        }
        else  {
            visualLightItensity = Mathf.Lerp(visualLightItensity, 0f, Time.deltaTime);
            if (visualLightItensity < 1.5f)   {
                idleLightFlickerTimer += Time.deltaTime * idleLightFlickerSpeed;
                visualLightItensity = Mathf.Sin(idleLightFlickerTimer) * idleLightFlickerAmount;
            }

            trailStartSize = Mathf.Lerp(trailStartSize, trailMaxStartSize, Time.deltaTime * 6f);
            trailEndSize = Mathf.Lerp(trailEndSize, trailMaxEndSize, Time.deltaTime * 6f);
            trailOpacity = Mathf.Lerp(trailOpacity, 0f, Time.deltaTime * 2f);
        }
        lightObject.intensity = visualLightItensity;

        // Asignamos los valores calculados a la cola
        trailObject.startWidth = trailStartSize;
        trailObject.endWidth = trailEndSize;
        trailObject.material.SetColor("_TintColor", new Color(0.8f, 0.8f, 0.8f, trailOpacity));
        Vector2 textureOffset = trailObject.material.GetTextureOffset("_MainTex");
        textureOffset.x += Time.deltaTime * 10f;
        trailObject.material.SetTextureOffset("_MainTex", textureOffset);

        // Spray
        spray.throttle = sprayThrottle;

        // Si la bave esta en booster
        if (ship.sim.isBoosting) {
            boosterRemainTimer = 0.1f;
            boosterFalloff = 0f;
            boosterLength = Mathf.Lerp(boosterLength, ship.config.boosterExtendLength, Time.deltaTime * 40f);
            boosterOpacity = Mathf.Lerp(boosterOpacity, 1f, Time.deltaTime * 4f);
            windOpacity = Mathf.Lerp(windOpacity, 1f, Time.deltaTime * 10f);
        }
        else {
            if (boosterRemainTimer > 0f) {
                boosterRemainTimer -= Time.deltaTime;
                boosterLength = Mathf.Lerp(boosterLength, ship.config.boosterExtendLength, Time.deltaTime * 40f);
                boosterOpacity = Mathf.Lerp(boosterOpacity, 1f, Time.deltaTime * 4f);
            }
            else {
                boosterFalloff = Mathf.Lerp(boosterFalloff, 120f, Time.deltaTime);
                boosterLength = Mathf.Lerp(boosterLength, 0f, Time.deltaTime * boosterFalloff);
                boosterOpacity = Mathf.Lerp(boosterOpacity, 0f, Time.deltaTime * 4f);
            }
            windOpacity = Mathf.Lerp(windOpacity, 0f, Time.deltaTime * 2.5f);
        }

        // Estela de viento izquierda al estar en modo booster
        Vector3 localScale = ship.config.boosterLeft.transform.localScale;
        ship.config.boosterLeft.transform.localScale = new Vector3(localScale.x, localScale.y, boosterLength);
        boosterLeft.material.SetColor("_TintColor", new Color(boosterColor.r, boosterColor.g, boosterColor.b, boosterOpacity));

        // Estela de viento derecha al estar en modo booster
        Vector3 localScale2 = ship.config.boosterRight.transform.localScale;
        ship.config.boosterRight.transform.localScale = new Vector3(localScale2.x, localScale2.y, boosterLength);
        boosterRight.material.SetColor("_TintColor", new Color(boosterColor.r, boosterColor.g, boosterColor.b, boosterOpacity));

        // Estela de viento del ala izquierda
        windLeft.material.SetColor("_TintColor", new Color(1f, 1f, 1f, windOpacity));
        textureOffset = windLeft.material.GetTextureOffset("_MainTex");
        textureOffset.x += Time.deltaTime * 10f;
        windLeft.material.SetTextureOffset("_MainTex", textureOffset);

        // Estela de viento del ala derecha
        windRight.material.SetColor("_TintColor", new Color(1f, 1f, 1f, windOpacity));
        textureOffset = windRight.material.GetTextureOffset("_MainTex");
        textureOffset.x += Time.deltaTime * 10f;
        windRight.material.SetTextureOffset("_MainTex", textureOffset);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        isColliding = false;
    }
}
