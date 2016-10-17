using UnityEngine;
using System.Collections;

public class DynamicResolution : MonoBehaviour {

    private float timer = 0f;
    private Camera camera;

    public int pixelSize = 4;
    Texture2D tex;
    public FilterMode filterMode = FilterMode.Bilinear;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Screen.currentResolution);
        camera.pixelRect = new Rect(0, 0, Screen.width / pixelSize, Screen.height / pixelSize);
        timer += Time.deltaTime;

        if (timer > 3f) {
            //Screen.SetResolution(1440, 900, true);
            //camera.rect = new Rect(0, 0, 0.5f, 0.5f);
            if (timer > 6f)
                timer = 0f;
        }
        else {
            //camera.rect = new Rect(0, 0, 1f, 1f);
            //camera.ResetProjectionMatrix();
        }
        //Screen.SetResolution(2880, 1800, true);

    }

    void OnGUI() {
        if (Event.current.type == EventType.Repaint)
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);
    }

    void OnPostRender() {
        DestroyImmediate(tex);

        tex = new Texture2D(Mathf.FloorToInt(camera.pixelWidth), Mathf.FloorToInt(camera.pixelHeight));
        tex.filterMode = filterMode;
        tex.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
        tex.Apply();
    }
}
