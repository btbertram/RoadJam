using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class which represents the current level.
public class Level : MonoBehaviour
{
    //The number of roads allowed to be used for the level.
    [SerializeField]
    public int roadLimit;

    //All tiles in play.
    public GameObject[] tileList;

    //The current roadway from start to end.
    public List<GameObject> trail;


    private void Awake()
    {
        tileList = GameObject.FindGameObjectsWithTag("Tile");
        //Look through grid to find start tile
        foreach (var tileObject in tileList)
        {
            if (tileObject.GetComponent<Tile>().tileType == ETileType.EStart)
            {
                //make start tile start of trail
                trail.Add(tileObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
