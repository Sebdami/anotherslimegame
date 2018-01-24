﻿using System.Collections;
using System.Collections.Generic;
using DatabaseClass;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance;

    private DatabaseClass.Database db;

    public static Database Db
    {
        get
        {
            return instance.db;
        }

        set
        {
            instance.db = value;
        }
    }

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
            Db = Resources.Load("Database") as DatabaseClass.Database;
        }

        else
            DontDestroyOnLoad(instance);
	}

}
