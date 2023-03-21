using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents the player.
public class PlayerCursor : MonoBehaviour
{
    Level currentLevel;
    Tile currentTargetTile;
    
    public float cursorSpeed;

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
        EDirection currentDirection = CaptureMoveDirection();
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
    EDirection CaptureMoveDirection()
    {
        EDirection moveDirection = EDirection.EDefaultDirection;

        //check right
        if (Input.GetButtonDown(EInputs.Right.ToString()))
        {
            moveDirection = EDirection.EEast;
        }
        //check left
        else if (Input.GetButtonDown(EInputs.Left.ToString()))
        {
            moveDirection = EDirection.EWest;
        }
        //check up
        else if (Input.GetButtonDown(EInputs.Up.ToString()))
        {
            moveDirection = EDirection.ENorth;
        }
        //check down
        else if (Input.GetButtonDown(EInputs.Down.ToString()))
        {
            moveDirection = EDirection.ESouth;
        }

        return moveDirection;
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
                        currentTargetTile = currentTargetTile.Neighbors[(int)moveDirection].GetComponent<Tile>();
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
    //graphics call- update model/materials or add overlay, etc. DO NOT CHANGE TILE TYPE
    //add current tile to grid's trail
    ///Tile.ShowRoad();
    ///SoundManager.PlayBuildSound() ???

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
                //Building on city tile costs no resources, increase gamestate variable
                break;
            case ETileType.EPlains:
                if(true/*GameState.RoadsRemaining > 0*/)
                {
                    canBuild = true;
                    //GameState.RoadsRemaining -= 1;
                }
                break;
            case ETileType.EForest:
                //requires axe
                break;
            case ETileType.ERiver:
                //Can't build from river to river
                //Can't build on river corners (check neighbors)
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
