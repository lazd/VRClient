﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerAttributes : NetworkBehaviour {

  public const int maxHealth = 100;

  [SyncVar]
  public int currentHealth = maxHealth;

  public RectTransform healthBar;

  public void TakeDamage(int amount)
  {
    if (!isServer)
    {
      return;
    }
    
    currentHealth -= amount;
    if (currentHealth <= 0)
    {
      currentHealth = 0;
      Debug.Log("Dead!");
    }

    healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
  }
}
