﻿using UWPAndXInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ScoreScreen : MonoBehaviour {
    enum ScoreScreenChildren { ScorePanel }

    GameObject scorePanel;

    public GameObject prefabPlayerScore;

    private int valueCoins = 5;
    private int valueTime = 100;

    public Dictionary<Player, GameObject> scorePanelPlayer = new Dictionary<Player, GameObject>();
    public int rank = 0;

    uint nbrOfPlayersAtTheEnd = 0;

    // A GameObject that contains children + a camera 
    [SerializeField]
    Transform podium;

    private void Awake()
    {
        GameManager.Instance.RegisterScoreScreenPanel(this);
        scorePanel = transform.GetChild((int)ScoreScreenChildren.ScorePanel).gameObject;
        gameObject.SetActive(false);

    }

    public void Init()
    {

        //for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        //{
        //    GameObject playerScore = Instantiate(prefabPlayerScore, scorePanel.transform);
        //    scorePanelPlayer.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerScore);
        //}
    }

    public void RefreshScores(Player player)
    {
        float time = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = (int)time % 60;
        rank++;

        if (rank == 1)
        {
            GameManager.Instance.LaunchFinalTimer();
        }

        String timeStr = string.Format("{0:00} : {1:00}", minutes, seconds);

        transform.GetChild(rank - 1).GetComponent<PlayerScore>().SetScoreDefault(
            (int)player.PlayerController.PlayerIndex, 
            GameManager.Instance.isTimeOver ? "Timeout" : timeStr, 
            (player.Collectables[(int)CollectableType.Points]).ToString()
        );

        transform.GetChild(rank - 1).gameObject.SetActive(true);

        player.rank = rank;

        if (rank == 1)
            player.Anim.SetBool("hasFinishedFirst", true);
        else if (rank == 4)
            player.Anim.SetBool("hasFinishedLast", true);
        else
            player.Anim.SetBool("hasFinished", true);

        nbrOfPlayersAtTheEnd++;
        CheckEndGame();
    }

    public void RefreshScoresTimeOver(Player[] _remainingPlayers)
    {
        for (int i = 0; i < _remainingPlayers.Length; i++)
            RefreshScores(_remainingPlayers[i]);
    }

    void CheckEndGame()
    {
        if (nbrOfPlayersAtTheEnd == GameManager.Instance.PlayerStart.ActivePlayersAtStart)
        {
            List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
            for (int i = 0; i < players.Count; i++)
            {
                Player curPlayer = players[i].GetComponent<Player>();
                curPlayer.cameraReference.SetActive(false);
                curPlayer.transform.SetParent(podium.GetChild(curPlayer.rank));
                curPlayer.transform.localPosition = Vector3.zero;
                curPlayer.transform.localRotation = Quaternion.identity;
            }
            podium.GetChild(5).gameObject.SetActive(true);
            GameManager.UiReference.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    void Update()
    {

        // TODO : Multi to be handle
        if (!GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PlayerIndexSet)
            return;

        if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().IsUsingAController)
        {
            if (GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.Start == ButtonState.Pressed)
                ExitToMainMenu();

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ExitToMainMenu();
        }
        // TODO: handle pause input here?
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
