using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum Type
    {
        A,
        B,
        C
    }

    public enum State
    {
        Moving,
        Waiting,
        Transforming,
        OnHold
    }

    public Type CurrentType { get; private set; }
    public State CurrentState { get; private set; } = State.Waiting;

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
        if (IsInState(State.Moving) && targetTransform != null)
        {
            MoveToTarget();
        }
    }

    private void UpdateVisuals()
    {
        switch (CurrentType)
        {
            case Type.A:
                meshRenderer.material.color = Color.blue;
                break;
            case Type.B:
                meshRenderer.material.color = Color.green;
                break;
            case Type.C:
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
        if (distance < currentTargetTransformer.ArrivalThreshold && IsInState(State.Moving))
        {
            OnTargetReached();
        }
    }

    private void OnTargetReached()
    {

        DisablePhysics();
        IsInState(State.Waiting);

        // Inform EcosystemController that we've arrived
        /* EcosystemController.Instance.OnResourceArrivedAtTransformer(currentTargetTransformer, this); */

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
        if (IsInState(State.Moving))
        {
            Debug.LogWarning($"Resource {name} already has a target and is moving! " + $"(Current Transformer: {currentTargetTransformer?.name}, " + $"New Transformer: {transformer.name})");
        }

        targetTransform = target;
        currentTargetTransformer = transformer;
    }

    public void SetState(State newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case State.Transforming:
                DisablePhysics();
                break;
            case State.Moving:
                EnablePhysics();
                break;
            case State.Waiting:
                DisablePhysics();
                break;
            case State.OnHold:
                DisablePhysics();
                break;
        }

        UpdateVisuals();
    }


    public void SetTypeState(Type newState)
    {
        CurrentType = newState;
        UpdateVisuals();
    }



    public bool IsInState(State state)
    {
        return CurrentState == state;
    }


}
