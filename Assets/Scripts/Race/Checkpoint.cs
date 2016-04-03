using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{

    //public Transform[] checkPointArray;
    public GameObject ship;
    public GameObject track;
    public ShipController m_control;
    public Checkpoints m_points;
    public bool passed;

    // Use this for initialization
    void Start()
    {
        m_control = ship.GetComponent<ShipController>();
        m_points = track.GetComponent<Checkpoints>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        // Si no es la nave del jugador no se continua
        if (!other.CompareTag("Player"))
            return;
        //Debug.Log(other.tag);
        //Debug.Log(transform.position + " - " + m_points.checkPointArray[m_control.currentCheckpoint].transform.position);
        if (transform == m_points.checkPointArray[m_control.currentCheckpoint].transform) { // m_control null cuando el objeto Ship no esta bien asignado
            // Punto de respawn de la nave
            m_control.respawnPoint = transform.position;
            if (m_control.currentCheckpoint + 1 < m_points.checkPointArray.Length) {
                if (m_control.currentCheckpoint == 0) {
                    if (m_control.totalS < m_control.bestLap && m_control.currentLap != 0){
                        m_control.bestLap = m_control.totalS;
                    }
                    if (m_control.laps.Count + 1 == m_control.currentLap)
                            m_control.laps.Add(m_control.totalS); 
                    m_control.totalS = 0;
                    m_control.currentLap++;
                }
                m_control.currentCheckpoint++;
            }
            else {
                m_control.currentCheckpoint = 0;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {

    }

    public void SetBool(bool passed) {
        this.passed = passed;
    }
}
