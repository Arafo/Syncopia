using UnityEngine;
using System.Collections;

public class Helpers : MonoBehaviour {

    public static TrackTile[] SectionGetTiles(TrackSegment section) {
        return section.tiles;
    }

    public static Quaternion SectionGetRotation(TrackSegment section) {
        Vector3 sectionPosition = section.position;
        Vector3 nextPosition = section.next.position;

        Vector3 forward = (nextPosition - sectionPosition);
        Vector3 normal = section.normal;

        return Quaternion.LookRotation(forward.normalized, normal.normalized);
    }

    public static TrackSegment SectionFromIndex(int index, Data data) {
        int i = 0;
        for (i = 0; i < data.sections.Count; ++i) {
            if (data.sections[i].index == index)
                return data.sections[i];
        }
        return null;
    }

    public static TrackTile TileFromTriangleIndex(int index, TrackTile[] mappedTiles) {
        if (mappedTiles[index * 3] != null)
            return mappedTiles[index * 3];
        else
            return null;
    }

    public static TrackSegment TileGetSection(TrackTile tile) {
        return tile.segment;
    }
}
