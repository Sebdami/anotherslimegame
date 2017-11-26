﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {
    public Vector3 buoyancyCentreOffset;
    public float bounceDamp;
    public float waterLevel;
    public float floatHeight;
    public float compensationGravity;

    StatBuff movestatbuff;
    StatBuff dashstatbuff;
    StatBuff jumpstatbuff;
    public float tolerance;
    public float waterResistance;

    public GameObject WaterToActivateAtRuntime;

    public GameObject WaterParticleSystemToInstantiate;



    //The scene's default fog settings
    private bool defaultFog ;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;
    private Material noSkybox;


    public void Start()
    {
        WaterToActivateAtRuntime.SetActive(true);

        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            if (other.transform.GetChild((int)PlayerChildren.WaterEffect).GetComponent<ParticleSystem>())
            {          
                // SEB C'est pour toi
                Instantiate(WaterParticleSystemToInstantiate, other.transform.position + (Vector3.up *2), other.transform.rotation, null);
                other.transform.GetChild((int)PlayerChildren.WaterEffect).gameObject.SetActive(true);
            }
     
          
            if (waterResistance != 0)
            {
                movestatbuff = new StatBuff(Stats.StatType.GROUND_SPEED, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(movestatbuff);

                dashstatbuff = new StatBuff(Stats.StatType.DASH_FORCE, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(dashstatbuff);


                jumpstatbuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(jumpstatbuff);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();

            // Niveau de floattabilté, fonction de la hauteur du joueur
            Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);

            float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
            Vector3 gravity = new Vector3(0, other.GetComponent<JumpManager>().GetGravity(), 0) * compensationGravity;
            if (other.transform.position.y > waterLevel - tolerance)
            {
                other.GetComponent<PlayerController>().IsGrounded = true;
            } else
            {
                if (forceFactor > 0f)
                {
                    Vector3 uplift = gravity * ((forceFactor - rigidbody.velocity.y) * bounceDamp);


                    rigidbody.AddForceAtPosition(uplift, actionPoint);
                }
            }

            // TODO ; per camera ? 
            if (other.GetComponent<Player>().cameraReference.transform.GetChild(0).position.y < WaterToActivateAtRuntime.transform.position.y)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
                RenderSettings.fogDensity = 0.04f;
                RenderSettings.skybox = noSkybox;

                other.GetComponent<Player>().cameraReference.transform.GetChild(0).GetComponent<Camera>().renderSet
            }
            else
            { 
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetChild((int)PlayerChildren.WaterEffect).GetComponent<ParticleSystem>())
        {
            // SEB C'est pour toi
            other.transform.GetChild((int)PlayerChildren.WaterEffect).gameObject.SetActive(false);
        }

        if (waterResistance != 0)
        {
            // TODO : Need a contains buff ?
            other.GetComponent<PlayerController>().stats.RemoveBuff(movestatbuff);
            other.GetComponent<PlayerController>().stats.RemoveBuff(dashstatbuff);
            other.GetComponent<PlayerController>().stats.RemoveBuff(jumpstatbuff);
        }

        RenderSettings.fog = defaultFog;
        RenderSettings.fogColor = defaultFogColor;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.skybox = defaultSkybox;
    }
}
