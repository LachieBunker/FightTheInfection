using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class A_NormalShot : BaseEnemyAbilityClass {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CheckTriggerCondition(GameObject _player, int dirFacing)
    {
        Vector3 pos = new Vector3(_player.transform.position.x + (1 * dirFacing), _player.transform.position.y, _player.transform.position.z);
        Vector3 dir = Vector3.right * dirFacing;
        RaycastHit hit;
        if(Physics.Raycast(pos, dir, out hit, 14))
        {
            //Debug.Log("Hit: " + hit.transform.gameObject);
            if(hit.transform.tag == "Player")
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
        GameObject bullet = (GameObject)Instantiate(_bullet, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.Euler(0, -90, 0));
    }
}
