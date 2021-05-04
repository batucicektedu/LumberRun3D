using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowingCollectable : MonoBehaviour
{
    public float collectableSpeed;
    public float followStartDelay;

    private GameObject player;

    private float elapsedFollowingTime;

    private void Start()
    {
        InitConnections();
        InitStats();

        StartCoroutine(StartFollowingPlayer());
    }

    private void InitStats()
    {
        collectableSpeed = GameManager.Instance.collectableSpeed;
        followStartDelay = GameManager.Instance.followStartDelay;

    }

    private void InitConnections()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator StartFollowingPlayer()
    {
        yield return new WaitForSeconds(followStartDelay);

        while (true)
        {
            if(elapsedFollowingTime > 2f)
            {
                Destroy(gameObject);
            }

            elapsedFollowingTime += Time.deltaTime;

            gameObject.GetComponent<MeshCollider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;

            Vector3 differenceVector = ((player.transform.position + new Vector3(0, 0.5f ,0)) - transform.position);

            transform.position += differenceVector * Time.deltaTime * collectableSpeed / differenceVector.magnitude;

            yield return new WaitForEndOfFrame();
        }
    }
}
