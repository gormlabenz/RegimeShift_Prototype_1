using UnityEngine;
using System;

public class Resource : MonoBehaviour
{
    public event Action<Resource, Transformer> OnTargetReached;

    public enum ResourceState
    {
        Moving,
        Waiting,
        Transforming,
        Unassigned
    }

    public ProductionTypes.ResourceType CurrentType { get; private set; }
    public ResourceState CurrentState { get; private set; } = ResourceState.Waiting;

    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private Transformer currentTargetTransformer;
    private Transform currentTargetTransformerTransform;

    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
    private void FixedUpdate()
    {
        if (IsInState(ResourceState.Moving) && currentTargetTransformerTransform != null)
        {
            MoveToTarget();
        }
    }

    private void UpdateVisuals()
    {
        switch (CurrentType)
        {
            case ProductionTypes.ResourceType.A:
                meshRenderer.material.color = Color.blue;
                break;
            case ProductionTypes.ResourceType.B:
                meshRenderer.material.color = Color.green;
                break;
            case ProductionTypes.ResourceType.C:
                meshRenderer.material.color = Color.red;
                break;
        }
    }

    private void MoveToTarget()
    {
        float step = moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTargetTransformerTransform.position, step);

        // Use MovePosition for kinematic rigidbody movement
        if (rb.isKinematic)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }

        float distance = Vector3.Distance(transform.position, currentTargetTransformerTransform.position);
        if (distance < currentTargetTransformer.ArrivalThreshold && IsInState(ResourceState.Moving))
        {
            DisablePhysics();
            SetState(ResourceState.Waiting);
            OnTargetReached?.Invoke(this, currentTargetTransformer);
        }
    }


    private void DisablePhysics()
    {
        rb.isKinematic = true;
    }

    private void EnablePhysics()
    {
        rb.isKinematic = false;
    }


    public void SetTargetTransformer(Transformer transformer)
    {
        if (IsInState(ResourceState.Moving))
        {
            Debug.LogWarning($"Resource {name} already has a target and is moving! " + $"(Current Transformer: {currentTargetTransformer?.name}, " + $"New Transformer: {transformer.name})");
        }

        currentTargetTransformer = transformer;
        currentTargetTransformerTransform = transformer.transform;
        SetState(ResourceState.Moving);
    }

    public void SetState(ResourceState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case ResourceState.Transforming:
                DisablePhysics();
                break;
            case ResourceState.Moving:
                EnablePhysics();
                break;
            case ResourceState.Waiting:
                DisablePhysics();
                break;
            case ResourceState.Unassigned:
                DisablePhysics();
                break;
        }

        UpdateVisuals();
    }


    public void SetTypeState(ProductionTypes.ResourceType newState)
    {
        CurrentType = newState;
        UpdateVisuals();
    }



    public bool IsInState(ResourceState state)
    {
        return CurrentState == state;
    }


}
