using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBehavior : MonoBehaviour
{
    [SerializeField]
    public Button confirmButton;
    [SerializeField]
    public Button cancelButton;
    [SerializeField]
    public bool confirmPressed;
    [SerializeField]
    public GameObject spaceRef;
    [SerializeField]
    public GameObject originalSpaceRef;
    ContextBehavior contextBehaviorRef;

    void Start()
    {
        confirmButton = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Button>();
        cancelButton = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Button>();
        confirmButton.onClick.AddListener(ConfirmPress);
        cancelButton.onClick.AddListener(CancelPress);
    }

    void Update()
    {

    }

    public void ConfirmPress()
    {
        confirmPressed = true;
        Destroy(this.gameObject, 1);
    }

    void CancelPress()
    { 
        if (originalSpaceRef != null)
        {
            if (originalSpaceRef.GetComponent<SpaceProperties>().menuOpen == true)
            {
                contextBehaviorRef = GameObject.Find("ContextMenus(Clone)").gameObject.GetComponent<ContextBehavior>();
                contextBehaviorRef.CancelButtonPress();
                Destroy(this.gameObject);
            }
            if (originalSpaceRef.GetComponent<SpaceProperties>().menuOpen == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
