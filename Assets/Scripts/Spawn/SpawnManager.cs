﻿using System.Collections.Generic;
using UnityEngine;



public enum Shapes { None, Circle, Line, Grid }
public class SpawnManager : MonoBehaviour{

    // Spawned Items Location
    public Dictionary<int, Transform> dicSpawnItemsLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnItems = 0;

    // Spawned Monsters Location
    public Dictionary<int, Transform> dicSpawnMonstersLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnMonsters = 0;

    // Spawned Islands Locations
    public Dictionary<int, Transform> dicSpawnEvolutionIslandLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnEvolutionIsland = 0;

    // List of all evolutions in the level
    private List<CollectableType> evolutionsLeftToSpawn = new List<CollectableType>();



    // Spawned Points Locations
    public Dictionary<int, Transform> dicSpawnPointsIslandLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnPointsIsland = 0;

    // Frame control on item
    private int spawnedItemsCountAtTheSameTime = 0;
    private const int MAXSPAWNITEMSCOUNTATTHESAMETIME = 1000;

    // Frame control on monster
    private int spawnedMonsterCountAtTheSameTime = 0;
    private const int MAXSPAWNMONSTERSCOUNTATTHESAMETIME = 20;

    // Frame control on iles
    private int spawnedEvolutionIslandCountAtTheSameTime = 0;
    private const int MAXSPAWNEVOLUTIONISLANDSCOUNTATTHESAMETIME = 4;

    // Frame control on iles
    private int spawnedPointIslandCountAtTheSameTime = 0;
    private const int MAXSPAWNPOINTSISLANDSCOUNTATTHESAMETIME = 10;

    #region Accessors

    public int SpawnedMonsterCount
    {
        get
        {
            return spawnedMonsterCountAtTheSameTime;
        }

        set
        {
            spawnedMonsterCountAtTheSameTime = value;
        }
    }

    public int SpawnedItemsCount
    {
        get
        {
            return spawnedItemsCountAtTheSameTime;
        }

        set
        {
            spawnedItemsCountAtTheSameTime = value;
        }
    }

    public int SpawnedEvolutionIslandCount
    {
        get
        {
            return spawnedEvolutionIslandCountAtTheSameTime;
        }

        set
        {
            spawnedEvolutionIslandCountAtTheSameTime = value;
        }
    }

    public int SpawnedPointIslandCount
    {
        get
        {
            return spawnedPointIslandCountAtTheSameTime;
        }

        set
        {
            spawnedPointIslandCountAtTheSameTime = value;
        }
    }
    #endregion

    public void Awake()
    {
        ResetInstance();
    }

    public void ResetInstance()
    {
        spawnedItemsCountAtTheSameTime = 0;
        spawnedMonsterCountAtTheSameTime = 0;
        spawnedEvolutionIslandCountAtTheSameTime = 0;
        spawnedPointIslandCountAtTheSameTime = 0;

        evolutionsLeftToSpawn = new List<CollectableType>();
        evolutionsLeftToSpawn.Add(CollectableType.AgileEvolution1);
        evolutionsLeftToSpawn.Add(CollectableType.PlatformistEvolution1);
        evolutionsLeftToSpawn.Add(CollectableType.StrengthEvolution1);
        evolutionsLeftToSpawn.Add(CollectableType.GhostEvolution1);

        dicSpawnItemsLocations = new Dictionary<int, Transform>();
        lastInsertedKeySpawnItems = 0;

        dicSpawnMonstersLocations = new Dictionary<int, Transform>();
        lastInsertedKeySpawnMonsters = 0;

        dicSpawnEvolutionIslandLocations = new Dictionary<int, Transform>();
        lastInsertedKeySpawnEvolutionIsland = 0;

        dicSpawnPointsIslandLocations = new Dictionary<int, Transform>();
        lastInsertedKeySpawnPointsIsland = 0;

        Utils.Shuffle(evolutionsLeftToSpawn);
    }

