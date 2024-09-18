using UnityEngine;
public class Billboard : MonoBehaviour
{
    public Camera _camera;
    private RectTransform _rt;

    public void Awake()
    {
        if (this._camera == null)
        {
            this._camera = Camera.main;
        }
        this._rt = GetComponent<RectTransform>();
    }
    public void Update()
    {
        this._rt.LookAt(this._camera.transform);
    }
}