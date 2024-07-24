using UnityEngine;
using System.IO;

public class TestSceneManager : MonoBehaviour
{
    public ObjectsDatabaseSO database;
    public ObjectPlacer objectPlacer;
    public LevelData currentLevelData;
    public GridData gridData;
    public Grid grid;
    public PlacementSystem placementSystem;
    public GameObject enemyPrefab;

    private PathfindingGrid pathfindingGrid;

    private string savedLevelsDirectory = "Assets/SavedLevels";

    void Start()
    {
        pathfindingGrid = FindObjectOfType<PathfindingGrid>();
        if (pathfindingGrid != null)
        {
            pathfindingGrid.CreateGrid();
            Debug.Log("Grid created successfully.");
        }
        else
        {
            // Debug.LogError("PathfindingGrid not found!");
        }

        if (PlayerPrefs.HasKey("levelToLoad"))
        {
            string levelName = PlayerPrefs.GetString("levelToLoad");
            PlayerPrefs.DeleteKey("levelToLoad");
            LoadLevelFromFile(levelName);
        }

        ObjectData spawnPointData = database.objectsData.Find(obj => obj.Type == ObjectType.SpawnPoint);
        if (spawnPointData == null)
        {
            Debug.LogWarning("Spawn point object not found in database.");
            return;
        }
        Vector2Int spawnPointSize = spawnPointData.Size;
        Vector3 spawnPoint = new Vector3(
            PlayerPrefs.GetFloat("SpawnPointX", 0) + spawnPointSize.x / 2f,
            PlayerPrefs.GetFloat("SpawnPointY", 0),
            PlayerPrefs.GetFloat("SpawnPointZ", 0) + spawnPointSize.x / 2f);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPoint;
            //Debug.Log($"Player spawned at {spawnPoint}");
        }
        else
        {
            Debug.LogWarning("Player object not found.");
        }

        if (currentLevelData != null)
        {

            if (currentLevelData.enemyPatrolPointA != Vector3.zero || currentLevelData.enemyPatrolPointB != Vector3.zero)
            {
                ObjectData enemySpawnPointData = database.objectsData.Find(obj => obj.Type == ObjectType.EnemySpawnPoint);
                if (enemySpawnPointData == null)
                {
                    Debug.LogWarning("Enemy spawn point object not found in database.");
                    return;
                }
                Vector2Int enemySpawnPointSize = enemySpawnPointData.Size;

                Vector3 patrolPointA = new Vector3(
                    currentLevelData.enemyPatrolPointA.x + enemySpawnPointSize.x / 2f,
                    currentLevelData.enemyPatrolPointA.y,
                    currentLevelData.enemyPatrolPointA.z + enemySpawnPointSize.y / 2f);

                Vector3 patrolPointB = new Vector3(
                    currentLevelData.enemyPatrolPointB.x + enemySpawnPointSize.x / 2f,
                    currentLevelData.enemyPatrolPointB.y,
                    currentLevelData.enemyPatrolPointB.z + enemySpawnPointSize.y / 2f);

                if (patrolPointA != Vector3.zero)
                {
                    GameObject enemy = Instantiate(enemyPrefab, patrolPointA, Quaternion.identity);
                    EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();

                    GameObject pointAObject = new GameObject("PointA");
                    pointAObject.transform.position = patrolPointA;
                    patrol.pointA = pointAObject.transform;

                    if (patrolPointB != Vector3.zero)
                    {
                        GameObject pointBObject = new GameObject("PointB");
                        pointBObject.transform.position = patrolPointB;
                        patrol.pointB = pointBObject.transform;
                    }
                    else
                    {
                        patrol.pointB = null;
                    }
                }
            }
        }
    }

    void UpdatePathfindingGrid(Vector3Int position, Vector2Int size, ObjectType type, Vector3 rotation)
    {
        //Debug.Log($"Updating pathfinding grid at position {position} with size {size} for type {type}");
        if (type == ObjectType.Floor)
        {
            pathfindingGrid.MarkWalkable(position, size, rotation);
        }
        else if (type == ObjectType.Furniture)
        {
            pathfindingGrid.MarkUnwalkable(position, size, rotation);
        }
    }

    void LoadLevelFromFile(string levelName)
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

                Vector3Int position = new Vector3Int(
                    Mathf.RoundToInt(placedObjectData.position.x),
                    Mathf.RoundToInt(placedObjectData.position.y),
                    Mathf.RoundToInt(placedObjectData.position.z)
                );

                Vector3 worldPosition = new Vector3(
                    placedObjectData.position.x,
                    placedObjectData.position.y,
                    placedObjectData.position.z
                );

                int index = objectPlacer.PlaceObject(objectData.Prefab, worldPosition, Vector3.zero);
                selectedData.AddObjectAt(position, size, objectData.ID, index);
                objectPlacer.SetObjectRotation(index, placedObjectData.rotation);

                UpdatePathfindingGrid(position, size, objectData.Type, placedObjectData.rotation);

                if (objectData.Type == ObjectType.SpawnPoint || objectData.Type == ObjectType.EnemySpawnPoint)
                {
                    GameObject placedObject = objectPlacer.placedGameObjects[index];
                    if (placedObject != null)
                    {
                        placedObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Prefab not found for object with ID: " + placedObjectData.prefabID);
            }
        }

        PlayerPrefs.SetFloat("SpawnPointX", currentLevelData.spawnPoint.x);
        PlayerPrefs.SetFloat("SpawnPointY", currentLevelData.spawnPoint.y);
        PlayerPrefs.SetFloat("SpawnPointZ", currentLevelData.spawnPoint.z);

        PlayerPrefs.SetFloat("EnemyPatrolPointAX", currentLevelData.enemyPatrolPointA.x);
        PlayerPrefs.SetFloat("EnemyPatrolPointAY", currentLevelData.enemyPatrolPointA.y);
        PlayerPrefs.SetFloat("EnemyPatrolPointAZ", currentLevelData.enemyPatrolPointA.z);

        PlayerPrefs.SetFloat("EnemyPatrolPointBX", currentLevelData.enemyPatrolPointB.x);
        PlayerPrefs.SetFloat("EnemyPatrolPointBY", currentLevelData.enemyPatrolPointB.y);
        PlayerPrefs.SetFloat("EnemyPatrolPointBZ", currentLevelData.enemyPatrolPointB.z);
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
}
