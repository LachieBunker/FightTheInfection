﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : BaseCharacterClass {

    public bool usingAbility;
    public List<BaseEnemyAbilityClass> abilities;
    public float xDeSpawnBound;
    public float abilityTimer;
    public float abilityCoolDown;
    public bool invul;

	// Use this for initialization
	void Start () {
        SetEnemyAbilities();
        float tempSpeed = moveSpeed + (GameObject.FindWithTag("GameManager").GetComponent<GameManager>().levelNum * 0.005f);
        if(tempSpeed > 0.15f)
        {
            moveSpeed = 0.15f;
        }
        else
        {
            moveSpeed = tempSpeed;
        }
        //bulletPrefab.GetComponent<MeshRenderer>().material = gameObject.GetComponentsInChildren<MeshRenderer>()[1].material;
        invul = true;
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if(abilityTimer < Time.time && !invul)
        {
            for(int i = 0; i < abilities.Count; i++)
            {
                if (abilities[i].abilityTypes.Contains(AbilityTypes.Attack) && attackCoolDownTimer > Time.time)
                {

                }
                else
                {
                    if (abilities[i].available && abilities[i].CheckTriggerCondition(gameObject, direction))
                    {
                        if(abilities[i].abilityTypes.Contains(AbilityTypes.Attack))
                        {
                            attackCoolDownTimer = Time.time + attackCoolDownDuration;
                        }
                        abilities[i].Activate(gameObject, bulletPrefab);
                        abilities[i].Disable();
                        abilities[i].DelayRenabling();
                        abilityTimer = Time.time + abilityCoolDown;
                    }
                }
                
            }
        }
		if(!usingAbility)
        {
            transform.Translate(moveSpeed * direction, 0, 0);
        }

        if(transform.position.x < -characterBounds.x)
        {
            transform.position = new Vector3(-characterBounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.z < -characterBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -characterBounds.z);
        }
        if (transform.position.z > characterBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, characterBounds.z);
        }

        if(transform.position.x < xDeSpawnBound)
        {
            EnemyEscaped();
        }
    }

    public void SetEnemyAbilities(List<BaseEnemyAbilityClass> _abilities)
    {
        abilities = _abilities;
    }

    private void SetEnemyAbilities()
    {
        BaseEnemyAbilityClass[] _abilities = gameObject.GetComponents<BaseEnemyAbilityClass>();
        abilities = new List<BaseEnemyAbilityClass>();
        for(int i = 0; i < _abilities.Length; i++)
        {
            abilities.Add(_abilities[i]);
        }
    }

    public void EnemyEscaped()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EnemyRemoved("Escaped");
        Destroy(gameObject);
    }

    public override void CharacterHit(int _damage)
    {
        if (!invul)
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
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EnemyRemoved("Dead");
        Destroy(gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "SpawnZone")
        {
            invul = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            other.GetComponent<PlayerController>().CharacterHit(damage);
            CharacterHit(1);
        }
    }
}
