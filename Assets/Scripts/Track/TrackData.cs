using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TrackData : MonoBehaviour {

    [Header("[ MESHES ]")]
    public Data trackData;
    public MeshFilter trackFloor;
    public MeshFilter trackWall;

    [Header("[ OPCIONES ]")]
    public bool generated;
    public bool drawTrackData;
    public int[] drawTileOrder = new int[4] { 0, 1, 2, 3 };
    public float maxWallWidth = 10.0f;
    public float maxWallHeight = 1.5f;

    public List<Vector3> spawnPositions = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> spawnCameraLocations = new List<Vector3>();
    [HideInInspector]
    public List<Quaternion> spawnRotations = new List<Quaternion>();


    private void OnDrawGizmos() {
        if (trackData == null)
            return;

        if (!drawTrackData)
            return;

        Vector3 position1 = Vector3.zero;
        Vector3 position2 = Vector3.zero;
        for (int i = 0; i < trackData.sections.Count; i++) {
            Gizmos.color = Color.white;
            if (trackData.sections[i].index == trackData.editorSectionCurrent)
                Gizmos.color = Color.red;

            if (trackData.sections[i].index == trackData.editorSectionNext)
                Gizmos.color = Color.green;

            if (i == trackData.sectionStart)
                Gizmos.color = Color.blue;

            position1 = trackData.sections[i].position;
            position2 = position1 + trackData.sections[i].normal * trackData.sections[i].height;
            Gizmos.DrawWireSphere(position1, 0.2f);
            Gizmos.DrawLine(position1, position2);

            // Linea entre las secciones
            if (i < trackData.sections.Count - 1) {
                position2 = trackData.sections[i].next.position;
                Gizmos.DrawLine(position1, position2);
            }

            // Orientacion
            position1 = position1 + trackData.sections[i].normal * 0.5f;
            position2 = position1 + (SectionGetRotation(trackData.sections[i]) * Vector3.forward) * 1;
            Gizmos.DrawLine(position1, position2);

            // Anchura
            Gizmos.color = Color.red;
            position1 = trackData.sections[i].position;
            position2 = position1 + (SectionGetRotation(trackData.sections[i]) * Vector3.right) * trackData.sections[i].width;
            Gizmos.DrawLine(position1, position2);

            position2 = position1 - (SectionGetRotation(trackData.sections[i]) * Vector3.right) * trackData.sections[i].width;
            Gizmos.DrawLine(position1, position2);

            // Racing line
            //Gizmos.color = Color.green;
            //position1 = trackData.racingLine[i];
            //if (i < trackData.racingLine.Count - 1) {
                //position2 = trackData.racingLine[i + 1];
                //Gizmos.DrawLine(position1, position2);
            //}
        }
    }

    private void Update() {
        if (!generated) {
            generated = true;
            UpdateTrackData();
        }
    }

    public void UpdateTrackData() {
        if (trackData == null)
            trackData = GenerateTrackData.GenerateTrack(trackFloor.sharedMesh, trackWall.sharedMesh, trackWall.transform);
        else
            GenerateTrackData.GenerateTrack(trackFloor.sharedMesh, trackWall.sharedMesh, trackWall.transform);
        FindTrackDimensions();
    }

    public void FindSpawnTiles() {
        spawnPositions.Clear();
        spawnRotations.Clear();

        for (int i = 0; i < trackData.tiles.Count; i++) {
            if (trackData.tiles[i].type == E_TILETYPE.SPAWN) {
                Vector3 spawnPos = trackData.tiles[i].position;
                spawnPos.y += 3.5f;
                spawnPositions.Add(spawnPos);
                spawnRotations.Add(SectionGetRotation(trackData.tiles[i].segment));

                //Vector3 cameraPos = trackData.tiles[i].section.position;
                //cameraPos.y += 0.5f;
                //spawnCameraLocations.Add(cameraPos);
            }
        }
    }

    private void FindTrackDimensions() {
        int i;
        float j;
        List<TrackSegment> sections = trackData.sections;
        int length = sections.Count;
        bool foundWall;

        for (i = 0; i < length; ++i) {
            Vector3 right = SectionGetRotation(sections[i]) * Vector3.right;
            RaycastHit hit;
            foundWall = false;
            for (j = 0.05f; j <= maxWallHeight; j += 0.01f) {
                // Lado derecho
                if (Physics.Raycast(sections[i].position + new Vector3(0.0f, j, 0.0f), right, out hit, maxWallWidth, 1 << LayerMask.NameToLayer("Wall"))) {
                    Debug.DrawLine(sections[i].position + new Vector3(0.0f, j, 0.0f), hit.point);
                    foundWall = true;
                    sections[i].height = j;
                    sections[i].width = hit.distance;
                }
                else {
                    //Debug.DrawRay(sections[i].position + new Vector3(0.0f, j, 0.0f), new Vector3(right.x - maxWallWidth,0,0), Color.green);
                    if (!foundWall) {
                        sections[i].width = maxWallWidth;
                        sections[i].height = maxWallHeight;
                    }
                }
            }

            // Lado izquiedo
            if (!foundWall) {
                if (Physics.Raycast(sections[i].position + new Vector3(0.0f, j, 0.0f), right, out hit, maxWallWidth, 1 << LayerMask.NameToLayer("Wall"))) {
                    Debug.DrawLine(sections[i].position + new Vector3(0.0f, j, 0.0f), hit.point);
                    if (hit.distance < 10.0f) {
                        foundWall = true;
                        sections[i].height = j;
                        sections[i].width = hit.distance;
                    }
                }
                else {
                    if (!foundWall) {
                        sections[i].width = maxWallWidth;
                        sections[i].height = maxWallHeight;
                    }
                }
            }
        }
        trackData.sections = sections;
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
