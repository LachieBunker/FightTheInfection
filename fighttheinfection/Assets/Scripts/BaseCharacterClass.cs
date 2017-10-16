using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacterClass : MonoBehaviour {

    public float moveSpeed;
    public int direction;
    public int currentHealth;
    public int maxHealth;
    public float attackCoolDownTimer;
    public float attackCoolDownDuration;
    public int damage;
    public GameObject bulletPrefab;
    public Vector3 characterBounds;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void CharacterHit(int _damage)
    {
        currentHealth -= _damage;
        if(currentHealth <= 0)
        {
            CharacterDead();
        }
    }

    public virtual void CharacterDead()
    {

    }
}
