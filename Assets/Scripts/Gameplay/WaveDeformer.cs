using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveDeformer : MonoBehaviour
{
    [Header("Wave Settings")]
    public float amplitude = 0.2f; //height of the waves
    public float wavelength = 2f;  //distance between wave peaks
    public float speed = 1f; //how fast the waves move
    public Vector2 direction = new Vector2(1f, 1f); //the X and Z direction

    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        baseVertices = mesh.vertices;
        displacedVertices = new Vector3[baseVertices.Length];
        direction.Normalize();
    }

    void Update()
    {
        float time = Time.time * speed;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            float wave = Mathf.Sin((vertex.x * direction.x + vertex.z * direction.y) / wavelength + time) * amplitude;
            vertex.y = wave;
            displacedVertices[i] = vertex;
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
    }
}
