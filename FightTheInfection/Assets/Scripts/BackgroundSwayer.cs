using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwayer : MonoBehaviour {

    private Vector3 startPos;

    public string state;

    public float dir, moveMax, rotMax, moveSpeed, rotSpeed;

	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
        dir = 1;
        transform.Rotate(0, 0, rotMax, Space.Self);
        state = "Move";
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(state == "Move")
        {
            transform.Translate(0, moveSpeed * dir, 0);
            if (transform.position.y > startPos.y + moveMax)
            {
                dir *= -1;
                state = "Rotate";
                Debug.Log("Top");
            }
            else if (transform.position.y < startPos.y - moveMax)
            {
                dir *= -1;
                state = "Rotate";
                Debug.Log("Bottom");
            }
        }
        else if(state == "Rotate")
        {
            transform.Rotate(0, 0, rotSpeed * dir, Space.Self);
            if (transform.localEulerAngles.z > rotMax && transform.localEulerAngles.z < 180)
            {
                Debug.Log("Moving" + transform.localEulerAngles.z);
                state = "Move";
            }
            else if(transform.localEulerAngles.z < 360-rotMax && transform.localEulerAngles.z > 180)
            {
                state = "Move";
                Debug.Log(transform.localRotation.eulerAngles.z);
            }
        }
        
	}
}