    #region Items
    private void SpawnItem(int idLocation, CollectableType myItemType, bool forceSpawn = false)
    {

        if (dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }


        SpawnedItemsCount++;
        ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            dicSpawnItemsLocations[idLocation].transform.position,
            dicSpawnItemsLocations[idLocation].transform.rotation,
            dicSpawnItemsLocations[idLocation].transform,
            myItemType
        ).GetComponent<Collectable>().Init();
    }

    private void SpawnCircleShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false, float circleRadius = 1.0f)
    {
        if (dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }
        for (int i = 0; i < nbItems; i++)
        {
            SpawnedItemsCount++;
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnADividedCircle(dicSpawnItemsLocations[idLocation].transform.position, circleRadius, nbItems, Axis.XZ)[i],
                dicSpawnItemsLocations[idLocation].transform.rotation,
                dicSpawnItemsLocations[idLocation].transform,
                myItemType
            ).GetComponent<Collectable>().Init();
        }
    }

    private void SpawnLineShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false)
    {
        if (dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }
        for (int i = 0; i < nbItems; i++)
        {
            SpawnedItemsCount++;
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                GetVector3ArrayOnLine(dicSpawnItemsLocations[idLocation].transform.position, dicSpawnItemsLocations[idLocation].transform.forward, nbItems)[i],
                dicSpawnItemsLocations[idLocation].transform.rotation,
                dicSpawnItemsLocations[idLocation].transform,
                myItemType
            ).GetComponent<Collectable>().Init();
        }
    }

    private void SpawnGridShapedItems(int idLocation, int nbItems, CollectableType myItemType, bool forceSpawn = false)
    {
        if (dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max item reach");
            return;
        }

        // TMP heuristic
        int ligne = Mathf.RoundToInt(Mathf.Sqrt(nbItems));
        int colonne = Mathf.FloorToInt(nbItems/ ligne);
        for (int i = 0; i < colonne; i++)
        {
            for (int j = 0; j < ligne; j++)
            {
                SpawnedItemsCount++;
                ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                    GetVector3ArrayOnAGrid(dicSpawnItemsLocations[idLocation].transform.position, dicSpawnItemsLocations[idLocation].transform.forward, ligne, colonne)[i,j],
                    dicSpawnItemsLocations[idLocation].transform.rotation,
                    dicSpawnItemsLocations[idLocation].transform,
                    myItemType
                ).GetComponent<Collectable>().Init();

            }
        }
    }

    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnItemLocation(Transform mySpawnLocation, CollectableType myItemType, bool needSpawn = false, bool forceSpawn = false, Shapes shapes = Shapes.None, int nbItems = 1, float circleRadius = 1.0f)
    {
        dicSpawnItemsLocations.Add(lastInsertedKeySpawnItems, mySpawnLocation);

        if (needSpawn)
        {

            switch (shapes)
            {
                case Shapes.None:
                    SpawnItem(lastInsertedKeySpawnItems, myItemType, forceSpawn);
                    break;
                case Shapes.Circle:
                    SpawnCircleShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn, circleRadius);
                    break;
                case Shapes.Line:
                    SpawnLineShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn);
                    break;
                case Shapes.Grid:
                    SpawnGridShapedItems(lastInsertedKeySpawnItems, nbItems, myItemType, forceSpawn);
                    break;
            }

        }

        return lastInsertedKeySpawnItems++;
    }

    // call on destroy on a spawn item
    public void UnregisterSpawnItemLocation(int idToUnregister)
    {
        dicSpawnItemsLocations.Remove(idToUnregister);
    }

    #endregion

    #region Monsters
    private void SpawnMonster(int idLocation, MonsterType myMonsterType, bool forceSpawn = false)
    {
        if (dicSpawnMonstersLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedMonsterCount == MAXSPAWNMONSTERSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedMonsterCount++;
        ResourceUtils.Instance.refPrefabMonster.SpawnMonsterInstance(
            dicSpawnMonstersLocations[idLocation].transform.position,
            dicSpawnMonstersLocations[idLocation].transform.rotation,
            dicSpawnMonstersLocations[idLocation].transform,
            myMonsterType
        );
    }

    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnMonsterLocation(Transform mySpawnLocation, MonsterType myMonsterType, bool needSpawn = false, bool forceSpawn = false)
    {
        dicSpawnMonstersLocations.Add(lastInsertedKeySpawnMonsters, mySpawnLocation);

        if (needSpawn)
        {
            SpawnMonster(lastInsertedKeySpawnMonsters, myMonsterType, forceSpawn);
        }

        return lastInsertedKeySpawnMonsters++;
    }

    // call on destroy on a spawn monster
    public void UnregisterSpawnMonsterLocation(int idToUnregister)
    {
        dicSpawnMonstersLocations.Remove(idToUnregister);
    }
    #endregion


    // Won't work if there is no hub manager
    #region Evolution Islands 
    public int RegisterSpawnEvolutionIslandLocation(Transform mySpawnLocation, bool needSpawn = false, bool forceSpawn = false)
    {
        if (dicSpawnEvolutionIslandLocations.ContainsKey(lastInsertedKeySpawnEvolutionIsland))
        {
            dicSpawnEvolutionIslandLocations.Clear();
        }
        dicSpawnEvolutionIslandLocations.Add(lastInsertedKeySpawnEvolutionIsland, mySpawnLocation);

        if (needSpawn)
        {
            SpawnEvolutionIsland(lastInsertedKeySpawnEvolutionIsland, forceSpawn);
        }

        return lastInsertedKeySpawnEvolutionIsland++;
    }

    // call on destroy on a spawn ile
    public void UnregisterSpawnEvolutionIslandLocation(int idToUnregister)
    {
        dicSpawnEvolutionIslandLocations.Remove(idToUnregister);
    }

    [System.Obsolete("This is an obsolete method")]
    private void SpawnEvolutionIsland(int idLocation, bool forceSpawn)
    {
        /*
        if (dicSpawnEvolutionIslandLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedEvolutionIslandCount == MAXSPAWNEVOLUTIONISLANDSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedEvolutionIslandCount++;
        GameObject spawnedIsland = HUBManager.instance.islandSpawner.SpawnEvolutionIslandInstance(
            dicSpawnEvolutionIslandLocations[idLocation].transform.position,
            dicSpawnEvolutionIslandLocations[idLocation].transform.rotation,
            dicSpawnEvolutionIslandLocations[idLocation].transform
        );

        Transform evolutionSpawn = spawnedIsland.transform.GetChild(0);
        if (!evolutionSpawn.name.Contains("Evolution"))
        {
            Debug.LogWarning("WARNING: EvolutionSpawn should be first on island");
            return;
        }



        CollectableType evolutionType = GetNextEvolutionType();
        if (evolutionType == CollectableType.Size)
            return;

        evolutionSpawn.GetChild(0).GetComponentInChildren<CostArea>().InitForEvolution(evolutionType);
        //ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
        //    evolutionSpawn.position,
        //    evolutionSpawn.rotation,
        //    evolutionSpawn,
        //    evolutionType
        //).GetComponent<Collectable>().Init();


        MeshRenderer panneau = associatedShelter.transform.GetChild(0).GetComponent<MeshRenderer>();
        MeshRenderer panneau2 = associatedShelter.transform.GetChild(1).GetComponent<MeshRenderer>();

        if (panneau == null || (panneau != null && !panneau.name.Contains("Panneau"))) {
            Debug.LogWarning("WARNING: Panneau should be first on shelter");
            return;
        }
        if (panneau2 == null || (panneau2 != null && !panneau2.name.Contains("Panneau")))
        {
            Debug.LogWarning("WARNING: Panneau should be second on shelter");
            return;
        }

        switch (evolutionType)
        {
            case CollectableType.StrengthEvolution1:
                panneau.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Hammer);
                panneau.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Hammer);

                panneau2.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Hammer);
                panneau2.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Hammer);

                break;
            case CollectableType.AgileEvolution1:
                panneau.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Agile);
                panneau.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Agile);

                panneau2.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Agile);
                panneau2.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Agile);
                break;
            case CollectableType.GhostEvolution1:
                panneau.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Ghost);
                panneau.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Ghost);

                panneau2.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Ghost);
                panneau2.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Ghost);

                break;
            case CollectableType.PlatformistEvolution1:
                panneau.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Platformist);
                panneau.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Platformist);
      
                panneau2.material.SetTexture("_MainTex", ResourceUtils.Instance.spriteUtils.Platformist);
                panneau2.material.SetTexture("_EmissionMap", ResourceUtils.Instance.spriteUtils.Platformist);
                break;
        }

        panneau.material.SetFloat("_EmissionScaleUI", 0.2f);
        panneau2.material.SetFloat("_EmissionScaleUI", 0.2f);

        InitTeleporter initTeleporterComponent = associatedShelter.GetComponentInChildren<InitTeleporter>();
        initTeleporterComponent.evolutionType = evolutionType;

        Utils.Shuffle(HUBManager.instance.gameplayRoomStarters);
        foreach (Tampax tmp in HUBManager.instance.gameplayRoomStarters)
        {
            if(tmp.gameplayRoomStarter != null)
            {
                if (tmp.evolutionType == evolutionType)
                {
                    associatedShelter.GetComponentInChildren<PlatformGameplay>().teleporterTarget = tmp.gameplayRoomStarter.transform;

                    // Bind the target to return to origin
                    tmp.gameplayRoomStarter.GetComponent<PlatformGameplay>().teleporterTarget = associatedShelter.GetComponentInChildren<PlatformGameplay>().transform;
                    tmp.gameplayRoomStarter.GetComponent<InitTeleporter>().evolutionType = evolutionType;
                    //GameObject shelterFeedbackToSpawn = ResourceUtils.Instance.refPrefabIle.GetShelterFeedbackFromEvolutionName(evolutionType);
                    //if (shelterFeedbackToSpawn != null)
                    //{
                    //    GameObject shelterFeedback = Instantiate(shelterFeedbackToSpawn, associatedShelter.GetComponentInChildren<PlatformGameplay>().transform);
                    //    shelterFeedback.transform.localPosition = Vector3.up * 3.0f;
                    //}
                }
            } 
        }
        */
    }

    CollectableType GetNextEvolutionType()
    {
        if (evolutionsLeftToSpawn == null || evolutionsLeftToSpawn.Count == 0)
        {
            Debug.LogWarning("The four evolutions have already been spawned.");
            return CollectableType.Size;
        }

        CollectableType evolutionType = evolutionsLeftToSpawn[0];
        evolutionsLeftToSpawn.RemoveAt(0);
        return evolutionType;
    }


    #endregion

    #region Points Islands
    public int RegisterSpawnPointsIslandLocation(Transform mySpawnLocation, bool needSpawn = false, bool forceSpawn = false)
    {
        dicSpawnPointsIslandLocations.Add(lastInsertedKeySpawnPointsIsland, mySpawnLocation);

        if (needSpawn)
        {
            SpawnPointIsland(lastInsertedKeySpawnPointsIsland, forceSpawn);
        }

        return lastInsertedKeySpawnPointsIsland++;
    }

    // call on destroy on a spawn ile
    public void UnregisterSpawnPointsIslandLocation(int idToUnregister)
    {
        dicSpawnPointsIslandLocations.Remove(idToUnregister);
    }

    [System.Obsolete("This is an obsolete method")]
    private void SpawnPointIsland(int idLocation, bool forceSpawn)
    {
        /*
        if (dicSpawnPointsIslandLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedPointIslandCount == MAXSPAWNPOINTSISLANDSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max points island reach");
            return;
        }

        SpawnedPointIslandCount++;
        HUBManager.instance.islandSpawner.SpawnPointIslandInstance(
            dicSpawnPointsIslandLocations[idLocation].transform.position,
            dicSpawnPointsIslandLocations[idLocation].transform.rotation,
            dicSpawnPointsIslandLocations[idLocation].transform
        );
        */
    }
    #endregion

    // Problably should be moved to Utils
    #region Utils
    public enum Axis { XY, XZ, YZ };
    public static Vector3[] GetVector3ArrayOnADividedCircle(Vector3 center, float radius, int divider, Axis baseAXis)
    {
        Vector3[] toReturn = new Vector3[divider];
        for (int i = 0; i < toReturn.Length; i++)
        {
            switch (baseAXis)
            {
                case Axis.XY:
                    toReturn[i] = new Vector3(
                        center.x + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                        center.y + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                        center.z
                    );

                    break;
                case Axis.XZ:
                    toReturn[i] = new Vector3(
                            center.x + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                            center.y,
                            center.z + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius)
                        );

                    break;
                case Axis.YZ:
                    toReturn[i] = new Vector3(
                center.x,
                center.y + (float)(Mathf.Sin(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius),
                center.z + (float)(Mathf.Cos(Mathf.Deg2Rad * GetAngleForIndexedDividedCircle(i, divider)) * radius)
                 );

                    break;
            }
        }
        return toReturn;
    }

    public static Vector3[] GetVector3ArrayOnLine(Vector3 origin, Vector3 direction, int nbPoint, bool isCentered = false)
    {
        Vector3[] toReturn = new Vector3[nbPoint];
      
        if (isCentered)
        {
            origin = origin - (direction / 2f);
        }

        for (int i=0; i <nbPoint; i++)
        {
            toReturn[i] = (10 * i * (direction / nbPoint)) + origin;
        }
        return toReturn;
    }

    public static Vector3[,] GetVector3ArrayOnAGrid(Vector3 origin, Vector3 direction, int nbLine = 1, int nbColonne =1 )
    {
        Vector3[,] toReturn = new Vector3[nbColonne, nbLine];
        Vector3[] line = new Vector3[nbLine]; 
        for (int i = 0; i < nbLine; i++)
        {
            line = SpawnManager.GetVector3ArrayOnLine(origin, direction, nbLine);
        }

        for (int i = 0; i < nbColonne; i++)
        {
            for (int j = 0;j < nbLine; j++)
            {
                toReturn[i, j] = line[j] + (10 * i *(new Vector3(1, 0, 0) / nbColonne));
            }
        }
        return toReturn;
    }

    public static float GetAngleForIndexedDividedCircle(int index, int divider)
    {
        return (360.0f / divider) * index;
    }
    #endregion 

}
