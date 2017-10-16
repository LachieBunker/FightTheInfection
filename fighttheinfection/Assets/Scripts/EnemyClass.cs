using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : BaseCharacterClass {

    public bool usingAbility;
    public List<BaseEnemyAbilityClass> abilities;
    public float xDeSpawnBound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Time.time%0.5f == 0)
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

        if(transform.position.x < xDeSpawnBound)
        {
            EnemyEscaped();
        }
    }

    public void SetEnemyAbilities(List<BaseEnemyAbilityClass> _abilities)
    {
        abilities = _abilities;
    }

    public void EnemyEscaped()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EnemyRemoved("Escaped");
        Destroy(gameObject);
    }

    public override void CharacterDead()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EnemyRemoved("Dead");
        Destroy(gameObject);
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
