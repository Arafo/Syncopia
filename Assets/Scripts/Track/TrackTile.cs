﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrackTile {

    // El indice de esta tile
    public int index;

    // La posicion de esta tile
    public Vector3 position;

    // El tipo de esta tile
    public E_TILETYPE type;

    // Los indices de los vertices que forman esta tile
    public int[] indices;

    // La seccion a la que pertenece esta tile
    public TrackSegment section;
}

public enum E_TILETYPE {
    FLOOR = 0,
    WALL = 1,
    BOOST = 2,
    SPAWN = 3
}
