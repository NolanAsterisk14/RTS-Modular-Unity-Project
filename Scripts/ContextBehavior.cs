using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextBehavior : MonoBehaviour
{
    [SerializeField]
    private Button moveButton;
    [SerializeField]
    private Button endButton;
    [SerializeField]
    public Button cancelButton;
    [SerializeField]
    public Button cancelButton2;
    [SerializeField]
    private Button attackButton;
    [SerializeField]
    public GameObject spaceRef;
    [SerializeField]
    private List<GameObject> spacesCasted = new List<GameObject>();
    [SerializeField]
    private List<GameObject> oldSpaces = new List<GameObject>();
    [SerializeField]
    private List<GameObject> spacesCastedFrom = new List<GameObject>();
    [SerializeField]
    public bool moveSelected;
    [SerializeField]
    public bool attackSelected;
    SpaceProperties spacePropScriptRef;
    [SerializeField]
    private int spacesCanMove;
    [SerializeField]
    private int spacesRemaining;
    public LayerMask yes;

    void Start()
    {
        //Assign reference for movement script here
        spacePropScriptRef = spaceRef.GetComponent<SpaceProperties>();
        //Assign reference for buttons here
        moveButton = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Button>();
        endButton = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Button>();
        cancelButton = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<Button>();
        cancelButton2 = this.gameObject.transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<Button>();
        attackButton = this.gameObject.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Button>();
        moveButton.onClick.AddListener(MoveButtonPress);
        cancelButton.onClick.AddListener(CancelButtonPress);
        cancelButton2.onClick.AddListener(CancelButtonPress);
        attackButton.onClick.AddListener(AttackButtonPress);
        spacesCanMove = spacePropScriptRef.occupyingUnit.transform.GetChild(0).gameObject.GetComponent<UnitProperties>().moveDist;
        spacesRemaining = spacesCanMove;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 back = transform.TransformDirection(Vector3.back);
        Vector3 left = transform.TransformDirection(Vector3.left);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        if (moveSelected == true)
        {
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
                        Debug.LogError("ContextBehavior's case statement got messed up. Check code.");
                        break;
                }

                if (Physics.Raycast(spaceRef.transform.position, rayDirection, out hit, 8f, layerMask, QueryTriggerInteraction.Collide))
                {
                    Debug.DrawRay(spaceRef.transform.position, rayDirection * hit.distance, Color.red, 2f);
                    Debug.Log("Did Hit");
                    if (spacesCasted.Contains(hit.transform.gameObject) == false)
                    {
                        spacesCasted.Add(hit.transform.gameObject);
                    }
                }
                else
                {
                    Debug.DrawRay(spaceRef.transform.position, rayDirection * 8, Color.white, 2f);
                    Debug.Log("Did not Hit");
                }
            }

            foreach (GameObject space in spacesCasted)
            {
                if (oldSpaces.Contains(space) == false)
                {
                    space.transform.parent.gameObject.GetComponent<SpaceProperties>().canMoveTo = true;
                    ColorBlock cb = space.transform.parent.gameObject.GetComponent<Button>().colors;
                    Color moveColor = Color.blue;
                    cb.normalColor = moveColor;
                    space.transform.parent.gameObject.GetComponent<Button>().colors = cb;
                }
            }

            DecrementCheck:
            spacesRemaining--;

            if (spacesRemaining > 0)
            {
                foreach (GameObject spaceCasted in spacesCasted) //I made this confusing, but this will be the list of objects that will raycast next.
                {
                    oldSpaces.Add(spaceCasted);
                }
                foreach (GameObject spaceCastedFrom in spacesCastedFrom)
                {
                    oldSpaces.Remove(spaceCastedFrom);
                }

                foreach (GameObject spaceOld in oldSpaces)
                {
                    if (spacesCastedFrom.Contains(spaceOld) == false)
                    {
                        spacesCastedFrom.Add(spaceOld);
                    }
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
                                Debug.LogError("ContextBehavior's case statement got messed up. Check code.");
                                break;
                        }
                        if (Physics.Raycast(spaceOld.transform.position, rayDirection, out hit, 8f, layerMask, QueryTriggerInteraction.Collide))
                        {
                            Debug.DrawRay(spaceOld.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 2f);
                            Debug.Log("Did Hit");

                            if (spacesCasted.Contains(hit.transform.gameObject) == false && hit.transform.gameObject != spaceRef.transform.GetChild(1).gameObject)
                            {
                                spacesCasted.Add(hit.transform.gameObject);
                            }

                        }
                        else
                        {
                            Debug.DrawRay(spaceRef.transform.position, rayDirection * 8, Color.white, 2f);
                            Debug.Log("Did not Hit");
                        }
                    }
                }
                foreach (GameObject space in spacesCasted)
                {
                    if (oldSpaces.Contains(space) == false)
                    {
                        space.transform.parent.gameObject.GetComponent<SpaceProperties>().canMoveTo = true;
                        ColorBlock cb = space.transform.parent.gameObject.GetComponent<Button>().colors;
                        Color moveColor = Color.blue;
                        cb.normalColor = moveColor;
                        space.transform.parent.gameObject.GetComponent<Button>().colors = cb;
                    }
                }
                goto DecrementCheck;
            }
            moveSelected = false;

        }
    }

    void Update()
    {

    }

    public void MoveButtonPress()
    {
        moveSelected = true;
    }

    public void AttackButtonPress()
    {
        attackSelected = true;
    }

    public void CancelButtonPress()
    {
        spacePropScriptRef.menuOpen = false;
        foreach (GameObject space in spacesCasted)
        {
            space.transform.parent.gameObject.GetComponent<SpaceProperties>().canMoveTo = false;
            ColorBlock cb = space.transform.parent.gameObject.GetComponent<Button>().colors;
            Color moveColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            cb.normalColor = moveColor;
            space.transform.parent.gameObject.GetComponent<Button>().colors = cb;
        }
        Destroy(this.gameObject);

    }

}
