using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using KoboldTools.Logging;

namespace KoboldTools
{
    public class Alert : VCBehaviour<IPanel>
    {
        public Image background;
        public Text textfield;
        public Text titlefield;
        public Button closeButton;
        public Button genericButton;
        public Image spriteImage;

        public AlertSkin normalSkin;
        public AlertSkin shoutSkin;

        private Pool<Button> genericButtonPool;
        private Vector2 defaultSize = Vector2.zero;
        private static string currentTutorialText;
        private static AlertParams currentTutorialParams;

        public struct AlertParams
        {
            public string title;
            public string closeText;
            public Sprite sprite;
            public AlertCallback[] callbacks;
            public bool useLocalization;
            public bool hideCloseButton;
            public Vector2 size;
        }

        public struct AlertCallback
        {
            public string buttonText;
            public Action callback;
            public bool mainButton;
        }

        private static Alert instance;

        public override void onModelChanged()
        {
            instance = this;
            defaultSize = background.rectTransform.sizeDelta;
            closeButton.onClick.AddListener(model.onClose);
            genericButtonPool = new Pool<Button>(genericButton);
        }

        public override void onModelRemoved()
        {
            closeButton.onClick.RemoveListener(model.onClose);
        }

        protected void show(string text, AlertParams config, AlertSkin skin)
        {
            //release the button pool for all generic buttons
            genericButtonPool.releaseAll();

            background.sprite = skin.background;
            background.color = skin.backgroundColor;

            //set the main text of the alert
            textfield.text = config.useLocalization ? Localisation.instance.getLocalisedText(text) : text;
            textfield.color = skin.textColor;

            //set size if specified
            if (config.size != Vector2.zero)
            {
                background.rectTransform.sizeDelta = config.size;
            }
            else
            {
                background.rectTransform.sizeDelta = defaultSize;
            }

            //set title if specified
            if (config.title != null)
            {
                titlefield.text = config.useLocalization ? Localisation.instance.getLocalisedText(config.title) : config.title;
                titlefield.gameObject.SetActive(true);
                textfield.color = skin.textColor;
            }
            else
            {
                titlefield.gameObject.SetActive(false);
            }

            //set sprite if specified
            if (config.sprite != null)
            {
                spriteImage.sprite = config.sprite;
                spriteImage.gameObject.SetActive(true);
            }
            else
            {
                spriteImage.gameObject.SetActive(false);
            }

            bool mainButtonSet = false;

            //set generic buttons if specified
            if(config.callbacks != null)
            {
                foreach (AlertCallback callback in config.callbacks)
                {
                    Button callbackButton = genericButtonPool.pop();
                    callbackButton.gameObject.GetComponentInChildren<Text>().text = config.useLocalization ? Localisation.instance.getLocalisedText( callback.buttonText ) : callback.buttonText;
                    callbackButton.gameObject.GetComponentInChildren<Text>().color = callback.mainButton ? skin.mainButtonTextColor : skin.buttonTextColor;
                    callbackButton.onClick.RemoveAllListeners();
                    callbackButton.onClick.AddListener(()=> { callback.callback.Invoke(); });
                    callbackButton.image.sprite = callback.mainButton ? skin.mainButtonSprite : skin.buttonSprite;
                    if (callback.mainButton)
                    {
                        mainButtonSet = true;
                    }
                    callbackButton.gameObject.SetActive(true);
                }
            }

            //set close button
            if (!config.hideCloseButton)
            {
                if (!String.IsNullOrEmpty(config.closeText))
                {
                    closeButton.gameObject.SetActive(true);
                    closeButton.GetComponentInChildren<Text>().text = config.useLocalization ? Localisation.instance.getLocalisedText(config.closeText) : config.closeText;
                    closeButton.image.sprite = mainButtonSet ? skin.buttonSprite : skin.mainButtonSprite;
                    closeButton.GetComponentInChildren<Text>().color = mainButtonSet ? skin.buttonTextColor : skin.mainButtonTextColor;
                }
                else
                {
                    RootLogger.Exception(this, "The close button is not hidden, but not text is given");
                }
            }
            else
            {
                closeButton.gameObject.SetActive(false);
            }

            model.onOpen();
        }

        public static bool open
        {
            get { return instance.model.isOpen || instance.model.isTransitioning; }
        }
        /// <summary>
        /// Initialize alert panel with default config
        /// </summary>
        /// <param name="text">The text to display in the alert panel</param>
        public static void info(string text)
        {
            instance.show(text, new AlertParams() { closeText = "OK" }, instance.normalSkin);
        }
        /// <summary>
        /// Initialize alert panel with custom config
        /// </summary>
        /// <param name="text">The text to display in the alert panel</param>
        /// <param name="config">The <see cref="AlertParams"/> for displaying</param>
        public static void info(string text, AlertParams config, AlertSkin overwriteSkin = null)
        {
            instance.show(text, config, overwriteSkin == null ? instance.normalSkin : overwriteSkin);
        }

        public static void shout(string text)
        {
            instance.show(text, new AlertParams() { closeText = "OK" }, instance.shoutSkin);
        }

        public static void shout(string text, AlertParams config)
        {
            instance.show(text, config, instance.shoutSkin);
        }

        public static void tutorial()
        {
            if (!String.IsNullOrEmpty(currentTutorialText))
            {
                instance.show(currentTutorialText, currentTutorialParams, instance.normalSkin);
            }
        }

        public static void tutorial(string text)
        {
            currentTutorialText = text;
            currentTutorialParams = new AlertParams() { closeText = "OK" };
            instance.show(currentTutorialText, currentTutorialParams, instance.normalSkin);
        }

        public static void tutorial(string text, AlertParams config)
        {
            currentTutorialText = text;
            currentTutorialParams = config;
            instance.show(currentTutorialText, currentTutorialParams, instance.normalSkin);
        }

        public static void tutorial(string text, AlertParams config, AlertSkin overwriteSkin)
        {
            currentTutorialText = text;
            currentTutorialParams = config;
            instance.show(currentTutorialText, currentTutorialParams, overwriteSkin == null ? instance.normalSkin : overwriteSkin);
        }

        public static void close()
        {
            if (Alert.open)
            {
                Alert.instance.model.onClose();
            }
        }
    }
}
