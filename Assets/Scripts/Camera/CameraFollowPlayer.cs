using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine;
using Cinemachine;
using Photon.Realtime;
using static Cinemachine.DocumentationSortingAttribute;

namespace Herman
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        public bool follow = true;
        public bool lookAt = true;
        public CinemachineVirtualCamera _camera;
        public GameObject _player;

        public void Awake()
        {
            this._camera = GetComponent<CinemachineVirtualCamera>();
        }
        public void characterChanged()
        {
            if (this._player != null)
            {
                this._camera.Priority = 1000;
                // Tell the virtual camera to follow the authoritative player.
                if (this.follow)
                {
                    this._camera.m_Follow = this._player.transform;
                }

                // Tell the virtual camera to look at the authoritative player.
                if (this.lookAt)
                {
                    this._camera.m_LookAt = this._player.transform;
                }
            }
        }
        public void unfocus()
        {
            this._camera.Priority = 10;
        }
    }
}
