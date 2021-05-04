using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("rock collision object name:  " + other.name);

        if (other.name == "CharacterParent" || other.CompareTag("Raft"))
        {
            GameManager.Instance.OnFail();
            
        }
    }


}
