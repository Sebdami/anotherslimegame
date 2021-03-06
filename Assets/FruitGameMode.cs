﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGameMode : GameMode {

    public float timer;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);

        LaunchTimer();
    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);

        }
    }

    public override void PlayerHasFinished(Player player)
    {
        GameManager.Instance.ScoreScreenReference.RankPlayersByPoints();
    }
}
