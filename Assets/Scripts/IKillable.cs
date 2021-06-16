using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    /// <summary>
    /// What to execute when this game object receives damage.
    /// </summary>
    public void TakeDamage();
}

public interface IAttacker
{
    public void CheckIfDamage();

    public void ApplyDamage(GameObject obj);
}
