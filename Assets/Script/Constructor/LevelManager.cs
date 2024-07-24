using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public List<PlacedObjectData> placedObjectsData;
    public Vector3 spawnPoint;
    public Vector3 enemyPatrolPointA;
    public Vector3 enemyPatrolPointB;
    public bool hasEnemyPatrolPointA;
    public bool hasEnemyPatrolPointB;
}

[Serializable]
public class PlacedObjectData
{
    public int prefabID;
    public Vector3 position;
    public Vector3 rotation;
}

public class LevelManager : MonoBehaviour
{
    public ObjectsDatabaseSO database;
    public ObjectPlacer objectPlacer;
    public LevelData currentLevelData;
    public GridData gridData;
    public Grid grid;
    public PlacementSystem placementSystem;

    public Camera screenshotCamera;

    private string savedLevelsDirectory = "Assets/SavedLevels";
    private Vector3 defaultSpawnPoint = new Vector3(-15, 0, -15);

    private void Start()
    {
        if (PlayerPrefs.HasKey("levelToLoad"))
        {
            string levelName = PlayerPrefs.GetString("levelToLoad");
            PlayerPrefs.DeleteKey("levelToLoad");
            LoadLevelFromFile(levelName);
            Debug.Log("Level successfully loaded!");
        }
        FindObjectOfType<LevelManagerUI>()?.PopulateLoadDropdown();
    }

    public void SaveLevel(string fileName)
    {
        ClearAllPointsFromPrefs(); 

        currentLevelData = new LevelData();
        currentLevelData.placedObjectsData = new List<PlacedObjectData>();
        foreach (GameObject placedObject in objectPlacer.placedGameObjects)
        {
            if (placedObject != null)
            {
                PlacedObjectData placedObjectData = new PlacedObjectData();
                ObjectData objectData = database.objectsData.Find(obj => obj.Prefab.name == placedObject.name.Replace("(Clone)", ""));
                if (objectData != null)
                {
                    placedObjectData.prefabID = objectData.ID;
                    placedObjectData.position = placedObject.transform.position;
                    placedObjectData.rotation = placedObject.transform.eulerAngles;
                    currentLevelData.placedObjectsData.Add(placedObjectData);
                }
            }
        }

        ObjectData spawnPointData = database.objectsData.Find(obj => obj.Type == ObjectType.SpawnPoint);
        if (spawnPointData != null && objectPlacer.placedGameObjects.Exists(obj => obj != null && obj.name.Contains(spawnPointData.Prefab.name)))
        {
            GameObject spawnPointObject = objectPlacer.placedGameObjects.Find(obj => obj != null && obj.name.Contains(spawnPointData.Prefab.name));
            currentLevelData.spawnPoint = spawnPointObject != null ? spawnPointObject.transform.position : defaultSpawnPoint;
        }
        else
        {
            currentLevelData.spawnPoint = defaultSpawnPoint;
        }

   
        ObjectData enemyPatrolPointData = database.objectsData.Find(obj => obj.Type == ObjectType.EnemySpawnPoint);
        if (enemyPatrolPointData != null)
        {
            List<GameObject> patrolPoints = objectPlacer.placedGameObjects.FindAll(obj => obj != null && obj.name.Contains(enemyPatrolPointData.Prefab.name));
            if (patrolPoints.Count > 0)
            {
                currentLevelData.hasEnemyPatrolPointA = true;
                currentLevelData.enemyPatrolPointA = patrolPoints[0].transform.position;
            }
            if (patrolPoints.Count > 1)
            {
                currentLevelData.hasEnemyPatrolPointB = true;
                currentLevelData.enemyPatrolPointB = patrolPoints[1].transform.position;
            }
        }

        if (!Directory.Exists(savedLevelsDirectory))
        {
            Directory.CreateDirectory(savedLevelsDirectory);
        }

        string filePath = Path.Combine(savedLevelsDirectory, fileName + ".json");
        string json = JsonUtility.ToJson(currentLevelData);
        File.WriteAllText(filePath, json);

        SaveScreenshot(fileName);
    }

