using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    TurnManager turnScriptRef;
    [SerializeField]
    SpaceProperties spacePropScriptRef;
    [SerializeField]
    ContextBehavior contextScriptRef;
    [SerializeField]
    ConfirmBehavior confirmScriptRef;
    [SerializeField]
    MovementTest moveScriptRef; //Assign this in the inspector! From the same prefab
    [SerializeField]
    private bool enemyTurn;
    [SerializeField]
    private bool turnInitiated;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform spaceTransform;
    [SerializeField]
    private GameObject pickedSpace; //This will do nothing. Just used for debugging.
    [SerializeField]
    private GameObject occupyingSpace;

    void Start()
    {
        turnScriptRef = GameObject.Find("PlaceableGrid").gameObject.transform.GetChild(3).gameObject.GetComponent<TurnManager>();
    }

    void Update()
    {
        if (turnScriptRef != null)
        {
            enemyTurn = !turnScriptRef.playerTurn;
        }
        if (enemyTurn == true && turnInitiated == false)
        {
            TurnStart();
        }
        if (enemyTurn == false && turnInitiated == true)
        {
            turnInitiated = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "AdjacentSpace" || other.gameObject.transform.parent.tag == "GridPart" || other.gameObject.transform.parent.tag == "EnemyOccupied")
        {
            occupyingSpace = other.gameObject.transform.parent.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "AdjacentSpace" || other.gameObject.transform.parent.tag == "GridPart" || other.gameObject.transform.parent.tag == "EnemyOccupied")
        {
            occupyingSpace = null;
        }
    }

    void TurnStart()
    {
        turnInitiated = true;
        spacePropScriptRef = occupyingSpace.gameObject.GetComponent<SpaceProperties>();
        spacePropScriptRef.SpaceSelected();
        contextScriptRef = GameObject.Find("ContextMenus(Clone)").gameObject.GetComponent<ContextBehavior>();
        contextScriptRef.MoveButtonPress();
        StartCoroutine(DetermineDelay());
    }

    IEnumerator DetermineDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        DetermineMove();
    }

    void DetermineMove()
    {
        List<GameObject> spacesPathed = new List<GameObject>();
        playerTransform = GameObject.Find("PlayerUnit(Clone)").transform;
        spaceTransform = spacePropScriptRef.gameObject.transform;
        Vector3 dir = (playerTransform.position - spaceTransform.position).normalized;
        float dist = Vector3.Distance(spaceTransform.position, playerTransform.position);
        RaycastHit[] hits;
        int layerMask = 1 << 9;
        hits = Physics.RaycastAll(spaceTransform.position, dir, dist, layerMask, QueryTriggerInteraction.Collide);
        Debug.DrawLine(spaceTransform.position, playerTransform.position, Color.white, 1f);
        foreach (RaycastHit hit in hits)
        {
            spacesPathed.Add(hit.transform.gameObject);
        }
        
        GameObject spacePicked = null;
        float closestDist = 0f / 0f;
        float farthestDist = 0f / 0f;
        List<GameObject> adjacentSpaces = null;
        
        foreach (GameObject space in spacesPathed)
        {
            if (space.tag == "AdjacentSpace")
            {
                adjacentSpaces.Add(space);
            }
        }

        if (adjacentSpaces != null)
        {
            foreach (GameObject space in adjacentSpaces)
            {
                if (spacePicked == null)
                {
                    spacePicked = space;
                    closestDist = Vector3.Distance(spaceTransform.position, space.transform.position);
                }
                else
                {
                    float distCalc = Vector3.Distance(spaceTransform.position, space.transform.position);
                    if (float.IsNaN(closestDist))
                    {
                        closestDist = distCalc;
                    }
                    if (closestDist > distCalc)
                    {
                        closestDist = distCalc;
                        spacePicked = space;
                    }
                }
            }
        }

        if (adjacentSpaces == null)
        {
            foreach (GameObject space in spacesPathed)
            {
                if (spacePicked == null)
                {
                    spacePicked = space;
                    farthestDist = Vector3.Distance(spaceTransform.position, space.transform.position);
                }
                else
                {
                    float distCalc = Vector3.Distance(spaceTransform.position, space.transform.position);
                    if (float.IsNaN(farthestDist))
                    {
                        farthestDist = distCalc;
                    }
                    if (farthestDist < distCalc)
                    {
                        farthestDist = distCalc;
                        spacePicked = space;
                    }
                }    
            }
        }

        if (spacePicked == null)
        {
            Debug.LogError("Oh boy, EnemyController didn't pick a space to move to...");
        }
        if (spacePicked != null)
        {
            pickedSpace = spacePicked;
            spacePicked.gameObject.transform.parent.gameObject.GetComponent<SpaceProperties>().SpaceSelected();
            StartCoroutine(EndDelay());
        }
        else
        {
            Debug.LogError("Wow, EnemyController REALLY did something wrong inside DetermineMove.");
        }
    }

    IEnumerator EndDelay()
    {
        yield return new WaitForSecondsRealtime(1);
        EndTurn();
    }

    void EndTurn()
    {
        confirmScriptRef = pickedSpace.transform.parent.gameObject.GetComponent<SpaceProperties>().confirmInstance.gameObject.GetComponent<ConfirmBehavior>();
        confirmScriptRef.ConfirmPress();
        Debug.Log("Confirm Has Been Pressed!!");
    }
}
