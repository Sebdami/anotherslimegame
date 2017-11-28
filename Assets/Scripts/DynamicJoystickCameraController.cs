﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using Cinemachine;

public class DynamicJoystickCameraController : MonoBehaviour {
    public PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    Vector3 startLowOffset;
    Vector3 startMidOffset;
    Vector3 startHighOffset;
    bool once = false;

    Cinemachine.CinemachineFreeLook freelookCamera;
    // Use this for initialization
    void Start () {
        freelookCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
        startHighOffset = ((CinemachineComposer)freelookCamera.GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Aim)).m_TrackedObjectOffset;
        startMidOffset = ((CinemachineComposer)freelookCamera.GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Aim)).m_TrackedObjectOffset;
        startLowOffset = ((CinemachineComposer)freelookCamera.GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Aim)).m_TrackedObjectOffset;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
#if UNITY_EDITOR

        if (GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponent<PlayerController>().IsUsingAController)
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (prevState.IsConnected != state.IsConnected)
            {
                freelookCamera.m_XAxis.m_InputAxisName = string.Empty;
                freelookCamera.m_YAxis.m_InputAxisName = string.Empty;
                once = false;
            }

            freelookCamera.m_XAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.X) > 0.1f ? -state.ThumbSticks.Right.X : 0;
            freelookCamera.m_YAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.Y) > 0.1f ? state.ThumbSticks.Right.Y : 0;
            //Need a more complex function ?
            //if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().IsGrounded)
            freelookCamera.m_XAxis.m_InputAxisValue += Mathf.Abs(state.ThumbSticks.Left.X) > 0.1f ? (freelookCamera.m_XAxis.m_InvertAxis?-1:1) * state.ThumbSticks.Left.X* Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(state.ThumbSticks.Left.X))/2.0f : 0;
            //else
            //    freelookCamera.m_XAxis.m_InputAxisValue += Mathf.Abs(state.ThumbSticks.Left.X) > 0.1f ? (-state.ThumbSticks.Left.X * Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(state.ThumbSticks.Left.X)))/2.0f : 0;
            /*CinemachineComposer cp;
            cp = ((CinemachineComposer)(freelookCamera.GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
            cp.m_TrackedObjectOffset = startHighOffset + (Mathf.Abs(state.ThumbSticks.Left.X) > 0.01f ? transform.right*2f : Vector3.zero);
            cp = ((CinemachineComposer)(freelookCamera.GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
            cp.m_TrackedObjectOffset = startMidOffset + (Mathf.Abs(state.ThumbSticks.Left.X) > 0.01f ? transform.right * 2f : Vector3.zero);
            cp = ((CinemachineComposer)(freelookCamera.GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
            cp.m_TrackedObjectOffset = startLowOffset + (Mathf.Abs(state.ThumbSticks.Left.X) > 0.01f ? transform.right * 2f : Vector3.zero);*/
            /*if(freelockCamera.LookAt.GetComponent<Rigidbody>().velocity.magnitude >0.01f)
            {
                freelockCamera.m_RecenterToTargetHeading.m_enabled = true;
            }
            else
            {
                freelockCamera.m_RecenterToTargetHeading.m_enabled = false;
            }*/
        }
        else
        {
            if (!once)
            {
                once = true;
                freelookCamera.m_XAxis.m_InputAxisName = "Mouse X";
                freelookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            }

        } 
#endif
    }
}
