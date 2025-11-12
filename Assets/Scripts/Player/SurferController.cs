using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SurferController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 6f;
    public float acceleration = 20f;
    public float deceleration = 10f;
    public float turnSpeed = 6f;

    [Header("Surface")]
    public float surfaceCheckDistance = 2f;
    public LayerMask surfaceMask;

    [Header("References")]
    public Transform cameraTransform; //cinemachine will follow the target or main camera
    public Transform visualRoot; //model that will tile (child of the player)

    Rigidbody rb;
    Vector3 currentVelocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; //control the rotation with code
    }

    void FixedUpdate()
    {
        //input
        float steerInput = Input.GetAxis("Horizontal");   //steering input (A/D)
        float forwardInput = Input.GetAxis("Vertical");   //forward acceleration (W/S)

        //steering
        if (Mathf.Abs(steerInput) > 0.01f)
        {
            float turnAmount = steerInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up * turnAmount);    //rotate yaw only (steering)
        }

        //forward direction and speed
        Vector3 desiredDir = transform.forward * Mathf.Clamp01(forwardInput);  //always move along facing direction
        float targetSpeed = desiredDir.magnitude > 0.01f ? maxSpeed : 0f;
        Vector3 targetVel = desiredDir * targetSpeed;

        //smooth acceleration/deceleration
        float accel = desiredDir.magnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVel, accel * Time.fixedDeltaTime);

        //project movement onto surface normal so player hugs waves
        Vector3 surfaceNormal = Vector3.up;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit,
                            surfaceCheckDistance, surfaceMask, QueryTriggerInteraction.Ignore))
        {
            surfaceNormal = hit.normal;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
        }

        //apply velocity to Rigidbody
        Vector3 newVel = currentVelocity;
        newVel.y = rb.linearVelocity.y;
        rb.linearVelocity = newVel;

        //visualRoot lean (surf carve lean)
        if (visualRoot != null)
        {
            float targetZ = -steerInput * 25f; // 💡 Lean based on steering, not input.x direction
            Vector3 angles = visualRoot.localEulerAngles;
            angles.z = Mathf.LerpAngle(angles.z, targetZ, Time.fixedDeltaTime * 6f);
            visualRoot.localEulerAngles = angles;
        }
    }
}