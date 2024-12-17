using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    public class MarketplaceEvent : UnityEvent<MarketplaceEvent> { }

    public interface IMarketplaceSet
    {
        IMarketplaceSetSyncProvider syncProvider { get; set; }
        List<Marketplace> marketplaces { get; }
        void addMarketplace(Marketplace marketplace);
        void removeMarketplace(Marketplace marketplace);
        Marketplace getByGuid(string guid);
    }

    public interface INetworkUnique
    {
        SerializableGuid guid { get; set; }
    }

    [CreateAssetMenu(fileName = "New Marketplace Runtime Set", menuName = "New Marketplace Runtime Set")]
    public class MarketplaceSet : ScriptableObject, IMarketplaceSet
    {
#if UNITY_EDITOR
        //handling initial offers reseting when exiting playmode in editor
        private List<Marketplace> _initialMarketplaces = new List<Marketplace>();
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += playModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= playModeChanged;
        }

        private void playModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                _initialMarketplaces = _marketplaces.Select(o => o).ToList();
            }
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                _marketplaces = _initialMarketplaces;
            }
        }
#endif

        private IMarketplaceSetSyncProvider _syncProvider = null;
        [SerializeField] private List<Marketplace> _marketplaces = new List<Marketplace>();

        public IMarketplaceSetSyncProvider syncProvider
        {
            get
            {
                return _syncProvider;
            }

            set
            {
                _syncProvider = value;
            }
        }

        public List<Marketplace> marketplaces
        {
            get
            {
                return _marketplaces;
            }
        }

        public void addMarketplace(Marketplace marketplace)
        {
            _marketplaces.Add(marketplace);
        }

        public void removeMarketplace(Marketplace marketplace)
        {
            _marketplaces.Remove(marketplace);
        }

        public Marketplace getByGuid(string guid)
        {
            Marketplace marketplace = _marketplaces.FirstOrDefault(mp => mp.guid.ToString() == guid);
            if (marketplace != null)
            {
                return marketplace;
            }
            else
            {
                RootLogger.Error(this, "Marketplace with Guid {0} does not exist. Instantiating default Marketplace.", guid);
                return ScriptableObject.CreateInstance<Marketplace>();
            }
        }
    }
}

