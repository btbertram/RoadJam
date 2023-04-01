using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which captures player input.
public class PlayerController : MonoBehaviour
{
    Player currentPlayer;
    EDirection inputButtonDirection;


    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = FindObjectOfType<Player>();   
    }

    // Update is called once per frame
    void Update()
    {
        CaptureMoveDirection(out inputButtonDirection);
        if (inputButtonDirection != EDirection.EDefaultDirection)
        {
            currentPlayer.BuildRoad(inputButtonDirection);
        }
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

    

}
