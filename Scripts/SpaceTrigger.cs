using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceTrigger : MonoBehaviour
{
    SpaceProperties spacePropScriptRef;
    TurnManager turnManagerRef;
    public GameObject triggerUnit;

    void Start()
    {
        spacePropScriptRef = this.gameObject.transform.parent.GetComponent<SpaceProperties>();
        turnManagerRef = GameObject.Find("PlaceableGrid").transform.Find("TurnManager").GetComponent<TurnManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (spacePropScriptRef != null)
        {
            spacePropScriptRef.occupied = true;

            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Player")
            {
                triggerUnit = other.transform.parent.parent.gameObject;
                if (turnManagerRef.playerTurn == true)
                {
                    spacePropScriptRef.playerSelectable = true;
                }
                spacePropScriptRef.enemySelectable = false;
                spacePropScriptRef.AdjacencyUpdate();
            }
            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Opponent")
            {
                triggerUnit = other.transform.parent.parent.gameObject;
                spacePropScriptRef.playerSelectable = false;
                if (turnManagerRef.playerTurn == true)
                {
                    spacePropScriptRef.enemySelectable = true;
                }
                spacePropScriptRef.AdjacencyUpdate();
            }
        }      
    }

    void OnTriggerStay(Collider other)
    {
        if (spacePropScriptRef != null)
        {
            spacePropScriptRef.occupied = true;

            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Player")
            {
                triggerUnit = other.transform.parent.parent.gameObject;
                if (turnManagerRef.playerTurn == true)
                {
                    spacePropScriptRef.playerSelectable = true;
                }
                spacePropScriptRef.enemySelectable = false;
                spacePropScriptRef.AdjacencyUpdate();
            }
            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Opponent")
            {
                triggerUnit = other.transform.parent.parent.gameObject;
                spacePropScriptRef.playerSelectable = false;
                if (turnManagerRef.playerTurn == true)
                {
                    spacePropScriptRef.enemySelectable = true;
                }
                spacePropScriptRef.AdjacencyUpdate();
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        triggerUnit = null;
        if (spacePropScriptRef != null)
        {
            spacePropScriptRef.occupied = false;

            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Player")
            {
                spacePropScriptRef.playerSelectable = false;
                spacePropScriptRef.enemySelectable = false;
            }
            if (other.tag == "UnitTrigger" && other.transform.parent.tag == "Opponent")
            {
                spacePropScriptRef.playerSelectable = false;
                spacePropScriptRef.enemySelectable = false;
            }
            spacePropScriptRef.AdjacencyUpdate();
        }
        
    }
}