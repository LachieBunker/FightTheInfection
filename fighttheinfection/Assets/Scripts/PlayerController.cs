using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterClass {

    public bool invul;
    public float invulDuration;
    public Material mat;
    public Color normalColor;
    public Color invulColor;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, moveSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, 0, -moveSpeed);
        }
        if (Input.GetKey(KeyCode.Space) && attackCoolDownTimer < Time.time)
        {
            attackCoolDownTimer = Time.time + attackCoolDownDuration;
            Attack();
        }

        if (transform.position.x < -characterBounds.x)
        {
            transform.position = new Vector3(-characterBounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x > characterBounds.x)
        {
            transform.position = new Vector3(characterBounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.z < -characterBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -characterBounds.z);
        }
        if (transform.position.z > characterBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, characterBounds.z);
        }

    }

    public void Attack()
    {
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
        bullet.GetComponent<BulletScript>().damage = damage;
    }

    public IEnumerator SetInvul()
    {
        float timer = Time.time + invulDuration;
        invul = true;
        while(timer > Time.time)
        {
            yield return new WaitForSeconds(0.25f);
            mat.color = invulColor;
            yield return new WaitForSeconds(0.25f);
            mat.color = normalColor;
        }
        invul = false;
    }

    public override void CharacterDead()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlayerDead(gameObject);
    }
}
