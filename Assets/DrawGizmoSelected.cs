using UnityEngine;

public class DrawGizmoSelected : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmosSelected()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Gizmos.color = Color.red;
        // lossyScale berücksichtigt die gesamte Hierarchie-Scale
        Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.lossyScale);
    }
}
