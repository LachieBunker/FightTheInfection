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
        mat = gameObject.GetComponent<MeshRenderer>().material;
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
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 0.75f), Quaternion.Euler(0, 90, 0));
        bullet.GetComponent<BulletScript>().damage = damage;
        bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 0.75f), Quaternion.Euler(0, 90, 0));
        bullet.GetComponent<BulletScript>().damage = damage;
    }

    public void StartInvul()
    {
        StartCoroutine(SetInvul());
    }

    private IEnumerator SetInvul()
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

    public override void CharacterHit(int _damage)
    {
        if(!invul)
        {
            currentHealth -= _damage;
            if (currentHealth <= 0)
            {
                CharacterDead();
            }
        }
    }

    public override void CharacterDead()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlayerDead(gameObject);
    }
}
