using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Süpürge : MonoBehaviour
{
    public bool destroyEverything;

    private void OnTriggerEnter(Collider other)
    {
        if (destroyEverything)
        {
            if (other.CompareTag("Log") && other.CompareTag("LeafFragment") && other.CompareTag("Tree") && other.CompareTag("Rock"))
            {
                Destroy(other.gameObject);
            }
        }
        else
        {
            if (other.CompareTag("LeafFragment"))
            {
                Destroy(other.gameObject);
            }
        }
        
    }
}
