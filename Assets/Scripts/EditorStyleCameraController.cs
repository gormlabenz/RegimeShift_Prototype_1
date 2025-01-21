using UnityEngine;

public class EditorStyleCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 20f;
    public float rotationSpeed = 2.0f;
    public float zoomSpeed = 5.0f;

    private Vector3 lastMousePosition;
    private bool isPanning;
    private bool isRotating;

    void Update()
    {
        HandleInput();
        HandlePanning();
        HandleRotation();
        HandleZoom();
    }

    void HandleInput()
    {
        // Panning mit mittlerer Maustaste oder Alt + linke Maustaste
        if (Input.GetMouseButtonDown(2) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0)))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2) || (!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonUp(0)))
        {
            isPanning = false;
        }

        // Rotation mit rechter Maustaste
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }
    }

    void HandlePanning()
    {
        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Bewegung in der Kamera-Ebene
            Vector3 movement = transform.right * (-mouseDelta.x) + transform.up * (-mouseDelta.y);
            movement *= panSpeed * Time.deltaTime;

            // Skaliere Bewegung basierend auf Distanz zur Szene
            float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);
            movement *= distanceFromOrigin / 100f;

            transform.position += movement;
            lastMousePosition = Input.mousePosition;
        }
    }

    void HandleRotation()
    {
        if (isRotating)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            transform.rotation *= Quaternion.Euler(
                -mouseDelta.y * rotationSpeed * Time.deltaTime,
                mouseDelta.x * rotationSpeed * Time.deltaTime,
                0
            );

            lastMousePosition = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Skaliere Zoom basierend auf Distanz zur Szene
        float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);
        float zoomAmount = scroll * zoomSpeed * distanceFromOrigin * 0.3f;

        transform.position += transform.forward * zoomAmount;
    }
}