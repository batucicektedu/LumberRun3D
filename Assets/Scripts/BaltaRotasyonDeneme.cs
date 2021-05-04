using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaltaRotasyonDeneme : MonoBehaviour
{
    void FixedUpdate()
    {
        if(transform.rotation.eulerAngles.y < 179f && transform.rotation.eulerAngles.y > 0)
        {
            print("if");
            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.y--;
            transform.rotation = Quaternion.Euler(rotationVector);
        }
        else
        {
            print("else");
            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.y++;
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }
}
