using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicVisualizerManager : MonoBehaviour {
    public GameObject cubePrefab;
    public int vizualizerSize = 75;
    public float posADD = 1.2F;
    public float cubesScaler = 20.0F;

    private List<GameObject> spawnedCubes = new List<GameObject>();
    public float sizerSmooth = 20.0F;

    public AudioSource audioSource;

    public float soundProgress = 0.0F;
    public Image progressPanel;

    public float waitForPlay = 5.0F;

    public bool fixedPosition = true;


    void Awake() {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    IEnumerator Start() {
        for (int i = 0; i < this.vizualizerSize; i++) {
            GameObject go = (GameObject)GameObject.Instantiate(this.cubePrefab, new Vector3(0.3f + this.transform.position.x,
                this.transform.position.y,
                -8f + this.transform.position.z + (i * this.posADD)),
                this.transform.rotation);
            yield return go;
            go.transform.parent = this.transform;
            go.name = "Key" + i;
            this.spawnedCubes.Add(go);
        }

        this.Invoke("Pl", this.waitForPlay);
    }

    public void Pl() {
        this.audioSource.Play();
    }

    public int a1 = 1024;
    public int a2 = 0;
    public FFTWindow a3 = FFTWindow.Hanning;

    void FixedUpdate() {
        this.soundProgress = (this.audioSource.time / this.audioSource.clip.length) * 100;
        //this.progressPanel.fillAmount = this.soundProgress / 100;

        float[] spectrumData = this.audioSource.GetSpectrumData(this.a1, this.a2, this.a3);

        for (int i = 0; i < this.spawnedCubes.Count; i++) {
            Vector3 s = this.spawnedCubes[i].transform.localScale;
            Vector3 p = this.spawnedCubes[i].transform.position;

            s.y = Mathf.Lerp(s.y, (spectrumData[i] * this.cubesScaler), Time.deltaTime * this.sizerSmooth);
            p.y = this.transform.position.y + (s.y / 2);

            this.spawnedCubes[i].transform.localScale = s;

            if (this.fixedPosition)
                this.spawnedCubes[i].transform.position = p;
        }

        float po = (spectrumData[26] * this.cubesScaler);
    }
}
