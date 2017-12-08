﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolChild : MonoBehaviour {

    public Transform poolParent;

    float timerReturnToPool = 15.0f; // SHould be defined by the parent TODO
    float currentTimer = 0.0f;

    private void OnEnable()
    {
        currentTimer = timerReturnToPool;
    }

    void Update () {
        currentTimer -= Time.deltaTime;
        if (currentTimer < 0.0f)
        {
            transform.SetParent(poolParent);
            gameObject.SetActive(false);
        }
	}
}
