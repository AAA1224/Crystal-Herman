using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using KoboldTools.Logging;

namespace KoboldTools
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        public bool deleteDoublicates = true;
        private static List<DontDestroyOnLoad> dontDestroyOnLoads = new List<DontDestroyOnLoad>();

        void Awake()
        {
            dontDestroyOnLoads.Add(this);
            DontDestroyOnLoad(this);
        }

        public static void destroyAll()
        {
            while(dontDestroyOnLoads.Count > 0)
            {
                DestroyImmediate(dontDestroyOnLoads[dontDestroyOnLoads.Count-1].gameObject);
                dontDestroyOnLoads.RemoveAt(dontDestroyOnLoads.Count - 1);
            }
        }
    }
}
