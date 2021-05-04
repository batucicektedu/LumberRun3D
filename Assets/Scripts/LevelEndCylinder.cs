using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndCylinder : MonoBehaviour
{
    public bool isJACKPOT;
    public int multiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isJACKPOT)
            {
                GameManager.Instance.lastTouchedLevelEndIndex = multiplier - 1;

                GameManager.Instance.EndLevel();
            }
            else
            {
                GameManager.Instance.lastTouchedLevelEndIndex = multiplier - 1;
            }
        }
    }



}
