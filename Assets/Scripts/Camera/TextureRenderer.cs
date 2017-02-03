using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona la resolución y el contenido de la textura que
/// utiliza la técnica del supersampling
/// </summary>
public class TextureRenderer : MonoBehaviour {

    public Material samplingMaterial;
    public RenderTexture texture;

    private Camera camara;

    /// <summary>
    /// Al inicio de la ejecución se asegura que la cámara
    /// esta asignada
    /// </summary>
    private void Awake() {
        camara = GetComponent<Camera>();
        if (camara == null) {
            Debug.LogError("No hay camara");
            enabled = false;
        }
    }

    /// <summary>
    /// Cambia el orden de las cámaras para no ver la segunda
    /// cámara
    /// </summary>
    private void LateUpdate() {
        if (SuperSampling.instance.camara.depth >= camara.depth) {
            camara.depth = SuperSampling.instance.camara.depth + 0.5f;
        }
    }

    /// <summary>
    /// Escala la textura a la resolución objetivo
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        texture = SuperSampling.renderTexture;
        if (samplingMaterial != null) {
            samplingMaterial.SetFloat("_textureWidth", (float)SuperSampling.renderTexture.width);
            samplingMaterial.SetFloat("_textureHeight", (float)SuperSampling.renderTexture.height);
            samplingMaterial.SetTexture("_MainTex", SuperSampling.renderTexture);
            Graphics.Blit(SuperSampling.renderTexture, destination, samplingMaterial);
        }
        else {
            Graphics.Blit(SuperSampling.renderTexture, destination);
        }

        SuperSampling.renderTexture.DiscardContents();
    }
}