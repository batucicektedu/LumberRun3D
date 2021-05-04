using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using UnityEngine.UIElements;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public interface CharacterControllerUiListener
{
    void OnComplete();
    void OnFail();

    void OnCoin();
}

public class CharacterController : MonoBehaviour
{
    //Settings
    public float forwardSpeed;
    public float sidewaysSpeed;
    public float maxOffset;

    public float rotationResetTweenDuration;
    public float rotateTweenDuration;
    public float rotationMultiplier;

    public float constantFallDownSpeed;

    private int layerMaskTrack;

    //References
    public GameObject[] axes;
    public GameObject raft;

    CharacterControllerUiListener listener;
    private Transform thisTransform;
    private Transform playerTransform;
    private Animator playerAnimator;


    //Gameplay Variables
    private Tween rotationTween;
    private bool moveDownTriggered;
    public bool buildingLadder;
    public bool buildingBridge;

    private bool obstacleOnLeft;
    private bool obstacleOnRight;

    private float runAndCutElapsedTime = 0;
    public bool cutting;
    public bool trackUnderneath;

    private void InitConnections()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        thisTransform = transform;
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void InitState()
    {
        forwardSpeed = GameManager.Instance.forwardSpeed;
        sidewaysSpeed = GameManager.Instance.sidewaysSpeed;
        maxOffset = GameManager.Instance.maxOffset;

        rotationResetTweenDuration = GameManager.Instance.rotationResetTweenDuration;
        rotateTweenDuration = GameManager.Instance.rotateTweenDuration;
        rotationMultiplier = GameManager.Instance.rotationMultiplier;

        constantFallDownSpeed = GameManager.Instance.constantFallDownSpeed;

        layerMaskTrack = 1 << 8;
    }

    private void Start()
    {
        InitConnections();
        InitState();

        StartCoroutine(MoveDownUntilGround());
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            rotationTween.Kill();
            rotationTween = transform.DORotate(Vector3.zero, rotationResetTweenDuration);
        }


        if (cutting && runAndCutElapsedTime < 1)
        {
            runAndCutElapsedTime += Time.deltaTime;
        }
        else if(cutting && runAndCutElapsedTime > 1)
        {
            cutting = false;

            print("*********** Cut finished triggered ************");

            //Trigger cut end
            playerAnimator.SetTrigger("CutFinished");

        }

        if (GameManager.Instance.startMovingDownAfterFail && !trackUnderneath)
        {
            GetComponentInParent<Transform>().position -= new Vector3(0, Time.deltaTime * 2f, 0);
        }

        CheckIfObstacleOnLeft();
        CheckIfObstacleOnRight();
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance._gameStopped)
        {
            MoveForward();
        }
    }

    private void MoveForward()
    {
        transform.position += new Vector3(0, 0, forwardSpeed * Time.deltaTime);
    }


    public void FollowRoad(Vector2 offset)
    {
        if (!GameManager.Instance._gameStopped)
        {
            Vector2 offsetDifference = offset * sidewaysSpeed * Time.fixedDeltaTime;
            offsetDifference.y = 0;

            Vector2 currentOffset = transform.position;
            

            if (offset.x > 0 && obstacleOnRight)
            {
                offsetDifference.x = 0;
            }
            else if(offset.x < 0 && obstacleOnLeft)
            {
                offsetDifference.x = 0;
            }
            else
            {
                RotateTowardsMoveDirection(offset.x);
            }

            currentOffset += offsetDifference;

            if (currentOffset.x > maxOffset)
            {
                currentOffset.x = maxOffset;
            }
            else if (currentOffset.x < -maxOffset)
            {
                currentOffset.x = -maxOffset;
            }

            transform.position = new Vector3(currentOffset.x, transform.position.y, transform.position.z);

        }
    }

    public void RotateTowardsMoveDirection(float offset)
    {
        offset = Mathf.Clamp(offset, -rotationMultiplier * 2f, rotationMultiplier * 2f);

        //Debug.Log("offset : " + offset);

        rotationTween.Kill();
        rotationTween = transform.DORotate(new Vector3(0, offset * rotationMultiplier, 0), rotateTweenDuration);
    }

    public void SetListener(CharacterControllerUiListener listener)
    {
        this.listener = listener;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("HillEnd") && !moveDownTriggered)
    //    {
    //        Debug.Log("Hello");
    //        moveDownTriggered = true;

    //        StartCoroutine(MoveDownUntilGround());

            
    //    }
    //}
    private IEnumerator MoveDownUntilGround()
    {
        while (true)
        {

            RaycastHit hit;
            if(Physics.Raycast(playerTransform.position, Vector3.down, out hit, Mathf.Infinity, layerMaskTrack) && !buildingLadder && !buildingBridge)
            {
                if(hit.distance > 0.25f)
                {
                    print("hit distance :  " + hit.distance + " -- Name : " + hit.collider.name);

                    Debug.DrawRay(playerTransform.position, Vector3.down, Color.blue);

                    playerTransform.position -= new Vector3(0, Time.deltaTime * constantFallDownSpeed, 0);

                }

                trackUnderneath = true;

                //else if(hit.distance < 0.5f)
                //{
                //    print("else if");
                //    moveDownTriggered = false;
                //    yield break;
                //}
                //else
                //{
                //    print("else");
                //    moveDownTriggered = false;
                //    yield break;
                //}
            }
            else
            {
                trackUnderneath = false;
            }

            yield return null;
        }
    }

    private void CheckIfObstacleOnLeft()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerTransform.position + new Vector3(0, 0.2f, 0), Vector3.left, out hit, 1.25f, layerMaskTrack))
        {
            
            if (hit.collider.CompareTag("Track"))
            {
                print("Left - Right raycast distance : " + hit.distance + " direction : left" + "*****HİT*****");

                obstacleOnLeft = true;
            }
            else
            {
                obstacleOnLeft = false;
            }
        }
        else
        {
            obstacleOnLeft = false;
        }

        
        Debug.DrawRay(playerTransform.position + new Vector3(0, 0.2f, 0), Vector3.left * 1.25f, Color.yellow);
    }

    private void CheckIfObstacleOnRight()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerTransform.position + new Vector3(0, 0.2f, 0), Vector3.right, out hit, 1.25f, layerMaskTrack))
        {

            if (hit.collider.CompareTag("Track"))
            {
                print("Left - Right raycast distance : " + hit.distance + " direction : right" + "*****HİT*****");

                obstacleOnRight = true;
            }
            else
            {
                obstacleOnRight = false;
            }
        }
        else
        {
            obstacleOnRight = false;
        }

        Debug.DrawRay(playerTransform.position + new Vector3(0, 0.2f, 0), Vector3.right * 1.25f, Color.yellow);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            Debug.Log("cutting = " + cutting + "   elapsed time : " + runAndCutElapsedTime);

            if (!cutting && !buildingBridge && !buildingLadder && !raft.activeSelf)
            {
                cutting = true;
                runAndCutElapsedTime = 0;

                //Play cutting anim
                playerAnimator.SetTrigger("CutStart");
            }
            else
            {
                runAndCutElapsedTime = 0;
            }
            
        }
    }


}
