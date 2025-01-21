using UnityEngine;

public class PlayingFieldController : MonoBehaviour
{
    [Header("Grundparameter für die Bewegung")]
    public float tiltMultiplier = 2.0f;       // Verstärkt/Verringert die generelle Neigung
    public float maxTiltAngle = 45f;          // Maximaler Neigungswinkel in Grad
    public float sphereRadius = 5f;             // Radius der Kugel

    [Header("Feder & Dämpfung")]
    public float springStiffness = 10f;       // Federkonstante
    public float dampingFactor = 0.5f;        // Dämpfung


    [Header("Referenzen")]
    public GameObject inputGameObject;        // Eltern-Objekt, das die Objekte mit Weight.cs enthält

    private Rigidbody rb;
    private Vector3 currentAngularVelocity;
    private Vector3 targetRotation;
    private Quaternion previousRotation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.eulerAngles;

        // Überprüfen, ob alle nötigen Objekte gesetzt sind
        if (inputGameObject == null)
        {
            Debug.LogError("Bitte inputGameObject zuweisen!");
            enabled = false;
            return;
        }

        previousRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Hier die kinematische Roll-Bewegung anwenden
        ApplyManualRolling();
    }

    void FixedUpdate()
    {
        CalculateCenterOfMass();
        UpdatePhysics();
    }

    /// <summary>
    /// Findet alle Weight-Komponenten in den Kindern von inputGameObject 
    /// und berechnet deren gewichteten Schwerpunkt.
    /// </summary>
    private void CalculateCenterOfMass()
    {
        // Alle Weight-Komponenten in den Kindern ermitteln
        Weight[] weights = inputGameObject.GetComponentsInChildren<Weight>(true);

        float totalWeight = 0f;
        Vector3 weightedPosition = Vector3.zero;

        foreach (Weight w in weights)
        {
            weightedPosition += w.transform.position * w.mass;
            totalWeight += w.mass;
        }

        // Nur wenn tatsächlich Gesamtmasse vorhanden ist
        if (totalWeight > 0f)
        {
            Vector3 centerOfMass = weightedPosition / totalWeight;
            UpdateTilt(centerOfMass, totalWeight);
        }
        else
        {
            // Keine Weight-Objekte vorhanden, Neigung zurücksetzen
            targetRotation = Vector3.zero;
        }
    }

    /// <summary>
    /// Berechnet die Zielrotation (targetRotation) basierend auf dem Schwerpunkt.
    /// </summary>
    private void UpdateTilt(Vector3 centerOfMass, float totalWeight)
    {
        // Berechne die Neigungsrichtung relativ zur aktuellen Position
        Vector3 tiltDirection = centerOfMass - transform.position;

        // Distanz in x/z-Ebene (scheibenförmige Spielfläche)
        float distanceFromCenter = new Vector2(tiltDirection.x, tiltDirection.z).magnitude;
        float distanceFactor = Mathf.Clamp01(distanceFromCenter / sphereRadius);

        // Nur in x/z-Richtung neigen, Y ausklammern
        Vector3 tiltVector = new Vector3(tiltDirection.x, 0, tiltDirection.z).normalized;

        // Zielrotation berechnen (einfaches Schema, X und Z kippen)
        targetRotation = new Vector3(
            tiltVector.z * totalWeight * tiltMultiplier * distanceFactor,
            0f,
            -tiltVector.x * totalWeight * tiltMultiplier * distanceFactor
        );

        // Begrenze die maximale Neigung
        targetRotation = Vector3.ClampMagnitude(targetRotation, maxTiltAngle);
    }

    /// <summary>
    /// Wirkt mit einer Feder-Dämpfungs-Logik auf die Rotation und bewegt das RigidBody.
    /// </summary>
    private void UpdatePhysics()
    {
        // Winkel-Differenz zwischen momentaner und Ziel-Rotation
        Vector3 rotationDifference = targetRotation - transform.eulerAngles;

        // Winkel in [-180, 180] normalisieren
        rotationDifference = new Vector3(
            NormalizeAngle(rotationDifference.x),
            NormalizeAngle(rotationDifference.y),
            NormalizeAngle(rotationDifference.z)
        );

        // Federkraft proportional zur Winkel-Differenz
        Vector3 springForce = rotationDifference * springStiffness;
        // Dämpfungskraft proportional zur aktuellen Winkelgeschwindigkeit
        Vector3 dampingForce = -currentAngularVelocity * dampingFactor;

        // Gesamtkraft = Feder + Dämpfung
        Vector3 totalForce = springForce + dampingForce;

        // Winkelgeschwindigkeit anpassen
        currentAngularVelocity += totalForce * Time.fixedDeltaTime;

        // Neue Rotation errechnen
        Vector3 newRotation = transform.eulerAngles + currentAngularVelocity * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(newRotation));
    }

    /// <summary>
    /// Normalisiert Winkel in den Bereich [-180, 180].
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// Berechnet anhand der Delta-Rotation den Weg, den die Kugel ohne Rutschen
    /// auf dem Boden zurücklegen würde, und verschiebt sie entsprechend.
    /// </summary>
    private void ApplyManualRolling()
    {
        Quaternion currentRotation = transform.rotation;

        // 1) Die relative Drehung ("deltaRotation") zwischen diesem Frame und dem Vor-Frame:
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);

        // 2) Drehung in Winkel + Achse umwandeln
        deltaRotation.ToAngleAxis(out float deltaAngleDeg, out Vector3 axis);

        // Winkel in den Bereich [-180, 180] normalisieren
        if (deltaAngleDeg > 180f)
            deltaAngleDeg -= 360f;

        // Grad in Radianten
        float deltaAngleRad = deltaAngleDeg * Mathf.Deg2Rad;

        // 3) Nur der horizontale Anteil der Achse bewirkt ein "Abrollen auf dem Boden"
        // (Achse senkrecht zur Up-Achse => echtes Abrollen in der Ebene)
        Vector3 axisOnPlane = Vector3.ProjectOnPlane(axis, Vector3.up).normalized;

        // 4) Rollbedingung: Weg = Radius * Winkel
        float distance = sphereRadius * deltaAngleRad;

        // Solange die AchseOnPlane nicht vernachlässigbar ist...
        if (axisOnPlane.sqrMagnitude > 1e-6f)
        {
            // Die Bewegungsrichtung in der Horizontalebene ist das Kreuzprodukt 
            // "Achse -> Up" (ggf. das Vorzeichen anpassen, falls es falsch herum rollt).
            Vector3 moveDir = Vector3.Cross(axisOnPlane, Vector3.up);

            // Wir verschieben den Mittelpunkt um diese Distanz in Weltkoordinaten
            transform.position += moveDir * distance;
        }

        // 5) Kugel zwingen, am Boden zu bleiben (Mittelpunkt auf y = sphereRadius)
        Vector3 pos = transform.position;
        transform.position = pos;

        // Rotation für den nächsten Frame merken
        previousRotation = currentRotation;
    }

}