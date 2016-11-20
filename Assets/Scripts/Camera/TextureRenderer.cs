using UnityEngine;
using System.Collections;

public class TextureRenderer : MonoBehaviour {

    public Material samplingMaterial;
    public RenderTexture texture;

    private Camera camara;

    private void Awake() {
        camara = GetComponent<Camera>();
        if (camara == null) {
            Debug.LogError("No hay camara");
            enabled = false;
        }
    }

    private void LateUpdate() {
        if (SuperSampling.instance.camara.depth >= camara.depth) {
            camara.depth = SuperSampling.instance.camara.depth + 0.5f;
        }
    }

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