    public void LoadLevelFromFile(string levelName)
    {
        string filePath = Path.Combine(savedLevelsDirectory, levelName + ".json");
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("File does not exist: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        currentLevelData = JsonUtility.FromJson<LevelData>(json);

        foreach (PlacedObjectData placedObjectData in currentLevelData.placedObjectsData)
        {
            ObjectData objectData = database.objectsData.Find(obj => obj.ID == placedObjectData.prefabID);
            if (objectData != null)
            {
                GridData selectedData = objectData.Type == ObjectType.Floor ? placementSystem.floorData : placementSystem.furnitureData;
                Vector2Int size = objectData.Size;
                if (Mathf.Abs(placedObjectData.rotation.y % 180) > 0.01f)
                {
                    size = new Vector2Int(size.y, size.x);
                }

                Vector3Int position = new Vector3Int(Mathf.RoundToInt(placedObjectData.position.x), Mathf.RoundToInt(placedObjectData.position.y), Mathf.RoundToInt(placedObjectData.position.z));
                Vector3 modelOffsetVector3 = position - GetRotationOffset(placedObjectData.rotation, size);
                Vector3Int modelOffset = new Vector3Int(Mathf.RoundToInt(modelOffsetVector3.x), Mathf.RoundToInt(modelOffsetVector3.y), Mathf.RoundToInt(modelOffsetVector3.z));

                int index = objectPlacer.PlaceObject(objectData.Prefab, grid.CellToWorld(position), new Vector3(0, 0, 0));
                selectedData.AddObjectAt(modelOffset, size, objectData.ID, index);
                objectPlacer.SetObjectRotation(index, placedObjectData.rotation);
            }
            else
            {
                Debug.LogWarning("Prefab not found for object with ID: " + placedObjectData.prefabID);
            }
        }

        if (currentLevelData.spawnPoint != Vector3.zero)
        {
            PlayerPrefs.SetFloat("SpawnPointX", currentLevelData.spawnPoint.x);
            PlayerPrefs.SetFloat("SpawnPointY", currentLevelData.spawnPoint.y);
            PlayerPrefs.SetFloat("SpawnPointZ", currentLevelData.spawnPoint.z);
        }
        else
        {
            currentLevelData.spawnPoint = defaultSpawnPoint;
            PlayerPrefs.SetFloat("SpawnPointX", defaultSpawnPoint.x);
            PlayerPrefs.SetFloat("SpawnPointY", defaultSpawnPoint.y);
            PlayerPrefs.SetFloat("SpawnPointZ", defaultSpawnPoint.z);
        }

        if (currentLevelData.hasEnemyPatrolPointA)
        {
            PlayerPrefs.SetFloat("EnemyPatrolPointAX", currentLevelData.enemyPatrolPointA.x);
            PlayerPrefs.SetFloat("EnemyPatrolPointAY", currentLevelData.enemyPatrolPointA.y);
            PlayerPrefs.SetFloat("EnemyPatrolPointAZ", currentLevelData.enemyPatrolPointA.z);
        }
        else
        {
            ClearPatrolPointsFromPrefs();
        }

        if (currentLevelData.hasEnemyPatrolPointB)
        {
            PlayerPrefs.SetFloat("EnemyPatrolPointBX", currentLevelData.enemyPatrolPointB.x);
            PlayerPrefs.SetFloat("EnemyPatrolPointBY", currentLevelData.enemyPatrolPointB.y);
            PlayerPrefs.SetFloat("EnemyPatrolPointBZ", currentLevelData.enemyPatrolPointB.z);
        }
        else
        {
            ClearPatrolPointsFromPrefs();
        }
    }

    private void ClearAllPointsFromPrefs()
    {
        PlayerPrefs.DeleteKey("EnemyPatrolPointAX");
        PlayerPrefs.DeleteKey("EnemyPatrolPointAY");
        PlayerPrefs.DeleteKey("EnemyPatrolPointAZ");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBX");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBY");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBZ");
        PlayerPrefs.DeleteKey("SpawnPointX");
        PlayerPrefs.DeleteKey("SpawnPointY");
        PlayerPrefs.DeleteKey("SpawnPointZ");
    }

    private void ClearPatrolPointsFromPrefs()
    {
        PlayerPrefs.DeleteKey("EnemyPatrolPointAX");
        PlayerPrefs.DeleteKey("EnemyPatrolPointAY");
        PlayerPrefs.DeleteKey("EnemyPatrolPointAZ");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBX");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBY");
        PlayerPrefs.DeleteKey("EnemyPatrolPointBZ");
    }

    private Vector3 GetRotationOffset(Vector3 rotation, Vector2Int objectSize)
    {
        float normalizedY = Mathf.Round(rotation.y % 360);
        normalizedY = (normalizedY < 0) ? normalizedY + 360 : normalizedY;

        switch (normalizedY)
        {
            case 0:
                return Vector3.zero;
            case 90:
                return new Vector3(0, 0, objectSize.y);
            case 180:
                return new Vector3(objectSize.x, 0, objectSize.y);
            case 270:
                return new Vector3(objectSize.x, 0, 0);
            default:
                return Vector3.zero;
        }
    }

    public void ClearScene()
    {
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (GameObject obj in objectPlacer.placedGameObjects)
        {
            if (obj != null)
            {
                Vector3Int basePosition = Vector3Int.FloorToInt(obj.transform.position);
                ObjectData objectData = database.objectsData.Find(x => x.Prefab.name == obj.name.Replace("(Clone)", ""));

                if (objectData != null)
                {
                    GridData selectedData = objectData.Type == ObjectType.Floor ? placementSystem.floorData : placementSystem.furnitureData;
                    selectedData.RemoveObjectAt(basePosition);
                    objectsToRemove.Add(obj);
                }
                else
                {
                    Debug.LogWarning($"Data not found for object {obj.name}, cannot clear properly.");
                }
            }
        }

        foreach (GameObject objToRemove in objectsToRemove)
        {
            objectPlacer.placedGameObjects.Remove(objToRemove);
            Destroy(objToRemove);
        }
    }

    private void SaveScreenshot(string levelName)
    {
        if (!Directory.Exists(savedLevelsDirectory))
        {
            Directory.CreateDirectory(savedLevelsDirectory);
        }

        string screenshotPath = Path.Combine(savedLevelsDirectory, levelName + ".png");

        screenshotCamera.transform.position = new Vector3(-12, 6, -20);
        screenshotCamera.transform.rotation = Quaternion.Euler(45, 0, 0);

        int resolution = 1024;
        RenderTexture renderTexture = new RenderTexture(resolution, resolution, 24);
        screenshotCamera.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
        screenshotCamera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        screenShot.Apply();
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(screenshotPath, bytes);
        Debug.Log("Level successfully saved!");
    }
}
