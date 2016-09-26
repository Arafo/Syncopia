using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class NetworkPlayerController : NetworkBehaviour {

    [SyncVar]
    Vector3 realPosition = Vector3.zero;
    [SyncVar]
    Quaternion realRotation;

    private float updateInterval;

    void Start() {
        realPosition = transform.position;
        realRotation = transform.rotation;
    }

    void Update() {
        if (isLocalPlayer) {

            // update the server with position/rotation
            updateInterval += Time.deltaTime;
            if (updateInterval > 0.11f) // 9 times per second
            {
                updateInterval = 0;
                CmdSync(transform.position, transform.rotation);
            }
        }
        else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation) {
        realPosition = position;
        realRotation = rotation;
    }
}