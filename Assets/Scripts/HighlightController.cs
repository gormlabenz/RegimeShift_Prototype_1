using UnityEngine;

public class HighlightController : MonoBehaviour
{

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float selectRadius = 5f;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isHighlighted = false;
    private GameObject player;

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.gameObject;
        }
        else
        {
            Debug.LogError("Player not found in scene!");
        }
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            originalMaterial = meshRenderer.material;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found!");
        }
    }

    private void Update()
    {
        if (!player || !meshRenderer) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);


        if (distance <= selectRadius && !isHighlighted)
        {
            meshRenderer.material = highlightMaterial;
            isHighlighted = true;
        }
        else if (distance > selectRadius && isHighlighted)
        {
            meshRenderer.material = originalMaterial;
            isHighlighted = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the selection radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, selectRadius);
    }
}