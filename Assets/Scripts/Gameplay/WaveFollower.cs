using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaveFollower : MonoBehaviour
{
    [Header("Wave Tracking")]
    public WaveDeformer waveSurface; //reference to the deformer script
    public float hoverHeight = 0.5f; //offset above the wave
    public float followSpeed = 5f;   //how fast to move vertically
    public float alignSpeed = 6f;    //how fast to align to surface normal

    Rigidbody rb;

    private float[] heightSamples = new float[5];
    private int heightIndex = 0;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!waveSurface) return;

        Vector3 pos = transform.position;
        Vector3 wavePos, waveNormal;
        SampleWaveAtPosition(pos, out wavePos, out waveNormal);

        //smooth vertical follow without physics fight
        float targetY = wavePos.y + hoverHeight;

        //cache samples to smooth small wave jitters
        heightSamples[heightIndex] = targetY;
        heightIndex = (heightIndex + 1) % heightSamples.Length;

        float avgY = 0f;
        for (int i = 0; i < heightSamples.Length; i++)
            avgY += heightSamples[i];
        avgY /= heightSamples.Length;

        pos.y = Mathf.Lerp(pos.y, avgY, followSpeed * Time.fixedDeltaTime);
        transform.position = pos;

        //keep upright, ignore wave tilt
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(flatForward, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, alignSpeed * Time.fixedDeltaTime));
    }

    //samples the wave mesh height and normal at a world position
    void SampleWaveAtPosition(Vector3 worldPos, out Vector3 wavePos, out Vector3 normal)
    {
        Mesh mesh = waveSurface.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        Vector3[] norms = mesh.normals;

        //convert surfer position to local space of the wave mesh
        Vector3 localPos = waveSurface.transform.InverseTransformPoint(worldPos);

        //estimate wave height at that local X/Z using sine function (matches WaveDeformer)
        float time = Time.time * waveSurface.waveSpeed;
        float waveValue = Mathf.Sin(
            (localPos.x * waveSurface.waveFrequency + time * waveSurface.waveDirection.x) +
            (localPos.z * waveSurface.waveFrequency + time * waveSurface.waveDirection.y)
        ) * waveSurface.waveHeight;

        Vector3 localWavePos = new Vector3(localPos.x, waveValue, localPos.z);
        wavePos = waveSurface.transform.TransformPoint(localWavePos);

        //approximate the normal
        Vector3 dx = new Vector3(1, waveSurface.waveHeight * waveSurface.waveFrequency * Mathf.Cos(localPos.x + time), 0);
        Vector3 dz = new Vector3(0, waveSurface.waveHeight * waveSurface.waveFrequency * Mathf.Cos(localPos.z + time), 1);
        normal = Vector3.Cross(dz, dx).normalized;
    }
}
