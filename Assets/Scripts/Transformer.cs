using UnityEngine;
using System;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    public enum TransformerState
    {
        Available,      // Ready to accept and transform resources
        Transforming,   // Currently transforming a resource
        Disabled,       // Transformer is not operational
        Lifted,         // Transformer is lifted
    }

    public event Action<Transformer, Resource> OnResourceTransformed;
    public event Action<Transformer, Resource> OnStartTransforming;
    public event Action<Transformer, Resource[]> OnTransformerDisabled;
    public event Action<Transformer> OnTransformerEnabled;
    public event Action<Transformer, Resource[]> OnTransformerLifted;


    [SerializeField] private ProductionTypes.TransformerType type;
    [SerializeField] private float transformTime = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    [SerializeField] private float resourceStackSpacing = 0.5f;
    [SerializeField] private Vector3 resourceStackOffset = Vector3.up;

    private Queue<Resource> transformingResourceQueue = new Queue<Resource>();
    private List<Resource> movingResources = new List<Resource>();
    private Resource currentResource;
    private float currentTransformTime;
    private TransformerState currentState = TransformerState.Available;

    public ProductionTypes.TransformerType Type => type;
    public float TransformTime => transformTime;
    public float ArrivalThreshold => arrivalThreshold;
    public TransformerState CurrentState => currentState;


    private ObjectNote noteComponent;

    private void Start()
    {
        noteComponent = GetComponent<ObjectNote>();
    }


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

    private void SetLabel()
    {
        if (noteComponent != null)
        {
            noteComponent.NoteText = $"Queue: {transformingResourceQueue.Count}\n" +
                                   $"Moving: {movingResources.Count}\n" +
                                   $"CurrentResource: {(currentResource != null ? "1" : "0")}\n" +
                                   $"Time: {TimeUntilAvailable}";
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


    private void OnDisable()
    {
        currentState = TransformerState.Disabled;
        var resources = ClearResources();
        OnTransformerDisabled?.Invoke(this, resources);
    }

    public void OnLifted()
    {
        currentState = TransformerState.Lifted;
        var resources = ClearResources();
        OnTransformerLifted?.Invoke(this, resources);
    }

    private void OnEnable()
    {
        currentState = TransformerState.Available;
        OnTransformerEnabled?.Invoke(this);
    }

    public void AddResourceToMovingList(Resource resource)
    {
        movingResources.Add(resource);
        SetLabel();
    }

    public void RemoveResourceFromMovingList(Resource resource)
    {
        movingResources.Remove(resource);
        SetLabel();
    }

    public bool IsResourceInMovingList(Resource resource)
    {

        return movingResources.Contains(resource);
    }

    public void AddResourceToTransformingQueue(Resource resource)
    {
        transformingResourceQueue.Enqueue(resource);
        SetResourcePositionsInQueue();
        SetLabel();

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

        SetResourcePositionsInQueue();

        OnStartTransforming?.Invoke(this, currentResource);
        SetLabel();
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
        SetLabel();
    }

    private void SetResourcePositionsInQueue()
    {
        if (currentResource != null)
        {
            currentResource.transform.position = transform.position;
        }

        Vector3 queueStartPosition = transform.position + resourceStackOffset * resourceStackSpacing;

        Resource[] queuedResources = transformingResourceQueue.ToArray();

        for (int i = 0; i < queuedResources.Length; i++)
        {
            if (queuedResources[i] != null)
            {
                Vector3 targetPosition = queueStartPosition + (resourceStackOffset * resourceStackSpacing * i);
                queuedResources[i].transform.position = targetPosition;
            }
        }
    }

    private Resource[] ClearResources()
    {
        var allResources = new List<Resource>(transformingResourceQueue);
        allResources.AddRange(movingResources);

        if (currentResource != null)
        {
            allResources.Add(currentResource);
        }

        transformingResourceQueue.Clear();
        movingResources.Clear();
        currentResource = null;

        return allResources.ToArray();
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

    public void setType(ProductionTypes.TransformerType type)
    {
        this.type = type;
    }
}