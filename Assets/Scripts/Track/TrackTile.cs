using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Clase que representa un tile de una pista
/// </summary>
public class TrackTile : MonoBehaviour {

    // El indice de esta tile
    public int index;

    // La posicion de esta tile
    public Vector3 position;

    // El tipo de esta tile
    public E_TILETYPE type;

    // Los indices de los vertices que forman esta tile
    public int[] indices;

    // La seccion a la que pertenece esta tile
    public TrackSegment segment;
}

public enum E_TILETYPE {
    NORMAL,
    WALL,
    BOOST,
    SPAWN
}
