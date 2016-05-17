using UnityEngine;
using System.Collections;
using System;

public class TrackSegment : MonoBehaviour {

    // El indice de esta seccion
    public int index;

    // La posicion de esta seccion
    public Vector3 position;

    // El tipo de esta seccion
    public E_SECTIONTYPE type;

    // La anchura de esta seccion
    public float width;

    // La altura de esta seccion
    public float height;

    // La normal de esta seccion
    public Vector3 normal;

    // La siguiente seccion
    public TrackSegment next;

    // Las tiles en esta seccion
    public TrackTile[] tiles;
}

public enum E_SECTIONTYPE
{
    NORMAL = 0,
    JUMP = 1,
    STARTLINE = 2
}