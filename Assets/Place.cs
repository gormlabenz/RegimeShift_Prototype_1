using UnityEngine;

public class Place : MonoBehaviour
{
    public enum PlaceState
    {
        Free,
        Planted,
        Occupied
    }

    public PlaceState state;
    public Transformer transformerPrefab;
    [SerializeField] private ProductionTypes.TransformerType type;
    void Start()
    {
        Transformer transformer = Instantiate(transformerPrefab, transform.position, Quaternion.identity, transform);

        transformer.setType(type);
    }


    void Update()
    {

    }
}
