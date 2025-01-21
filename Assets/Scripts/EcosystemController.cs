using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EcosystemController : MonoBehaviour
{



    List<Transformer> enabledTransformer = new List<Transformer>();

    public Resource resourcePrefab;


    void Start()
    {
        var transformers = GetComponentsInChildren<Transformer>();

        foreach (var transformer in transformers)
        {
            if (!transformer.gameObject.activeSelf) continue;
            enabledTransformer.Add(transformer);
            Debug.Log($"Transformer {transformer.name} is available");
        }

        foreach (var transformer in enabledTransformer)
        {
            Resource resource = Instantiate(resourcePrefab, transformer.transform.position, Quaternion.identity);

            ProductionTypes.ResourceType transformerOutputType = ProductionTypes.GetTransformerOutputType(transformer.Type);

            resource.SetTypeState(transformerOutputType);
            resource.OnTargetReached += HandleResourceReachedTransformerTarget;

            Transformer nextTransformer = GetNextTransformerTarget(resource);

            if (nextTransformer != null)
            {
                resource.SetTargetTransformer(nextTransformer);
                transformer.AddResourceToMovingList(resource);
            }

            transformer.OnResourceTransformed += HandleOnResourceTransformed;
            transformer.OnStartTransforming += HandleOnStartTransforming;
        }
    }

    private void HandleResourceReachedTransformerTarget(Resource resource, Transformer transformer)
    {
        Debug.Log($"Resource reached its target!");
        transformer.RemoveResourceFromMovingList(resource);
        transformer.AddResourceToTransformingQueue(resource);
    }

    private void HandleOnResourceTransformed(Transformer transformer, Resource resource)
    {
        ProductionTypes.ResourceType nextType = ProductionTypes.GetTransformerOutputType(transformer.Type);

        resource.SetTypeState(nextType);

        Transformer nextTransformer = GetNextTransformerTarget(resource);

        if (nextTransformer != null)
        {
            resource.SetTargetTransformer(nextTransformer);
            nextTransformer.AddResourceToMovingList(resource);
        }
    }

    private void HandleOnStartTransforming(Transformer transformer, Resource resource)
    {
        resource.SetState(Resource.ResourceState.Transforming);
    }


    private Transformer GetNextTransformerTarget(Resource resource)
    {
        ProductionTypes.ResourceType nextType = ProductionTypes.GetNextResourceType(resource.CurrentType);

        var nextTransformer =
            (from transformer in enabledTransformer
             where ProductionTypes.GetTransformerInputType(transformer.Type) == nextType
             orderby transformer.TimeUntilAvailable
             select transformer).FirstOrDefault();

        if (nextTransformer != null)
        {
            return nextTransformer;
        }
        else
        {
            Debug.LogWarning($"No transformer found for resource {resource.name} with type {nextType}");
            return null;
        }
    }
}
