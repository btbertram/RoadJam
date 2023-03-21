using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class which represents the current level.
public class Grid : MonoBehaviour
{
    //All tiles in play.
    public GameObject[] GridList;

    //The current roadway from start to end.
    public List<GameObject> trail;

    // Start is called before the first frame update
    void Start()
    {
        GridList = GameObject.FindGameObjectsWithTag("Tile");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
