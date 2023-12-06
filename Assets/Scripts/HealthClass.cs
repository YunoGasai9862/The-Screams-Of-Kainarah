using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthClass
{
    private float _entityHealth;
    public HealthClass(float entityHealth)
    {
        _entityHealth = entityHealth;
    }
    public float EntityHealth { get => _entityHealth; set => _entityHealth = value; }

    public void HealEntity(float healAmount)
    {
        _entityHealth += healAmount;
    }
    public void DamageEntity(float damageAmount)
    {
        _entityHealth -= damageAmount;
    }
}
