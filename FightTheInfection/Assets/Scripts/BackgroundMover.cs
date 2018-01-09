using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour {

    public float moveSpeed;
    public float despawnX;
    public Vector3 respawnPos;
    public bool respawn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.Translate(moveSpeed, 0, 0);

        if(transform.position.x <= despawnX)
        {
            if (respawn)
            {
                transform.position = respawnPos;
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
