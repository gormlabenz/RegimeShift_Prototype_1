using UnityEngine;

public class EcosystemController : MonoBehaviour
{
    // list input of transform objects
    public Resource[] resources;
    public Transformer transformers;
    void Start()
    {
        foreach (var resource in resources)
        {
            resource.SetTarget(transformers.transform, transformers);
            resource.SetState(Resource.State.Moving);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
