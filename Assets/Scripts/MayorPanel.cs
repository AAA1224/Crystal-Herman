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
        public bool Visible = false;
     
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
            Debug.Log("Show Menu");
            UpdatePanel();
            PanelAnimator.enabled = true;
            PanelAnimator.Play("Open_Menu");
            yield return new WaitForSeconds(1f);
            HideButton.interactable = true;
            Visible = true;
        }

        IEnumerator HideMenu()
        {
            Debug.Log("Hide Menu");
            PanelAnimator.Play("Close_Menu");
            yield return new WaitForSeconds(1f);
            HideButton.interactable = true;
            Visible = false;
        }
        #endregion
        public void InteractMenu()
        {
            if (Visible)
            {
                //HideButton.interactable = false;
                StartCoroutine(HideMenu());
            }
            else
            {
                //HideButton.interactable = false;
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

            int j = 0;
            //Turn on a panel for each player that is not the mayor
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties.TryGetValue("IsMayor", out object isMayorObj))
                {
                    bool isMayor = (bool)isMayorObj;
                    if (!isMayor)
                    {
                        panelList[j].SetActive(true);
                        panelList[j].transform.Find("Player_Name_Text").GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                        j++;
                    }
                }
                else
                {
                    panelList[j].SetActive(true);
                    panelList[j].transform.Find("Player_Name_Text").GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
                    j++;
                }
            }
        }
    }

}
