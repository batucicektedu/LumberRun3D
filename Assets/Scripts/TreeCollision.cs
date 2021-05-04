using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCollision : MonoBehaviour
{
    public GameObject fragmentedTree;
    public bool applyForce;
    public float appliedForce;
    public float woodIncrementPerTree;

    private void Start()
    {
        InitStats();
    }
    private void InitStats()
    {
        applyForce = GameManager.Instance.applyForce;
        appliedForce = GameManager.Instance.appliedForce;
        woodIncrementPerTree = GameManager.Instance.woodIncrementPerTree;
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            GameObject g = Instantiate(fragmentedTree, transform.position, transform.rotation);

            if (applyForce)
            {
                foreach (Rigidbody item in g.GetComponentsInChildren<Rigidbody>())
                {
                    item.AddExplosionForce(appliedForce, other.GetComponentInParent<Transform>().position, 50);
                }
            }

            //Destroy(g, 1.5f);

            GameManager.Instance.IncreaseCarriedWoodCount(woodIncrementPerTree);

            gameObject.SetActive(false);
        }
    }
}
