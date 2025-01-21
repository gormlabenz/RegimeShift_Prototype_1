using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // Platform tracking
    private Vector3 previousParentPosition;
    private Quaternion previousParentRotation;
    private Vector3 platformMovementDelta;
    private Vector3 platformRotationDelta;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (transform.parent != null)
        {
            previousParentPosition = transform.parent.position;
            previousParentRotation = transform.parent.rotation;
        }
    }

    private void Update()
    {
        // Zuerst die Plattformbewegung anwenden
        if (transform.parent != null)
        {
            // Position Delta berechnen
            platformMovementDelta = transform.parent.position - previousParentPosition;
            controller.Move(platformMovementDelta);

            // Rotation Delta berechnen und anwenden
            Quaternion rotationDelta = transform.parent.rotation * Quaternion.Inverse(previousParentRotation);
            Vector3 rotationDeltaEuler = rotationDelta.eulerAngles;

            // Nur anwenden wenn es tatsächlich eine Rotation gibt
            if (rotationDeltaEuler.magnitude > 0.0001f)
            {
                // Position relativ zum Parent berechnen
                Vector3 pivotPoint = transform.parent.position;
                Vector3 relativePosition = transform.position - pivotPoint;

                // Rotation um den Pivot-Punkt anwenden
                Vector3 rotatedPosition = pivotPoint + (rotationDelta * relativePosition);
                Vector3 positionDelta = rotatedPosition - transform.position;

                controller.Move(positionDelta);
            }

            // Parent-Transformation für nächsten Frame speichern
            previousParentPosition = transform.parent.position;
            previousParentRotation = transform.parent.rotation;
        }

        // Dann die eigentliche Spielerbewegung
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Bewegungsrichtung relativ zur Parent-Rotation berechnen
        Vector3 movement;
        if (transform.parent != null)
        {
            movement = transform.parent.right * horizontalInput + transform.parent.forward * verticalInput;
        }
        else
        {
            movement = transform.right * horizontalInput + transform.forward * verticalInput;
        }

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTransformParentChanged()
    {
        // Reset tracking when parent changes
        if (transform.parent != null)
        {
            previousParentPosition = transform.parent.position;
            previousParentRotation = transform.parent.rotation;
        }
    }
}