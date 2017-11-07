using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float moveSpeed;
    public float rotSpeed;
    public int damage;
    public GameObject bulletObj;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 5);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * moveSpeed);
        if(bulletObj != null)
        {
            bulletObj.transform.Rotate(0, rotSpeed, 0, Space.Self);
        }
	}

    public void DelayStart(float delay)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject + " hit " + other);
        if(other.tag == "Player" && gameObject.tag == "EnemyBullet")
        {
            other.GetComponent<PlayerController>().CharacterHit(damage);
            Destroy(gameObject);
        }
        if(other.tag == "Enemy" && gameObject.tag == "PlayerBullet")
        {
            other.GetComponent<EnemyClass>().CharacterHit(damage);
            Destroy(gameObject);
        }
        if(other.tag == "EnemyBullet" && gameObject.tag == "PlayerBullet")
        {
            Destroy(other);
            Destroy(gameObject);
        }
        if(other.tag == "PlayerBullet" && gameObject.tag == "EnemyBullet")
        {
            Destroy(other);
            Destroy(gameObject);
        }
    }
}
