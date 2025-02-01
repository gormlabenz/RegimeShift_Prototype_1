using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Highlight Settings")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private MeshRenderer meshRenderer;
    private bool isHighlighted = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (defaultMaterial == null)
        {
            defaultMaterial = meshRenderer.material;
        }
    }

    public void OnFocusEnter()
    {
        if (!isHighlighted && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
            isHighlighted = true;
        }
    }

    public void OnFocusExit()
    {
        if (isHighlighted && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
            isHighlighted = false;
        }
    }

    public void Interact(PlayerInteractionController player)
    {
        Debug.Log($"Interacting with {gameObject.name}");
        // Hier kommt die spezifische Interaktionslogik hin
        // z.B. Objekt aufheben, Tür öffnen, etc.
    }
}