using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents the player.
//TODO: Too much functionality in this class- currently is the player, the Cursor, and the controller. Seperate these.
public class PlayerCursor : MonoBehaviour
{
    Level currentLevel;
    Tile currentTargetTile;
    
    public float cursorSpeed;

    EDirection currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = GameObject.FindObjectOfType<Level>();
        currentTargetTile = currentLevel.trail[0].GetComponent<Tile>();
        SnapToCurrentTargetTile();
    }

    // Update is called once per frame
    void Update()
    {
        //Control catching/Movement here
        CaptureMoveDirection(out currentDirection);
        if(currentDirection != EDirection.EDefaultDirection)
        {
            BuildRoad(currentDirection);
            SnapToCurrentTargetTile();
        }


    }

    //Sets a new position for the player cursor based on the currently selected tile.
    void SnapToCurrentTargetTile()
    {
        //Place Cursor above tile
        this.gameObject.transform.position = currentTargetTile.transform.position + Vector3.up;   
    }

    //Captures player input
    void CaptureMoveDirection(out EDirection direction)
    {
        direction = EDirection.EDefaultDirection;

        //check right
        if (Input.GetButtonDown(EInputs.Right.ToString()))
        {
            direction = EDirection.EEast;
        }
        //check left
        else if (Input.GetButtonDown(EInputs.Left.ToString()))
        {
            direction = EDirection.EWest;
        }
        //check up
        else if (Input.GetButtonDown(EInputs.Up.ToString()))
        {
            direction = EDirection.ENorth;
        }
        //check down
        else if (Input.GetButtonDown(EInputs.Down.ToString()))
        {
            direction = EDirection.ESouth;
        }
    }

    void BuildRoad(EDirection moveDirection)
    {
        EDirection undoDirection = currentTargetTile.connectedRoadFrom;
        if (moveDirection == undoDirection)
        {
            //Undo road
            //For now: Play fail noise
        }
        else 
        {
            //Check if tile has neighbor in input direction
            if (currentTargetTile.Neighbors[(int)moveDirection] != null)
            {
                if (!currentTargetTile.Neighbors[(int)moveDirection].GetComponent<Tile>().hasRoad)
                {
                    //Try to place road on neighbor in the input direction
                    if (PlaceRoad(currentTargetTile.Neighbors[(int)moveDirection].GetComponent<Tile>(), moveDirection))
                    {
                        //Road placed, change current tile
                        currentTargetTile.UpdateTileAppearance();
                        currentTargetTile = currentTargetTile.Neighbors[(int)moveDirection].GetComponent<Tile>();
                        currentTargetTile.UpdateTileAppearance();

                        //update tile graphics
                        //Road placed, add tile to trail
                        currentLevel.trail.Add(currentLevel.gameObject);
                    }
                    else
                    {
                        //Play fail noise
                    }
                }
                else
                {
                    //Play fail noise
                }
            }
            else
            {
                //play fail noise
            }
        }

    }

    bool PlaceRoad(Tile nextTargetTile, EDirection buildDirection)
    {
        bool canBuild = false;

        //Check if player has necessary resources for building a road on the tile type
        switch (nextTargetTile.tileType)
        {
            case ETileType.EStart:
                //Can't build on start tile
                break;
            case ETileType.ECity:
                canBuild = true;
                currentLevel.citiesTrailed += 1;
                break;
            case ETileType.EPlains:
                if(currentLevel.currentBudget > 0)
                {
                    canBuild = true;
                    currentLevel.currentBudget-=1;
                }
                break;
            case ETileType.EForest:
                if (currentLevel.currentBudget > 0  && currentLevel.axesHeld > 0)
                {
                    canBuild = true;
                    currentLevel.currentBudget -= 1;
                    currentLevel.axesHeld -= 1;
                }
                break;
            case ETileType.ERiver:
                if (currentLevel.currentBudget > 0 && currentLevel.bridgesHeld > 0)
                {
                    //Can't build from river to river
                    if(currentTargetTile.GetComponent<Tile>().tileType == ETileType.ERiver)
                    {
                        break;
                    }

                    bool validTileIntersection = false;
                    switch (nextTargetTile.GetComponent<Tile>().tileIntersectionType)
                    {
                        case EIntersectionType.EEnd:
                            validTileIntersection = true;
                            break;
                        case EIntersectionType.EThrough:
                            validTileIntersection = true;
                            break;
                        case EIntersectionType.ECorner:
                            break;
                        case EIntersectionType.ETJunction:
                            break;
                        case EIntersectionType.ECross:
                            break;
                        case EIntersectionType.EDefaultNone:
                            validTileIntersection = true;
                            break;
                        default:
                            break;
                    }

                    if(validTileIntersection)
                    {
                        canBuild = true;
                        currentLevel.currentBudget -= 1;
                        currentLevel.bridgesHeld -= 1;
                    }
                }
                break;
            case ETileType.EMountain:
                //Can't build on mountain tile
                break;
            case ETileType.EDefaultType:
                Debug.Log("Selected Tile has no type.", nextTargetTile);
                break;
            default:
                break;
        }

        if (canBuild)
        {
            nextTargetTile.hasRoad = true;
            currentTargetTile.connectedRoadTo = buildDirection;
            switch (buildDirection)
            {
                case EDirection.ENorth:
                    nextTargetTile.connectedRoadFrom = EDirection.ESouth;
                    break;
                case EDirection.EEast:
                    nextTargetTile.connectedRoadFrom = EDirection.EWest;
                    break;
                case EDirection.ESouth:
                    nextTargetTile.connectedRoadFrom = EDirection.ENorth;
                    break;
                case EDirection.EWest:
                    nextTargetTile.connectedRoadFrom = EDirection.EEast;
                    break;
                case EDirection.EDefaultDirection:
                    break;
                default:
                    break;
            }



            //graphics call- update model/materials or add overlay, etc. DO NOT CHANGE TILE TYPE
            //add current tile to grid's trail
            ///Tile.ShowRoad();
            ///SoundManager.PlayBuildSound() ???

            return true;
        }       
        ///SoundManager.PlayBuzzerSound() ???
        return false;
    }
}
