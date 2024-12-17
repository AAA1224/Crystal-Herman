using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    public class OfferDisplay : VCBehaviour<Offer>
    {
        [Header("Tags")]
        public Image image;

        [Header("Buying Benefit and Cost")]
        public Text buyingBalanceText;
        public Image buyingEntityTypeIcon;
        public Image buyingCurrencyIcon;

        [Header("Selling Benefit and Cost")]
        public Text sellingBalanceText;
        public Image sellingEntityTypeIcon;
        public Image sellingCurrencyIcon;

        [Header("Other")]
        public Button buyButton;
        public Button removeButton;
        public MarketplaceSet marketPlaceSet;
        public OfferSetMarketPairSet offerMarketPairSet;
        public UiResource uiResource;
        public string FiatCurrencySymbolId = "fiatCurrencyLetter";
        public string QCurrencySymbolId = "qCurrencyLetter";
        public string ForFreeTextId = "forFreeText";
        public string applyOfferFailureTextId = "applyOfferFailureMoney";

        public override void onModelChanged()
        {
            //setup child views of benefits and costs
            if (this.buyingBalanceText != null)
            {
                CurrencyValue balance = this.model.BuyingBalance;
                string textId = balance.GetCurrency() == Currency.FIAT ? this.FiatCurrencySymbolId : this.QCurrencySymbolId;
                if (balance.value != 0)
                {
                    this.buyingBalanceText.text = String.Format("{0} {1}", balance.value, Localisation.instance.getLocalisedText(textId));
                }
                else
                {
                    this.buyingBalanceText.text = Localisation.instance.getLocalisedText(this.ForFreeTextId);
                }
            }

            if (this.buyingEntityTypeIcon != null)
            {
                this.buyingEntityTypeIcon.color = this.uiResource.GetColorByEntityType(this.model.BuyingEntityType);
            }

            if (this.buyingCurrencyIcon != null)
            {
                this.buyingCurrencyIcon.color = this.uiResource.GetColorByBalance(this.model.BuyingBalance);
            }

            if (this.sellingBalanceText != null)
            {
                CurrencyValue balance = this.model.SellingBalance;
                string textId = balance.GetCurrency() == Currency.FIAT ? this.FiatCurrencySymbolId : this.QCurrencySymbolId;
                if (balance.value != 0)
                {
                    this.sellingBalanceText.text = String.Format("{0} {1}", balance.value, Localisation.instance.getLocalisedText(textId));
                }
                else
                {
                    this.sellingBalanceText.text = Localisation.instance.getLocalisedText(this.ForFreeTextId);
                }
            }

            if (this.sellingEntityTypeIcon != null)
            {
                this.sellingEntityTypeIcon.color = this.uiResource.GetColorByEntityType(this.model.SellingEntityType);
            }

            if (this.sellingCurrencyIcon != null)
            {
                this.sellingCurrencyIcon.color = this.uiResource.GetColorByBalance(this.model.SellingBalance);
            }

            //set sprite if the offer gives any talent benefits
            if (image != null && uiResource != null)
            {
                foreach (TagSpritePair tsp in this.uiResource.tagSprites)
                {
                    if (model.tags.Contains(tsp.tag))
                    {
                        image.sprite = tsp.sprite;
                        break;
                    }
                }
            }

            // Find both the associated market and seller for our offer.
            Marketplace associatedMarket = null;
            PolyPlayer seller = null;
            if (marketPlaceSet != null)
            {
                foreach (Marketplace mp in marketPlaceSet.marketplaces)
                {
                    foreach (Offer o in mp.offers)
                    {
                        if (o.guid.Equals(this.model.guid))
                        {
                            associatedMarket = mp;
                            seller = mp.seller;
                        }
                    }
                }
            }

            if (seller != null)
            {
                // Auxiliary variables.
                PolyPlayer player = MainGameManager.Instance.localPlayer;
                bool a = (player == seller);
                bool b = player.Mayor;
                bool c = model.visibleToMayor;

                // Obtain a reference to the button of said UI object and register
                // a delegate that applies the offer's buying cost and benefit when
                // the player clicks on the button.  The conditional logic assign
                // special behaviour to the mayor and to offers visible only to the
                // mayor; such offers may only be bought by the mayor whilst the
                // mayor may not buy other offers. Furthermore players may not buy
                // their own offers.
                if (buyButton != null)
                {
                    Debug.Log("buy button found");
                    if (!a && (!b || c) && (b || !c))
                    {
                        string guid = model.guid.ToString();
                        buyButton.onClick.RemoveAllListeners();
                        buyButton.onClick.AddListener(() => {
                            Debug.Log("buy button click");
                            // Determine, whether the offer gives players new marketplaces.
                            bool specialOffer = this.offerMarketPairSet.offerMarketPairs.Any(p => p.offer.Equals(model));
                            if (specialOffer)
                            {
                                if (player.OwnedMarketplaces.Count == 0)
                                {
                                    RootLogger.Info(this, "Applying an offer that gives players a marketplace.");
                                    //player.ClientApplyOffer(guid);
                                    MainGameManager.Instance.applyOffer(player, seller, this.model);
                                    //AudioController.Play("buy_stuff");
                                }
                                else
                                {
                                    RootLogger.Info(this, "The player cannot buy another special building.");
                                    KoboldTools.Alert.info("buildingDeedDeniedContent", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "btnClose" });
                                }
                            }
                            else
                            {
                                // If buying the offer would cost more than the player had money available (minus debt allowance), do not apply the offer.
                                CurrencyValue buyingBalance = this.model.BuyingBalance;
                                Currency buyingCurrency = buyingBalance.GetCurrency();
                                int pocketValue;
                                player.Pocket.TryGetBalance(buyingCurrency, out pocketValue);
                                CurrencyValue pocketBalance = new CurrencyValue(buyingCurrency, pocketValue);
                                CurrencyValue maxDebt = MainGameManager.Instance.MaximumDebt.SingleOrDefault(e => e.GetCurrency() == buyingCurrency);

                                if (buyingBalance >= 0 || pocketBalance + maxDebt + buyingBalance >= 0)
                                {
                                    RootLogger.Info(this, "Applying a regular offer.");
                                    //player.ClientApplyOffer(guid);
                                    MainGameManager.Instance.applyOffer(player, seller, this.model);
                                    //AudioController.Play("buy_stuff");
                                }
                                else
                                {
                                    RootLogger.Warning(this, "Cannot apply offer due to insufficient money");
                                    KoboldTools.Alert.info(this.applyOfferFailureTextId, new KoboldTools.Alert.AlertParams { closeText = "btnClose", useLocalization = true });
                                }
                            }
                        });
                        buyButton.interactable = true;
                        RootLogger.Info(this, "Buy button Enabled");
                    }
                    else
                    {
                        buyButton.interactable = false;
                        RootLogger.Info(this, "Buy button Disabled");
                    }
                }

                // Players may remove an offer if they've created it.
                if (removeButton != null)
                {
                    if (a)
                    {
                        string guid = model.guid.ToString();
                        removeButton.onClick.RemoveAllListeners();
                        removeButton.onClick.AddListener(() => {
                            // Determine whether the offer resides on a special marketplace.
                            bool specialOffer = this.offerMarketPairSet.offerMarketPairs.Any(p => p.marketplace.Equals(associatedMarket) || p.offer.Equals(model));
                            if (specialOffer)
                            {
                                RootLogger.Info(this, "Offers on special marketplaces cannot be removed.");
                                KoboldTools.Alert.info("removeOfferDeniedContent", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "btnClose" });
                            }
                            else
                            {
                                Guid oGuid = new Guid(guid);
                                foreach (Marketplace market in player._marketplaceDb.marketplaces)
                                {
                                    foreach (Offer offer in market.offers)
                                    {
                                        if (offer.guid == oGuid)
                                        {
                                            player.Pocket.TimeAllowance += offer.creationCost.Time;
                                            Debug.Log("Adding " + offer.creationCost.Time + " time to player. " + player.Pocket.TimeAllowance + " total.");
                                            break;
                                        }
                                    }
                                }
                                RootLogger.Info(this, "Removing a regular offer.");
                                //player.ClientRemoveOffer(guid);
                                MainGameManager.Instance.removeOffer(player, this.model);
                            }
                        });

                        removeButton.interactable = true;
                        
                    }
                    else
                    {
                        removeButton.interactable = false;
                        
                    }
                }
            }
        }

        public override void onModelRemoved()
        {
            //do nothing
        }
    }
}
