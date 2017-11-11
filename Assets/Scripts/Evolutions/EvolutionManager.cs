﻿using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class Evolution
{
    int id;
    string name; 
    public float duration;
    CollectableType associatedCollectable;
    int cost;
    BodyPart bodyPart;
    Powers power;

        // GamePlay for each evolution
    
    public int Cost
    {
        get
        {
            return cost;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public CollectableType AssociatedCollectable
    {
        get
        {
            return associatedCollectable;
        }
    }

    public BodyPart BodyPart
    {
        get
        {
            return bodyPart;
        }
    }

    public Evolution(Powers _powerName, float _duration, CollectableType _associatedCollectable, int _cost, BodyPart _bodyPart)
    {
        id = (int)_powerName;
        name = _powerName.ToString();
        duration = _duration;
        associatedCollectable = _associatedCollectable;
        cost = _cost;
        bodyPart = _bodyPart;
    }


  
}

public enum BodyPart { Body, Wings, Hammer, Staff, Size }
public enum Powers { DoubleJump, Hover, Size, Strength, Agile }

public class EvolutionManager {

    // Evolution database handled in code
    Evolution doubleJumpEvolution = new Evolution(Powers.DoubleJump, 5, CollectableType.WingsEvolution1, 20, BodyPart.Wings);
    Evolution hoverEvolution = new Evolution(Powers.Hover, 3, CollectableType.WingsEvolution2, 30, BodyPart.Wings);
    Evolution strengthEvolution = new Evolution(Powers.Strength, 3, CollectableType.WingsEvolution2, 30, BodyPart.Hammer);
    Evolution agileEvolution = new Evolution(Powers.Agile, 3, CollectableType.WingsEvolution2, 30, BodyPart.Wings);

    public Evolution GetEvolutionByPowerName(Powers _powerName, bool isPermanent = false)
    {
        Evolution tmpEvolution;

        switch (_powerName)
        {
            case Powers.DoubleJump:
                tmpEvolution = doubleJumpEvolution;
                break;
            case Powers.Hover:
                tmpEvolution = hoverEvolution;
                break;
            case Powers.Strength:
                tmpEvolution = strengthEvolution;
                break;
                case Powers.Agile:
                tmpEvolution = agileEvolution;
                break;
            default:
                Debug.Log("Unknown power, something went wrong");
                return null;
        }

        if (isPermanent) tmpEvolution.duration = 0.0f;
        return tmpEvolution;
    }

    public void AddEvolutionComponent(GameObject gameObject, Evolution evolution, bool isPermanent = false)
    {
        Powers power = (Powers)evolution.Id;
        switch (power)
        {
            case Powers.DoubleJump:
                if (gameObject.GetComponent<DoubleJump>() != null) gameObject.GetComponent<DoubleJump>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<DoubleJump>();
                break;
            case Powers.Hover:
                if (gameObject.GetComponent<Hover>() != null) gameObject.GetComponent<Hover>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<Hover>(); break;
            case Powers.Strength:
                if (gameObject.GetComponent<EvolutionStrength>() != null) gameObject.GetComponent<EvolutionStrength>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionStrength>(); break;
            case Powers.Agile:
                if (gameObject.GetComponent<EvolutionAgile>() != null) gameObject.GetComponent<EvolutionAgile>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionAgile>(); break;
            default:
                Debug.Log("Unknown power, something went wrong");
                break;
        }
    }

    /*
     * Return the evolution linked to the collectable type or null if the collectable is not linked to any evolution (ex: Points)
     */
    public Evolution GetEvolutionByCollectableType(CollectableType _type)
    {
        switch (_type)
        {
            case CollectableType.WingsEvolution1:
                return doubleJumpEvolution;
            case CollectableType.WingsEvolution2:
                return hoverEvolution;
            case CollectableType.StrengthEvolution1:
                return strengthEvolution;
            default:
                Debug.LogError("Unhandle Evolution");
                return null;
        }
    }
}
