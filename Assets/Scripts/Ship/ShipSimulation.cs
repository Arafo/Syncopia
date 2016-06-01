using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipSimulation : ShipCore {

    // VARIABLES 
    public bool isGrounded;
    public bool wallSurfing;
    private Vector3 shipGravity;
    private Vector3 speedDirection;
    private float fallingDamper;
    private float groundDistance;
    private float airResistance;
    private float angularResistance;

    // MOTOR
    private float engineGain;
    private float engineFalloff;
    private float engineAcceleration;
    private float engineThrust;
    public float engineSpeed;
    private float enginePitchAmount;

    // BOOST
    public bool isBoosting;
    public bool boostingOverride;
    private float boostTimer;
    private bool hitBooster;

    // GIROS
    public float turnAmount;
    private float turnGain;
    private float turnFalloff;
    private bool turnAntiBand;

    // FRENOS
    public bool shipBraking;
    private float brakesAmount;
    private float brakesGain;
    private float brakesFalloff;
    public float airbrakeAmount;
    private float airbrakeGain;
    private float airbrakeFalloff;
    private float airbrakeResistance;
    private float airbrakeLeftRaise;
    private float airbrakeRightRaise;
    private float airbrakeSlip;
    private float airbrakeDrag;
    private bool airbrakeAntiBand;

    // INCLINACION O ALABEO
    public float bankAmount;
    private float bankGain;
    private float bankFalloff;
    private float bankAirbrake;
    public float bankVelocity;

    // HOVER
    private float hoverAnimTimer;
    private float hoverAnimSpeed = 1f;
    private float hoverAnimAmount = 0.2f;
    private Vector3 hoverAnimOffset;
    private Vector3 hoverCameraAnimOffset;

    // COLISION
    public bool isColliding;

    // BARREL ROLL
    public float BRBoostTimer;
    private float BRProgress;
    private float BRActual;
    private float BRTimer;
    private float BRLastValue;
    private float BRState;
    private float BRGain;
    private bool BRSuccess;
    private bool hasBR;
    private bool canBR;

    public override void OnStart() {
    }

    public override void OnUpdate() {
        if (ship.control.isRespawning) {
            ship.mesh.SetActive(false);
            engineThrust = 0f;
            engineAcceleration = 0f;
            BRSuccess = false;
            transform.position = Vector3.Lerp(transform.position, ship.position.respawnPosition, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, ship.position.respawnRotation, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, ship.position.respawnPosition) < 10f) {
                ship.control.isRespawning = false;
            }
        }
        else {
            ship.mesh.SetActive(true);
            ShipGravity();
            ShipDrag();
            ShipAcceleration();
            ShipHandling();
            ShipAnimations();
            ShipBarrelRolls();
        }
    }

    private void ShipGravity() {
        // Puntos de flotacion calculados con el tamaño de la nave
        List<Vector3> hoverPoints = new List<Vector3> {
            transform.TransformPoint(-ship.config.size.x * 0.5f, 0f, ship.config.size.z * 0.5f),
            transform.TransformPoint(ship.config.size.x * 0.5f, 0f, ship.config.size.z * 0.5f),
            transform.TransformPoint(ship.config.size.x * 0.5f, 0f, -ship.config.size.z * 0.5f),
            transform.TransformPoint(-ship.config.size.x * 0.5f, 0f, -ship.config.size.z * 0.5f)
        };

        // Si la nave esta en el suelo se aplica una fuerza de gravedad baja
        // Si esta en el aire se aplica mas fuerza para evitar que despegue
        if (isGrounded) {
            shipGravity = -transform.up * ship.config.gravity * 0.6f;
        }
        else {
            shipGravity = Vector3.down * ship.config.gravity;
        }
        ship.body.AddForce(shipGravity, ForceMode.Acceleration);

        // Si la nave esta en el suelo se interpola la caida del dumper hasta 0
        // Si esta en el aire se deja un valor mas alto
        if (isGrounded) {
            fallingDamper = Mathf.Lerp(fallingDamper, 0f, Time.deltaTime);
        }
        else {
            fallingDamper = Mathf.Lerp(fallingDamper, 0.1f, Time.deltaTime);
        }
        isGrounded = false;

        // Se itera para todos los puntos de flotacion aplicando en cada uno la fuerza necesaria
        int layerMask = 1 << LayerMask.NameToLayer("Track");
        RaycastHit hit;
        for (int i = 0; i < hoverPoints.Count; i++) {
            // Rayo lanzado desde el punto i hacia el eje y negativo y
            if (Physics.Raycast(hoverPoints[i], -transform.up, out hit, ship.config.hoverHeight, layerMask)) {
                // DEBUG
                if (hit.point != Vector3.zero)
                    Debug.DrawLine(hoverPoints[i], hit.point, Color.blue);
                // Si hay impacto la nave esta en el suelo
                isGrounded = true;
                // Error entre el punto de flotacion y el punto donde se ha impactado
                Vector3 errorPoint = hoverPoints[i] - hit.point;
                // Normalizacion del error entre el punto de flotacion y el punto donde se ha impactado
                Vector3 normPoint = errorPoint / errorPoint.magnitude;
                // Error entre la altura de flotacion y la distancia del impacto
                float errorValue = ship.config.hoverHeight - hit.distance;
                // Error entre el punto de error y la altura de flotacion
                float errorValue2 = errorPoint.magnitude - ship.config.hoverHeight;
                // Normalizacion ajustada de la altura de flotacion y la distancia del impacto
                float normValue = (ship.config.hoverHeight / hit.distance - 1f) * -1f;
                // Fuerza positiva a aplicar en el eje y
                float force = (normPoint * (errorValue2 * (errorValue * (-10f + normValue * 3f)))).y;
                // Ajuste negativo de la fuerza 
                force -= ship.body.GetPointVelocity(hoverPoints[i]).y * (0.3f + fallingDamper + enginePitchAmount * 0.05f);
                // Se aplica la fuerza en el eje y positivo del punto i
                ship.body.AddForceAtPosition(transform.up * force, hoverPoints[i], ForceMode.Acceleration);
                // La distancia de la nave al suelo es la distancia del impacto
                groundDistance = hit.distance;
            }
        }

        // Compensacion de la rotacion en el centro de masas de la nave
        if (Physics.Raycast(transform.position, -transform.up, out hit, ship.config.hoverHeight, layerMask)) {
            // DEBUG
            if (hit.point != Vector3.zero)
                Debug.DrawLine(transform.position, hit.point, Color.red);
            Vector3 vector = transform.right - Vector3.Dot(transform.forward, hit.normal) * hit.normal;
            float rotation = vector.x * vector.z * (Mathf.Clamp(Mathf.Abs(vector.y), 0f, 0.1f) * 10f);
            float offset = 20f + transform.InverseTransformDirection(ship.body.velocity).z * Time.deltaTime * 3f;
            transform.Rotate(Vector3.up * (rotation * Time.deltaTime) * offset);
            ship.body.AddForce(new Vector3(hit.normal.x, 0f, hit.normal.z) * Mathf.Abs(vector.x) * 22f);
        }

        // Comprobacion de que se pasa por encima de un booster
        layerMask = 1 << LayerMask.NameToLayer("Booster");
        if (Physics.Raycast(transform.position, -transform.up, out hit, ship.config.hoverHeight + 2f, layerMask)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Booster")) {
                speedDirection = hit.collider.gameObject.transform.forward * 800f;
                boostTimer = 0.27f;
                isBoosting = true;
                if (!hitBooster) {
                    hitBooster = true;
                }
            }
        }
        else {
            hitBooster = false;
        }

        // Si el temporizador del booster es mayor que cero se aplica la aceleracion
        if (boostTimer > 0f) {
            boostTimer -= Time.deltaTime;
            ship.body.AddForce(speedDirection, ForceMode.Acceleration);
        }
        else {
            isBoosting = false;
            hitBooster = false;
        }

        // Comprobacion de que se estan enlazando los boosters
        if (boostingOverride) {
            isBoosting = true;
        }
        boostingOverride = false;

        // Comprobacion del motor de alabeo 
        if (ship.input.m_PitchAxis != 0f) {
            enginePitchAmount = Mathf.Lerp(enginePitchAmount, ship.input.m_PitchAxis * 5f, Time.deltaTime * 4f);
        }
        else {
            enginePitchAmount = Mathf.Lerp(enginePitchAmount, 0f, Time.deltaTime * 4f);
        }

        // Comprobacion de cuanto torque hay que añadir
        if (isGrounded) {
            if (enginePitchAmount >= 0f) {
                ship.body.AddTorque(transform.right * (enginePitchAmount * 80f));
            }
            else {
                ship.body.AddTorque(transform.right * (enginePitchAmount * 70f));
            }
        }
        else if (enginePitchAmount >= 0f) {
            ship.body.AddTorque(transform.right * (enginePitchAmount * 45f));
            transform.Rotate(Vector3.right * 0.03f);
        }
    }

    private void ShipAcceleration() {
        float amount = ship.config.engineAmount;
        float acceleration = ship.config.engineAcceleration;

        // Si se esta acelerando se calcula la fuerza del motor, sino se decrementa
        if (ship.control.isThrusting) {
            engineGain = Mathf.Lerp(engineGain, ship.config.engineGain * 0.1f * Time.deltaTime, Time.deltaTime * 10f);
            engineAcceleration = Mathf.Lerp(engineAcceleration, acceleration, Time.deltaTime * engineGain);
            engineThrust = Mathf.Lerp(engineThrust, amount + enginePitchAmount * 25f, Time.deltaTime * (engineAcceleration * 0.1f));
            engineFalloff = 0f;
        }
        else {
            engineFalloff = Mathf.Lerp(engineFalloff, ship.config.engineFalloff * Time.deltaTime, Time.deltaTime);
            engineAcceleration = Mathf.Lerp(engineAcceleration, 0f, Time.deltaTime * engineFalloff);
            engineThrust = Mathf.Lerp(engineThrust, 0f, Time.deltaTime * engineFalloff);
            engineGain = 0f;
        }
        // Se aplica la fuerza de empuje a la nave
        ship.body.AddRelativeForce(Vector3.forward * engineThrust);
        // Velocidad
        engineSpeed = Mathf.Abs(transform.InverseTransformDirection(ship.body.velocity).z);
    }

    private void ShipDrag() {
        // Si se esta frenando se calcula la resistencia
        if (ship.input.m_LeftAirBrakeAxis != 0f || ship.input.m_RightAirBrakeAixs != 0f) {
            float resistanceAmount = Mathf.Clamp(Mathf.Abs(ship.input.m_LeftAirBrakeAxis + ship.input.m_RightAirBrakeAixs), 0f, 1f);
            airbrakeResistance = Mathf.Lerp(airbrakeResistance, Mathf.Abs(airbrakeAmount) * resistanceAmount, Time.deltaTime * (Mathf.Abs(transform.InverseTransformDirection(ship.body.velocity).x) * Time.deltaTime) * 0.15f);
            airbrakeSlip = Mathf.Lerp(airbrakeSlip, Mathf.Abs(airbrakeAmount * ship.config.slipAmount), Time.deltaTime * ship.config.slipGain);
        }
        else {
            airbrakeResistance = Mathf.Lerp(airbrakeResistance, 0f, Time.deltaTime * 4.5f);
            airbrakeSlip = Mathf.Lerp(airbrakeSlip, 0f, Time.deltaTime * ship.config.slipFalloff);
        }

        if (isGrounded) {
            airResistance = Mathf.Lerp(airResistance, 0f, Time.deltaTime * 4.5f);
            angularResistance = Mathf.Lerp(angularResistance, 0f, Time.deltaTime * 4.5f);
        }
        else {
            airResistance = Mathf.Lerp(airResistance, 0.4f, Time.deltaTime * (0.02f + enginePitchAmount * 0.003f));
            //float value = transform.InverseTransformDirection(ship.body.angularVelocity).x;
            //value = Mathf.Clamp(value, 0f, 1f);
            angularResistance = Mathf.Lerp(angularResistance, 0f, Time.deltaTime * 0.05f);
        }

        // Comprobacion de los frenos
        if (ship.input.m_RightAirBrakeAixs != 0f && ship.input.m_LeftAirBrakeAxis != 0f) {
            shipBraking = true;
            brakesFalloff = 0f;
            brakesGain = Mathf.Lerp(brakesGain, Time.deltaTime * 40f, Time.deltaTime);
            brakesAmount = Mathf.Lerp(brakesAmount, Time.deltaTime * 100f, Time.deltaTime * brakesGain);
        }
        else {
            shipBraking = false;
            brakesGain = 0f;
            brakesFalloff = Mathf.Lerp(brakesFalloff, 100f, Time.deltaTime);
            brakesAmount = Mathf.Lerp(brakesAmount, 0f, Time.deltaTime * brakesFalloff);
        }

        Vector3 Globaldirection = transform.InverseTransformDirection(ship.body.velocity);
        float localDirection = Mathf.Abs(transform.InverseTransformDirection(ship.body.velocity).z);
        Globaldirection.z *= 1f - (0.0015f + localDirection * Time.deltaTime * 0.007f);
        Globaldirection.z *= 1f - (airbrakeResistance + airResistance + angularResistance + brakesAmount);
        Globaldirection.y *= 0.999f;
        Vector3 velocity = transform.TransformDirection(Globaldirection);
        ship.body.velocity = velocity;

        // Si la nave esta en el suelo grip normal, sino se disminuye
        float grip = ship.config.grip;
        if (isGrounded) {
            grip = ship.config.grip - enginePitchAmount * 0.4f;
        }
        else {
            grip = ship.config.grip * 0.8f;
        }

        // Si el agarre es mayor que cero se compensa con los frenos, sino es 0
        grip = grip >= 0 ? grip - Mathf.Abs(airbrakeAmount * airbrakeSlip) : 0f;

        // Fuerza de frenado
        float brakeForce = -transform.InverseTransformDirection(ship.body.velocity).x - airbrakeAmount * ship.config.slipAmount;
        ship.body.AddForce(transform.right * brakeForce);

        // Fuerza de la nave
        velocity = ship.body.velocity;
        velocity.y = 0f;
        Vector3 force = transform.right * (-Vector3.Dot(transform.right, velocity) * grip);
        ship.body.AddForce(force, ForceMode.Acceleration);
    }

    private void ShipHandling() {

        // Si esta girando hacia una lado y la entrada es hacia el otro, se activa o desactiva el antibanding
        if ((ship.input.m_SteerAxis > 0f && turnAmount < 0f) || (ship.input.m_SteerAxis < 0f && turnAmount > 0f)) {
            if (!turnAntiBand) {
                bankGain = 0f;
                turnGain = 0f;
                turnAntiBand = true;
            }
        }
        else {
            turnAntiBand = false;
        }

        // Misma comprobacion que antes pero con los frenos
        float brakeAmount = ship.input.m_LeftAirBrakeAxis + ship.input.m_RightAirBrakeAixs;
        if ((brakeAmount > 0f && airbrakeAmount < 0f) || (brakeAmount < 0f && airbrakeAmount > 0f)) {
            if (!airbrakeAntiBand) {
                airbrakeGain = 0f;
                airbrakeAntiBand = true;
            }
        }
        else {
            airbrakeAntiBand = false;
        }

        // Si la nave esta girando se calculan los valores del giro
        if (ship.input.m_SteerAxis != 0f) {
            float turn = ship.config.turnAmount * Time.deltaTime * (float)Math.PI * 7.7f * 2f;
            if (Mathf.Abs(turnAmount) > Mathf.Abs(turn) / 2f) {
                turnGain = Mathf.Lerp(turnGain, ship.config.turnGain * 0.01f, Time.deltaTime * 2f);
            }
            else {
                turnGain = Mathf.Lerp(turnGain, ship.config.turnGain * 0.01f, Time.deltaTime * 1.2f);
            }

            turnAmount = Mathf.Lerp(turnAmount, ship.input.m_SteerAxis * turn, Time.deltaTime * turnGain);
            turnFalloff = 0f;
            bankGain = Mathf.Lerp(bankGain, 8f, Time.deltaTime * 1.5f);
            bankVelocity = Mathf.Lerp(bankVelocity, ship.input.m_SteerAxis * -45.0f, Time.deltaTime * bankGain);
            bankFalloff = 0f;
        }
        else {
            turnFalloff = Mathf.Lerp(turnFalloff, ship.config.turnFalloff * 0.01f, Time.deltaTime * 4f);
            turnAmount = Mathf.Lerp(turnAmount, 0f, Time.deltaTime * turnFalloff);
            turnGain = 0f;
            bankFalloff = Mathf.Lerp(bankFalloff, 10f, Time.deltaTime * 1.4f);
            bankVelocity = Mathf.Lerp(bankVelocity, ship.input.m_SteerAxis * -45.0f, Time.deltaTime * bankFalloff);
            bankGain = 0f;
        }

        bankAmount = Mathf.Lerp(bankAmount, bankVelocity, Time.deltaTime * 10f);
        float brakeForce = ship.config.airbrakeAmount * Time.deltaTime * (float)Math.PI * 1.8f;
        float brakeSpeed = transform.InverseTransformDirection(ship.body.velocity).z * Time.deltaTime * brakeForce;

        // Si se esta frenando se calculan los valores de los frenos
        if (brakeAmount != 0f) {
            airbrakeGain = Mathf.Lerp(airbrakeGain, ship.config.airbrakeGain * 0.01f, Time.deltaTime * 8f);
            airbrakeAmount = Mathf.Lerp(airbrakeAmount, brakeAmount * brakeSpeed, Time.deltaTime * (airbrakeGain + Mathf.Abs(turnAmount * ship.config.airbrakeTurnMultiplier)));
            airbrakeFalloff = 0f;
            bankAirbrake = Mathf.Lerp(bankAirbrake, -brakeAmount * 10f, Time.deltaTime * 2f);
        }
        else {
            airbrakeFalloff = Mathf.Lerp(airbrakeFalloff, ship.config.airbrakeFalloff * 0.01f, Time.deltaTime * 8f);
            airbrakeAmount = Mathf.Lerp(airbrakeAmount, 0f, Time.deltaTime * airbrakeFalloff);
            airbrakeGain = 0f;
            bankAirbrake = Mathf.Lerp(bankAirbrake, 0f, Time.deltaTime * 2f);
        }

        // Rotacion de la nave
        transform.Rotate(Vector3.up * (turnAmount + airbrakeAmount));
        // Alabeo de la nave al rotar
        ship.axis.transform.localRotation = Quaternion.Euler(0f, 0f, bankAmount + bankAirbrake + BRActual);
    }

    private void ShipAnimations() {
        // Calculo de los offsets de la flotacion
        if (Mathf.Abs(transform.InverseTransformDirection(ship.body.velocity).z) < 100f && isGrounded) {
            hoverAnimSpeed = 0.5f;
            hoverAnimAmount = 0.2f;
            hoverAnimTimer += Time.deltaTime * hoverAnimSpeed;
            hoverAnimOffset.x = Mathf.Sin(hoverAnimTimer * 1.5f) * (hoverAnimAmount * Mathf.Sin(hoverAnimTimer));
            hoverAnimOffset.y = Mathf.Cos(hoverAnimTimer * 4f) * (hoverAnimAmount / 5f);
            hoverCameraAnimOffset.x = Mathf.Sin(hoverAnimTimer * 6f) * (hoverAnimAmount / 2f);
            hoverCameraAnimOffset.y = Mathf.Cos(hoverAnimTimer * 3f) * (hoverAnimAmount / 2f);
        }
        else {
            hoverAnimTimer = 0f;
            hoverAnimOffset = Vector3.Lerp(hoverAnimOffset, Vector3.zero, Time.deltaTime);
            hoverCameraAnimOffset = Vector3.Lerp(hoverAnimOffset, Vector3.zero, Time.deltaTime);
        }
        ship.axis.transform.localPosition = hoverAnimOffset;
    }

    private void ShipBarrelRolls() {
        // Decremento en el temporizador de los barrel rolls
        if (BRBoostTimer > 0f) {
            BRBoostTimer -= Time.deltaTime;
        }

        // Si la nave esta en el suelo, reiniciar maquina de estado, no se puede hacer barrel rol
        // Sino si que se puede
        if (isGrounded) {
            BRProgress = Mathf.LerpAngle(BRProgress, 0f, Time.deltaTime * 10f);
            BRGain = 0f;
            BRActual = BRProgress;
            // Si hay exito en el barrel rol, booster
            if (BRSuccess) {
                // Reproducir sonido de barrel roll
                BRBoostTimer = 0.24f;
                BRSuccess = false;
            }
            BRLastValue = 0f;
            BRState = 0f;
            canBR = false;
            hasBR = false;
        }
        else {
            canBR = true;
        }

        // Si puede hacer un barrel roll, se 
        if (canBR) {
            BRTimer -= Time.deltaTime;
            // Temporizador a 0, no se puede hacer barrel rol
            if (BRTimer < 0f) {
                BRLastValue = 0f;
                BRState = 0f;
                BRTimer = 0f;
                //
                if (!hasBR) {
                    BRSuccess = false;
                }
            }

            // Si todavia no se ha hecho un barrel rol
            if (!hasBR) {
                BRProgress = 0f;
                BRGain = 0f;

                /*
                Barrel Rol Izquierda - Derecha
                */

                // Primer movimiento - izquierda
                if (ship.input.m_SteerAxis < 0f && BRState == 0f) {
                    BRLastValue = -1f;
                    BRTimer = 0.2f;
                    BRState += 1f;
                }

                // Segundo movimiento - derecha
                if (ship.input.m_SteerAxis > 0f && BRLastValue == -1f && BRState == 1f && BRTimer > 0f) {
                    BRLastValue = 1f;
                    BRTimer = 0.2f;
                    BRState += 1f;
                }

                // Tercer movimiento - izquierda
                if (ship.input.m_SteerAxis < 0f && BRLastValue == 1f && BRState == 2f && BRTimer > 0f) {
                    hasBR = true;
                    // Reproducir sonido de giro
                    ship.control.performedBarrelRolls++;
                }

                /* 
                Barrel Rol Derecha - Izquierda
                */

                // Primer movimiento - derecha
                if (ship.input.m_SteerAxis > 0f && BRState == 0f) {
                    BRLastValue = 1f;
                    BRTimer = 0.2f;
                    BRState += 1f;
                }

                // Segundo movimiento - izquierda
                if (ship.input.m_SteerAxis < 0f && BRLastValue == 1f && BRState == 1f && BRTimer > 0f) {
                    BRLastValue = -1f;
                    BRTimer = 0.2f;
                    BRState += 1f;
                }

                // Tercer movimiento - izquierda
                if (ship.input.m_SteerAxis > 0f && BRLastValue == -1f && BRState == 2f && BRTimer > 0f) {
                    hasBR = true;
                    // Reproducir sonido de giro
                    ship.control.performedBarrelRolls++;
                }
            }
            else {
                BRState = 0f;
                BRLastValue = 0f;

                // Si el giro es menor que 260º se avanza mas rapido en el giro que si es mayor o igual
                if (BRProgress < 260f) {
                    BRGain = Mathf.Lerp(BRGain, 1000f, Time.deltaTime * 10f);
                }
                else {
                    BRGain = Mathf.Lerp(BRGain, 200f, Time.deltaTime * 10f);
                }
                BRProgress = Mathf.MoveTowards(BRProgress, 360f, Time.deltaTime * BRGain);

                // Con un angulo mayot que 250º se considera un barrel roll exitoso
                if (BRProgress > 250f) {
                    BRSuccess = true;
                    canBR = false;
                }

                // Comprobacion de barrel roll terminado, para no pasarse de girar
                if (BRProgress > 360f) {
                    BRProgress = 360f;
                }

                // Comprobacion del estado del ultimo movimiento
                if (BRLastValue == 1f) {
                    BRActual = BRProgress;
                }
                else {
                    BRActual = -BRProgress;
                }
            }
        }
        else {
            BRLastValue = 0f;
            BRTimer = 0f;
            BRSuccess = false;
            hasBR = false;
            BRState = 0f;
        }
    }

    private void OnCollisionEnter(Collision other) {
        // Comprobacion de colision con la pared
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            float collision = Vector3.Dot(other.contacts[0].normal, transform.forward);
            Vector3 vector = transform.InverseTransformDirection(ship.body.GetPointVelocity(other.contacts[0].point));
            if ((Mathf.Abs(vector.x) >= 65f && collision > 0f) || (Mathf.Abs(vector.x) >= 65f)) {
                // Reproducir sonido de golpe con la pared
            }

            // Obtener el valor del impacto
            float impact = Vector3.Dot(other.contacts[0].normal, other.relativeVelocity);
            float hitDot = Vector3.Dot(other.contacts[0].normal, transform.forward);
            Vector3 impactDir = transform.InverseTransformPoint(other.contacts[0].point);

            float impactAllowance = (hitDot < -0.1f) ? 1.5f : 4.0f;
            if (Mathf.Abs(impact) > impactAllowance || hitDot < -0.2f) {
                isColliding = true;

                Vector3 lv = transform.InverseTransformDirection(ship.body.velocity);
                lv.x *= -0.1f;
                lv.y = 0.0f;
                lv.z *= 0.05f;
                Vector3 wv = transform.TransformDirection(lv);
                ship.body.velocity = wv;

                // Reducir la potencia y aceleracion del motor
                engineAcceleration *= 0.9f;
                engineThrust *= 0.3f;

                // Alejar la nave de la pared
                Vector3 dir = other.contacts[0].normal;
                dir.y = 0;

                Vector3 pushForce = dir * 4;
                pushForce = Vector3.ClampMagnitude(pushForce, 3.0f);
                ship.body.AddForce(pushForce, ForceMode.Impulse);

                float collisionBounce = Mathf.Abs(impact * 0.2f);
                collisionBounce = Mathf.Clamp(collisionBounce, 0.0f, 1.2f);

                ship.perfectLap = false;

            }
        }

        if (other.gameObject.tag == "Ship") {

            Vector3 lv = transform.InverseTransformDirection(ship.body.velocity);
            lv.y = 0;
            Vector3 wv = transform.TransformDirection(lv);
            if (!isGrounded)
                ship.body.velocity = wv;

            // Alejar de la otra nave
            Vector3 dir = other.contacts[0].normal;
            dir.y = 0;
            ship.body.AddForce(dir * 4.5f, ForceMode.Impulse);

            Vector3 pushDir = other.relativeVelocity;
            pushDir.x = 0.0f;
            pushDir.z = 0.0f;
            ship.body.AddForce(pushDir, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision other) {
        // Comprobacion de colision con la pared
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            ship.body.angularVelocity = Vector3.zero;
            isColliding = true;
            float collision = Vector3.Dot(other.contacts[0].normal, transform.forward);
            // Si el valor de la colision es bajo
            if (collision < 0.1f) {
                // Se reduce el empuje del motor
                if (engineThrust > 100f) {
                    engineThrust *= 0.95f;
                }

                // Se reduce la aceleracion del motor
                if (engineAcceleration > 5f) {
                    engineAcceleration *= 0.98f;
                }
                wallSurfing = false;

                // Si el valor es muy bajo, se para la nave
                if (collision < -0.8f) {
                    engineThrust = 0f;
                    engineAcceleration = 0f;
                }
            }

            // Segun el valor de la colision, se rota la nave en la direccion correspondiente
            Vector3 vector = transform.InverseTransformPoint(other.contacts[0].point);
            if (collision < 0f) {
                transform.Rotate(Vector3.up * (collision * 0.3f * (vector.x * 0.2f)));
            }
        }
    }

    private void OnCollisionExit(Collision other) {
        wallSurfing = false;
        isColliding = false;
    }
}
