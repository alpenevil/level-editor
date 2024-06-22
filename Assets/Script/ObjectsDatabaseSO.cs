using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public ObjectType Type { get; private set; }
    [field: SerializeField]
    public bool IsRandomSelectionEnabled;
    [field: SerializeField]
    public ObjectPurpose Purpose { get; private set; } 

    [field: SerializeField]
    public Sprite Icon; 

    public void SetIconSprite(Sprite iconSprite)
    {
        Icon = iconSprite;
    }
}

public enum ObjectType 
{
    Floor,
    Furniture,
    SpawnPoint,
    EnemySpawnPoint
}

public enum ObjectPurpose
{
    None,
    Tree,
    Ground,
    Building,
    Decoration,
    Game
}
