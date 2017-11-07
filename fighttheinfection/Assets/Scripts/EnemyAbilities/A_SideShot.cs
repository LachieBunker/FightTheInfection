using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_SideShot : BaseEnemyAbilityClass {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CheckTriggerCondition(GameObject _player, int dirFacing)
    {
        //Vector3 pos = new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z + 1);
        Vector3 dir = Vector3.left;
        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, Vector3.forward, out hit))
        {
            //Debug.Log("Hit: " + hit);
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        else if(Physics.Raycast(_player.transform.position, Vector3.back, out hit))
        {
            //Debug.Log("Hit: " + hit);
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        else
        {
            //Debug.Log("Didn't hit anything");
            return false;
        }
        return false;

    }

    public override void Activate(GameObject _player, GameObject _bullet)
    {
        Instantiate(_bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), Quaternion.Euler(0, 0, 0));
        Instantiate(_bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.Euler(0, 180, 0));
    }
}
