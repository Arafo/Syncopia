using UnityEngine;
using System.Collections;

/// <author>Rafael Marcen Altarriba</author>
/// <summary>
/// Se encarga de gestionar la posición de la nave en la pista
/// </summary>
public class ShipPosition : ShipCore {

    // Datos del circuito
    public int cSectionIndex;
    public int iSectionIndex;
    public bool iSectionFound;
    public bool onJump;
    private TrackSegment expectedLandSection;

    // Respawn
    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    public override void OnStart() {
        // Posicion de respawn inicial
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    public override void OnUpdate() {
        if (!ship.isRespawning) {
            UpdateInitialSection();
            UpdateCurrentSection();
            UpdateDirection();
        }
    }

    private void UpdateInitialSection() {
        float distance = Mathf.Infinity;
        float newDistance = distance;
        int i = 0;
        int length = RaceSettings.trackData.trackData.sections.Count;
        for (i = 0; i < length; ++i) {
            newDistance = Vector3.Distance(transform.position, RaceSettings.trackData.trackData.sections[i].position);
            if (newDistance < distance) {
                distance = newDistance;
                iSectionFound = true;
                ship.currentSection = RaceSettings.trackData.trackData.sections[i];
            }
        }

    }

    private void UpdateCurrentSection() {
        if (!iSectionFound) return;

        // Actualiza la posicion del respawn
        respawnPosition = ship.currentSection.next.position;
        respawnPosition.y += ship.config.hoverHeight;
        respawnRotation = SectionGetRotation(ship.currentSection.next);
    }

    private void UpdateDirection() {
        Vector3 trackRot = SectionGetRotation(ship.currentSection) * Vector3.forward;

        float dot = Vector3.Dot(transform.forward, trackRot.normalized);
        if (dot > 0)
            ship.facingFoward = true;
        else if (dot < 0)
            ship.facingFoward = false;
    }

    public static Quaternion SectionGetRotation(TrackSegment section) {
        // Obtiene las posiciones
        Vector3 sectionPosition = section.position;
        Vector3 nextPosition = section.next.position;

        // Obtiene la direccion y la normal de las posiciones
        Vector3 forward = (nextPosition - sectionPosition);
        Vector3 normal = section.normal;

        // Devuelve lookat
        return Quaternion.LookRotation(forward.normalized, normal.normalized);
    }

}
