using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//A class which represents the player.
public class PlayerCursor : MonoBehaviour
{
    GameObject currentGrid;
    GameObject currentTargetTile;

    // Start is called before the first frame update
    void Start()
    {
        currentGrid = GameObject.FindObjectOfType<Grid>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Control catching/Movement here
        


    }

    void FindStartTile()
    {
        //Look through grid to find start tile
        foreach (var tileObject in currentGrid.GetComponent<Grid>().GridList)
        {
            if(tileObject.GetComponent<Tile>().tileType == ETileType.EStart)
            {
                currentTargetTile = tileObject;
            } 
        }
        //Place Cursor above tile
        this.gameObject.transform.position = currentTargetTile.transform.position + Vector3.up;
    }

    void MoveCursor(/*Pass in caught input*/)
    {
        //switch based on input
        ///Check if direction matches connected from for "undo"
        ///Just block the command for now
        //Check if tile has neighbor
        //If yes, attempt placeroad
        //if true cursor changes currentTargetTile
        //Else stays
    }

    void PlaceRoad(Tile targetTile)
    {
        bool canBuild = false;

        //Check if player has necessary resources for building a road on the tile type
        switch (targetTile.tileType)
        {
            case ETileType.EStart:
                break;
            case ETileType.ECity:

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
                //Can't build on.
                break;
            case ETileType.EDefaultType:
                break;
            default:
                break;
        }

        if (canBuild)
        {
            //Set next tile's hasRoad to true
            //Set current tile's connectedRoadTo to next tile
            //Set next tile's connectedRoadFrom to current tile
            //change current tile/prev tile accordingly
            //graphics call- update model/materials or add overlay, etc. DO NOT CHANGE TILE TYPE
            //add current tile to grid's trail
            ///Tile.ShowRoad();
            ///SoundManager.PlayBuildSound() ???
        }
        else
        {
            ///SoundManager.PlayBuzzerSound() ???
        }


    }


}
