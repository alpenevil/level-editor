using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action<Vector3Int> OnStartSelection;
    public event Action<Vector3Int> OnUpdateSelection;
    public event Action<Vector3Int> OnEndSelection;

    public event Action OnClicked;
    public event Action OnExit;
    public event Action OnClickedRelease;
    public event Action OnRotatePressed;

    public event Action OnRemovePressed; 

    public event Action<Vector3Int> OnCursorMovement; 
    public event Action<Vector3Int> OnDoubleClick;

    private float lastClickTime = 0;
    private float doubleClickDelay = 0.3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            Vector3Int position = GetSelectedMapPosition();
            if (Time.time - lastClickTime < doubleClickDelay && position == lastPosition)
            {
                OnDoubleClick?.Invoke(position);
            }
            lastClickTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();

        if (Input.GetMouseButtonUp(0))
        {
            OnClickedRelease?.Invoke();
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            OnStartSelection?.Invoke(GetSelectedMapPosition());
        }

        if (Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            OnUpdateSelection?.Invoke(GetSelectedMapPosition());
        }

        if (Input.GetMouseButtonUp(0) && !IsPointerOverUI())
        {
            OnEndSelection?.Invoke(GetSelectedMapPosition());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRotatePressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnRemovePressed?.Invoke();
        }

        UpdateCursorMovement();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3Int GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return Vector3Int.FloorToInt(lastPosition);
    }

    private void UpdateCursorMovement()
    {
        Vector3Int currentGridPosition = GetSelectedMapPosition();
        if (currentGridPosition != Vector3Int.FloorToInt(lastPosition))
        {
            lastPosition = currentGridPosition;
            OnCursorMovement?.Invoke(currentGridPosition);
        }
    }
}
