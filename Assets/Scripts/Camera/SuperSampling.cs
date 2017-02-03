using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Implementa la técnica del supersampling (SSAA)
/// </summary>
public class SuperSampling : MonoBehaviour {

    public Camera camara;
    public static RenderTexture renderTexture;
    private static GameObject renderCam;
    private static TextureRenderer textureRenderer;

    public int screenWidth;
    public int screenHeight;

    public int targetWidth;
    public int targetHeight;

    public float scaleWidth = 1f;
    public float scaleHeight = 1f;
    // Bilinear = 0, Lanczos = 1, Nearest Neighbor = 2
    public int filter = 0;

    public bool restart;
    private bool active;

    private Shader[] SamplingShader = new Shader[2];

    public static SuperSampling instance;

    /// <summary>
    /// Habilita el supersampling
    /// </summary>
    public void OnEnable() {
        camara = GetComponent<Camera>();
        if (camara == null) {
            Debug.LogError("No hay camara");
            enabled = false;
            return;
        }

        SamplingShader[0] = (Resources.Load("Bilinear") as Shader);
        SamplingShader[1] = (Resources.Load("Lanczos") as Shader);

        targetWidth = Screen.width;
        targetHeight = Screen.height;

        new Shader();
        if (Application.isEditor) {
            //restart = true;
            //return;
        }
        Start();
    }

    /// <summary>
    /// Deshabilita el supersampling
    /// </summary>
    public void OnDisable() {
        Stop();
    }

    /// <summary>
    /// Calcula la resolución objetivo, define la textura y la cámara
    /// auxiliar para pintar la imagen a dicha resolución
    /// </summary>
    public void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        int num = (int)((float)Screen.width * scaleWidth);
        int num2 = (int)((float)Screen.height * scaleHeight);

        if (renderTexture == null || renderTexture.width != num || renderTexture.height != num2) {
            if (renderTexture != null) {
                renderTexture.Release();
            }
            renderTexture = new RenderTexture(num, num2, 24, 0);
            renderTexture.name = "SamplingTexture";
        }

        instance = this;

        if (renderCam == null) {
            renderCam = new GameObject("SamplingCamera");
            Camera camera = renderCam.AddComponent<Camera>();
            camera.CopyFrom(camara);
            camera.cullingMask = 0;
            camera.targetTexture = null;
            textureRenderer = renderCam.AddComponent<TextureRenderer>();

            if (SamplingShader != null) {
                switch (filter) {
                    case 0:
                        textureRenderer.samplingMaterial = new Material(SamplingShader[0]);
                        break;
                    case 1:
                        textureRenderer.samplingMaterial = new Material(SamplingShader[1]);
                        break;
                    case 2:
                        textureRenderer.samplingMaterial = null;
                        break;
                }
            }
        }
        active = true;
    }

    /// <summary>
    /// Para el supersampling
    /// </summary>
    public void Stop() {
        if (camara != null && camara.targetTexture != null) {
            camara.targetTexture = null;
        }

        active = false;
        instance = null;
        if (instance == null) {
            if (renderCam != null) {
                Destroy(renderCam);
                renderCam = null;
            }

            if (renderTexture != null) {
                renderTexture.Release();
                renderTexture = null;
            }
        }
    }

    /// <summary>
    /// Reinicia el supersampling
    /// </summary>
    public void Restart() {
        Stop();
        Start();
        restart = false;
    }

    /// <summary>
    /// En cada frame se comprueba si es necesario cambiar la
    /// resolución
    /// </summary>
    public void Update() {
        if (screenWidth != Screen.width || screenHeight != Screen.height) {
            restart = true;
        }

        if (restart) {
            Restart();
        }
    }

    /// <summary>
    /// Establece la resolución objetivo y la textura de la cámara antes
    /// de hacer el culling
    /// </summary>
    public void OnPreCull() {
        if (active && (screenWidth != Screen.width || screenHeight != Screen.height)) {
            targetWidth = Screen.width;
            targetHeight = Screen.height;
            restart = true;
        }

        if (active) {
            camara.targetTexture = renderTexture;
        }

        StartCoroutine(ResetCamera());
    }

    /// <summary>
    /// Coroutine para reiniciar la cámara el siguiente frame
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetCamera() {
        yield return new WaitForEndOfFrame();
        if (active) {
            camara.targetTexture = null;
        }
        yield break;
    }
}