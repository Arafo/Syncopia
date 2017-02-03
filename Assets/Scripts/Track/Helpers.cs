using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Clase auxiliar para realizar distintas operaciones con los datos de una pista
/// </summary>
public class Helpers : MonoBehaviour {

    /// <summary>
    /// Devuelve la tiles que forman un segmento
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    public static TrackTile[] SectionGetTiles(TrackSegment section) {
        return section.tiles;
    }

    /// <summary>
    /// Devuelve la rotación de un segmento
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    public static Quaternion SectionGetRotation(TrackSegment section) {
        Vector3 sectionPosition = section.position;
        Vector3 nextPosition = section.next.position;

        Vector3 forward = (nextPosition - sectionPosition);
        Vector3 normal = section.normal;

        return Quaternion.LookRotation(forward.normalized, normal.normalized);
    }

    /// <summary>
    /// Devuelve el segmento a partir del indice
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static TrackSegment SectionFromIndex(int index, Data data) {
        int i = 0;
        for (i = 0; i < data.sections.Count; ++i) {
            if (data.sections[i].index == index)
                return data.sections[i];
        }
        return null;
    }

    /// <summary>
    /// Devuelve el tile a partir de un triangulo de tiles
    /// </summary>
    /// <param name="index"></param>
    /// <param name="mappedTiles"></param>
    /// <returns></returns>
    public static TrackTile TileFromTriangleIndex(int index, TrackTile[] mappedTiles) {
        if (mappedTiles[index * 3] != null)
            return mappedTiles[index * 3];
        else
            return null;
    }

    /// <summary>
    /// Devuelve el segmento al que pertenece un tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static TrackSegment TileGetSection(TrackTile tile) {
        return tile.segment;
    }
}
