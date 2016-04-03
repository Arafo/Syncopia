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

    public static float Clamp(float val, float min, float max) {
        if (val < min) {
            return min;
        }

        if (val > max) {
            return max;
        }

        return val;
    }
}
