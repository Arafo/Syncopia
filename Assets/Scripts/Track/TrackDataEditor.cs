#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;


[CustomEditor(typeof(TrackDataEditorMono))]
public class TrackDataEditor : Editor {
    private static TrackDataEditorMono tClass;
    private static TrackData managerData;
    private static bool isActive = false;

    private static int toolsMenu;
    private static int tilePaintType;
    private static int segmentEditMode;

    // Variables de las tiles
    private static string[] tileInfo = new string[5];
    public static bool tileDrawTransparent = true;
    public static float tileFillTransparency = 0.2f;
    public static float tileStrokeTransparency = 0.3f;
    public static bool tileDrawColors = true;

    // Variables de los segmentos
    private static string[] segmentInfo = new string[7];
    public static int customIndex;
    private static string customIndexString = "";
    private static bool updatedSelected;
    private static TrackSegment selectedSegment;

    /// <summary>
    /// 
    /// </summary>
    [MenuItem("Syncopia/Track Editor")]
    public static void Init() {
        // Solo se crea la ventana si existen los datos del circuito
        managerData = GameObject.Find("MANAGER_TRACK").GetComponent<TrackData>();
        if (managerData == null) {
            isActive = false;
            return;
        }

        if (isActive) {
            if (tClass != null)
                DestroyImmediate(tClass.gameObject);
            isActive = false;
        }
        else if (!isActive) {
            isActive = true;
            tClass = new GameObject("TRACK EDITOR").AddComponent<TrackDataEditorMono>();
            tClass.hideFlags = HideFlags.HideInHierarchy;

            // Selecciona el editor
            GameObject[] gm = new GameObject[1];
            gm[0] = tClass.gameObject;
            Selection.objects = gm;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnSceneGUI() {
        // Desactivadas las selecciones en el editor
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        // Actualizar toda la interfaz
        GUIUpdate();

        if (toolsMenu == 0) {
            DrawTiles();
            TilePainter();
        }
        else if (toolsMenu == 1) {
            SegmentEditor();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void GUIUpdate() {
        Handles.BeginGUI();
        GUILayout.Window(0, new Rect(25, 25, 300, 250), EditorWindow, "Track Data Tools");
        Handles.EndGUI();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawTiles() {
        HandleUtility.Repaint();

        // Obtener las tiles del circuito
        Mesh m = managerData.trackFloor.sharedMesh;
        Transform t = managerData.trackFloor.transform;
        TrackTile[] tiles = managerData.trackData.tiles.ToArray();
        Vector3[] meshVerts = m.vertices;
        Vector3[] verts = new Vector3[4];

        // Asignar los colores para pintar
        Color fill = Color.white;
        Color stroke = Color.black;

        // Pintar
        int i = 0;
        int tilesLength = tiles.Length;
        for (i = 0; i < tilesLength; i++) {
            try {
                if (meshVerts.Length > tiles[i].indices[0]) { 
                    verts[managerData.drawTileOrder[0]] = t.TransformPoint(meshVerts[tiles[i].indices[0]]);
                    verts[managerData.drawTileOrder[1]] = t.TransformPoint(meshVerts[tiles[i].indices[1]]);
                    verts[managerData.drawTileOrder[2]] = t.TransformPoint(meshVerts[tiles[i].indices[2]]);
                    verts[managerData.drawTileOrder[3]] = t.TransformPoint(meshVerts[tiles[i].indices[3]]);
                }
            }
            catch (Exception) {
                Debug.Log(meshVerts[tiles[i].indices[0]]);
            }

            // Cambiar los coloes segun el tipo de tile
            fill = Color.white;
            stroke = Color.black;
            if (tileDrawColors) {
                switch (tiles[i].type) {
                    case E_TILETYPE.BOOST:
                        fill = Color.blue;
                        break;
                    case E_TILETYPE.SPAWN:
                        fill = Color.yellow;
                        break;
                }
            }

            // Bajar la opacidad si no se pintar tiles opacas
            if (tileDrawTransparent) {
                fill.a = tileFillTransparency;
                stroke.a = tileStrokeTransparency;
            }

            // Pintar todo
            Handles.DrawSolidRectangleWithOutline(verts, fill, stroke);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void TilePainter() {
        tileInfo[0] = "";
        tileInfo[1] = "";

        // Obtener la mesh del circutio
        Mesh m = managerData.trackFloor.sharedMesh;

        // Raycast del puntero del raton a las tiles
        RaycastHit hit;
        Vector3[] verts = new Vector3[4];
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        int tri;
        int triIndex;
        int triLength = m.triangles.Length;
        Debug.Log(1 << LayerMask.NameToLayer("Track"));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Track"))) {
            tri = hit.triangleIndex;
            TrackTile tile = Helpers.TileFromTriangleIndex(tri, managerData.trackData.mappedTiles);

            if (hit.collider.GetComponent<MeshCollider>().sharedMesh == m) {
                // Actualizar la informacion de la tile apuntada
                tileInfo[0] = "Tile Index: " + tile.index;
                tileInfo[1] = "Tile Type: " + (tile.type.ToString());

                // Dibujar la mesh
                verts = new Vector3[4];
                int t = hit.triangleIndex * 3;

                triIndex = Mathf.RoundToInt(Mathf.Repeat(tile.indices[0], triLength));
                verts[managerData.drawTileOrder[0]] = hit.transform.TransformPoint(m.vertices[m.triangles[triIndex]]);
                triIndex = Mathf.RoundToInt(Mathf.Repeat(tile.indices[1], triLength));
                verts[managerData.drawTileOrder[1]] = hit.transform.TransformPoint(m.vertices[m.triangles[triIndex]]);
                triIndex = Mathf.RoundToInt(Mathf.Repeat(tile.indices[2], triLength));
                verts[managerData.drawTileOrder[2]] = hit.transform.TransformPoint(m.vertices[m.triangles[triIndex]]);
                triIndex = Mathf.RoundToInt(Mathf.Repeat(tile.indices[3], triLength));
                verts[managerData.drawTileOrder[3]] = hit.transform.TransformPoint(m.vertices[m.triangles[triIndex]]);

                // Cambiar el color de las mesh dependiendo del tipo
                Color col = Color.grey;
                Color outline = col;

                switch (tile.type) {
                    case E_TILETYPE.BOOST:
                        col = Color.blue;
                        break;
                    case E_TILETYPE.SPAWN:
                        col = Color.yellow;
                        break;
                }

                switch (tilePaintType) {
                    case 1:
                        outline = Color.blue;
                        break;
                    case 2:
                        outline = Color.yellow;
                        break;
                }

                outline *= 0.8f;
                col.a = 0.2f;

                Handles.DrawSolidRectangleWithOutline(verts, col, outline);

                if ((Event.current.type == EventType.mouseDrag || Event.current.type == EventType.mouseDown) && Event.current.button == 0) {
                    switch (tilePaintType) {
                        case 0:
                            tile.type = E_TILETYPE.NORMAL;
                            break;
                        case 1:
                            tile.type = E_TILETYPE.BOOST;
                            break;
                        case 2:
                            tile.type = E_TILETYPE.SPAWN;
                            break;
                    }
                    managerData.trackData.tiles[tile.index] = tile;
                }
            }
        }
        Debug.Log(hit.collider);

    }

    /// <summary>
    /// 
    /// </summary>
    private void SegmentEditor() {
        if (segmentEditMode == 0) {
            if (selectedSegment != null) {
                selectedSegment.position = Handles.PositionHandle(selectedSegment.position, Helpers.SectionGetRotation(selectedSegment));
            }
        }

        RaycastHit hit;
        Vector3[] verts = new Vector3[4];
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // reset hover selection
        managerData.trackData.editorSectionHover = -1;

        // Borrar la info del segmento
        for (int i = 0; i < segmentInfo.Length; ++i)
            segmentInfo[i] = "";

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Track"))) {
            HandleUtility.Repaint();

            // Obtener tile de la mesh
            Mesh m = hit.transform.GetComponent<MeshFilter>().sharedMesh;
            TrackTile tile = Helpers.TileFromTriangleIndex(hit.triangleIndex, managerData.trackData.mappedTiles);

            // Seleccionar segmento (Left Ctrl)
            if ((Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.LeftControl) || updatedSelected) {
                updatedSelected = false;
                selectedSegment = tile.segment;
                customIndex = tile.segment.index;
                managerData.trackData.editorSectionCurrent = selectedSegment.index;
                managerData.trackData.editorSectionNext = selectedSegment.next.index;
            }

            // Asignar siguiente segmento
            if (segmentEditMode == 1) {
                if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Space) {
                    selectedSegment.next = tile.segment;
                    managerData.trackData.editorSectionNext = tile.segment.index;
                }
            }
            managerData.trackData.editorSectionHover = tile.segment.index;
        }

        // Actualizar la información del segmento
        if (selectedSegment != null) {
            segmentInfo[0] = "Segment Index: " + selectedSegment.index;
            segmentInfo[1] = "Segment Type: " + selectedSegment.type;
            segmentInfo[2] = "Segment Tiles: " + selectedSegment.tiles[0].index + " & " + selectedSegment.tiles[1].index;
            segmentInfo[3] = "Segment Width: " + selectedSegment.width;
            segmentInfo[4] = "Segment Height: " + selectedSegment.height;
            segmentInfo[5] = "Segment Next: " + selectedSegment.next.index;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="windowID"></param>
    private static void EditorWindow(int windowID) {
        toolsMenu = GUILayout.Toolbar(toolsMenu, new string[2] { "Tiles Mode", "Segment Mode" });
        if (toolsMenu == 0)
            TilesPaint();
        else if (toolsMenu == 1)
            SegmentEdit();
    }

    /// <summary>
    /// 
    /// </summary>
    private static void TilesPaint() {
        tilePaintType = GUILayout.Toolbar(tilePaintType, new string[3] { "Normal", "Boost", "Spawn" });

        // Configuracion
        GUILayout.Label("SETTINGS:");
        tileDrawColors = GUILayout.Toggle(tileDrawColors, "Draw Tile Colors");
        tileDrawTransparent = GUILayout.Toggle(tileDrawTransparent, "Draw Transparent Tiles");
        if (tileDrawTransparent) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tile Fill Opacity");
            tileFillTransparency = GUILayout.HorizontalSlider(tileFillTransparency, 0.0f, 1.0f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tile Stroke Opacity");
            tileStrokeTransparency = GUILayout.HorizontalSlider(tileStrokeTransparency, 0.0f, 1.0f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        for (int i = 0; i < tileInfo.Length; ++i)
            GUILayout.Label(tileInfo[i]);
    }

    private static void SegmentEdit() {
        segmentEditMode = GUILayout.Toolbar(segmentEditMode, new string[2] { "Set Position", "Set Next" });

        if (selectedSegment != null) {
            if (GUILayout.Button("Selext Next Segment")) {
                HandleUtility.Repaint();

                // Asignar el segmento siguiente al seleccionado
                updatedSelected = true;
                selectedSegment = selectedSegment.next;
                customIndex = selectedSegment.index;
            }
        }

        if (GUILayout.Button("Set Selected To Start")) {
            int i;
            for (i = 0; i < managerData.trackData.sections.Count; ++i) {
                if (managerData.trackData.sections[i] == selectedSegment)
                    managerData.trackData.sectionStart = i;
            }
        }

        if (GUILayout.Button("Reverse Track")) {
            int i;
            for (i = 0; i < managerData.trackData.sections.Count; ++i) {
                if (i > 0)
                    managerData.trackData.sections[i].next = managerData.trackData.sections[i - 1];
                else
                    managerData.trackData.sections[i].next = managerData.trackData.sections[managerData.trackData.sections.Count - 1];
            }
        }

        // info
        GUILayout.Space(10);
        for (int i = 0; i < segmentInfo.Length; ++i)
            GUILayout.Label(segmentInfo[i]);
    }
}

#endif