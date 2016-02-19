using System;
using UnityEngine;

public class ShipConfig : MonoBehaviour
{
    [Header("CAMARA")]
    public Vector3 cameraClosePos;  // Posicion de la camara en primera persona
    public Vector3 cameraFarPos;    // Posicion de la camara en tercera persona
    public float cameraSpeed; // Velocidad de la camara
    public float cameraFoV; // El campo de vision base de la camara
    public float cameraTurnSensitivity; // Sensibilidad de la camara al girar
    public float cameraTurnDamp; // Cuanto le cuesta a la camara actualizarse al girar

    [Header("MOTOR")]
    public float engineAmount; // La maxima potencia del motor
    public float engineAcceleration; // La maxima aceleracion del motor
    public float engineGain; // La ganancia de la aceleracion del motor
    public float engineFalloff; // La velocidad con la que cae la potencia y aceleracion del motor

    [Header("GIROS")]
    public float turnAmount; // La maxima velocidad a la que se puede girar
    public float turnGain; // La velocidad a la que se gira
    public float turnFalloff; // La velocidad a la que se deja de girar

    [Header("FRENOS")]
    public float airbrakeAmount; // La sensibilidad de los frenos
    public float airbrakeGain; // La velocidad a la que los frenos actuan
    public float airbrakeFalloff; // La velocidad a la que los frenos dejan de actuar
    public float airbrakeTurnMultiplier; // Cantidad de giro hasta que comienzan a actuar los frenos

    [Header("GRAVEDAD")]
    public float gravity_Low; // Fuerza de la gravedad
    public float gravity_High; // Fuerza de la gravedad
    public float gravity;

    [Header("AGARRE")]
    public float grip_Low; // Resistencia a deslizarse
    public float grip_High; // Resistencia a deslizarse
    public float grip;

    [Header("DESLIZAMIENTO")]
    public float slipAmount; // Cantidad de deslizamiento por el uso de los frenos
    public float slipGain; // La velocidad a la que se desliza usando los frenos
    public float slipFalloff; // La velocidad a la que se desliza sin usar los frenos
    public float slipMagnitude; // Multiplicador del deslizamiento (1000)
    public float slipVelocityMult; // Multiplicador del deslizamiento por la velocidad en eje x

    [Header("OTROS")]
    public float hoverHeight; // Altura a la que la nave flota
    public Vector3 size; // Tamaño de la nave [3.5, 1.7, 9]

    //[Header("EFECTOS")]
    //public int airbrakeRotationMode;
    //public GameObject windTrailLeft; // Estela de aire del ala izquierda
    //public GameObject windTrailRight; // Estela de aire del ala derecha
    //public GameObject engineTrail;
    //public GameObject engineLight;
    //public GameObject engineFlare; // Estela de la combustion del motor
    //public GameObject boosterLeft;
    //public GameObjecb boosterRight;
    //public float boosterExtendLength;
}
