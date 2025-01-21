using UnityEngine;

public class EcosystemController : MonoBehaviour
{

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
        AssignNewTarget(resource);
    }

    private void AssignNewTarget(Resource resource)
    {
        Transformer transformerWithLeastResources = transformers[1];

        resource.SetTarget(transformerWithLeastResources.transform, transformerWithLeastResources);

    }
}
