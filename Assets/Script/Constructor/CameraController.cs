using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float rotationSpeed = 20f;
    public float minY = 1f;
    public float maxY = 80f;

    private Vector3 dragOrigin;
    private Vector3 rotationOrigin;
    private Vector3 currentRotation;

    void Start()
    {
        currentRotation = transform.eulerAngles;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandlePanning();
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(direction * panSpeed * Time.deltaTime, Space.Self);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 pos = Input.mousePosition - dragOrigin;
            currentRotation.y += pos.x * rotationSpeed * Time.deltaTime;
            currentRotation.x -= pos.y * rotationSpeed * Time.deltaTime;

            currentRotation.x = Mathf.Clamp(currentRotation.x, minY, maxY);

            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            dragOrigin = Input.mousePosition;
        }
    }

    void HandlePanning()
    {

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * panSpeed, -pos.y * panSpeed, 0);

            transform.Translate(move, Space.Self);
            dragOrigin = Input.mousePosition;
        }
    }
}
