﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class APlayerUI : MonoBehaviour {

    public GameObject UIref;

    public virtual void Init()
    {
        for (int i = 0; i < UIref.transform.childCount; i++)
        {
            if (GameManager.Instance.PlayerStart.PlayersReference[i] != null)
                UIref.transform.GetChild(i).gameObject.SetActive(true);
            else
                UIref.transform.GetChild(i).gameObject.SetActive(false);
        }
        foreach (GameObject p in GameManager.Instance.PlayerStart.PlayersReference)
            p.GetComponent<Player>().OnValuesChange = new UIfct[(int)CollectableType.Size];

        foreach (TextChange obj in GameObject.FindObjectsOfType<TextChange>())
        {
            obj.Init();
        }

    }

    public virtual void OnValueChange(TextChange text)
    {
        // feedback 
    }
}
