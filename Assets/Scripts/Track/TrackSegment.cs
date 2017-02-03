using UnityEngine;
using System.Collections;
using System;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Clase que representa un segmento de una pista
/// </summary>
public class TrackSegment : MonoBehaviour {

    // El indice de esta seccion
    public int index;

    // La posicion de esta seccion
    public Vector3 position;

    // El tipo de esta seccion
    public E_SEGMENTTYPE type;

    // La anchura de esta seccion
    public float width;

    // La altura de esta seccion
    public float height;

    // La normal de esta seccion
    public Vector3 normal;

    // La siguiente seccion
    public TrackSegment next;

    // El siguiente cruce
    public TrackSegment junction;

    // Las tiles en esta seccion
    public TrackTile[] tiles;
}

public enum E_SEGMENTTYPE
{
    NORMAL,
    JUMP,
    STARTLINE,
    JUNCTION_START,
    JUNCTION_END
}