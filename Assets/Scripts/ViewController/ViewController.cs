using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    [System.Obsolete("Please Use VCBehaviour<T> instead", false)]
    /// <summary>
    /// A viewcontroller compoment that can be attached to any gameobject. At Start() it looks for a model component of his type on the same gameobjects and sets the new model. 
    /// </summary>
    /// <typeparam name="T">Type of the model for this viewcontroller</typeparam>
    public abstract class ViewController<T> : MonoBehaviour, IViewController where T: class
    {
        /// <summary>
        /// initializing function of a monobehaviour
        /// </summary>
        public void Start()
        {
            Object foundModelComponent = null;
            try
            {
                foundModelComponent = GetComponent<T>() as Object;
            }
            catch
            {

            }
            if (foundModelComponent != null)
            {
                onSetModel(foundModelComponent);
            }
            else
            {
                onInitializedWithNoModel();
            }
        }

        private T _model = default(T);
        /// <summary>
        /// model of this viewcontroller
        /// </summary>
        public T model
        {
            get
            {
                return _model;
            }
        }


        /// <summary>
        /// method called to set a new model
        /// </summary>
        /// <param name="newModel">new model of the viewcontroller</param>
        public void onSetModel(System.Object newModel)
        {
            if (!(newModel is T))
                return;

            if (_model != null)
                onRemoveModel();

            _model = (T)newModel;
            onModelChanged();
        }

        public void onRemoveModel(System.Object modelTypeToRemove)
        {
            if (modelTypeToRemove is T || modelTypeToRemove == typeof(T))
            {
                onRemoveModel();
            }
        }

        public void onRemoveModel()
        {
            if (_model != null)
            {
                onModelRemoved();
                _model = default(T);
            }

        }

        public virtual void onModelChanged() { }
        public virtual void onModelRemoved() { }
        public virtual void onInitializedWithNoModel() { }

        //statics for backwards compatibility
        public static void addModelToAllControllers(T model, GameObject go, bool includeChildren = false)
        {
            VC<T>.addModelToAllControllers(model, go, includeChildren);
        }
        public static void removeModelFromAllControllers(T model, GameObject go, bool includeChildren = false)
        {
            VC<T>.removeModelFromAllControllers(model, go, includeChildren);
        }

    }
}
