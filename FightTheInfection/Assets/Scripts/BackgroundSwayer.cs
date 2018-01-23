using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwayer : MonoBehaviour {

    private Vector3 startPos;
    private float startX;

    public string state;

    public float dir, moveMax, rotMax, moveSpeed, rotSpeed, weighting, startBuffer;
    
	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
        startX = transform.localPosition.x;
        dir = 1;
        transform.Rotate(0, 0, rotMax, Space.Self);
        transform.position = new Vector3(startPos.x, startPos.y + startBuffer, startPos.z);
        state = "Move";
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(state == "Move")
        {
            transform.Translate(0, (moveSpeed * dir * weighting), 0, Space.Self);
            if (transform.position.y > startPos.y + moveMax)
            {
                dir *= -1;
                state = "Rotate";
            }
            else if (transform.position.y < startPos.y - moveMax)
            {
                dir *= -1;
                state = "Rotate";
            }
        }
        else if(state == "Rotate")
        {
            transform.Rotate(0, 0, (rotSpeed * dir * weighting), Space.Self);
            if (transform.localEulerAngles.z > rotMax && transform.localEulerAngles.z < rotMax *2 && transform.position.y < startPos.y)
            {
                state = "Move";
                transform.localPosition = new Vector3(startX, transform.localPosition.y, transform.localPosition.z);
            }
            else if(transform.localEulerAngles.z < 360-rotMax && transform.localEulerAngles.z > 360-(rotMax*2))
            {
                state = "Move";
                transform.localPosition = new Vector3(startX, transform.localPosition.y, transform.localPosition.z);
            }
        }
        
	}
}
