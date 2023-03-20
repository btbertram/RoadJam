using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [SerializeField]
    public ETileType tileType;
        
    List <Tile> Nieghbors;

    
    // Start is called before the first frame update
    void Start()
    {
        FindNeighbors();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Finds other tiles in the scene placed directly adjacent to itself.
    void FindNeighbors()
    {

        RaycastHit hit;
        //North
        if(Physics.Raycast(transform.position, Vector3.forward, out hit, 1))
        {
            Nieghbors[0] = hit.collider.gameObject.GetComponent<Tile>();
        }
        //East
        if(Physics.Raycast(transform.position, Vector3.right, out hit, 1))
        {
            Nieghbors[0] = hit.collider.gameObject.GetComponent<Tile>();
        }
        //South
        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1))
        {
            Nieghbors[0] = hit.collider.gameObject.GetComponent<Tile>();
        }
        //West
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1))
        {
            Nieghbors[0] = hit.collider.gameObject.GetComponent<Tile>();
        }
    }

}
