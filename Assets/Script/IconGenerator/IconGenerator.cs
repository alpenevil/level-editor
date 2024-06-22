using UnityEngine;
using System.IO;
using UnityEditor;

public class IconGenerator : MonoBehaviour
{
    public Camera captureCamera;
    public string savePath = "Assets/Icons";
    public Vector2 iconSize = new Vector2(256, 256);
    public ObjectsDatabaseSO objectsDatabase; 

    private void Start()
    {
        try
        {
            GenerateIcons();
            SaveDatabase();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"An error occurred during icon generation: {ex.Message}");
        }
    }

    private void GenerateIcons()
    {
        if (captureCamera == null || objectsDatabase == null)
        {
            Debug.LogError("CaptureCamera or ObjectsDatabase is not assigned.");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        foreach (var objectData in objectsDatabase.objectsData)
        {
            if (objectData == null)
            {
                Debug.LogWarning("ObjectData is null.");
                continue;
            }

            GameObject prefab = objectData.Prefab;
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab for {objectData.Name} is null.");
                continue;
            }

            try
            {
               
                GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            
                instance.SetActive(true);

              
                SetRenderersActive(instance, true);

              
                captureCamera.clearFlags = CameraClearFlags.SolidColor;
                captureCamera.backgroundColor = new Color(0, 0, 0, 0);

            
                Bounds bounds = CalculateBounds(instance);

                captureCamera.orthographic = true;
                captureCamera.orthographicSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2;

                Vector3 cameraPosition = bounds.center + new Vector3(0, 0, -10);
                captureCamera.transform.position = cameraPosition;
                captureCamera.transform.LookAt(bounds.center);

                RenderTexture rt = new RenderTexture((int)iconSize.x, (int)iconSize.y, 24, RenderTextureFormat.ARGB32);
                captureCamera.targetTexture = rt;

                Texture2D screenShot = new Texture2D((int)iconSize.x, (int)iconSize.y, TextureFormat.RGBA32, false);
                captureCamera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, (int)iconSize.x, (int)iconSize.y), 0, 0);
                screenShot.Apply();

                byte[] bytes = screenShot.EncodeToPNG();
                string fileName = Path.Combine(savePath, objectData.Name + ".png");
                File.WriteAllBytes(fileName, bytes);

                string assetPath = "Assets/Icons/" + objectData.Name + ".png";
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (texture != null)
                {
                    Sprite iconSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    string spritePath = "Assets/Icons/" + objectData.Name + "_Sprite.asset";
                    AssetDatabase.CreateAsset(iconSprite, spritePath);
                    objectData.SetIconSprite(iconSprite);
                }

                instance.SetActive(false);
                Destroy(instance);
                Destroy(rt);
                captureCamera.targetTexture = null;
                RenderTexture.active = null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while processing {objectData.Name}: {ex.Message}");
            }

            EditorUtility.SetDirty(objectsDatabase);
        }
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private void SetRenderersActive(GameObject obj, bool isActive)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isActive;
        }
    }

    private void SaveDatabase()
    {
        if (objectsDatabase != null)
        {
            try
            {
                EditorUtility.SetDirty(objectsDatabase);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while saving the database: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("ObjectsDatabase is not assigned.");
        }
    }
}
