using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveDeformer : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveHeight = 0.5f;          //height of waves
    public float waveFrequency = 1.0f;       //frequency of waves
    public float waveSpeed = 1.0f;           //how fast waves move
    public Vector2 waveDirection = new Vector2(1, 0); //XZ direction

    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] deformedVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        baseVertices = mesh.vertices;
        deformedVertices = new Vector3[baseVertices.Length];
    }

    void Update()
    {
        float time = Time.time * waveSpeed;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float wave = Mathf.Sin(
                (v.x * waveFrequency + time * waveDirection.x) +
                (v.z * waveFrequency + time * waveDirection.y)
            );

            v.y = wave * waveHeight;
            deformedVertices[i] = v;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals(); //ensures lighting reacts properly
    }
}
