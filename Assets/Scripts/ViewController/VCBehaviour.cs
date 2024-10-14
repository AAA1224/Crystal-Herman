using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoboldTools {

    /// <summary>
    /// decorator for a viewcontroller to be used as a unity component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VCBehaviour<T> : MonoBehaviour, IViewController where T : class {

        private VC<T> vc;

        public T model
        {
            get { return vc.model; }
        }

        public void Start() {
            if(vc == null)
                vc = new VC<T>(this);

            vc.onModelChanged.AddListener(onModelChanged);
            vc.onModelRemoved.AddListener(onModelRemoved);

            //initialize
            if(vc.model != null)
            {
                onModelChanged();
            }
        }

        private void OnDestroy()
        {
            if (vc != null)
            {
                vc.onRemoveModel();
                vc.onModelChanged.RemoveListener(onModelChanged);
                vc.onModelRemoved.RemoveListener(onModelRemoved);
            }
        }

        public void onRemoveModel(object modelTypeToRemove)
        {
            vc.onRemoveModel(modelTypeToRemove);
        }

        public void onSetModel(object newModel)
        {
            if (vc == null)
                vc = new VC<T>(this);

            vc.onSetModel(newModel);
        }

        public virtual void onModelChanged() { }
        public virtual void onModelRemoved() { }
    }
}
