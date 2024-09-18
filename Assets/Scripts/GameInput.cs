using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{

    public float cancelDelta = 0.2f;
    public ParticleSystem walkIndicator = null;
    private RaycastHit hit;
    private Vector3 pointerDown = Vector3.zero;
    private bool blockedByUI = false;

    // Start is called before the first frame update
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            blockedByUI = EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
            if (!blockedByUI/* && EventSystem.current.currentSelectedGameObject == null*/)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Debug.Log("Move To " + hit.point);
                    Debug.Log(hit.collider.tag + " " + hit.collider.name);
                    //if we hit an object tagged with ground, set the loaded characters steering target to that point
                    if (hit.collider.tag == "Ground")
                    {
                        pointerDown = hit.point;
                    }
                }
            }
        }

        //check for input
        if (Input.GetMouseButtonUp(0))
        {
            blockedByUI = EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
            Debug.Log("GameInput Mouse Up, " + blockedByUI);
            //check if the finger is over any ui element
            if (!blockedByUI/* && EventSystem.current.currentSelectedGameObject == null*/)
            {
                //send raycast to check for object and position
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Debug.Log(Vector3.Distance(hit.point, pointerDown));
                    if (Vector3.Distance(hit.point, pointerDown) < cancelDelta)
                    {
                        Debug.Log(hit.collider.tag);
                        //if we hit an object tagged with ground, set the loaded characters steering target to that point
                        if (hit.collider.tag == "Ground")
                        {
                            Debug.Log("Move To " + hit.point);
                            var properties = new ExitGames.Client.Photon.Hashtable
                            {
                                { "steeringTarget", hit.point }
                            };
                            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
                            walkIndicator.transform.position = hit.point + Vector3.up;
                            walkIndicator.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                            walkIndicator.Play();
                        }
                    }
                }
            }
            else
            {
                //do nothing, touch is handled by unity ui
            }
        }
    }
}
