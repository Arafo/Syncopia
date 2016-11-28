using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class DynamicResolution : MonoBehaviour {

    public SuperSampling superSampling;

    public bool m_DynamicResolutionEnabled = true;
    public bool debug = true;

    public float minTargetScale = 0.7f;
    public float maxTargetScale = 2f;
    public int percentStep = 5;

    private List<float> m_DynamicResolutionScaleArray = new List<float>();
    private int m_DynamicResolutionLevel;

    private int m_DynamicResolutionFrameCountLastChanged = 0;
    // Buffer en anillo para guardar los ms que tardan los ultimos frames
    private float[] m_DynamicResolutionBuffer = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    private int m_DynamicResolutiongBufferPos = 0;
    private int m_LastQualityFrameCount = -1;


    void OnEnable() {
        superSampling = GetComponent<SuperSampling>();
        StartDynamicResolution();
    }

    void OnDisable() {

    }

    void LateUpdate() {

    }

    void OnPreCull() {
       UpdateDynamicResolution();
    }

    private void StartDynamicResolution() {

        m_DynamicResolutionScaleArray.Clear();
        m_DynamicResolutionScaleArray.Add(minTargetScale);

        // Añadir todas las escalas a la lista
        float currentTargetScale = minTargetScale;
        while (currentTargetScale <= maxTargetScale) {
            m_DynamicResolutionScaleArray.Add(currentTargetScale);
            currentTargetScale = Mathf.Sqrt(((float)(percentStep + 100) / 100.0f) * currentTargetScale * currentTargetScale);
        }
    }

    private void UpdateDynamicResolution() {
        if (!m_DynamicResolutionEnabled) {
            return;
        }

        if (m_LastQualityFrameCount == Time.frameCount)
            return;
        m_LastQualityFrameCount = Time.frameCount;

        int oldResolutionLevel = m_DynamicResolutionLevel;

        // Añadir la ultima medición al buffer
        int bufferSize = m_DynamicResolutionBuffer.GetLength(0);
        m_DynamicResolutiongBufferPos = (m_DynamicResolutiongBufferPos + 1) % bufferSize;
        m_DynamicResolutionBuffer[m_DynamicResolutiongBufferPos] = Time.deltaTime * 1000.0f;

        // Milisegundos que tarda un frame
        float singleFrameMs = (Screen.currentResolution.refreshRate > 0.0f) ? (1000.0f / Screen.currentResolution.refreshRate) : (1000.0f / 60.0f);

        // Limites de tiempo (en milisegundos) de los frames
        //float lowFrameMs = 0.8f * singleFrameMs;
        float extrapolationFrameMs = 1f * singleFrameMs;
        float highFrameMs = 1.2f * singleFrameMs;

        // Ultimos 3 frames
        float frame0 = m_DynamicResolutionBuffer[(m_DynamicResolutiongBufferPos - 0 + bufferSize) % bufferSize];
        float frame1 = m_DynamicResolutionBuffer[(m_DynamicResolutiongBufferPos - 1 + bufferSize) % bufferSize];
        float frame2 = m_DynamicResolutionBuffer[(m_DynamicResolutiongBufferPos - 2 + bufferSize) % bufferSize];

        // Reducir la resolucion siempre 2 niveles excepto cuando se cae del nivel 2
        int resolutionLevelDropTarget = (oldResolutionLevel == 2) ? 1 : (oldResolutionLevel - 2);

        // Reducir la resolucion 2 niveles si el ultimo frame es critico
        if (Time.frameCount >= m_DynamicResolutionFrameCountLastChanged + 2 + 1) {
            if (frame0 > highFrameMs) {
                int newLevel = Mathf.Clamp(resolutionLevelDropTarget, 0, m_DynamicResolutionScaleArray.Count - 1);
                if (newLevel != oldResolutionLevel) {
                    if (debug)
                        Debug.Log( "CAIDA CRITICA nivel " + newLevel + ". Frames = " + frame2 + "ms, " + frame1 + "ms, " + frame0 + "ms > " + highFrameMs + "\n" );
                    m_DynamicResolutionLevel = newLevel;
                    m_DynamicResolutionFrameCountLastChanged = Time.frameCount;
                    return;
                }
            }
        }

        // Reducir la resolucion 2 niveles si los ultimos 3 frames son caros
        if (Time.frameCount >= m_DynamicResolutionFrameCountLastChanged + 2 + 3) {
            if ((frame0 > highFrameMs) && (frame1 > highFrameMs) &&  (frame2 > highFrameMs)) {
                int newLevel = Mathf.Clamp(resolutionLevelDropTarget, 0, m_DynamicResolutionScaleArray.Count - 1);
                if (newLevel != oldResolutionLevel) {
                    if (debug)
                        Debug.Log("CAIDA MAXIMA nivel " + newLevel + ". Frames = " + frame2 + "ms, " + frame1 + "ms, " + frame0 + "ms > " + highFrameMs + "\n" );
                    m_DynamicResolutionLevel = newLevel;
                    m_DynamicResolutionFrameCountLastChanged = Time.frameCount;
                }
            }
        }

        // Pedrecir el coste del siguiente frame usando interpolacion lineal: max( frame-1 a frame+1, frame-2 a frame+1 )
        if (Time.frameCount >= m_DynamicResolutionFrameCountLastChanged + 2 + 2) {
            if (frame0 > extrapolationFrameMs) {
                float delta = frame0 - frame1;

                // Usar frame-2 si esta disponible
                if (Time.frameCount >= m_DynamicResolutionFrameCountLastChanged + 2 + 3) {
                    float delta2 = (frame0 - frame2) * 0.5f;
                    delta = Mathf.Max(delta, delta2);
                }

                if ((frame0 + delta) > highFrameMs) {
                    int newLevel = Mathf.Clamp(resolutionLevelDropTarget, 0, m_DynamicResolutionScaleArray.Count - 1);
                    if (newLevel != oldResolutionLevel) {
                        if (debug)
                            Debug.Log("CAIDA PREDECIDA nivel " + newLevel + ". Frames = " + frame2 + "ms, " + frame1 + "ms, " + frame0 + "ms > " + highFrameMs + "\n" );
                        m_DynamicResolutionLevel = newLevel;
                        m_DynamicResolutionFrameCountLastChanged = Time.frameCount;
                    }
                }
            }
        }

        // Aumentar la resolucion si los ultimos 3 frames son baratos
        if (Time.frameCount >= m_DynamicResolutionFrameCountLastChanged + 2 + 3) {
            if ((frame0 < highFrameMs) && (frame1 < highFrameMs) && (frame2 < highFrameMs)) {
                int newLevel = Mathf.Clamp(oldResolutionLevel + 1, 0, m_DynamicResolutionScaleArray.Count - 1);
                if (newLevel != oldResolutionLevel) {
                    if (debug)
                        Debug.Log("SUBIDA MINIMA nivel " + newLevel + ". Frames = " + frame2 + "ms, " + frame1 + "ms, " + frame0 + "ms < " + highFrameMs + "\n" );
                    m_DynamicResolutionLevel = newLevel;
                    m_DynamicResolutionFrameCountLastChanged = Time.frameCount;
                }
            }
        }

        if (superSampling != null) {
            superSampling.scaleHeight = m_DynamicResolutionScaleArray[m_DynamicResolutionLevel];
            superSampling.scaleWidth = m_DynamicResolutionScaleArray[m_DynamicResolutionLevel];
            superSampling.filter = 1;

            // Aplicar el nuevo nivel solo si ha cambiado
            if (m_DynamicResolutionLevel != oldResolutionLevel)
                superSampling.restart = true;
        }
        else {
            superSampling = GetComponent<SuperSampling>();
            superSampling.enabled = true;
        }
    }
}