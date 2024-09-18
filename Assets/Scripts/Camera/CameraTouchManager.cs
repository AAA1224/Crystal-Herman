using Cinemachine;
using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraTouchManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _panEnabled = true;
    public bool panEnabled
    {
        set { _panEnabled = value; }
    }
    private float _cumulatedMagnitude = 0f;
    private bool _blocked = false;
    public bool blocked
    {
        get
        {
            return _blocked;
        }
    }
    public bool yUp = false;
    public bool inverse = false;
    public float panSpeed = 1f;
    public Rect containRect = new Rect();
    private Vector3 lastMouse;
    private float sinceLastTap = 0f;


    void LateUpdate()
    {
        //if (this.gameObject == null)
        //    return;

        //if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
        //    return;

        //movement
        Vector2 deltaWorld = Vector2.zero;


        //Touch  
        if (Input.touchCount == 1)
        {
            //get pan
            Touch touchZero = Input.GetTouch(0);
            if (touchZero.phase == TouchPhase.Moved)
            {
                deltaWorld = new Vector2(
                        2f * Camera.main.orthographicSize * Camera.main.aspect * (touchZero.deltaPosition.x / Screen.width),
                        2f * Camera.main.orthographicSize * (touchZero.deltaPosition.y / Screen.height)
                    );
            }

        }

        //Mouse
        if (Input.GetMouseButtonDown(0))
        {
            lastMouse = Input.mousePosition;
            Debug.Log("Mouse down");
        }

        if (Input.GetMouseButton(0))
        {
            //get pan
            Vector3 deltaPosition = Input.mousePosition - lastMouse;

            deltaWorld = new Vector2(
                    2f * Camera.main.orthographicSize * Camera.main.aspect * (deltaPosition.x / Screen.width),
                    2f * Camera.main.orthographicSize * (deltaPosition.y / Screen.height)
                );
            lastMouse = Input.mousePosition;

        }

        //Both
        if (deltaWorld != Vector2.zero)
        {
            deltaWorld *= panSpeed;
            _cumulatedMagnitude += deltaWorld.magnitude;
            //Gamestate.instance.addState((int)Gamestates.ZOOMPANNING);

            if (inverse)
                deltaWorld = -deltaWorld;

            if (yUp)
            {
                Debug.Log(deltaWorld);
                centerOnPosition(new Vector2(
                    transform.position.x + deltaWorld.x,
                    transform.position.z + deltaWorld.y
                    ));
            }
            else
            {
                centerOnPosition(new Vector2(
                    transform.position.x + deltaWorld.x,
                    transform.position.y + deltaWorld.y
                    ));
            }
        }
        else
        {
            //Gamestate.instance.removeState((int)Gamestates.ZOOMPANNING);
        }

        if (!_blocked && _cumulatedMagnitude > 1f)
        {
            _blocked = true;
        }

        if (_blocked && Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            _cumulatedMagnitude = 0f;
            _blocked = false;
        }
    }

    public void centerOnPosition(Vector2 newPosition)
    {
        //if (containRect.size != Vector2.zero)
        //{
        //    newPosition = new Vector2(
        //        Mathf.Max(Mathf.Min(containRect.xMax, newPosition.x), containRect.xMin),
        //        Mathf.Max(Mathf.Min(containRect.yMax, newPosition.y), containRect.yMin)
        //        );
        //}
        Debug.Log(newPosition);

        if (yUp)
        {
            this.gameObject.transform.position = new Vector3(newPosition.x, this.gameObject.transform.position.y, newPosition.y);
        }
        else
        {
            this.gameObject.transform.position = new Vector3(newPosition.x, newPosition.y, this.gameObject.transform.position.z);
        }


    }
}
