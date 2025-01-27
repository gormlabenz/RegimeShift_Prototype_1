using UnityEngine;

public class SurfaceController : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    private Collider targetCollider;

    private void Start()
    {
        if (targetObject != null)
        {
            targetCollider = targetObject.GetComponent<Collider>();

            // Stelle sicher, dass das Zielobjekt ein PhysicMaterial hat
            if (targetCollider.material == null)
            {
                Debug.LogWarning("Target object has no PhysicsMaterial. Creating a new one.");
                targetCollider.material = new PhysicsMaterial();
            }
            else
            {
                Debug.Log("Target object has a PhysicsMaterial");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Prüfe ob das kollidierende Objekt im PlayingObjects Layer ist
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayingObjects"))
        {
            Debug.Log("Collision with playing object");
            // Mache das Material des Zielobjekts rutschig
            targetCollider.material.dynamicFriction = 0;
            targetCollider.material.staticFriction = 0;
        }
    }

    /* private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayingObjects"))
        {
            Debug.Log("Collision with playing object ended");
            // Setze normale Reibungswerte zurück
            targetCollider.material.dynamicFriction = 0.6f;
            targetCollider.material.staticFriction = 0.6f;
        }
    } */
}