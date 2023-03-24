using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class which represents the current level.
public class Level : MonoBehaviour
{
    //The number of roads allowed to be used for the level.
    [SerializeField]
    public int startingBudget;
    public int currentBudget;

    [SerializeField]
    public int citiesRequired;
    public int citiesTrailed;

    public int bridgesHeld = 0;
    public int axesHeld = 0;

    public bool tileCostOverride = false;
    public int overridePlainsCost;
    public int overrideCitiesCost;
    public int overrideForestsCost;
    public int overrideRiversCost;
    public int overrideMountainCost;


    //public int tileCost = 0;

    //All tiles in play.
    public GameObject[] tileList;

    //The current roadway from start to end.
    public List<GameObject> trail;

    bool trailComplete = false;

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
        currentBudget = startingBudget;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
