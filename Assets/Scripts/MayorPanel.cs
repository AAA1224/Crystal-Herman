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
    public class MayorPanel : MonoBehaviourPunCallbacks
    {
        public Button HideButton;
        public Animator PanelAnimator;
        public GameObject MayorPanelObject;
     
        public List<GameObject> panelList = new List<GameObject>();

        #region UNITY
        // Start is called before the first frame update
        void Start()
        {
            HideButton.onClick.AddListener(InteractMenu);
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
        #region COROUTTINES

        IEnumerator ShowMenu()
        {
            //Update the Panel
            UpdatePanel();
            PanelAnimator.enabled = true;
            PanelAnimator.Play("Open_Menu");
            yield return new WaitForSeconds(1f);
            HideButton.interactable = true;
        }

        IEnumerator HideMenu()
        {
            PanelAnimator.Play("Close_Menu");
            yield return new WaitForSeconds(1f);
            HideButton.interactable = true;
        }
        #endregion
        public void InteractMenu()
        {
            if (MayorPanelObject.activeInHierarchy)
            {
                HideButton.interactable = false;
                StartCoroutine(HideMenu());
            }
            else
            {
                HideButton.interactable = false;
                StartCoroutine(ShowMenu());
            }
        }
        protected void UpdatePanel()
        {
            PanelAnimator.enabled = false;
            MayorPanelObject.SetActive(true);
            //Turn off all panels
            for (int i = 0; i < panelList.Count; i++)
            {
                panelList[i].SetActive(false);
            }

            //Turn on a panel for each player that is not the mayor
            //for (int i = 0; i < Level.instance.allPlayers.Count; i++)
            //{
            //    if (!Level.instance.allPlayers[i].Mayor)
            //    {
            //        panelList[i].SetActive(true);
            //    }
            //}
        }
    }

}
