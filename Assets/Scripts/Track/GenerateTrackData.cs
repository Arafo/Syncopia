﻿using UnityEngine;
using System.Collections.Generic;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Algoritmo que se encarga de leer el modelo de una pista y generar los datos correspondientes
/// </summary>
public class GenerateTrackData {

    public List<GameObject> checkpoints = new List<GameObject>();
    public List<Vector3> racingLine = new List<Vector3>();

    public int editorSectionHover;
    public int editorSectionCurrent;
    public int editorSectionNext;

    public static Data GenerateTrack(Mesh trackFloor, Mesh trackWall, Transform floorT) {
        if (trackFloor == null || trackWall == null) {
            return null;
        }

        GameObject genData = new GameObject("DATA");
        Data data = genData.AddComponent<Data>();
        genData.SetActive(false);

        data.dataParent = genData;

        data.sectionsObject = new GameObject("Sections");
        data.sectionsObject.transform.parent = data.dataParent.transform;

        data.tilesObject = new GameObject("Tiles");
        data.tilesObject.transform.parent = data.dataParent.transform;

        data.wallTilesObject = new GameObject("Wall Tiles");
        data.wallTilesObject.transform.parent = data.dataParent.transform;

        GenerateTrackData gen = new GenerateTrackData();
        gen.rebuildMesh(trackFloor);
        //gen.rebuildMesh(trackWall);

        // 
        Vector3[] verts = trackFloor.vertices;
        TrackTile[] tiles = new TrackTile[2];
        int[] tris = trackFloor.triangles;
        TrackTile[] mappedFloor = new TrackTile[tris.Length];
        Vector3[] normals = trackFloor.normals;


        gen.createTiles(tris, verts, mappedFloor, floorT, data);
        gen.createSections(normals, verts, tiles, floorT, data);
        //gen.createRacingLine();
        gen.createCheckpoints();


        int sectionCount = data.sections.Count;
        for (int i = 0; i < sectionCount; ++i) {
            if (i == sectionCount - 1)
                data.sections[i].next = data.sections[0]; 
            else
                data.sections[i].next = data.sections[i + 1];
        }

        //
        //for (int i = 0; i < mappedFloor.Length; i++)
        //gen.tilesMapped.Add(mappedFloor[i]);

        // add mapped tiles to gendata
        data.mappedTiles = mappedFloor;

        // update data objects to reflect amount of data they hold
        data.tilesObject.name = string.Format("Tiles ({0})", data.tiles.Count);
        data.wallTilesObject.name = string.Format("Wall Tiles ({0})", data.tiles.Count);
        data.sectionsObject.name = string.Format("Sections ({0})", data.sections.Count);

        return data;
    }

    private void rebuildMesh(Mesh oldMesh) {
        int triLength = oldMesh.triangles.Length;
        Vector3[] newVertices = new Vector3[triLength];
        Vector3[] oldVerts = oldMesh.vertices;
        Vector2[] newUV = new Vector2[triLength];
        Vector2[] oldUV = oldMesh.uv;
        Vector3[] newNormals = new Vector3[triLength];
        Vector3[] oldNormals = oldMesh.normals;
        int[] newTriangles = new int[triLength];
        int[] oldTriangles = oldMesh.triangles;
        for (int i = 0; i < triLength; i++) {
            newVertices[i] = oldVerts[oldTriangles[i]];
            //newUV[i] = oldUV[oldTriangles[i]];
            newNormals[i] = oldNormals[oldTriangles[i]];
            newTriangles[i] = i;
        }

        oldMesh.vertices = newVertices;
        oldMesh.uv = newUV;
        oldMesh.normals = newNormals;
        oldMesh.triangles = newTriangles;
    }

    private void createTiles(int[] tris, Vector3[] verts, TrackTile[] mappedFloor, Transform floorT, Data gen) {
        // Crear tiles
        TrackTile newTile;
        int index = 0;
        for (int i = 0; i < tris.Length - 3; i += 6) {
            // Crear tile y añadirla a lista de tiles
            newTile = createTile(tris, verts, floorT, i, index, gen);
            gen.tiles.Add(newTile);

            mappedFloor[i + 0] = newTile;
            mappedFloor[i + 1] = newTile;
            mappedFloor[i + 2] = newTile;
            mappedFloor[i + 3] = newTile;

            index++;
        }
    }

