using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public TMP_Text title;
    public Image alertImage;
    public TMP_Text text;
    public Button button, close;
    public static Alert instance;
    public static bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        instance = this;
        button.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick()
    {
        open = false;
        this.gameObject.SetActive(false);
    }

    public static void show(bool useLocalization, string alertTitle, string content, Sprite icon, string button1, string button2 = null)
    {
        Alert alert = instance;
        if (useLocalization)
        {
            if (alertTitle == null)
            {
                alert.title.gameObject.SetActive(false);
            }
            else
            {
                alert.title.gameObject.SetActive(true);
                alert.title.text = Localisation.instance.getLocalisedText(alertTitle);
            }
            alert.text.text = Localisation.instance.getLocalisedText(content);
            if (icon == null)
            {
                alert.alertImage.gameObject.SetActive(false);
            }
            else
            {
                alert.alertImage.sprite = icon;
                alert.alertImage.gameObject.SetActive(true);
            }
            alert.button.gameObject.GetComponentInChildren<TMP_Text>().text = Localisation.instance.getLocalisedText(button1);
            if (button2 == null)
            {
                alert.close.gameObject.SetActive(false);
            }
            else
            {
                alert.close.gameObject.SetActive(true);
                alert.close.gameObject.GetComponentInChildren<TMP_Text>().text = Localisation.instance.getLocalisedText(button2);
            }
        }
        else
        {
            if (alertTitle == null)
            {
                alert.title.gameObject.SetActive(false);
            }
            else
            {
                alert.title.gameObject.SetActive(true);
                alert.title.text = alertTitle;
            }
            alert.text.text = content;
            if (icon == null)
            {
                alert.alertImage.gameObject.SetActive(false);
            }
            else
            {
                alert.alertImage.sprite = icon;
                alert.alertImage.gameObject.SetActive(true);
            }
            alert.button.gameObject.GetComponentInChildren<TMP_Text>().text = button1;
            if (button2 == null)
            {
                alert.close.gameObject.SetActive(false);
            }
            else
            {
                alert.close.gameObject.SetActive(true);
                alert.close.gameObject.GetComponentInChildren<TMP_Text>().text = button2;
            }
        }
        alert.gameObject.SetActive(true);
        open = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
