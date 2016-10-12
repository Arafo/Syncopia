using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class ShipAudio : ShipCore {

    // Audio
    public GameObject audioContainer;
    private AudioSource engineSFX;

    public float engineVolume;
    private float enginePitch = 0.5f;
    private float turbulanceVolume = 0.0f;

    // Use this for initialization
    public override void OnStart() {

        float aiMinDistance = 0.5f;
        float aiMaxDistance = 3.0f;
        audioContainer = new GameObject("AudioContainer");
        audioContainer.transform.parent = transform;
        audioContainer.transform.localPosition = Vector3.zero;

        engineSFX = AttachNewSound(ship.config.SFX_ENGINE, (ship.isAI || ship.isNetworked), aiMinDistance, aiMaxDistance, true, true);
    }

    // Update is called once per frame
    public override void OnUpdate() {
        if (RaceSettings.shipsRestrained) {
            enginePitch = 0.5f + (ship.sim.engineSpeed * 0.4f);
        }
        else {
            float wantedPitch = ((ship.transform.InverseTransformDirection(ship.body.velocity).z * 7) * Time.deltaTime);
            wantedPitch = Mathf.Clamp(wantedPitch, 0.5f, 2.0f);
            enginePitch = Mathf.Lerp(enginePitch, wantedPitch, Time.deltaTime * 1.5f);
        }

        //
        if (ship.input.m_AccelerationButton)
            engineVolume = Mathf.Lerp(engineVolume, 1f, Time.deltaTime * 1f);
        else
            engineVolume = Mathf.Lerp(this.engineVolume, 0f, Time.deltaTime);

        if (!ship.isAI) {
            if (ship.input.m_AccelerationButton && engineVolume < 0.6f)
                ship.PlayClip(ship.config.SFX_IGNITION);

            if (ship.input.m_AccelerationButton && this.engineVolume > 0.5f)
                ship.PlayClip(ship.config.SFX_ENGINE_RELEASE);
        }

        engineSFX.volume = this.engineVolume;
        engineSFX.pitch = enginePitch;
    }

    private AudioSource AttachNewSound(AudioClip clip, bool isAi, float minDistance, float maxDistance, bool loop, bool play) {
        AudioSource newSound = audioContainer.AddComponent<AudioSource>();
        newSound.clip = clip;

        if (isAi)
            newSound.spatialBlend = 1;
        else
            newSound.spatialBlend = 0;
        newSound.maxDistance = maxDistance;
        newSound.minDistance = minDistance;

        if (loop)
            newSound.loop = true;
        if (play)
            newSound.Play();

        return newSound;
    }
}