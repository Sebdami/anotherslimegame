﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ptrStateFct();
public class PlayerState {

    protected PlayerControllerHub playerController;
    public bool stateAvailable = true;

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    public float maxCoolDown = 0;

    float dragForce = 0.025f;
    float dragForceDash = 0.01f; // Dash value

    float airControlFactor = 0.75f;
    float timerApplyDrag = 0.02f;

    #region getterSetters
    public ptrStateFct CurActionFct
    {
        get{return curUpdateFct;}
        protected set{curUpdateFct = value;}
    }
    public ptrStateFct CurFixedUpdateFct
    {
        get{return curFixedUpdateFct;}
        set{curFixedUpdateFct = value;}
    }
    #endregion


    public PlayerState(PlayerControllerHub _playerController)
    {
        playerController = _playerController;
    }
    public virtual void OnBegin()
    {
    }
    public virtual void OnEnd()
    {
        stateAvailable = false;
        playerController.StartCoroutine(StateCooldown(maxCoolDown));
    }
    public IEnumerator StateCooldown(float maxCoolDown)
    {
        yield return new WaitForSeconds(maxCoolDown);
        stateAvailable = true;
        yield return null;
    }

    public virtual void OnUpdate()
    {
        if (curUpdateFct != null)
            curUpdateFct();
    }
    public virtual void OnFixedUpdate()
    {
        if (CurFixedUpdateFct != null)
            CurFixedUpdateFct();
    }
    /// ACTIONS /////////////////////////////////////////////////////////

    public virtual Vector3 HandleSpeedWithController()
    {
        Vector3 initialVelocity = new Vector3(playerController.State.ThumbSticks.Left.X, 0.0f, playerController.State.ThumbSticks.Left.Y);
        initialVelocity.Normalize();

        initialVelocity *= playerController.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Utils.Abs(playerController.State.ThumbSticks.Left.X) + Utils.Abs(playerController.State.ThumbSticks.Left.Y) < 0.95f)
        {
            initialVelocity /= 2;
        }

        return initialVelocity;
    }

    public Vector3 GetVelocity3DThirdPerson(Vector3 initialVelocity)
    {
        Vector3 camVectorForward = new Vector3(playerController.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerController.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + Vector3.up * playerController.Player.Rb.velocity.y;
        // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
        if (!playerController.IsGrounded && playerController.jumpState.nbJumpMade == 2)
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right * airControlFactor;
        else
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right;

        return velocityVec;
    }
    public Vector3 GetVelocity3DSideView(Vector3 initialVelocity)
    {
        Debug.LogError("GetVelocity3DSideView Not Impleented");
        return Vector3.zero;
    }
    /// <summary>
    ///  I assume a side2d scene is still in the same orientation ( z = profondeur.)
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <returns></returns>
    public Vector3 GetVelocity2DSideView(Vector3 initialVelocity)
    {
        Vector3 velocityVec =  Vector3.up * playerController.Player.Rb.velocity.y;
        // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
        if (!playerController.IsGrounded && playerController.jumpState.nbJumpMade == 2)
            velocityVec += initialVelocity.x * Vector3.right * airControlFactor;
        else
            velocityVec += initialVelocity.x * Vector3.right;
        return velocityVec;
    }

    public virtual void Move(Vector3 initialVelocity)
    {

        Vector3 velocityVec = Vector3.zero;
        switch (GameManager.Instance.CurrentGameMode.ViewMode)
        {
            case ViewMode.thirdPerson3d:
                velocityVec = GetVelocity3DThirdPerson(initialVelocity);
                break;
            case ViewMode.sideView3d:
                velocityVec = GetVelocity3DSideView(initialVelocity);
                break;
            case ViewMode.sideView2d:
                velocityVec = GetVelocity2DSideView(initialVelocity);
                break;
            default:
                Debug.LogError("Invalid view in GameMode");
                break;
        }

        if (initialVelocity.magnitude > 1f)
        {
            if (!playerController.forceCameraRecenter)
            {
                // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
                playerController.Player.Rb.velocity = velocityVec;
                playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);
            }
        }
        else
        {
            Vector3 tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);
            Vector3 fwd = playerController.transform.forward;

            float dragForceUsed = dragForce;//(playerController.PreviousPlayerState == playerController.dashState) ? dragForceDash : dragForce;

            if (tmp.sqrMagnitude > 7.0f)// && Vector3.Dot(playerController.transform.forward, tmp) > 0)
            {
                if ((tmp.x > 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed < 0)
                || (tmp.x < 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed > 0)
                || (tmp.z > 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed < 0)
                || (tmp.z < 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed > 0))
                    playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
                else
                    playerController.Player.Rb.velocity -= (tmp.normalized * dragForceUsed * tmp.sqrMagnitude) * ((Time.deltaTime / timerApplyDrag));

                tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);

                if (Vector3.Dot(fwd, tmp) < 0)
                    playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;

            }
            else
            {
                playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
            }
        }

    }

    public virtual void HandleGravity()
    {
        if (playerController.isGravityEnabled)
        {
            float gravity = playerController.GetComponent<JumpManager>().GetGravity();
            playerController.Player.Rb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
        }
    }
    public virtual void OnJumpPressed()
    {
        if (playerController.wallJumpState.WallJumpTest())
        {
            playerController.PlayerState = playerController.wallJumpState;
        }
        else if (playerController.jumpState.nbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
        {
            playerController.PlayerState = playerController.jumpState;
        }
    }
    public virtual void OnDashPressed()
    {
        if (playerController.dashState.nbDashMade < 1)
        {
            playerController.PlayerState = playerController.dashState;
            if (playerController.jumpState.nbJumpMade > 0)
                playerController.dashState.nbDashMade++;
        }
    }
    public virtual void OnDownDashPressed()
    {
        if (playerController.downDashState.nbDashDownMade < 1 && !playerController.IsGrounded)
        {
            playerController.PlayerState = playerController.downDashState;
            if (playerController.jumpState.nbJumpMade > 0)
                playerController.downDashState.nbDashDownMade++;
        }
    }
    public virtual void PushPlayer(Vector3 force)
    {
        playerController.PlayerState = playerController.expulsedState;
        playerController.Rb.velocity = new Vector3(force.x, playerController.Rb.velocity.y, force.z);
    }
    public virtual void HandleControllerAnim()
    {
        playerController.Player.Anim.SetFloat("MouvementSpeed", Utils.Abs(playerController.State.ThumbSticks.Left.X) > Utils.Abs(playerController.State.ThumbSticks.Left.Y) ? Utils.Abs(playerController.State.ThumbSticks.Left.X) + 0.2f : Utils.Abs(playerController.State.ThumbSticks.Left.Y) + 0.2f);
        playerController.Player.Anim.SetBool("isWalking", ((Utils.Abs(playerController.State.ThumbSticks.Left.X) > 0.02f) || Utils.Abs(playerController.State.ThumbSticks.Left.Y) > 0.02f) && playerController.IsGrounded);
    }


    public virtual void CollisionEnter(Collision collision)
    { }
    public virtual void CollisionStay(Collision collision)
    { }
    public virtual void CollisionExit(Collision collision)
    { }



    public virtual void DrawGizmo()
    {}
}