    private TrackTile createTile(int[] tris, Vector3[] verts, Transform floorT, int i, int index, Data data) {
        // Crear una tile
        TrackTile newTile = new TrackTile();
        newTile = data.sectionsObject.AddComponent<TrackTile>();
        newTile.hideFlags = HideFlags.HideInInspector;

        // Añadir los indices de la tile
        newTile.indices = new int[4];
        newTile.indices[0] = tris[i + 0];
        newTile.indices[1] = tris[i + 1];
        newTile.indices[2] = tris[i + 2];
        newTile.indices[3] = tris[i + 5];

        newTile.type = E_TILETYPE.NORMAL;
        newTile.index = index;

        // Posicion media de los vertices
        Vector3 position1 = floorT.TransformPoint(verts[tris[i]]);
        Vector3 position2 = floorT.TransformPoint(verts[tris[i + 1]]);
        Vector3 positionMid = (position1 + position2) / 2;
        newTile.position = positionMid;

        return newTile;
    }

    private void createSections(Vector3[] normals, Vector3[] verts, TrackTile[] tiles, Transform floorT, Data gen) {

        // Crear las secciones del circuito
        TrackSegment newSection;
        int index = 0;
        for (int i = 0; i < gen.tiles.Count - 1; i += 2) {
            tiles[0] = gen.tiles[i + 0];
            tiles[1] = gen.tiles[i + 1];

            // Crear la seccion y añadirla a lista de secciones
            newSection = createSection(normals, verts, tiles, floorT, gen, i, index);
            gen.sections.Add(newSection);

            tiles[0].segment = newSection;
            tiles[1].segment = newSection;

            gen.tiles[i].segment = newSection;
            gen.tiles[i + 1].segment = newSection;

            index++;
        }

        // Configurar las siguientes secciones de cada seccion
        for (int i = 0; i < gen.sections.Count; i++) {
            if (i == gen.sections.Count - 1)
                gen.sections[i].next = gen.sections[0];
            else
                gen.sections[i].next = gen.sections[i + 1];
        }
    }

    private TrackSegment createSection(Vector3[] normals, Vector3[] verts, TrackTile[] tiles, Transform floorT, Data gen, int i, int index) {
        TrackSegment newSection = new TrackSegment();
        newSection = gen.sectionsObject.AddComponent<TrackSegment>();
        newSection.hideFlags = HideFlags.HideInInspector;

        tiles[0] = gen.tiles[i];
        tiles[1] = gen.tiles[i + 1];

        newSection.type = E_SEGMENTTYPE.NORMAL;
        newSection.tiles = tiles;
        newSection.index = index;

        // Establecer la posicion de la seccion
        Vector3 position1 = floorT.transform.TransformPoint(verts[tiles[0].indices[0]]);
        Vector3 position2 = floorT.transform.TransformPoint(verts[tiles[1].indices[1]]);
        Vector3 positionMid = (position1 + position2) / 2;
        newSection.position = positionMid;

        // Establecer la normal de la seccion
        Vector3 normal1 = floorT.transform.TransformDirection(normals[tiles[0].indices[0]]);
        Vector3 normal2 = floorT.transform.TransformDirection(normals[tiles[1].indices[1]]);
        Vector3 normalMid = (normal1 + normal2) / 2;
        newSection.normal = normalMid;

        return newSection;
    }

    private void createRacingLine() {
        //// PROVISIONAL
        //for (int i = 0; i < sections.Count - 1; i++) {

        //    Vector3 k0 = SectionGetRotation(sections[i]) * Vector3.forward;
        //    Vector3 k1 = SectionGetRotation(sections[i + 1]) * Vector3.forward;
        //    //Vector3 k2 = SectionGetRotation(sections[i + 2]) * Vector3.forward;
        //    float a = k0.x > 0 ? -k0.x : k0.x;
        //    float b = (k0.x + k1.x + k0.z + k1.z) / 4;
        //    float c = k0.z < 0 ? -k0.z : k0.z;

        //    racingLine.Add(new Vector3(sections[i].position.x + a * 40, sections[i].position.y, sections[i].position.z));


        //    //Debug.Log(a);
        //}
    }

    private void createCheckpoints() {
        if (checkpoints.Count <= 0)
            checkpoints.Clear();

        //for (int i = 0; i < sections.Count - 1; i++) {
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.localScale = new Vector3(120, 80, 1);
        //cube.transform.rotation = sections[i].rotation;
        //cube.transform.position = new Vector3(sections[i].position.x, sections[i].position.y + 80/2, sections[i].position.z);
        //checkpoints.Add(cube);
        //}
    }

    private Quaternion SectionGetRotation(TrackSegment section) {
        // Obtener las posiciones de la seccion y su siguiente
        Vector3 sectionPosition = section.position;
        Vector3 nextPosition = section.next.position;

        // Calcular la distacia entre la posiciones y la normal
        Vector3 forward = (nextPosition - sectionPosition);
        Vector3 normal = section.normal;
        // Devolver la rotacion
        return Quaternion.LookRotation(forward.normalized, normal.normalized);
    }
}
