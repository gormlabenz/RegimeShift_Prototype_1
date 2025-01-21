using UnityEngine;
using System;

public class TargetInfo
{
    public Vector3 Position { get; set; }
    public float Distance { get; set; }
    public bool IsReached { get; set; }
}

public class Resource : MonoBehaviour
{
    public event Action<Resource> OnTargetReached;

    public enum ResourceType
    {
        TypeA,
        TypeB,
        TypeC

    }

    public enum ResourceState
    {
        Moving,
        Waiting,
        Transforming,
        Unassigned
    }

    public ResourceType CurrentType { get; private set; }
    public ResourceState CurrentState { get; private set; } = ResourceState.Waiting;

    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private Transform targetTransform;
    private Transformer currentTargetTransformer;

    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
    private void FixedUpdate()
    {
        if (IsInState(ResourceState.Moving) && targetTransform != null)
        {
            MoveToTarget();
        }
    }

    private void UpdateVisuals()
    {
        switch (CurrentType)
        {
            case ResourceType.TypeA:
                meshRenderer.material.color = Color.blue;
                break;
            case ResourceType.TypeB:
                meshRenderer.material.color = Color.green;
                break;
            case ResourceType.TypeC:
                meshRenderer.material.color = Color.red;
                break;
        }
    }

    private void MoveToTarget()
    {
        float step = moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetTransform.position, step);

        // Use MovePosition for kinematic rigidbody movement
        if (rb.isKinematic)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }

        float distance = Vector3.Distance(transform.position, targetTransform.position);
        if (distance < currentTargetTransformer.ArrivalThreshold && IsInState(ResourceState.Moving))
        {
            DisablePhysics();
            SetState(ResourceState.Waiting);
            OnTargetReached?.Invoke(this);
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


    public void SetTarget(Transform target, Transformer transformer)
    {
        if (IsInState(ResourceState.Moving))
        {
            Debug.LogWarning($"Resource {name} already has a target and is moving! " + $"(Current Transformer: {currentTargetTransformer?.name}, " + $"New Transformer: {transformer.name})");
        }

        targetTransform = target;
        currentTargetTransformer = transformer;
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


    public void SetTypeState(ResourceType newState)
    {
        CurrentType = newState;
        UpdateVisuals();
    }



    public bool IsInState(ResourceState state)
    {
        return CurrentState == state;
    }


}
