﻿using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;

public class Menu : MonoBehaviour {
    public enum MenuState { TitleScreen, ModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreen;

    int currentCursor = 0;
    GamePadState prevControllerState;
    GamePadState controllerState;

    int selectedMode = -1;
    int nbPlayers = -1;

    Button currentlySelectedButton;
    bool buttonNeedUpdate = false;

    public GameObject playerCustomScreenPrefab;

    List<GameObject> playerCustomScreens = new List<GameObject>();

    public List<Color> customColors;

    GamePadState[] prevControllerStates = new GamePadState[4];
    GamePadState[] controllerStates = new GamePadState[4];

    int[] selectedColors = new int[4];
    int[] selectedFaces = new int[4];
    int maxFacesNumber = 4;
    int[] currentCursorsRow = new int[4];

    public void SetMode(int _modeSelected)
    {
        selectedMode = _modeSelected;
    }

    public void SetNbPlayers(int _nbPlayersSelected)
    {
        nbPlayers = _nbPlayersSelected;
    }

    private void Start()
    {
        SetState(MenuState.TitleScreen);
        //for (int i = 0; i < 4; i++)
        //{
        //    prevControllerStates.Add(GamePad.GetState((PlayerIndex)i));
        //    controllerStates.Add(GamePad.GetState((PlayerIndex)i));
        //}
    }

    void Update()
    {
        if (currentState != MenuState.CustomisationScreen)
        {
            prevControllerState = controllerState;
            controllerState = GamePad.GetState(0);

            if (Input.anyKey && currentState == MenuState.TitleScreen)
            {
                GoToNextState();
            }

            if ((controllerState.ThumbSticks.Left.X > 0.5f && prevControllerState.ThumbSticks.Left.X < 0.5f)
                    || (controllerState.ThumbSticks.Left.Y < -0.75f && prevControllerState.ThumbSticks.Left.Y > -0.75f))
            {
                buttonNeedUpdate = true;
                currentCursor++;
            }
            else if ((controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
                    || (controllerState.ThumbSticks.Left.Y > 0.75f && prevControllerState.ThumbSticks.Left.Y < 0.75f))
            {
                buttonNeedUpdate = true;
                currentCursor--;
            }
        

            // Update visual feedback
            if (buttonNeedUpdate)
            {
                if (currentState == MenuState.NumberOfPlayers && selectedMode == 1)
                {
                    if (currentCursor < 0)
                        currentCursor = 3;
                    else
                        currentCursor = currentCursor % 4;
                    currentlySelectedButton = transform.GetChild((int)currentState).GetChild(3).GetChild(currentCursor).GetComponent<Button>();
                    currentlySelectedButton.Select();
                }
                else
                {
                    if (currentCursor < 0)
                        currentCursor = 1;
                    else
                        currentCursor = currentCursor % 2;
                    currentlySelectedButton = transform.GetChild((int)currentState).GetChild(2).GetChild(currentCursor).GetComponent<Button>();
                    currentlySelectedButton.Select();
                }
                buttonNeedUpdate = false;
            }

            if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
            {
                if (currentlySelectedButton != null)
                {
                    currentlySelectedButton.onClick.Invoke();
                    GoToNextState();
                }
            }

            else if (prevControllerState.Buttons.B == ButtonState.Released && controllerState.Buttons.B == ButtonState.Pressed)
            {
                if (currentState != MenuState.TitleScreen)
                    ReturnToPreviousState();
            }
        }
        else
        {
            if (nbPlayers == -1)
                return;

            for (int i = 0; i < nbPlayers; i++)
            {
                prevControllerStates[i] = controllerStates[i];
                controllerStates[i] = GamePad.GetState((PlayerIndex)i);

                if (controllerStates[i].ThumbSticks.Left.Y > 0.5f && prevControllerStates[i].ThumbSticks.Left.Y < 0.5f
                    || (controllerState.ThumbSticks.Left.Y < -0.5f && prevControllerState.ThumbSticks.Left.Y > -0.5f))
                {
                    currentCursorsRow[i]++;
                    currentCursorsRow[i] = currentCursorsRow[i] % 2;
                }
                else if (controllerStates[i].ThumbSticks.Left.X > 0.5f && prevControllerStates[i].ThumbSticks.Left.X < 0.5f)
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        selectedColors[i]++;
                        selectedColors[i] = selectedColors[i] % customColors.Count;
                        // Update text and character
                        playerCustomScreens[i].transform.GetChild(1).GetComponent<Text>().text = (selectedColors[i]+1).ToString();
                        playerCustomScreens[i].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(customColors[selectedColors[i]]);
                    }
                    else
                    {
                        selectedFaces[i]++;
                        selectedFaces[i] = selectedFaces[i] % maxFacesNumber;
                        // Update text and character
                        playerCustomScreens[i].transform.GetChild(2).GetComponent<Text>().text = (selectedFaces[i]+1).ToString();
                        playerCustomScreens[i].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)selectedFaces[i];
                    }
                }
                else if (controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        selectedColors[i]--;
                        if (selectedColors[i] < 0)
                            selectedColors[i] = customColors.Count - 1;
                        else
                            selectedColors[i] = selectedColors[i] % customColors.Count;
                        // Update text and character
                        playerCustomScreens[i].transform.GetChild(1).GetComponent<Text>().text = (selectedColors[i]+1).ToString();
                        playerCustomScreens[i].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(customColors[selectedColors[i]]);
                    }
                    else
                    {
                        selectedFaces[i]--;
                        if (selectedFaces[i] < 0)
                            selectedFaces[i] = maxFacesNumber - 1;
                        else
                            selectedFaces[i] = selectedFaces[i] % maxFacesNumber;

                        // Update text and character
                        playerCustomScreens[i].transform.GetChild(2).GetComponent<Text>().text = (selectedFaces[i]+1).ToString();
                        playerCustomScreens[i].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)selectedFaces[i];
                    }
                }
            }
        }
    }

    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);
        if (currentState == MenuState.ModeSelection) selectedMode = -1;
        if (currentState == MenuState.NumberOfPlayers) nbPlayers = -1;

        if (currentState == MenuState.CustomisationScreen)
        {
            playerCustomScreens.Clear();
            for (int i = 0; i < nbPlayers; i++)
            {
                GameObject go = Instantiate(playerCustomScreenPrefab, transform.GetChild((int)MenuState.CustomisationScreen));
                go.GetComponentInChildren<Text>().text = "Player " + (i + 1);

                if (nbPlayers == 1)
                    go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                if (nbPlayers == 2)
                    go.transform.localPosition = new Vector3(-(250) + (2*i) * (250), 0.0f, 0.0f);
                if (nbPlayers == 3)
                    go.transform.localPosition = new Vector3((-(250) + i *(250)), 0.0f, 0.0f);
                if (nbPlayers == 4)
                    go.transform.localPosition = new Vector3(-(300) + (i * (200)), 0.0f, 0.0f);

                playerCustomScreens.Add(go);
            }
        }

    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        SetState((MenuState)((int)currentState - 1));
    }
}
