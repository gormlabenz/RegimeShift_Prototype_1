using UnityEngine;

public class EcosystemController : MonoBehaviour
{


    public Transformer[] transformers;
    public Resource resourcePrefab;


    void Start()
    {
        foreach (var transformer in transformers)
        {
            Resource resource = Instantiate(resourcePrefab, transformer.transform.position, Quaternion.identity);

            ProductionTypes.ResourceType transformerOutputType = ProductionTypes.GetTransformerOutputType(transformer.Type);

            resource.SetTypeState(transformerOutputType);
            resource.OnTargetReached += HandleResourceReachedTransformerTarget;
            FindNextTransformerTarget(resource);

            transformer.OnResourceProcessed += HandleOnResourceProcessed;
            transformer.OnStartProcessing += HandleOnStartProcessing;
        }

    }

    private void HandleResourceReachedTransformerTarget(Resource resource, Transformer transformer)
    {
        Debug.Log($"Resource reached its target!");
        transformer.AddResourceToQueue(resource);
    }

    private void HandleOnResourceProcessed(Transformer transformer, Resource resource)
    {
        ProductionTypes.ResourceType nextType = ProductionTypes.GetTransformerOutputType(transformer.Type);

        resource.SetTypeState(nextType);
        FindNextTransformerTarget(resource);
    }

    private void HandleOnStartProcessing(Transformer transformer, Resource resource)
    {
        resource.SetState(Resource.ResourceState.Transforming);
    }


    private void FindNextTransformerTarget(Resource resource)
    {
        ProductionTypes.ResourceType currentType = resource.CurrentType;

        ProductionTypes.ResourceType nextType = ProductionTypes.GetNextResourceType(currentType);

        foreach (var transformer in transformers)
        {
            if (ProductionTypes.GetTransformerInputType(transformer.Type) == nextType)
            {
                resource.SetTargetTransformer(transformer);
                return;
            }
        }

        Debug.LogWarning($"No transformer found for resource {resource.name} with type {nextType}");
    }
}
