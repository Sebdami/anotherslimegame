﻿using UnityEngine;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            Respawner.RespawnProcess(other.GetComponent<Player>());
        }
    }
}
