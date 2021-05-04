using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LadderBuild : MonoBehaviour
{
    public int ladderCount;
    public Transform ladder;
    public float secondsBetweenLadderSpawns;
    public Vector3 ladderSpawnOffset;

    private int spawnedLadders = 0;
    public int decraseWoodPerLadder;

    private Transform characterParentTransform;
    private CharacterController characterController;
    private bool transformFected;

    private Animator playerAnimator;
    private bool ladderAborted;

    private void Start()
    {
        InitConnections();
        InitState();

    }

    private void InitConnections()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void InitState()
    {
        ladderAborted = false;
        decraseWoodPerLadder = GameManager.Instance.decraseWoodPerLadder;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!transformFected)
            {
                characterController = other.GetComponentInParent<CharacterController>();

                if (!characterController.buildingLadder)
                {
                    characterController.cutting = false;
                    characterController.buildingLadder = true;
                    playerAnimator.SetTrigger("LadderStart");

                    characterParentTransform = other.GetComponentInParent<Transform>();
                    transformFected = true;

                    StartCoroutine(BuildLadder());
                }
                
            }
        }
    }

    private IEnumerator BuildLadder()
    {
        GameManager.Instance._gameStopped = true;


        for (int i = 0; i < ladderCount; i++)
        {
            if (!ladderAborted)
            {
                Instantiate(ladder, characterParentTransform.position + ladderSpawnOffset, Quaternion.identity);

                characterParentTransform.DOMoveY(characterParentTransform.position.y + (ladderSpawnOffset.y), secondsBetweenLadderSpawns);

                spawnedLadders++;

                if (spawnedLadders % decraseWoodPerLadder == 0)
                {
                    GameManager.Instance.DisableWoods(1);

                    if (GameManager.Instance.carriedWoodCount == 0)
                    {
                        ladderAborted = true;
                        characterController.buildingLadder = false;

                        GameManager.Instance.OnFail();

                        yield break;
                    }
                }

                
                yield return new WaitForSeconds(secondsBetweenLadderSpawns); 
            }
        }

        
        GameManager.Instance._gameStopped = false;

        playerAnimator.SetTrigger("LadderFinished");

        //Wait before enabling fake gravity
        yield return new WaitForSeconds(0.5f);

        characterController.buildingLadder = false;
    }

}
