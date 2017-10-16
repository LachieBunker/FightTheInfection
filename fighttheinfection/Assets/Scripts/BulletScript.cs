using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float moveSpeed;
    public int damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * moveSpeed);
	}

    public void DelayStart(float delay)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
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
    }
}
