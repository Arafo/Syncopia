using UnityEngine;
using System.Collections;
using UnityStandardAssets.CinematicEffects;
using UnityStandardAssets.ImageEffects;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Gestiona todas las técnicas de antialiasing utilizadas de 
/// forma que no interfieran unas con otras
/// </summary>
public class AntiAliasingManager : MonoBehaviour {

    public int selected;
    public string AA;

    private Antialiasing standardAA;
    private AntiAliasing cinematicAA;
    private TemporalAntiAliasing temporalAA;
    private SuperSampling samplingAA;

    // Use this for initialization
    void Start () {
        standardAA = gameObject.AddComponent<Antialiasing>();
        standardAA.ssaaShader = Shader.Find("Hidden/SSAA");
        standardAA.dlaaShader = Shader.Find("Hidden/DLAA");
        standardAA.nfaaShader = Shader.Find("Hidden/NFAA");
        standardAA.shaderFXAAPreset2 = Shader.Find("Hidden/FXAA Preset 2");
        standardAA.shaderFXAAPreset3 = Shader.Find("Hidden/FXAA Preset 3");
        standardAA.shaderFXAAII = Shader.Find("Hidden/FXAA II");
        standardAA.shaderFXAAIII = Shader.Find("Hidden/FXAA III (Console)");

        cinematicAA = gameObject.AddComponent<AntiAliasing>();
        temporalAA = gameObject.AddComponent<TemporalAntiAliasing>();
        samplingAA = gameObject.AddComponent<SuperSampling>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Deshabilita todos los tipos de AA
    /// </summary>
    /// <returns></returns>
    private bool TurnOffAll() {
        QualitySettings.antiAliasing = 0;
        if (standardAA == null || cinematicAA == null || temporalAA == null || samplingAA == null)
            return false;

        standardAA.enabled = false;
        cinematicAA.enabled = false;
        temporalAA.enabled = false;
        // Super Sampling no debe desactivarse cuando la resolucion dinamica esta activa
        if (GameSettings.GS_DYNAMICRESOLUTION == 0)
            samplingAA.enabled = false;
        return true;
    }

    /// <summary>
    /// Selecciona el tipo de AA definido por type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="camera"></param>
    public void SelectAA(int type, Camera camera) {
        selected = type;
        AA = ((Enumerations.E_AA)type).ToDescription();

        // Si no se han podido desactivar los AA, no se hace nada
        if (!TurnOffAll())
            return;

        switch (type) {
            case 0 :
                // No hay que hacer nada, ya se ha desactivado el AA
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                break;
            case 1:
                // FXAA
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                standardAA.enabled = true;
                break;
            case 2:
                // FXAA (Extreme)
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                cinematicAA.enabled = true;
                cinematicAA.method = 1;
                FXAA fXAA = cinematicAA.current as FXAA;
                fXAA.preset = FXAA.Preset.extremeQualityPreset;
                break;
            case 3:
                // SMAA (Ultra)
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                cinematicAA.enabled = true;
                cinematicAA.method = 0;
                SMAA sMAA = cinematicAA.current as SMAA;
                sMAA.settings.quality = SMAA.QualityPreset.Ultra;
                break;
            case 4:
                // MSAA x2
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 2;
                break;
            case 5:
                // MSAA x4
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 4;
                break;
            case 6:
                // MSAA x8
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 8;
                break;
            case 7:
                // TAA (8 samples)
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0f;
                temporalAA.settings.jitterSettings.sampleCount = 16;
                temporalAA.settings.jitterSettings.sampleCount = 8;
                break;
            case 8:
                // TAA (32 samples)
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0f;
                temporalAA.settings.jitterSettings.sampleCount = 16;
                temporalAA.settings.jitterSettings.sampleCount = 32;
                break;
            case 9:
                // TAA (Sharpening)
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0f;
                temporalAA.settings.jitterSettings.sampleCount = 16;
                temporalAA.settings.sharpenFilterSettings.amount = 0.33f;
                break;
            case 10:
                // MSAA x2 + TAA
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 2;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0.25f;
                temporalAA.settings.jitterSettings.sampleCount = 8;
                break;
            case 11:
                // MSAA x4 + TAA
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 4;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0.25f;
                temporalAA.settings.jitterSettings.sampleCount = 8;
                break;
            case 12:
                // MSAA x8 + TAA
                camera.renderingPath = RenderingPath.Forward;
                QualitySettings.antiAliasing = 8;
                temporalAA.enabled = true;
                temporalAA.settings.sharpenFilterSettings.amount = 0.25f;
                temporalAA.settings.jitterSettings.sampleCount = 8;
                break;
            case 13:
                // SSAA x2
                if (GameSettings.GS_DYNAMICRESOLUTION == 1)
                    return;
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                samplingAA.filter = 0;
                samplingAA.scaleHeight = 2;
                samplingAA.scaleWidth = 2;
                samplingAA.enabled = true;
                break;
            case 14:
                // SSAA x4
                if (GameSettings.GS_DYNAMICRESOLUTION == 1)
                    return;
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                samplingAA.filter = 1;
                samplingAA.scaleHeight = 4;
                samplingAA.scaleWidth = 4;
                samplingAA.enabled = true;
                break;
            case 15:
                // SSAA x8
                if (GameSettings.GS_DYNAMICRESOLUTION == 1)
                    return;
                camera.renderingPath = RenderingPath.UsePlayerSettings;
                samplingAA.filter = 1;
                samplingAA.scaleHeight = 8;
                samplingAA.scaleWidth = 8;
                samplingAA.enabled = true;
                //samplingAA.restart = true;
                break;
        }
    }
}
