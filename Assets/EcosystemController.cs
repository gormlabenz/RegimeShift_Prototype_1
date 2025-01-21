using UnityEngine;

public class EcosystemController : MonoBehaviour
{


    public Transformer[] transformers;
    public Resource resourcePrefab;


    void Start()
    {
        foreach (var transformer in transformers)
        {
            // create new resource object at Transformer transform
            Resource resource = Instantiate(resourcePrefab, transformer.transform.position, Quaternion.identity);
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
        resource.SetTargetTransformer(transformerWithLeastResources);
    }
}
