using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    private Button resumeButton; //Store values for menu buttons
    private Button endTurnButton; 
    private Button restartButton;
    private GameObject menuCanvas; //Store value for canvas
    private bool isMenuOpen = false; //Check bool for menu being active

    void Start()
    {
        resumeButton = this.gameObject.transform.GetChild(0).transform.Find("Resume").GetComponent<Button>(); //assign references to buttons
        endTurnButton = this.gameObject.transform.GetChild(0).transform.Find("End Turn").GetComponent<Button>(); 
        restartButton = this.gameObject.transform.GetChild(0).transform.Find("Restart").GetComponent<Button>();
        menuCanvas = this.gameObject.transform.GetChild(0).gameObject;
        if (menuCanvas.activeSelf == true)
        {
            menuCanvas.SetActive(false);
        }
        resumeButton.onClick.AddListener(MenuOperation);
    }

    void Update()
    {
        if (Input.GetButtonDown("Tab"))
        {
            MenuOperation();
        }
    }

    void MenuOperation()
    {
        isMenuOpen = !isMenuOpen;
        if(isMenuOpen == false)
        {
            menuCanvas.SetActive(false);
        }
        if(isMenuOpen == true)
        {
            menuCanvas.SetActive(true);
        }
    }
}
