using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject bridge;
    public GameObject raft;
    public float delayBetweenBridges;
    public int decreaseWoodPerBridge;

    private int spawnedBridges = 0;
    private CharacterController characterController;

    private Animator playerAnimator;

    private void Start()
    {
        InitStats();
        InitConnections();
        StartCoroutine(SpawnBridges());
    }

    private void InitConnections()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void InitStats()
    {
        delayBetweenBridges = GameManager.Instance.delayBetweenBridges;
        decreaseWoodPerBridge = GameManager.Instance.decreaseWoodPerBridge;
    }

    private IEnumerator SpawnBridges()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position - new Vector3(0, 0.8f, 0), Vector3.down, out hit) && !raft.activeSelf)
            {
                if ((hit.collider.CompareTag("Fog") || hit.collider.CompareTag("FinishWater")) && !GameManager.Instance._gameStopped)
                {
                    characterController.cutting = false;
                    characterController.buildingBridge = true;
                    playerAnimator.SetTrigger("BridgeStart");

                    Instantiate(bridge, transform.position - new Vector3(0, 1, 0), transform.rotation);

                    spawnedBridges++;

                    if(spawnedBridges % decreaseWoodPerBridge == 0)
                    {
                        GameManager.Instance.DisableWoods(1);

                        if (GameManager.Instance.carriedWoodCount == 0)
                        {
                            characterController.buildingBridge = false;

                            if (GameManager.Instance.levelEndReached)
                            {
                                GameManager.Instance.EndLevel();
                            }
                            else
                            {
                                GameManager.Instance.OnFail();
                            }
                        }
                    }

                    print("hit collider name : " + hit.collider.name);
                }
                else
                {
                    if (characterController.buildingBridge)
                    {
                        print("not building a bridge" + "hitting" + hit.collider.name);

                        characterController.buildingBridge = false;

                        playerAnimator.SetTrigger("BridgeFinished");
                    }
                    
                }
            }

            yield return new WaitForSeconds(delayBetweenBridges);
        }
    }

    //void FixedUpdate()
    //{
        

    //    Debug.DrawRay(transform.position - new Vector3(0, 0.8f, 0), Vector3.down * 1000);
            
    //}
}
