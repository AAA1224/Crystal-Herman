using Herman;
using KoboldTools;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerDisplayIncident : MonoBehaviour
{
    [Serializable]
    public class Tags
    {
        public List<string> tags;
    }

    [Header("Filter Settings")]
    public string type;
    public List<Tags> filterTags;

    [Header("Dependencies")]
    public Button IncidentButton;
    public UiResource Resource;
    public string CloseButtonTextId = "incidentPopUpCloseButton";
    public string PayButtonTextId = "incidentPopUpPayButton";
    public string ReceiveButtonTextId = "incidentPopUpReceiveButton";
    public string ResolveButtonTextId = "incidentPopUpResolveButton";
    public string FiatCurrencySymbolId = "fiatCurrencyLetter";
    public string QCurrencySymbolId = "qCurrencyLetter";

    private Player LocalPlayer;
    private Incident CurrentIncident;

    public IEnumerator Start()
    {
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        while (MainGameManager.Instance.localPlayer == null)
        {
            yield return null;
        }

        this.onAuthoritativePlayerChanged();

        if (this.IncidentButton != null)
        {
            this.IncidentButton.onClick.AddListener(this.onClickIncidentSymbol);
        }
    }

    private void onAuthoritativePlayerChanged()
    {
        MainGameManager.Instance.localPlayer.PlayerStateChanged.AddListener(this.onPlayerStateChanged);
        this.onPlayerStateChanged();
    }

    private void onPlayerStateChanged()
    {
        if (this.IncidentButton != null)
        {
            Incident incident = MainGameManager.Instance.localPlayer.Incidents.Find(e => e.State == IncidentState.UNTOUCHED && ((e.Type == this.type) || (this.filterTags.Any(f => e.EquivalentTags(f.tags)))));
            if (incident != null)
            {
                this.IncidentButton.gameObject.SetActive(true);
                // VC<Incident>.addModelToAllControllers(incident, this.IncidentButton.gameObject, true);
                this.IncidentButton.gameObject.GetComponent<IncidentUI>().model = incident;
                this.CurrentIncident = incident;
            }
            else
            {
                this.IncidentButton.gameObject.SetActive(false);
            }
        }
    }

    private void onClickIncidentSymbol()
    {
         Talent matchingTalent = MainGameManager.Instance.localPlayer.Talents.FirstOrDefault(e => e.EquivalentTags(this.CurrentIncident.Tags));
        CurrencyValue balance = this.CurrentIncident.ApplicationBalance;

        string textId = balance.GetCurrency() == Currency.FIAT ? this.FiatCurrencySymbolId : this.QCurrencySymbolId;
        string description = String.IsNullOrEmpty(this.CurrentIncident.LocalisedDescription) ? String.Empty : String.Format("{0}\n", this.CurrentIncident.LocalisedDescription);
        string content = String.Format("{0}{1} {2}", description, balance.value, Localisation.instance.getLocalisedText(textId));
        string title = this.CurrentIncident.LocalisedTitle;
        string closeText = Localisation.instance.getLocalisedText(this.CloseButtonTextId);
        string applyText = balance.value >= 0 ? Localisation.instance.getLocalisedText(this.ReceiveButtonTextId) : Localisation.instance.getLocalisedText(this.PayButtonTextId);
        string buttonSound = balance.value >= 0 ? "receive_stuff" : "buy_stuff";
        string resolveText = Localisation.instance.getLocalisedText(this.ResolveButtonTextId);
        string resolveButtonSound = "resolve_things";

        // Always provide a button that applies the incident.
        List<KoboldTools.Alert.AlertCallback> callbacks = new List<KoboldTools.Alert.AlertCallback> {
                    new KoboldTools.Alert.AlertCallback {
                        buttonText = applyText,
                        callback = () => {
                            KoboldTools.Logging.RootLogger.Info(this, "Applying the incident {0} to player {1}", this.CurrentIncident, this.LocalPlayer);
                            if(MainGameManager.Instance.localPlayer.Mayor)
                            {
                                int cost = 0;
                                CurrentIncident.ApplicationCost.TryGetExpenses(Currency.FIAT, out cost);
                                print("Mayor Cost: " + cost);
                                int mayorMoney = 0;
                                MainGameManager.Instance.localPlayer.Pocket.TryGetBalance(Currency.FIAT, out mayorMoney);
                                print("Mayor Money: " + mayorMoney);
                                if (mayorMoney < cost)
                                {
                                    Vector2 alertSize = new Vector2(800, 600);
                                    KoboldTools.Alert.info("noMayorDebt", new KoboldTools.Alert.AlertParams { useLocalization = true, closeText = "btnOk", size = alertSize });
                                }
                                else
                                {
                                    MainGameManager.Instance.applyIncident(this.CurrentIncident, false);
                                    this.CurrentIncident = null;
                                    this.IncidentButton.gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                MainGameManager.Instance.applyIncident(this.CurrentIncident, false);
                                this.CurrentIncident = null;
                                this.IncidentButton.gameObject.SetActive(false);
                            }

                            KoboldTools.Alert.close();
                        },
                        mainButton = (matchingTalent == null),
                    },
            };

        // If the player has a matching talent, also provide a resolve button.
        if (matchingTalent != null)
        {
            callbacks.Add(new KoboldTools.Alert.AlertCallback
            {
                buttonText = resolveText,
                callback = () => {
                    KoboldTools.Logging.RootLogger.Info(this, "Resolving the incident {0} for player {1} with talent {2}", this.CurrentIncident, this.LocalPlayer, matchingTalent);
                    //this.LocalPlayer.ClientResolveIncident(this.CurrentIncident);
                    this.CurrentIncident = null;
                    // AudioController.Play(resolveButtonSound);
                    KoboldTools.Alert.close();
                },
                mainButton = true,
            });
        }

        // Show the alert.
        KoboldTools.Alert.info(content, new KoboldTools.Alert.AlertParams
        {
            title = title,
            closeText = closeText,
            sprite = this.Resource.GetSpriteByTags(this.CurrentIncident.Tags),
            callbacks = callbacks.ToArray(),
        });
    }
}
