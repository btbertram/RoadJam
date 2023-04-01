using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which represents the player state.
public class Player : MonoBehaviour
{
    int currentBudget;
    Tile currentTilePosition;
    Level currentLevel;
    PlayerCursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = GameObject.FindObjectOfType<Level>();
        cursor = FindObjectOfType<PlayerCursor>();
        currentTilePosition = currentLevel.trail[0].GetComponent<Tile>();
        cursor.SetTargetTile(currentTilePosition);
        cursor.SnapToCurrentTargetTile();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Attempts to build a road in the given direction away from the player.
    public void BuildRoad(EDirection moveDirection)
    {
        EDirection undoDirection = currentTilePosition.connectedRoadFrom;
        if (moveDirection == undoDirection)
        {
            //Undo road
            //For now: Play fail noise
        }
        else
        {
            //Check if tile has neighbor in input direction
            if (currentTilePosition.Neighbors[(int)moveDirection] != null)
            {
                if (!currentTilePosition.Neighbors[(int)moveDirection].GetComponent<Tile>().hasRoad)
                {
                    //Try to place road on neighbor in the input direction
                    if (PlaceRoad(currentTilePosition.Neighbors[(int)moveDirection].GetComponent<Tile>(), moveDirection))
                    {
                        //Road placed, change current tile
                        currentTilePosition.UpdateTileAppearance();
                        currentTilePosition = currentTilePosition.Neighbors[(int)moveDirection].GetComponent<Tile>();
                        currentTilePosition.UpdateTileAppearance();

                        //Road placed, add tile to trail
                        currentLevel.trail.Add(currentLevel.gameObject);

                        cursor.SetTargetTile(currentTilePosition);
                        cursor.SnapToCurrentTargetTile();
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

    //Checks if a road can be built, and builds a road if it can, taking resources necessary to do so.
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
                if (currentLevel.currentBudget > 0)
                {
                    canBuild = true;
                    currentLevel.currentBudget -= 1;
                }
                break;
            case ETileType.EForest:
                if (currentLevel.currentBudget > 0 && currentLevel.axesHeld > 0)
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
                    if (currentTilePosition.GetComponent<Tile>().tileType == ETileType.ERiver)
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

                    if (validTileIntersection)
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
            currentTilePosition.connectedRoadTo = buildDirection;
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
