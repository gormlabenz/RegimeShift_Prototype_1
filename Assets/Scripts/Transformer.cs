using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Transformer : MonoBehaviour
{
    public enum TransformerState
    {
        Available,      // Ready to accept and transform resources
        Transforming,   // Currently transforming a resource
        Disabled        // Transformer is not operational
    }

    public event Action<Transformer, Resource> OnResourceTransformed;
    public event Action<Transformer, Resource> OnStartTransforming;

    [SerializeField] private ProductionTypes.TransformerType type;
    [SerializeField] private float transformTime = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Queue<Resource> transformingResourceQueue = new Queue<Resource>();
    private List<Resource> movingResources = new List<Resource>();
    private Resource currentResource;
    private float currentTransformTime;
    private TransformerState currentState = TransformerState.Available;

    public ProductionTypes.TransformerType Type => type;
    public float TransformTime => transformTime;
    public float ArrivalThreshold => arrivalThreshold;
    public TransformerState CurrentState => currentState;

    public float TimeUntilAvailable
    {
        get
        {
            float queueTime = transformingResourceQueue.Count * transformTime;
            float movingResourcesTime = movingResources.Count * transformTime;

            if (currentState == TransformerState.Transforming)
            {
                return (transformTime - currentTransformTime) + queueTime + movingResourcesTime;
            }

            return queueTime + movingResourcesTime;
        }
    }

    private void Update()
    {
        if (currentState == TransformerState.Disabled) return;

        if (currentState == TransformerState.Transforming)
        {
            TransformCurrentResource();
        }
        else if (transformingResourceQueue.Count > 0)
        {
            StartTransformingNextResource();
        }
    }

    public void AddResourceToMovingList(Resource resource)
    {
        movingResources.Add(resource);
    }

    public bool RemoveResourceFromMovingList(Resource resource)
    {
        return movingResources.Remove(resource);
    }

    public bool IsResourceInMovingList(Resource resource)
    {
        return movingResources.Contains(resource);
    }

    public void AddResourceToTransformingQueue(Resource resource)
    {
        transformingResourceQueue.Enqueue(resource);

        if (currentState == TransformerState.Available)
        {
            StartTransformingNextResource();
        }
    }

    private void StartTransformingNextResource()
    {
        if (transformingResourceQueue.Count == 0) return;

        currentResource = transformingResourceQueue.Dequeue();
        currentTransformTime = 0f;
        currentState = TransformerState.Transforming;

        OnStartTransforming?.Invoke(this, currentResource);
    }

    private void TransformCurrentResource()
    {
        currentTransformTime += Time.deltaTime;

        if (currentTransformTime >= transformTime)
        {
            FinishTransformingResource();
        }
    }

    private void FinishTransformingResource()
    {
        OnResourceTransformed?.Invoke(this, currentResource);

        currentResource = null;
        currentTransformTime = 0f;

        if (transformingResourceQueue.Count > 0)
        {
            StartTransformingNextResource();
        }
        else
        {
            currentState = TransformerState.Available;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, arrivalThreshold);
    }

    public void SetDisabled(bool disabled)
    {
        currentState = disabled ? TransformerState.Disabled : TransformerState.Available;
    }
}