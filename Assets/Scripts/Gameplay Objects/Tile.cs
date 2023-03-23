using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents a piece of the enviornment.
public class Tile : MonoBehaviour
{
    [SerializeField]
    public ETileType tileType;

    public bool hasRoad = false;
    
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
        //SetTileAppearance();
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
            if (gameObject != null)
            {
                if (gameObject.GetComponent<Tile>().tileType == tileType)
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
                            tileFacing = EDirection.ENorth;                            
                        }
                        else
                        {
                            tileFacing = EDirection.EWest;                            
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
                    if (connectedRoadTo == EDirection.ENorth)
                    {
                        if (connectedRoadFrom == EDirection.EEast)
                        {
                            roadFacing = EDirection.ESouth;
                        }
                        else
                        {
                            roadFacing = EDirection.EEast;
                        }

                    }
                    else
                    {
                        if (connectedRoadFrom == EDirection.EWest)
                        {
                            tileFacing = EDirection.ENorth;
                        }
                        else
                        {
                            tileFacing = EDirection.EWest;
                        }
                    }
                }
            }
        }
    }

    //Checks Neighbors to determine appearance. By default, the apparance of the tile will match the type, determined by the prefab in Unity.
    void SetTileAppearance()
    {
        if (hasRoad)
        {
            //Find connected roads, find facing
            //CheckMatchingType(hasRoad);


            //give it a road tile
            switch (roadIntersectionType)
            {
                case EIntersectionType.EEnd:
                    break;
                case EIntersectionType.EThrough:
                    break;
                case EIntersectionType.ECorner:
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
                            break;
                        case EIntersectionType.EThrough:
                            break;
                        case EIntersectionType.ECorner:
                            break;
                        case EIntersectionType.ETJunction:
                            break;
                        case EIntersectionType.ECross:
                            break;
                        case EIntersectionType.EDefaultNone:
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;

            }
        }

    }

}
