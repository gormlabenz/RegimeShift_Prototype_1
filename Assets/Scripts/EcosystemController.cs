using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EcosystemController : MonoBehaviour
{
    List<Transformer> enabledTransformer = new List<Transformer>();
    public Resource resourcePrefab;
    public Transform resourceParent;

    void Start()
    {
        var transformers = GetComponentsInChildren<Transformer>();

        foreach (var transformer in transformers)
        {
            if (!transformer.gameObject.activeSelf)
            {
                continue;
            }
            else
            {
                RegisterTransformer(transformer);
            }
        }

        foreach (var transformer in enabledTransformer)
        {
            Resource resource = Instantiate(resourcePrefab, transformer.transform.position, Quaternion.identity, resourceParent);

            ProductionTypes.ResourceType transformerOutputType = ProductionTypes.GetTransformerOutputType(transformer.Type);

            resource.SetTypeState(transformerOutputType);

            SubscribeToResourceEvents(resource);

            Transformer nextTransformer = GetNextTransformerTarget(resource);
            if (nextTransformer != null)
            {
                resource.SetTargetTransformer(nextTransformer);
                nextTransformer.AddResourceToMovingList(resource);
            }
        }
    }

    private void RegisterTransformer(Transformer transformer)
    {
        enabledTransformer.Add(transformer);
        SubscribeToTransformerEvents(transformer);
        Debug.Log($"Transformer {transformer.name} is available");
    }

    private void SubscribeToTransformerEvents(Transformer transformer)
    {
        transformer.OnResourceTransformed += HandleOnResourceTransformed;
        transformer.OnStartTransforming += HandleOnStartTransforming;
        transformer.OnTransformerDisabled += HandleOnTransformerDisabled;
        transformer.OnTransformerEnabled += HandleOnTransformerEnabled;
    }

    private void UnsubscribeFromTransformerEvents(Transformer transformer)
    {
        transformer.OnResourceTransformed -= HandleOnResourceTransformed;
        transformer.OnStartTransforming -= HandleOnStartTransforming;
        transformer.OnTransformerDisabled -= HandleOnTransformerDisabled;
        transformer.OnTransformerEnabled -= HandleOnTransformerEnabled;
    }

    private void SubscribeToResourceEvents(Resource resource)
    {
        resource.OnTargetReached += HandleResourceReachedTransformerTarget;
    }

    private void UnsubscribeFromResourceEvents(Resource resource)
    {
        resource.OnTargetReached -= HandleResourceReachedTransformerTarget;
    }


    private void HandleResourceReachedTransformerTarget(Resource resource, Transformer transformer)
    {
        transformer.RemoveResourceFromMovingList(resource);
        transformer.AddResourceToTransformingQueue(resource);
        resource.SetState(Resource.ResourceState.Waiting);

        resource.transform.SetParent(transformer.transform, worldPositionStays: true);
    }

    private void HandleOnStartTransforming(Transformer transformer, Resource resource)
    {
        resource.SetState(Resource.ResourceState.Transforming);
    }

    private void HandleOnResourceTransformed(Transformer transformer, Resource resource)
    {
        ProductionTypes.ResourceType nextType = ProductionTypes.GetTransformerOutputType(transformer.Type);
        resource.SetTypeState(nextType);


        resource.transform.SetParent(resourceParent, worldPositionStays: true);

        Transformer nextTransformer = GetNextTransformerTarget(resource);
        if (nextTransformer != null)
        {
            resource.SetTargetTransformer(nextTransformer);
            nextTransformer.AddResourceToMovingList(resource);
        }
    }
    private void HandleOnTransformerDisabled(Transformer transformer, Resource[] resources)
    {
        enabledTransformer.Remove(transformer);

        foreach (var resource in resources)
        {
            resource.SetState(Resource.ResourceState.Unassigned);
            Transformer nextTransformer = GetNextTransformerTarget(resource);
            if (nextTransformer != null)
            {
                resource.SetState(Resource.ResourceState.Moving);
                resource.SetTargetTransformer(nextTransformer);
                nextTransformer.AddResourceToMovingList(resource);
            }
        }
    }

    private void HandleOnTransformerEnabled(Transformer transformer)
    {
        if (!enabledTransformer.Contains(transformer))
        {
            enabledTransformer.Add(transformer);
        }
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

    private void OnDestroy()
    {
        foreach (var transformer in enabledTransformer.ToList())
        {
            UnsubscribeFromTransformerEvents(transformer);
        }
    }
}
