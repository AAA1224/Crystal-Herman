using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoboldTools;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Herman
{

    public class OfferEditorDisplay : VCBehaviour<Offer>
    {
        public Text baseTalent;
        public InputField titleInput;
        public InputField descriptionInput;
        public Button applyButton;
        public Button cancelButton;
        public Button newPriceButton;
        public Transform priceTemplate;
        public Transform offerTitle;
        public Button newOfferButton;
        public Transform offerTemplate;
        public UiResource uiResource;
        public Image offerImage;
        public string tradeOfferBaseTalentTextId = "createTradeOfferSubtitle";

        private LinkedPool<Transform> prices;
        private LinkedPool<Transform> offers;
        private Dictionary<string, string> currencyNames = new Dictionary<string, string>();
        //+ buying cost -> selling benefit = price
        //+ buying benefit -> selling cost = offer
        // -> currencyvalue display for editor

        public override void onModelChanged()
        {
            //set currency names
            foreach(string key in Enum.GetNames(typeof(Currency)).Where(n => MainGameManager.Instance.PolymoneyIntroduced || n != Currency.Q.ToString()).ToList())
            {
                string name = Localisation.instance.getLocalisedText(string.Format("{0}Name", key));
                if (!currencyNames.ContainsKey(name))
                {
                    currencyNames.Add(name,key );
                }
                else
                {
                    currencyNames[name] = key;
                }
            }

            titleInput.text = model.Title;
            descriptionInput.text = model.Description;

            Dropdown priceDropdown = priceTemplate.GetComponentInChildren<Dropdown>();
            Dropdown offerDropdown = offerTemplate.GetComponentInChildren<Dropdown>();

            priceDropdown.ClearOptions();
            offerDropdown.ClearOptions();

            priceDropdown.AddOptions(currencyNames.Keys.ToList());
            offerDropdown.AddOptions(currencyNames.Keys.ToList());

            if (prices == null)
            {
                prices = new LinkedPool<Transform>(priceTemplate);
            }
            else
            {
                prices.releaseAll();
            }
            if (offers == null)
            {
                offers = new LinkedPool<Transform>(offerTemplate);
            }
            else
            {
                offers.releaseAll();
            }

            //applyButton.onClick.AddListener(applyValues);
            newOfferButton.onClick.AddListener(newOffer);
            newPriceButton.onClick.AddListener(newPrice);

            //always create a default price
            newPrice();

            //set sprite if the offer gives any talent benefits
            if (offerImage != null && uiResource != null)
            {
                foreach (TagSpritePair tsp in this.uiResource.tagSprites)
                {
                    if (model.tags.Contains(tsp.tag))
                    {
                        offerImage.sprite = tsp.sprite;
                        break;
                    }
                }
            }

            //trade specific display
            if (model.tags.Count > 0 && model.tags[0] == "Trade")
            {
                //this offer has no talent tags. make trade offer available:
                offerTitle.gameObject.SetActive(true);
                newOffer();
                baseTalent.text = Localisation.instance.getLocalisedText(this.tradeOfferBaseTalentTextId);
            }
            else
            {
                //this offer has talent tags. disable trade offer:
                offerTitle.gameObject.SetActive(false);
                baseTalent.text = String.Empty;
            }
        }

        public override void onModelRemoved()
        {
            //applyButton.onClick.RemoveListener(applyValues);
            newOfferButton.onClick.RemoveListener(newOffer);
            newPriceButton.onClick.RemoveListener(newPrice);
        }

        private void newPrice()
        {
            Transform priceTransform = prices.pop();

            Dropdown priceDropdown = priceTransform.GetComponentInChildren<Dropdown>();
            priceDropdown.ClearOptions();
            priceDropdown.AddOptions(currencyNames.Keys.ToList());

            Button deleteButton = priceTransform.GetComponentInChildren<Button>(true);
            deleteButton.onClick.AddListener(() => { deleteButton.onClick.RemoveAllListeners(); prices.releaseOne(priceTransform); LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)priceTemplate.parent); });
            priceTransform.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)priceTemplate.parent);
        }

        private void newOffer()
        {
            Transform offerTransform = offers.pop();

            Dropdown offerDropdown = offerTransform.GetComponentInChildren<Dropdown>();
            offerDropdown.ClearOptions();
            offerDropdown.AddOptions(currencyNames.Keys.ToList());

            Button deleteButton = offerTransform.GetComponentInChildren<Button>(true);
            deleteButton.onClick.AddListener(() => { deleteButton.onClick.RemoveAllListeners(); offers.releaseOne(offerTransform); LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)offerTemplate.parent); });
            offerTransform.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)offerTemplate.parent);
        }

        public void applyValues()
        {

            //set title and description
            model.Title = titleInput.text;
            model.Description = descriptionInput.text;

            //set price and offer
            model.buyingCost.Expenses = new List<CurrencyValue>();
            model.sellingBenefit.Income = new List<CurrencyValue>();

            foreach(Transform t in prices.getUsed())
            {
                Dropdown d = t.GetComponentInChildren<Dropdown>();
                string currency = currencyNames[d.options[d.value].text];
                int amount = t.GetComponentInChildren<InputField>().text == "" ? 0 : int.Parse(t.GetComponentInChildren<InputField>().text);

                model.buyingCost.Expenses.Add(new CurrencyValue(currency, amount));
                model.sellingBenefit.Income.Add(new CurrencyValue(currency, amount));
            }

            foreach(Transform t in offers.getUsed())
            {
                Dropdown d = t.GetComponentInChildren<Dropdown>();
                string currency = currencyNames[d.options[d.value].text];
                int amount = t.GetComponentInChildren<InputField>().text == "" ? 0 : int.Parse(t.GetComponentInChildren<InputField>().text);

                model.sellingCost.Expenses.Add(new CurrencyValue(currency, amount));
                model.buyingBenefit.Income.Add(new CurrencyValue(currency, amount));
            }
        }

    }
}
