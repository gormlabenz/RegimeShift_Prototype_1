using UnityEngine;

public class Transformer : MonoBehaviour
{
    public enum TransformerType
    {
        TypeA,
        TypeB,
        TypeC
    }

    public enum TransformerState
    {
        Free,
        Transforming,
        Blocked
    }
    [SerializeField] private TransformerType type;
    [SerializeField] private float processTime = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    public TransformerType Type => type;
    public float ProcessTime => processTime;

    public float ArrivalThreshold => arrivalThreshold;
    private TransformerState currentState = TransformerState.Free;
    public TransformerState CurrentState => currentState;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
