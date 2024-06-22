using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private Grid grid; 

    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;
    private Vector2Int originalSize = Vector2Int.one; 

    private Renderer cellIndicatorRenderer;
    private Vector3 previewOffset = Vector3.zero;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void ShowSelectionArea(Vector3Int start, Vector3Int end)
    {
        Vector3Int min = Vector3Int.Min(start, end);
        Vector3Int max = Vector3Int.Max(start, end);
        Vector3 size = new Vector3(max.x - min.x + 1, 1, max.z - min.z + 1);
        Vector3 position = grid.CellToWorld(new Vector3Int(min.x, 0, min.z));
        cellIndicator.transform.localScale = new Vector3(size.x, 1, size.z);
        cellIndicatorRenderer.material.mainTextureScale = new Vector2(size.x, size.z);
        cellIndicator.transform.position = position;
        Color c = Color.white;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
        cellIndicator.SetActive(true);
    }

    public void UpdateSelectionArea(Vector3Int start, Vector3Int end)
    {
        ShowSelectionArea(start, end);
    }

    public void HideSelectionArea(Vector3Int position)
    {
        cellIndicator.transform.localScale = new Vector3(1, 1, 1);
        cellIndicatorRenderer.material.mainTextureScale = new Vector2(1, 1);
        MoveCursor(position);
    }


    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        Debug.Log(prefab != null ? "Starting new preview for prefab: " + prefab.name + " with size: " + size : "Starting new preview with no prefab, size: " + size);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (previewObject != null)
            Destroy(previewObject); 

        if (prefab != null)
        {
            cellIndicator.SetActive(true);
            previewObject = Instantiate(prefab);
            PreparePreview(previewObject);
            PrepareCursor(size);
            originalSize = size;
            MovePreview(cursorPosition, Vector3.zero);
        }
        else
        {
            cellIndicator.SetActive(false);
            previewObject = null;
        }
    }



    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
        previewOffset = previewObject.transform.position - cellIndicator.transform.position;
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity, Vector3 modelOffset)
    {
        if (previewObject != null)
        {
            MovePreview(position, modelOffset);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    public void UpdatePreviewRotation(Vector3 rotation, Vector3 cursorPosition, Vector3 modelOffset)
    {
        if (previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.Euler(rotation);

            MovePreview(cursorPosition, modelOffset);

            UpdatePreviewCursor(originalSize, rotation);
        }

    }

    private void UpdatePreviewCursor(Vector2Int size, Vector3 rotation)
    {
        Vector2Int adjustedSize = size;
        if (Mathf.Approximately(rotation.y, 90f) || Mathf.Approximately(rotation.y, 270f))
        {
            adjustedSize = new Vector2Int(size.y, size.x);
        }

        cellIndicator.transform.localScale = new Vector3(adjustedSize.x, 1, adjustedSize.y);
        cellIndicatorRenderer.material.mainTextureScale = adjustedSize;
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    public void MovePreview(Vector3 cursorPosition, Vector3 modelOffset)
    {
        if (previewObject != null)
        {
            
            Vector3Int gridPosition = grid.WorldToCell(cursorPosition);

            
            previewObject.transform.position = new Vector3(
                gridPosition.x + modelOffset.x,
                cursorPosition.y + previewYOffset + modelOffset.y,
                gridPosition.z + modelOffset.z);
        }
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}