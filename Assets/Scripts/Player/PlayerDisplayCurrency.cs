using Herman;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayCurrency : MonoBehaviour
{
    public Currency currency;
    public Text valueText;
    public Text deltaText;
    public int deltaTextTimeout = 5;
    public Color positiveTextColor;
    public Color neutralTextColor;
    public Color negativeTextColor;
    private int currentValue;
    private PolyPlayer model;

    public IEnumerator Start()
    {
        this.deltaText.enabled = false;
        this.valueText.text = "0";
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        while (MainGameManager.Instance.localPlayer == null)
        {
            yield return null;
        }
        model = MainGameManager.Instance.localPlayer;
        onModelChanged();
    }

    public void onModelChanged()
    {
        this.model.PlayerStateChanged.AddListener(this.onPlayerStateChanged);
        this.onPlayerStateChanged();
        if (this.deltaText != null)
        {
            this.deltaText.enabled = false;
        }
    }

    public void onModelRemoved()
    {
        this.model.PlayerStateChanged.RemoveListener(this.onPlayerStateChanged);
    }

    private void onPlayerStateChanged()
    {
        int newValue = 0;
        this.model.Pocket.TryGetBalance(this.currency, out newValue);
        Debug.Log("Balance" + newValue);

        int delta = newValue - this.currentValue;
        this.currentValue = newValue;

        // Display the actual value of the pocket at this time.
        if (this.valueText != null)
        {
            if (this.currentValue >= 0)
            {
                this.valueText.color = this.neutralTextColor;
            }
            else
            {
                this.valueText.color = this.negativeTextColor;
            }
            this.valueText.text = String.Format("{0:D}", this.currentValue);
        }

        // Display the delta value.
        if (this.deltaText != null)
        {
            if (delta != 0)
            {
                if (delta > 0)
                {
                    this.deltaText.color = this.positiveTextColor;
                }
                else
                {
                    this.deltaText.color = this.negativeTextColor;
                }
                this.deltaText.text = String.Format("{0:D}", delta);
                StartCoroutine(this.showDeltaText());
            }
        }
    }

    private IEnumerator showDeltaText()
    {
        if (this.deltaText != null)
        {
            this.deltaText.enabled = true;
            yield return new WaitForSeconds(this.deltaTextTimeout);
            this.deltaText.enabled = false;
        }
    }
}
