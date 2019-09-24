﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    #region healthpack_variables
    [SerializeField]
    [Tooltip("Assign the healing value of the healthpack")]
    private int healAmount;
    #endregion

    #region functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().Heal(healAmount);
            Destroy(this.gameObject);
        }
    }
    #endregion
}