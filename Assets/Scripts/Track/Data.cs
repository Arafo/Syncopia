using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Estructura para almacenar toda la información que compone una pista
/// </summary>
[Serializable]
public class Data : MonoBehaviour {

    // Datos
    [SerializeField]
    public List<TrackTile> tiles = new List<TrackTile>();
    public List<TrackTile> wallTiles = new List<TrackTile>();
    public TrackTile[] mappedTiles;
    public TrackTile[] mappedWallTiles;
    [SerializeField]
    public List<TrackSegment> sections = new List<TrackSegment>();

    // Objetos
    public GameObject dataParent;
    public GameObject tilesObject;
    public GameObject wallTilesObject;
    public GameObject sectionsObject;

    public int sectionStart;

    public int editorSectionHover;
    public int editorSectionCurrent;
    public int editorSectionNext;
}
