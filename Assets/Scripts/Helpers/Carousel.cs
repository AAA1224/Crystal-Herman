using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KoboldTools.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools {
    public abstract class Carousel<I> : VCBehaviour<List<I>> where I : class {
        public Panel panelTemplate;
        public GameObject emptyCarousel;
        public Button nextButton;
        public Button previousButton;

        private Pool<Panel> panelPool;
        private int currentPanel = 0;
        private Panel[] currentPanels;

        private void Awake() {
            panelPool = new Pool<Panel>(panelTemplate);
        }

        public override void onModelChanged() {
            RootLogger.Debug(this, "Adding the model to the carousel");
            nextButton.onClick.AddListener(next);
            previousButton.onClick.AddListener(previous);
            initialize();
        }

        public override void onModelRemoved() {
            RootLogger.Debug(this, "Removing the model from the carousel");
            nextButton.onClick.RemoveListener(next);
            previousButton.onClick.RemoveListener(previous);

            foreach (Panel p in currentPanels) {
                p.onClose();
            }
        }

        public void initialize() {
            RootLogger.Debug(this, "Initialising the carousel");
            //release pool
            panelPool.releaseAll();

            //set current
            currentPanel = 0;

            //set current panel array
            currentPanels = new Panel[model.Count];

            //setup panels
            for (int i = 0; i < model.Count; i++) {
                //get from pool
                Panel newPanel = panelPool.pop();
                currentPanels[i] = newPanel;
                newPanel.gameObject.SetActive(true);
                VC<I>.addModelToAllControllers(model[i], newPanel.gameObject);
                newPanel.canvasGroup.alpha = 0.0f;
                newPanel.canvasGroup.interactable = true;
                newPanel.canvasGroup.blocksRaycasts = true;

                //set visibility
                if (i != currentPanel) {
                    newPanel.onClose();
                } else {
                    newPanel.onOpen();
                }
            }

            this.emptyCarousel.SetActive(model.Count == 0);
            RootLogger.Debug(this, "Carousel: {0} models, {1} panels, {2} panels open, {3} panels closed, {4} transitioning panels", model.Count, currentPanels.Length, currentPanels.Count(p => p.isOpen), currentPanels.Count(p => !p.isOpen), currentPanels.Count(p => p.isTransitioning));
        }

        private void next() {
            if (currentPanels.Length == 0) {
                return;
            }

            RootLogger.Debug(this, "Next panel in carousel requested");
            int nextPanel = (currentPanel + 1) % currentPanels.Length;
            currentPanels[currentPanel].onClose();
            currentPanels[nextPanel].onOpen();
            currentPanel = nextPanel;
        }

        private void previous() {
            if (currentPanels.Length == 0) {
                return;
            }

            RootLogger.Debug(this, "Previous panel in carousel requested");
            int previousPanel = currentPanel == 0 ? (currentPanels.Length - 1) : (currentPanel - 1);
            currentPanels[currentPanel].onClose();
            currentPanels[previousPanel].onOpen();
            currentPanel = previousPanel;
        }
    }
}