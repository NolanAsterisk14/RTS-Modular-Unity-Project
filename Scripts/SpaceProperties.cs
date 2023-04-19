using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpaceProperties : MonoBehaviour
{
    public bool occupied;
    public bool playerAdjacent;
    public bool enemyAdjacent;
    public bool rayCheckFired = false;
    public bool obstructed;
    public bool breakable;
    public bool building;
    public bool playerSelectable = false;
    public bool enemySelectable = false;
    public bool menuOpen = false;
    public bool canMoveTo = false;
    public bool moveConfirmed = false;
    public bool attackSelected = false;
    public List<GameObject> adjacentSpaces = new List<GameObject>();
    [SerializeField]
    [Tooltip("This will set the height the menu appears at when a space is selected by the player")]
    [Range(1, 100)]
    private float menuHeight = 5;
    [SerializeField]
    [Tooltip("This will set the X coordinate the menu appears at when a space is selected by the player")]
    [Range(-50, 50)]
    private float menuX = 3;
    [SerializeField]
    [Tooltip("This will set the Z coordinate the menu appears at when a space is selected by the player")]
    [Range(-50, 50)]
    private float menuZ = -1;
    [SerializeField]
    public GameObject occupyingUnit;
    [SerializeField]
    private GameObject contextMenus; //Assign this value in the inspector!
    [SerializeField]
    private GameObject confirmMenu; //Assign this value in the inspector!
    [SerializeField]
    public GameObject healthBar; //Assign this value in the inspector!
    [SerializeField]
    private GameObject unitContext;
    [SerializeField]
    private GameObject attackContext;
    [SerializeField]
    public GameObject confirmInstance;
    SpaceTrigger spaceTriggerScriptRef;
    ContextBehavior contextBehaviorRef;
    ContextBehavior contextBehaviorConfirmRef;
    [SerializeField]
    ConfirmBehavior confirmBehaviorRef;
    MovementTest movementTestRef;
    TurnManager turnManagerRef;

    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(SpaceSelected);
        unitContext = contextMenus.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        attackContext = contextMenus.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        unitContext.SetActive(false);
        attackContext.SetActive(false);
        turnManagerRef = GameObject.Find("PlaceableGrid").transform.Find("TurnManager").GetComponent<TurnManager>();
        spaceTriggerScriptRef = this.gameObject.transform.GetChild(1).gameObject.GetComponent<SpaceTrigger>();       
    }

    void Update()
    {
        if (spaceTriggerScriptRef != null)
        {
            occupyingUnit = spaceTriggerScriptRef.triggerUnit;
        }
        if (occupyingUnit != null && occupyingUnit.name == "EnemyUnit(Clone)")
        {
            this.gameObject.tag = "EnemyOccupied";
        }
        if (occupyingUnit != null && occupyingUnit.name == "PlayerUnit(Clone)")
        {
            this.gameObject.tag = "PlayerOccupied";
        }
        if (canMoveTo == true && spaceTriggerScriptRef.gameObject.layer == 0 && this.gameObject.tag != "PlayerOccupied")
        {
            spaceTriggerScriptRef.gameObject.layer = 9;
        }
        if (canMoveTo == false && spaceTriggerScriptRef.gameObject.layer == 9)
        {
            spaceTriggerScriptRef.gameObject.layer = 0;
        }
        if (confirmBehaviorRef != null)
        {
            moveConfirmed = confirmBehaviorRef.confirmPressed;
        }
        if (contextBehaviorConfirmRef != null)
        {
            attackSelected = contextBehaviorConfirmRef.attackSelected;
            moveConfirmed = contextBehaviorConfirmRef.attackSelected;
        }
        if ((playerAdjacent == true || enemyAdjacent == false) && this.gameObject.tag != "AdjacentSpace")
        {
            this.gameObject.tag = "AdjacentSpace";
        }
        if (playerAdjacent == false && enemyAdjacent == false && this.gameObject.tag == "AdjacentSpace")
        {
            this.gameObject.tag = "GridPart";
        }
        
    }

    void FixedUpdate()
    {
        if (rayCheckFired == false)
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.up);
            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 back = transform.TransformDirection(Vector3.down);
            Vector3 left = transform.TransformDirection(Vector3.left);
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            for (int i = 0; i < 4; i++)
            {
                Vector3 rayDirection = new Vector3();

                switch (i)
                {
                    case 0:
                        rayDirection = fwd;
                        break;
                    case 1:
                        rayDirection = right;
                        break;
                    case 2:
                        rayDirection = back;
                        break;
                    case 3:
                        rayDirection = left;
                        break;
                    default:
                        Debug.Log("SpaceProperties' case statement got messed up. Check code.");
                        break;
                }

                if (Physics.Raycast(this.gameObject.transform.position, rayDirection, out hit, 8f, layerMask, QueryTriggerInteraction.Collide))
                {
                    Debug.DrawRay(this.gameObject.transform.position, rayDirection * hit.distance, Color.red);
                    Debug.Log("Did Hit");
                    if (adjacentSpaces.Contains(hit.transform.gameObject) == false)
                    {
                        adjacentSpaces.Add(hit.transform.gameObject);
                    }
                }
                else
                {
                    Debug.DrawRay(this.gameObject.transform.position, rayDirection * 8, Color.white);
                    Debug.Log("Did not Hit");
                }
            }
            if (occupied == true)
            {
                foreach (GameObject spaceNear in adjacentSpaces)
                {
                    spaceNear.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().playerAdjacent = true;
                }
            }
            rayCheckFired = true;
            AdjacencyUpdate();
        }
        
    }

    public void SpaceSelected()
    {
        if (playerSelectable == false && enemySelectable == false && canMoveTo == true && this.gameObject.tag != "AdjacentSpace")
        {
            Vector3 adjustedPosition = new Vector3(menuX, menuHeight, menuZ);
            GameObject menuInstance = Instantiate(confirmMenu, (this.transform.position + adjustedPosition), confirmMenu.transform.rotation);
            confirmBehaviorRef = menuInstance.gameObject.GetComponent<ConfirmBehavior>();
            menuInstance.gameObject.GetComponent<ConfirmBehavior>().spaceRef = this.gameObject;
            menuInstance.gameObject.GetComponent<ConfirmBehavior>().originalSpaceRef = GameObject.Find("ContextMenus(Clone)").GetComponent<ContextBehavior>().spaceRef;
            confirmInstance = menuInstance;
            StartCoroutine(ConfirmMovement());
        }
        if (playerSelectable == false && enemySelectable == false && canMoveTo == true && this.gameObject.tag == "AdjacentSpace")
        {
            Vector3 adjustedPosition = new Vector3(menuX, menuHeight, menuZ);
            GameObject menuInstance = Instantiate(contextMenus, (this.transform.position + adjustedPosition), contextMenus.transform.rotation);
            menuInstance.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
            menuInstance.gameObject.GetComponent<ContextBehavior>().spaceRef = this.gameObject;
            contextBehaviorConfirmRef = menuInstance.GetComponent<ContextBehavior>();
            menuOpen = true;
            StartCoroutine(ConfirmMovement());
        }
        if (playerSelectable == true && menuOpen == false && canMoveTo == false && turnManagerRef.playerTurn == true)
        {
            Vector3 adjustedPosition = new Vector3(menuX, menuHeight, menuZ);
            GameObject menuInstance = Instantiate(contextMenus, (this.transform.position + adjustedPosition), contextMenus.transform.rotation);
            menuInstance.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
            menuInstance.gameObject.GetComponent<ContextBehavior>().spaceRef = this.gameObject;
            contextBehaviorRef = menuInstance.GetComponent<ContextBehavior>();
            menuOpen = true;
        }
        if (enemySelectable == true && menuOpen == false && canMoveTo == false && turnManagerRef.playerTurn == false)
        {
            Vector3 adjustedPosition = new Vector3(menuX, menuHeight, menuZ);
            GameObject menuInstance = Instantiate(contextMenus, (this.transform.position + adjustedPosition), contextMenus.transform.rotation);
            menuInstance.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
            menuInstance.gameObject.GetComponent<ContextBehavior>().spaceRef = this.gameObject;
            contextBehaviorRef = menuInstance.GetComponent<ContextBehavior>();
            menuOpen = true;
        }
    }

    public void AdjacencyUpdate()
    {
        if (rayCheckFired == true && adjacentSpaces != null)
        {
            if (occupied == true)
            {
                if (occupyingUnit.name == "PlayerUnit(Clone)")
                {
                    foreach (GameObject spaceNear in adjacentSpaces)
                    {
                        spaceNear.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().playerAdjacent = true;
                    }
                }
                if (occupyingUnit.name == "EnemyUnit(Clone)")
                {
                    foreach (GameObject spaceNear in adjacentSpaces)
                    {
                        spaceNear.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().enemyAdjacent = true;
                    }
                }
            }
            if (occupied == false)
            {
                foreach (GameObject spaceNear in adjacentSpaces)
                {
                    spaceNear.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().playerAdjacent = false;
                    spaceNear.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().enemyAdjacent = false;
                }
            }
        }
    }

    IEnumerator WaitForUnit()
    {
        if (occupyingUnit == null)
        {
            Debug.Log("Waiting for player unit to arrive...");
        }
        while (occupyingUnit == null)
        {
            yield return null;
        }
        if (occupyingUnit != null)
        {
            Debug.Log("Unit arrived! Counting down...");
            yield return new WaitForSecondsRealtime(2);
            Debug.Log("Count reached!");
            movementTestRef.target = null;
            movementTestRef.moveSelected = false;
            movementTestRef.shouldMove = false;
            if (this.gameObject.tag == "GridPart")
            {
                contextBehaviorRef.CancelButtonPress();
            }
            if (this.gameObject.tag == "AdjacentSpace")
            {
                contextBehaviorConfirmRef.CancelButtonPress();
            }
            //yield return new WaitForSecondsRealtime(2);
            if (attackSelected == false)
            {
                turnManagerRef.TurnChange();
            }
            if (attackSelected == true)
            {

            }
        }
    }

    IEnumerator ConfirmMovement()
    {
        Debug.Log("ComfirmMovement Started");
        yield return new WaitWhile(() => moveConfirmed == false);
        Debug.Log("WaitWhile In ConfirmMovement Passed");
        GameObject playerSpace = GameObject.FindWithTag("PlayerOccupied");
        movementTestRef = playerSpace.GetComponent<SpaceProperties>().occupyingUnit.gameObject.GetComponent<MovementTest>();
        movementTestRef.moveSelected = true;
        movementTestRef.shouldMove = true;
        movementTestRef.target = this.gameObject;
        if (occupyingUnit == null)
        {
            StartCoroutine(WaitForUnit());
            yield return null;
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("Attack Started");
        GameObject unitAttacked = null;
        UnitProperties unitAttackedPropRef;
        foreach (GameObject spaceNear in adjacentSpaces)
        {
            if (spaceNear.GetComponent<SpaceProperties>().occupied == true)
            {
                unitAttacked = spaceNear.GetComponent<SpaceProperties>().occupyingUnit;
            }
        }
        unitAttackedPropRef = unitAttacked.GetComponent<UnitProperties>();
        Vector3 adjustedPosition = new Vector3(menuX, menuHeight, menuZ);
        GameObject healthInstance = Instantiate(healthBar, (unitAttacked.transform.position + adjustedPosition), healthBar.transform.rotation);
        Slider healthSlider = healthInstance.transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
        healthSlider.value = unitAttackedPropRef.health;
        yield return null;
    }

}
