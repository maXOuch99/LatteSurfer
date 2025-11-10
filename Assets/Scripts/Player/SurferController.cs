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
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //camera-relative movement direction
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        //desired velocity
        Vector3 desiredDir = (camForward * input.y + camRight * input.x).normalized;
        float targetSpeed = desiredDir.magnitude > 0.01f ? maxSpeed : 0f;
        Vector3 targetVel = desiredDir * targetSpeed;

        //smooth acceleration/deceleration
        float accel = desiredDir.magnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVel, accel * Time.fixedDeltaTime);

        //project movement onto surface
        Vector3 surfaceNormal = Vector3.up;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit,
                            surfaceCheckDistance, surfaceMask, QueryTriggerInteraction.Ignore))
        {
            surfaceNormal = hit.normal;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
        }

        //apply velocity to Rigidbody
        Vector3 newVel = currentVelocity;
        newVel.y = rb.linearVelocity.y;   //preserve vertical motion
        rb.linearVelocity = newVel;

        //rotate player (yaw only)
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 moveDir = currentVelocity;
            moveDir.y = 0f; // ignore vertical for rotation
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up); // yaw only
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            }
        }

        //visualRoot tilt (roll) only
        if (visualRoot != null)
        {
            float targetZ = -Input.GetAxis("Horizontal") * 25f;      // left/right tilt
            Vector3 angles = visualRoot.localEulerAngles;
            angles.z = Mathf.LerpAngle(angles.z, targetZ, Time.fixedDeltaTime * 6f);
            visualRoot.localEulerAngles = angles;
        }
    }
}