using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents the player's visual representation.
//TODO: Too much functionality in this class- currently is the player, the Cursor, and the controller. Seperate these.
public class PlayerCursor : MonoBehaviour
{
    Tile currentTargetTile;
    public float cursorSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Sets a new position for the player cursor based on the currently selected tile.
    public void SnapToCurrentTargetTile()
    {
        //Place Cursor above tile
        this.gameObject.transform.position = currentTargetTile.transform.position + Vector3.up;   
    }

    public void SetTargetTile(Tile targetTile)
    {
        currentTargetTile = targetTile;
    }




}
