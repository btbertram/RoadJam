using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//A class which represents a piece of the enviornment.
public class Tile : MonoBehaviour
{
    [SerializeField]
    public ETileType tileType;

    [SerializeField]
    bool hasRoad = false;
    
    //A list of adjacent GameObjects with Tile components, ordered NESW.
    public List <GameObject> Neighbors;
    EDirection connectedRoadTo = EDirection.EDefaultDirection;
    EDirection connectedRoadFrom = EDirection.EDefaultDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        FindNeighbors();
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

    void BuildRoad()
    {
        //Change mesh/material on building a road
    }
}
