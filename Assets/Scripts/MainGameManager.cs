using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;


namespace Herman
{
    public class MainGameManager : MonoBehaviourPunCallbacks
    {
        public static MainGameManager Instance = null;
        
        public Transform[] SpawnPoints;

        #region UNITY
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            SpawnPlayer();
        }
        #endregion

        #region COROUTTINES
        private IEnumerable Spawn()
        {
            while (true)
            {

            }
        }
        #endregion


        void SpawnPlayer()
        {
            
        }
    }

}

