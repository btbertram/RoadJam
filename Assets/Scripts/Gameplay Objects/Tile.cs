using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents a piece of the enviornment.
public class Tile : MonoBehaviour
{
    [SerializeField]
    public ETileType tileType;
    public bool hasRoad = false;
    public int tileCost;
    
    //A list of adjacent GameObjects with Tile components, ordered NESW.
    public List <GameObject> Neighbors;
    public EDirection connectedRoadTo = EDirection.EDefaultDirection;
    public EDirection connectedRoadFrom = EDirection.EDefaultDirection;

    //These are distinct so that a road can be overlaid on a tile, and reverted later.
    public EIntersectionType tileIntersectionType = EIntersectionType.EDefaultNone;
    public EIntersectionType roadIntersectionType = EIntersectionType.EDefaultNone;

    //Tiles face outwards, eg if a T Junction road connected N,S,W, it would face E
    //Corners face outwards based on a left handed curve, eg a N facing corner would connect S and W
    public EDirection tileFacing = EDirection.EDefaultDirection;
    public EDirection roadFacing = EDirection.EDefaultDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        FindNeighbors();
        CheckMatchingType();
        SetTileCost(FindObjectOfType<Level>().tileCostOverride);
        UpdateTileAppearance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Populates the neighbor list so that it can be accessed.
    void InitializeNeighbors()
    {
        Neighbors.Clear();
        for(int x = 0; x < (int)EDirection.EDefaultDirection; x++)
        {
            Neighbors.Add(null);
        }
    }

