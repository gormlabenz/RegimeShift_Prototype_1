using UnityEngine;
using System.Collections.Generic;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Debug")]
    [SerializeField] private bool showDebugSphere = true;

    private IInteractable nearestInteractable;
    private readonly Collider[] hitColliders = new Collider[10]; // Array-Pool für Performance

    private void Update()
    {
        FindNearestInteractable();
        HandleInteractionInput();
    }

    private void FindNearestInteractable()
    {
        // Physics.OverlapSphereNonAlloc ist performanter als OverlapSphere
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, hitColliders, interactableLayer);

        float nearestDistance = float.MaxValue;
        IInteractable nearestObject = null;

        for (int i = 0; i < numColliders; i++)
        {
            var interactable = hitColliders[i].GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = interactable;
            }
        }

        // Wenn sich das nächste Objekt geändert hat
        if (nearestObject != nearestInteractable)
        {
            // Highlight vom alten Objekt entfernen
            if (nearestInteractable != null)
            {
                nearestInteractable.OnFocusExit();
            }

            // Neues Objekt highlighten
            nearestInteractable = nearestObject;
            if (nearestInteractable != null)
            {
                nearestInteractable.OnFocusEnter();
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && nearestInteractable != null)
        {
            nearestInteractable.Interact(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugSphere) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

// Interface für interagierbare Objekte
public interface IInteractable
{
    void Interact(PlayerInteractionController player);
    void OnFocusEnter();
    void OnFocusExit();
}