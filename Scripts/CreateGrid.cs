using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    [SerializeField]
    [Header ("If you place the \"PlaceableGrid\" don't worry about assigning these values.")]
    [Space]
    [Header("\"MainCamera\" tag or your grid will not be visible!")]
    [Header ("Make sure the camera you use for gameplay has the")]   
    [Tooltip("Set this to the \"GridSpace\" prefab after placing the \"GridSpawner\" prefab on your map.")]
    private GameObject gridSpace; //This reference is used for instantiating the grid spaces. Set this to the PREFAB "GridSpace" after placing it in your desired location.
    [SerializeField]
    [Tooltip("Set this to the \"GridParent\" object after placing the prefab for it on your map.")]
    private GameObject gridParent; //Place the "GridParent" prefab in the world and assign it in the inspector here
    [SerializeField]
    [Tooltip("Set this to the \"PlayerUnit\" object after placing the prefab for it on your map.")]
    private GameObject playerUnit; //Place the "PlayerUnit" prefab in the world and assign it in the inspector here
    [SerializeField]
    [Tooltip("Set this to the \"EnemyUnit\" object after placing the prefab for it on your map.")]
    private GameObject enemyUnit; //Place the "EnemyUnit" prefab in the world and assign it in the inspector here
    [SerializeField]
    private GameObject gridCanvas;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("The width of each space on the grid")]
    private int spaceX = 80;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("The width of each space on the grid")]
    private int spaceY = 80;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("The number of spaces wide the grid should be")]
    public int gridWidth = 1;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("The number of spaces high the grid should be")]
    public int gridHeight = 1;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("This will determine the starting position of the player unit (X, Y) on grid")]
    private int playerStartX;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("This will determine the starting position of the player unit (X, Y) on grid")]
    private int playerStartY;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("This will determine the starting position of the enemy unit (X, Y) on grid")]
    private int enemyStartX;
    [SerializeField]
    [Range(1, 1000)]
    [Tooltip("This will determine the starting position of the enemy unit (X, Y) on grid")]
    private int enemyStartY;
    [SerializeField]
    [Tooltip("This will determine how far from the ground the unit will spawn.")]
    private int playerHeightAdjust;
    [SerializeField]
    [Tooltip("This will determine how far from the ground the unit will spawn.")]
    private int enemyHeightAdjust;
    [SerializeField]
    //Below are variables used in the formula for instantiation
    private float spacePadding;
    private float spaceHeight;
    private float spaceWidth;
    private float spaceScaleX;
    private float spaceScaleY;


    void Start()
    {
        //Assign reference of canvas
        gridCanvas = gridParent.transform.GetChild(0).gameObject;
        gridCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        //Adjust size of prefab if modified
        gridSpace.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(spaceX, spaceY);
        //check size of prefab for calculations
        spaceWidth = gridSpace.transform.GetComponent<RectTransform>().sizeDelta.x;
        spaceHeight = gridSpace.transform.GetComponent<RectTransform>().sizeDelta.y;
        spaceScaleX = gridSpace.transform.localScale.x;
        spaceScaleY = gridSpace.transform.localScale.y;
        //extra check just to be safe
        if ((gridWidth + gridHeight) > 0)
        {
            SpawnGrid();
        }
    }

    void SpawnGrid()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            Vector3 adjustedPosition = this.gameObject.transform.position + new Vector3(0, 0, (spaceHeight * spaceScaleY * i + (spacePadding * spaceScaleY * i)));
            GameObject spaceInstance = Instantiate(gridSpace, adjustedPosition, gridSpace.transform.rotation, gridCanvas.transform) as GameObject;
            int rowNum = i;
            if(playerStartY == (i + 1) && playerStartX == 1)
            {
                Vector3 heightAdjust = new Vector3(0, playerHeightAdjust, 0);
                Instantiate(playerUnit, (spaceInstance.transform.position + heightAdjust), playerUnit.transform.rotation);
            }
            if (enemyStartY == (i + 1) && enemyStartX == 1)
            {
                Vector3 heightAdjust = new Vector3(0, enemyHeightAdjust, 0);
                Instantiate(enemyUnit, (spaceInstance.transform.position + heightAdjust), enemyUnit.transform.rotation);
            }
            FillRow(spaceInstance, rowNum);
        }
    }

    void FillRow(GameObject rowSpace, int row)
    {
        for (int i = 1; i < gridWidth; i++)
        {
            Vector3 adjustedPosition = rowSpace.transform.position + new Vector3((spaceWidth * spaceScaleX * i + (spacePadding * spaceScaleX * i)), 0, 0);
            GameObject spaceInstance = Instantiate(gridSpace, adjustedPosition, gridSpace.transform.rotation, gridCanvas.transform);
            if(playerStartX == (i + 1) && playerStartY == (row + 1))
            {
                Vector3 heightAdjust = new Vector3(0, playerHeightAdjust, 0);
                Instantiate(playerUnit, (spaceInstance.transform.position + heightAdjust), playerUnit.transform.rotation);
            }
            if (enemyStartX == (i + 1) && enemyStartY == (row + 1))
            {
                Vector3 heightAdjust = new Vector3(0, enemyHeightAdjust, 0);
                Instantiate(enemyUnit, (spaceInstance.transform.position + heightAdjust), enemyUnit.transform.rotation);
            }
        }
    }
}
