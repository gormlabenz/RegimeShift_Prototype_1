using UnityEngine;

public class EcosystemController : MonoBehaviour
{
    // list input of transform objects
    public Resource[] resources;
    public Transformer[] transformers;
    void Start()
    {
        foreach (var resource in resources)
        {
            resource.SetTarget(transformers[0].transform, transformers[0]);
            resource.OnTargetReached += HandleResourceReachedTarget;

        }
    }

    private void HandleResourceReachedTarget(Resource resource)
    {
        Debug.Log($"Resource reached its target!");
        AssignNewTarget(resource); // Optional: Neues Ziel zuweisen
    }

    private void AssignNewTarget(Resource resource)
    {
        // Find transformer with the least amount of resources
        Transformer transformerWithLeastResources = transformers[1];

        resource.SetTarget(transformerWithLeastResources.transform, transformerWithLeastResources);

    }
}
