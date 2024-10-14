using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace KoboldTools
{

    /// <summary>
    /// A viewcontroller object. At instantiation it looks for a model component of his type on the same component and holds the reference to the model.
    /// </summary>
    /// <typeparam name="T">Type of the model for this viewcontroller</typeparam>

    public class VC<T> : IViewController where T : class
    {
        /// <summary>
        /// initializing function of a monobehaviour
        /// </summary>
        public VC(Component component, bool searchModelInChilds = false)
        {
            Object foundModelComponent = null;
            try
            {
                foundModelComponent = searchModelInChilds ? component.GetComponentInChildren<T>() as Object : component.GetComponent<T>() as Object;
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
                //model is null on start
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
            onModelChanged.Invoke();
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
                onModelRemoved.Invoke();
                _model = default(T);
            }

        }

        public UnityEvent onModelChanged = new UnityEvent();
        public UnityEvent onModelRemoved = new UnityEvent();

        public static void addModelToAllControllers(T model, GameObject go, bool includeChildren = false)
        {
            if (!includeChildren)
            {
                foreach (IViewController vc in go.GetComponents<IViewController>())
                {
                    vc.onSetModel(model);
                }
            }
            else
            {
                foreach (IViewController vc in go.GetComponentsInChildren<IViewController>(true))
                {
                    vc.onSetModel(model);
                }
            }
        }
        public static void removeModelFromAllControllers(T model, GameObject go, bool includeChildren = false)
        {
            if (!includeChildren)
            {
                foreach (IViewController vc in go.GetComponents<IViewController>())
                {
                    vc.onRemoveModel(model);
                }
            }
            else
            {
                foreach (IViewController vc in go.GetComponentsInChildren<IViewController>())
                {
                    vc.onRemoveModel(model);
                }
            }
        }
    }

}
