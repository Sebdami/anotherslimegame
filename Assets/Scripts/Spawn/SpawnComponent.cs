﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType { Item, Monster, EvolutionIsland, PointIsland }
public class SpawnComponent : MonoBehaviour {

    [SerializeField]
    public SpawnType mySpawnType;

    public MonsterType myMonsterType;

    public CollectableType myItemType;

    public Shapes shape;
    public int nbItems = 1;

    private int mySpawnId;
    
    [Tooltip("Spawn a spawnable in registry")]
    public bool needSpawn = true;

    [Tooltip("Force spawn despite reaching the max spawn unit at the same time")]
    public bool forceSpawn = false;

    public float circleRadius = 1.0f;

	// Use this for initialization
	void Start () {

        // TODO: shitty tool to see where we put spawners (need a tool?)
        if(GetComponent<MeshRenderer>())
            Destroy(GetComponent<MeshRenderer>());
        if (GetComponent<MeshFilter>())
            Destroy(GetComponent<MeshFilter>());

        switch (mySpawnType) {
            case SpawnType.Item:
                if (myItemType == CollectableType.Size)
                {
                    Debug.LogError("Are you serious with your shit? Size is not a real collectable type -_-");
                    break;
                }
                mySpawnId = HUBManager.instance.spawnManagerRef.RegisterSpawnItemLocation(transform, myItemType, needSpawn, forceSpawn, shape, nbItems, circleRadius);
                break;
            case SpawnType.Monster:
                mySpawnId = HUBManager.instance.spawnManagerRef.RegisterSpawnMonsterLocation(transform, myMonsterType, needSpawn, forceSpawn);
                break;
            case SpawnType.EvolutionIsland:
                mySpawnId = HUBManager.instance.spawnManagerRef.RegisterSpawnEvolutionIslandLocation(transform, needSpawn, forceSpawn);
                break;
            case SpawnType.PointIsland:
                mySpawnId = HUBManager.instance.spawnManagerRef.RegisterSpawnPointsIslandLocation(transform, needSpawn, forceSpawn);
                break;
            default:
                Debug.Log("Unknowned Spawn Type");
                break;
        }
    }

    private void OnDestroy()
    {
        switch (mySpawnType)
        {
            case SpawnType.Item:
                HUBManager.instance.spawnManagerRef.UnregisterSpawnItemLocation(mySpawnId);
                break;
            case SpawnType.Monster:
                HUBManager.instance.spawnManagerRef.UnregisterSpawnMonsterLocation(mySpawnId);
                break;
            case SpawnType.EvolutionIsland:
                HUBManager.instance.spawnManagerRef.UnregisterSpawnEvolutionIslandLocation(mySpawnId);
                break;
            case SpawnType.PointIsland:
                HUBManager.instance.spawnManagerRef.UnregisterSpawnPointsIslandLocation(mySpawnId);
                break;
            default:
                Debug.Log("Unknowned Spawn Type");
                break;
        }
    }

}
