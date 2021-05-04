using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<CharacterController>().maxOffset *= 4;

            GameManager.Instance.levelEndReached = true;
        }
    }
}