    //Finds other tiles in the scene placed directly adjacent to itself.
    void FindNeighbors()
    {
        InitializeNeighbors(); 
        RaycastHit hit;
        //North
        if(Physics.Raycast(transform.position, Vector3.forward, out hit, 1))
        {
            Neighbors[0] = hit.collider.gameObject;
        }
        //East
        if(Physics.Raycast(transform.position, Vector3.right, out hit, 1))
        {
            Neighbors[1] = hit.collider.gameObject;
        }
        //South
        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1))
        {
            Neighbors[2] = hit.collider.gameObject;
        }
        //West
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1))
        {
            Neighbors[3] = hit.collider.gameObject;
        }
    }

    void CheckMatchingType()
    {
        int matches = 0;
        bool[] matchDirections = { false, false, false, false };

        for (int x = 0; x < Neighbors.Count; x++)
        {
            if (Neighbors[x] != null)
            {
                if (Neighbors[x].GetComponent<Tile>().tileType == tileType)
                {
                    matches += 1;
                    matchDirections[x] = true;
                }                
            }
        }

        switch (matches)
        {
            case 0:
                break;
            case 1:
                tileIntersectionType = EIntersectionType.EEnd;
                //find facing direction
                for (int x = 0; x < 4; x++)
                {
                    if (matchDirections[x])
                    {
                        int directionNumber = x;
                        if(x <= (int)EDirection.EEast)
                        {
                            directionNumber += 2;
                        }
                        else
                        {
                            directionNumber -= 2;
                        }
                    
                        tileFacing = (EDirection)directionNumber;
                    }
                }
                break;
            case 2:
                //Test two opposite ends to see if through, North South chosen
                if (matchDirections[(int)EDirection.ENorth] == matchDirections[(int)EDirection.ESouth])
                {
                    tileIntersectionType = EIntersectionType.EThrough;
                    //Find facing direction
                    if (matchDirections[(int)EDirection.EEast])
                    {
                        tileFacing = EDirection.ENorth;                        
                    }
                    else
                    {
                        tileFacing = EDirection.EEast;                        
                    }
                }
                else
                {
                    tileIntersectionType = EIntersectionType.ECorner;
                    //Find corner facing direction
                    //North
                    if (matchDirections[(int)EDirection.ENorth])
                    {
                        if (matchDirections[(int)EDirection.EEast])
                        {
                           tileFacing = EDirection.ESouth;
                        
                        }
                        else
                        {
                            tileFacing = EDirection.EEast;                            
                        }
                    }
                    //South
                    else
                    {
                        if (matchDirections[(int)EDirection.EEast])
                        {
                            tileFacing = EDirection.EWest;                            
                        }
                        else
                        {
                            tileFacing = EDirection.ENorth;                            
                        }
                    }
                }
                break;
            case 3:
                tileIntersectionType = EIntersectionType.ETJunction;
                //find facing direction
                for (int x = 0; x < 4; x++)
                {
                    if (!matchDirections[x])
                    {
                        tileFacing = (EDirection)x;
                        x += 4;
                    }
                }
                break;
            case 4:
                tileIntersectionType = EIntersectionType.ECross;
                //No facing direction required
                break;
            default:
                break;
        }
    }

    //Updates the tile's road status.
    void UpdateRoad()
    {
        //If connected from a previous road...
        if(connectedRoadFrom != EDirection.EDefaultDirection)
        {
            int oppositeDirection = (int)connectedRoadFrom;
            if ((int)connectedRoadFrom <= (int)EDirection.EEast)
            {
                oppositeDirection += 2;
            }
            else
            {
                oppositeDirection -= 2;
            }

            //but not to another road...
            if (connectedRoadTo == EDirection.EDefaultDirection)
            {
                roadIntersectionType = EIntersectionType.EEnd;
                roadFacing = (EDirection)oppositeDirection;

            }
            else
            {
                //Figure out if it's a through or corner
                if((EDirection)oppositeDirection == connectedRoadTo)
                {
                    roadIntersectionType = EIntersectionType.EThrough;
                    if(connectedRoadFrom == EDirection.ENorth || connectedRoadFrom == EDirection.ESouth)
                    {
                        roadFacing = EDirection.EEast;
                    }
                    else
                    {
                        roadFacing = EDirection.ENorth;
                    }

                }
                else
                {
                    roadIntersectionType = EIntersectionType.ECorner;

                    switch (connectedRoadFrom)
                    {
                        case EDirection.ENorth:
                            if(connectedRoadTo == EDirection.EEast)
                            {
                                roadFacing = EDirection.ESouth;
                            }
                            else
                            {
                                roadFacing = EDirection.EEast;
                            }
                            break;
                        case EDirection.EEast:
                            if(connectedRoadTo == EDirection.ENorth)
                            {
                                roadFacing = EDirection.ESouth;
                            }
                            else
                            {
                                roadFacing = EDirection.EWest;
                            }
                            break;
                        case EDirection.ESouth:
                            if(connectedRoadTo == EDirection.EEast)
                            {
                                roadFacing = EDirection.EWest;
                            }
                            else
                            {
                                roadFacing = EDirection.ENorth;
                            }
                            break;
                        case EDirection.EWest: 
                            if( connectedRoadTo == EDirection.ENorth)
                            {
                                roadFacing = EDirection.EEast;
                            }
                            else
                            {
                                roadFacing = EDirection.ENorth;
                            }

                            break;   
                    }
                }
            }
        }
    }

    //Checks Neighbors to determine appearance. By default, the apparance of the tile will match the type, determined by the prefab in Unity.
    public void UpdateTileAppearance()
    {
        if (hasRoad)
        {
            UpdateRoad();
            //give it a road tile
            switch (roadIntersectionType)
            {
                case EIntersectionType.EEnd:
                    ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_RoadTileEndCap.fbx", 0);
                    break;
                case EIntersectionType.EThrough:
                    ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_RoadTileMiddle.fbx", 90);
                    break;
                case EIntersectionType.ECorner:
                    ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_RoadTileCorner.fbx", 0);
                    break;
            }
        }
        //if it needs setting or rotating, do it here
        else
        {
            switch(tileType)
            {                
                case ETileType.ERiver:

                    switch (tileIntersectionType)
                    {
                        case EIntersectionType.EEnd:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_WaterTileRiverEndCap.fbx", 90);
                            break;
                        case EIntersectionType.EThrough:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_WaterTileRiverMiddle.fbx", 0);
                            break;
                        case EIntersectionType.ECorner:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_WaterTileRiverCorner.fbx", 0);
                            break;
                        case EIntersectionType.ETJunction:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_WaterTileRiverT.fbx", 0);
                            break;
                        case EIntersectionType.ECross:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_WaterTile4InsideCorners.fbx", 0);
                            break;
                        case EIntersectionType.EDefaultNone:
                            break;
                        default:
                            break;
                    }
                    break;

                case ETileType.EMountain:

                    switch (tileIntersectionType)
                    {
                        case EIntersectionType.EEnd:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_MountainTileEndCap.fbx", -90);
                            break;
                        case EIntersectionType.EThrough:
                            ReassignMeshByJunctionAndFacing("Assets/Meshes/Mesh_MountainTileRidge.fbx", 0);
                            break;
                    }

                    break;


                default:
                    break;

            }
        }

    }

    void ReassignMaterials()
    {
        Material[] materials = new Material[0];
        GameObject targetObject;

        if(hasRoad)
        {
            materials = new Material[1];
            materials[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Textures + Materials/PlaceholderMat.mat", typeof(Material));
            targetObject = gameObject.transform.Find("RoadMesh").gameObject;
            targetObject.GetComponent<MeshRenderer>().materials = materials;
        }
        else
        {
            switch (tileType)
            {
                case ETileType.EPlains:
                    materials = new Material[1];
                    materials[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Textures + Materials/FieldTex.png", typeof(Material));
                    break;
                case ETileType.EForest:
                    materials = new Material[2];
                    materials[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Textures + Materials/TreePlaneTex.png", typeof (Material));
                    materials[1] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Textures + Materials/TreeTex.png", typeof (Material));
                    break;
                default:
                    materials = new Material[1];
                    materials[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Textures + Materials/PlaceholderMat.mat", typeof(Material));
                    break;
            }
            targetObject = gameObject.transform.Find("TileMesh").gameObject;
            targetObject.GetComponent<MeshRenderer>().materials = materials;
        }
    }

    void ReassignMeshByJunctionAndFacing(string assetPath, float offset)
    {
        Mesh newMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mesh));
        GameObject meshGameObject;
        Vector3 rotation;

        if(hasRoad)
        {
            meshGameObject = gameObject.transform.Find("RoadMesh").gameObject;
            meshGameObject.GetComponent<MeshFilter>().mesh = newMesh;
            switch (roadFacing)
            {
                case EDirection.ENorth:
                    rotation = new Vector3(0, 0 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;
                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.EEast:
                    rotation = new Vector3(0, 90 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;
                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.ESouth:
                    rotation = new Vector3(0, 180 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;

                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.EWest:
                    rotation = new Vector3(0, 270 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;
                    meshGameObject.transform.Rotate(rotation);
                    break;
            }
        }
        else
        {
            meshGameObject = gameObject.transform.Find("TileMesh").gameObject;
            meshGameObject.GetComponent<MeshFilter>().mesh = newMesh;
            switch (tileFacing)
            {
                case EDirection.ENorth:
                    rotation = new Vector3(0, 0 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;

                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.EEast:
                    rotation = new Vector3(0, 90 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;

                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.ESouth:
                    rotation = new Vector3(0, 180 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;

                    meshGameObject.transform.Rotate(rotation);
                    break;
                case EDirection.EWest:
                    rotation = new Vector3(0, 270 + offset, 0);
                    meshGameObject.transform.rotation = Quaternion.identity;

                    meshGameObject.transform.Rotate(rotation);
                    break;
            }
        }
        ReassignMaterials();
    }

    void SetTileCost(bool doesLevelOverride)
    {
        if (doesLevelOverride)
        {
            switch (tileType)
            {
                case ETileType.ECity:
                    tileCost = FindObjectOfType<Level>().overrideCitiesCost;
                    break;
                case ETileType.EPlains:
                    tileCost = FindObjectOfType<Level>().overridePlainsCost;
                    break;
                case ETileType.EForest:
                    tileCost = FindObjectOfType<Level>().overrideForestsCost;
                    break;
                case ETileType.ERiver:
                    tileCost = FindObjectOfType<Level>().overrideRiversCost;
                    break;
                case ETileType.EMountain:
                    tileCost = FindObjectOfType<Level>().overrideMountainCost;
                    break;
            }
        }

    }

}
