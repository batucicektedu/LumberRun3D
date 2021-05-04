using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject levelCompletedUI;
    public GameObject levelFailedUI;
    public GameObject levelStartUI;
    public GameObject HUD;


    public void ActivateLevelCompleteUI()
    {
        levelCompletedUI.SetActive(true);
    }

    public void ActivateLevelFailedUI()
    {
        levelFailedUI.SetActive(true);
    }

    public void ActivateLevelStartUI()
    {
        levelStartUI.SetActive(true);
    }

    public void ActivateHUD()
    {
        HUD.SetActive(true);
    }





    public void DisableLevelCompleteUI()
    {
        levelCompletedUI.SetActive(false);
    }

    public void DisableLevelFailedUI()
    {
        levelFailedUI.SetActive(false);
    }

    public void DisableLevelStartUI()
    {
        levelStartUI.SetActive(false);
    }

    public void DisableHUD()
    {
        HUD.SetActive(false);
    }
}
