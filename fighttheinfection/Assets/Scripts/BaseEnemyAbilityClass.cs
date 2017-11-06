using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseEnemyAbilityClass : MonoBehaviour {

    public bool available = true;
    public float coolDownDuration;
    public List<AbilityTypes> abilityTypes;

    public abstract bool CheckTriggerCondition(GameObject _player, int dirFacing);

    public abstract void Activate(GameObject _player, GameObject _bullet);

    public BaseEnemyAbilityClass GetAbilityClass()
    {
        return this;
    }

    public IEnumerator DelayRenabling()
    {
        yield return new WaitForSeconds(coolDownDuration);
        Enable();
    }

    public void Disable()
    {
        available = false;
    }

    public void Enable()
    {
        available = true;
    }
}

public enum AbilityTypes { Attack, Movement }
