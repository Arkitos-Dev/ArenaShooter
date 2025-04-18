using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public float healthValue = 50;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponentInParent<PlayerStats>();
        if (stats != null)
        {
            stats.AddHealth(healthValue);
            Destroy(gameObject);
        }
    }
}
