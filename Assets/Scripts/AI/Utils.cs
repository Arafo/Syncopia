using UnityEngine;
using System.Collections;

public static class Utils {

    public static float RandomFloat() {
        float rand = (float)Random.Range(0.0f, int.MaxValue);
        return rand / int.MaxValue + 1.0f;
    }

    public static float RandomClamped() {
        return RandomFloat() - RandomFloat();
    }
}
