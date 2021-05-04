using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour
{
    private Transform playerTransform;

    private Vector3 startOffset;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        startOffset = transform.position - playerTransform.transform.position;
    }
    private void LateUpdate()
    {
        transform.position = playerTransform.position + startOffset;
    }
}
