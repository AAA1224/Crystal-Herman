using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace KoboldTools
{
    public class Interactions : MonoBehaviour
    {
        public enum InteractionType
        {
            POINTERUP,
            POINTERDOWN
        }

        public float dragCancelDelta = 0.5f;
        public Camera[] checkForCameras;
        public LayerMask excludeLayers;
        public MonoBehaviour[] interactiveBlockers;

        private List<IInteractiveBlocker> _interactiveBlockers = new List<IInteractiveBlocker>();
        private Vector2 lastPos = Vector2.zero;
        private bool isTouch = false;
        private bool pointerOverObject = true;

        private void Start()
        {
            foreach(MonoBehaviour mb in interactiveBlockers)
            {
                if(mb is IInteractiveBlocker)
                {
                    _interactiveBlockers.Add((IInteractiveBlocker)mb);
                }
            }
        }

        private void Update()
        {

            

            if (Application.isMobilePlatform)
            {
                pointerOverObject = EventSystem.current.IsPointerOverGameObject(0);
                if (Input.touchCount > 0)
                {
                    if (!pointerOverObject)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            isTouch = true;
                            check(Input.GetTouch(0).position, InteractionType.POINTERDOWN);
                        }
                    }
                    if (isTouch && Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        isTouch = false;
                        check(Input.GetTouch(0).position, InteractionType.POINTERUP);
                    }
                }
            }
            else
            {
                pointerOverObject = EventSystem.current.IsPointerOverGameObject();
                if (pointerOverObject)
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    lastPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    check(lastPos, InteractionType.POINTERDOWN);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                    if (Vector2.Distance(lastPos, pos) > dragCancelDelta)
                        return;

                    check(pos, InteractionType.POINTERUP);
                }
           }



        }

        public void check(Vector2 pos, InteractionType interactionType)
        {
            foreach(IInteractiveBlocker blocker in interactiveBlockers)
            {
                if (blocker.blocked)
                {
                    return;
                }
            }


            foreach (Camera cam in checkForCameras)
            {
                RaycastHit2D hitInfo2D;
                RaycastHit hitInfo3D;
                bool hit3D = false;

                hitInfo2D = Physics2D.Raycast(cam.ScreenToWorldPoint(pos), Vector2.zero, Mathf.Infinity, cam.cullingMask);
                hit3D = Physics.Raycast(cam.ScreenPointToRay(pos), out hitInfo3D, Mathf.Infinity, cam.cullingMask);

                // RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
                if (hitInfo2D || hit3D)
                {
                    IInteractive[] interactives = cam.orthographic ? hitInfo2D.transform.gameObject.GetComponents<IInteractive>() : hitInfo3D.transform.gameObject.GetComponents<IInteractive>();
                    foreach (IInteractive interactive in interactives)
                    {
                        switch (interactionType)
                        {
                            case InteractionType.POINTERUP:
                                interactive.onPointerUp();
                                break;
                            case InteractionType.POINTERDOWN:
                                interactive.onPointerDown();
                                break;
                        }
                    }
                }
            }
        }
    }
}
