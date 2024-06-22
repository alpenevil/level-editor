using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float mouseSensitivity = 2.0f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private float verticalRotation = 0;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        rb.freezeRotation = true;  
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, mouseX, 0);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized * walkSpeed;
        Vector3 newVelocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            newVelocity = new Vector3(0, rb.velocity.y, 0);
        }

        rb.velocity = newVelocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < 0 && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GameLevelObject"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("GameLevelObject"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("GameLevelObject"))
        {
            isGrounded = false;
        }
    }
}

