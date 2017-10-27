﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected OutlineApplier m_OutlineApplier;

    //OVRPlayerController, The GameObject that holds our OVRCameraRig
    //& input modules, as well as player options
    public OVRPlayerController m_player;

    //OVRGazePointer, the reticle object we use for our playercontroller
    public OVRGazePointer m_reticle;

    private float m_MaxDistance;
    //The user's ingame score
    [SerializeField] protected int m_Score;

    //The number of hints remaining
    [SerializeField] protected int m_Hints;

    //flag that tells us the object selection mode
    //true is hazard mode, false is safety mode.
    [SerializeField] protected bool m_HazardMode;

    /// <summary>
    /// Time left in the round, displayed on User interface
    /// Unit is seconds
    /// </summary>
    [SerializeField] protected float m_timeLeft = 300.0f;

    //Accessor for score
    public int Score { get { return m_Score; } }

    //Accessor for hints
    public int Hints { get { return m_Hints; } }

    //Accessor for our current mode; True is hazard mode, false is safety mode.
    public bool HazardMode { get { return m_HazardMode; } }

    //Accessor for remaining time, in seconds
    public float TimeLeft { get { return m_timeLeft; } }

    protected virtual void Start()
    {
        //associate m_player with the OVRPlayerController
        m_player = GameObject.FindObjectOfType<OVRPlayerController>();

        m_MaxDistance = 1.5f;
    }

    protected virtual void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) //pressed the A button
        {
            GameObject obj = Raycast();
            if (obj != null)
                Interact(obj);
        }

        if (OVRInput.GetDown(OVRInput.Button.Two)) //pressed the B button
        {
            GameObject obj = Raycast();
            if (obj != null)
                Hinteract(obj);
        }

    }

    protected GameObject Raycast()
    {
        #region Raycasting/object interaction code
        RaycastHit hit;
        Vector3 reticleScreen = Camera.main.WorldToScreenPoint(m_reticle.transform.position);

        Ray ray = Camera.main.ScreenPointToRay(reticleScreen);//new Ray(m_player.transform.position, m_reticle.transform.position);

        if (Physics.Raycast(ray, out hit, m_MaxDistance))
            return hit.transform.gameObject;

        return null;
        /**
        if (OVRInput.GetDown(OVRInput.Button.One)) //pressed the A button
            if (Physics.Raycast(ray, out hit, m_MaxDistance)) //if we're raycasting onto an object 
                Interact(hit.transform.gameObject); //try interacting with the object we raycasted onto

        if (OVRInput.GetDown(OVRInput.Button.Two) && m_Hints > 0) //B button
        {
            if (Physics.Raycast(ray, out hit, m_MaxDistance))
            {
                //Get a hint about the object
                //and add to review panel
                Hinteract(hit.transform.gameObject);
            }
        }*/

        #endregion
    }

    /// <summary>
    /// interact with obj; check validity of interaction and actually execute the interaction.
    /// </summary>
    public virtual void Interact(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //is our object meant to be interacted with?
        if (objInfo == null)
        {
            Debug.Log("tried to interact with a non-interactable object");
            return;
        }
        else if (objInfo.Interacted) //have we already interacted with the object?
        {
            Debug.Log("tried to interact with an object we've already interacted with");
            return;
        }

        //update interaction flag.
        objInfo.Interact();

        //string for comparing tags
        string currentMode = (m_HazardMode) ? "hazard" : "safety";

        //does our mode correspond to this object's tag?
        bool correctTag = obj.CompareTag(currentMode);

        //did we select the correct object type for the object?
        if (correctTag)
        {
            //Apply a green (correct) outline to the object model and add score
            m_OutlineApplier.ApplyGreenOutline(obj);
            AddScore(objInfo.BaseScore);
        }
        else
        {
            //Apply a red (incorrect) outline to the object
            m_OutlineApplier.ApplyRedOutline(obj);
            SubtractScore(objInfo.BaseScore);
        }
    }

    public virtual void Hinteract(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //did we add add the object information component?
        if (objInfo == null)
        {
            Debug.LogError("Tried to hint an object that has no ObjectInformation.");
            return;
        }

        //was the object already hinteracted with?
        if (objInfo.Hinted)
        {
            Debug.Log("Hinted an object that we've already hinted.");
            return;
        }

        //we know we can now hint the object, and we will
        objInfo.Hint();

        //add orange (hinted) outline to the object
        m_OutlineApplier.ApplyOrangeOutline(obj);

        //decrement the amount of hints the player has remaining
        --m_Hints;
    }

    protected virtual void AddScore(int baseScore)
    {
        m_Score += baseScore;
    }

    protected virtual void SubtractScore(int baseScore)
    {
        m_Score -= baseScore;
    }
}