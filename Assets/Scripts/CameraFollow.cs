using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

	public Transform target;

	public float smoothTime = 0.1f;
	[SerializeField]
	public Vector3 offset;
	private Vector3 vel;
	
    private void Start()
    {
		InitOffset();
	}

    void LateUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, smoothTime);
	}

	void InitOffset()
    {
		offset = transform.position - target.position; 
    }

}
