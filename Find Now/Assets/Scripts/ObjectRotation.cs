using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // Object to rotate (provided in Inspector)

    [Header("Rotation Settings")]
    public float rotationSpeed = 100.0f; // Adjust rotation speed

    [Header("Camera Movement Settings")]
    public float moveSpeed = 5f; // Adjust movement speed
    public float minZ = -10f; // Closest camera distance
    public float maxZ = 0f; // Farthest camera distance

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private bool isHorizontalMovement = false; // Determines if rotation or movement is applied
    private float cameraZ;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        cameraZ = transform.localPosition.z; // Store the initial Z position
    }

    void Update()
    {
        if (!HoldAllItems.instance.miniMapOpen)
        {

            HandleRotationAndMovement();
        }


    }

    void HandleRotationAndMovement()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to start dragging
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) // Stop dragging on release
        {
            isDragging = false;
        }

        if (isDragging) // Rotate or move only when dragging
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaMouse = currentMousePosition - lastMousePosition;

            // Determine whether X (horizontal) or Y (vertical) movement is greater
            if (Mathf.Abs(deltaMouse.x) > Mathf.Abs(deltaMouse.y))
            {
                isHorizontalMovement = true; // Rotate object
            }
            else
            {
                isHorizontalMovement = false; // Move camera
            }

            if (isHorizontalMovement && target != null) // Left-Right Drag → Rotate Target Object
            {
                float yRotation = deltaMouse.x * rotationSpeed * Time.deltaTime;
                target.Rotate(0, -yRotation, 0, Space.World);
            }
            else // Up-Down Drag → Move Camera Forward & Backward (Z Position)
            {
                cameraZ -= deltaMouse.y * moveSpeed * Time.deltaTime;
                cameraZ = Mathf.Clamp(cameraZ, minZ, maxZ); // Clamp between min & max
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, cameraZ);
            }

            lastMousePosition = currentMousePosition; // Update last position
        }
    }
}
