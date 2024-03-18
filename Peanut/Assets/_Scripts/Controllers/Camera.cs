using UnityEngine;
//Credits to Niek Melet for helping with the code

public class Camera : MonoBehaviour
{

    // The safe quaternion for camera rotation (defaulted to no rotation)
    public Quaternion safeQuad { get; set; } = Quaternion.identity;

    [SerializeField] private Vector3 ogPos; // Original position of the camera's child
    [Range(1, 25)][SerializeField] private float adjustSpeed = 25; // Speed for adjusting camera position
    [Range(1, 5)][SerializeField] private float slowAdjustSpeed = 5; // Speed for the camera to adjust slowely if there is a object close
    [Range(0, 90)][SerializeField] private float maxRotationX = 60; // Sets the maxium rotation on the x axis
    [Range(0, 5)][SerializeField] private float slowRange = 1; // The distance at which the camera should slow down
    [SerializeField] private float sphereSize = 0.7f; // Size of the box that stops the camera from moving
    [SerializeField] private Transform child; // Child object of the camera
    [SerializeField] private Transform player; // Player object the camera should follow
    [SerializeField] private bool stop; // Flag indicating if camera movement should stop
    [SerializeField] private bool isSlow; // Flag indicating if the camera should move slowely
    [SerializeField] private LayerMask layer; // Layer mask for camera collision detection
    [SerializeField] GameObject obj;

    private void Start()
    {
        // Initialize camera rotation to the safe quaternion
        transform.rotation = safeQuad;
    }

    private void Update()
    {
        transform.position = player.position;
        RotateCamera();
    }

    void FixedUpdate()
    {
        CamCollsionCheck();
    }

    private void LateUpdate()
    {
        OnCamCollision();
    }

    /// <summary>
    /// Rotates the camera based on mouse input along the local X-axis, limited to -maxRotationX to maxRotationX degrees.
    /// </summary>
    void RotateCamera()
    {
        float value = Input.GetAxis("Mouse Y");

        // Calculate current rotation angle around the local X-axis
        float angleX = transform.localEulerAngles.x;
        if (transform.localEulerAngles.x > 256)
        {
            angleX = transform.localEulerAngles.x - 360;
        }

        // Clamp the new rotation angle within the specified range
        float currentRot = Mathf.Clamp(angleX + value, -maxRotationX, maxRotationX);

        // Apply the new rotation to the camera
        transform.rotation = Quaternion.Euler(currentRot, transform.eulerAngles.y + Input.GetAxis("Mouse X"), transform.rotation.z);
    }

    /// <summary>
    /// Handles logic for camera collision and adjusts camera position accordingly.
    /// </summary>
    void OnCamCollision()
    {
        float speed = isSlow ? slowAdjustSpeed : adjustSpeed;

        if (IsColliding())
        {
            // Adjust the child's position to avoid colliding with the ground
            child.localPosition = Vector3.MoveTowards(child.localPosition, 
                new Vector3(child.localPosition.x, child.localPosition.y, transform.position.z), 
                speed * Time.deltaTime);
        }
        else if (!IsColliding() && !stop)
        {
            // Move the child back to its original position if not colliding with the ground and not stopped
            child.localPosition = Vector3.MoveTowards(child.localPosition,
                ogPos, slowAdjustSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks for camera collision with objects and updates collision-related flags.
    /// </summary>
    void CamCollsionCheck()
    {
        // Check if any colliders are overlapping with the child's bounding sphere
        stop = CollCheck(Physics.OverlapSphere(child.position, sphereSize, layer), out GameObject tempObj); // Set flag if the camera is near a object and should stop adjusting

        if (tempObj != null) obj = tempObj;

        if (obj == null) return;

        isSlow = Vector3.Distance(obj.transform.position, child.position) < slowRange && 
                 Vector3.Dot(-child.forward, obj.transform.position - transform.position) > 0.0f || 
            Physics.Raycast(child.position, -child.forward, slowRange, layer); // Set flag if a object is behind the camera within a the range of slowRange
    }


    /// <summary>
    /// Checks if something is inbetween the player and the camera
    /// </summary>
    /// <returns>Returns true if something is inbetween the player and the camera else false</returns>
    bool IsColliding()
    {
        return Physics.Raycast(child.position, (transform.position - child.position).normalized, 
                   Vector3.Distance(child.position, transform.position), layer) ||
            Physics.Raycast(transform.position, (child.position - transform.position).normalized,
                Vector3.Distance(child.position, transform.position), layer) ||
            CollCheck(Physics.OverlapSphere(child.position, transform.localScale.x / 2.1f, layer));
    }

    /// <summary>
    /// Checks if any Collider in the array is colliding with something.
    /// </summary>
    /// <param name="col">An array of Colliders to check.</param>
    /// <returns>True if any Collider in the array is colliding, otherwise false.</returns>
    private bool CollCheck(Collider[] col)
    {
        // Iterate through the array of Colliders
        foreach (var coll in col)
        {
            // If any Collider is not null (indicating a collision), return true
            if (coll != null) return true;
        }

        // If no Colliders were found to be colliding, return false
        return false;
    }

    /// <summary>
    /// Checks if any Collider in the array is colliding with something.
    /// </summary>
    /// <param name="col">An array of Colliders to check.</param>
    /// <returns>True if any Collider in the array is colliding, otherwise false.</returns>
    private bool CollCheck(Collider[] col, out GameObject obj)
    {
        obj = null;
        // Iterate through the array of Colliders
        foreach (var coll in col)
        {
            // If any Collider is not null (indicating a collision), return true
            if (coll != null) { obj = coll.gameObject; return true; }
        }
        // If no Colliders were found to be colliding, return false
        return false;
    }
}
