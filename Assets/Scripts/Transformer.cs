using UnityEngine;
using System;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    public enum TransformerState
    {
        Available,      // Ready to accept and process resources
        Processing,     // Currently processing a resource
        Disabled       // Transformer is not operational
    }

    public event Action<Transformer, Resource> OnResourceProcessed;
    public event Action<Transformer, Resource> OnStartProcessing;

    [SerializeField] private ProductionTypes.TransformerType type;
    [SerializeField] private float processTime = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Queue<Resource> resourceQueue = new Queue<Resource>();
    private Resource currentResource;
    private float currentProcessTime;
    private TransformerState currentState = TransformerState.Available;

    public ProductionTypes.TransformerType Type => type;
    public float ProcessTime => processTime;
    public float ArrivalThreshold => arrivalThreshold;
    public TransformerState CurrentState => currentState;
    public int QueueCount => resourceQueue.Count;

    private void Update()
    {
        if (currentState == TransformerState.Disabled) return;

        if (currentState == TransformerState.Processing)
        {
            ProcessCurrentResource();
        }
        else if (resourceQueue.Count > 0)
        {
            StartProcessingNextResource();
        }
    }

    public void AddResourceToQueue(Resource resource)
    {
        resourceQueue.Enqueue(resource);

        if (currentState == TransformerState.Available)
        {
            StartProcessingNextResource();
        }
    }

    private void StartProcessingNextResource()
    {
        if (resourceQueue.Count == 0) return;

        currentResource = resourceQueue.Dequeue();
        currentProcessTime = 0f;
        currentState = TransformerState.Processing;

        OnStartProcessing?.Invoke(this, currentResource);
    }

    private void ProcessCurrentResource()
    {
        currentProcessTime += Time.deltaTime;

        if (currentProcessTime >= processTime)
        {
            FinishProcessingResource();
        }
    }

    private void FinishProcessingResource()
    {
        OnResourceProcessed?.Invoke(this, currentResource);

        currentResource = null;
        currentProcessTime = 0f;

        if (resourceQueue.Count > 0)
        {
            StartProcessingNextResource();
        }
        else
        {
            currentState = TransformerState.Available;
        }
    }

    public void SetDisabled(bool disabled)
    {
        currentState = disabled ? TransformerState.Disabled : TransformerState.Available;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, arrivalThreshold);
    }
}