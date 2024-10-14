using UnityEngine;

namespace KoboldTools
{
    public class DataLog : DataLogger
    {
        #region Singleton
        //Here is a private reference only this class can access
        [SerializeField]
        private static ILogger _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ILogger instance
        {
            get
            {
                //If _instance hasn't been set yet, we grab it from the scene!
                //This will only happen the first time this reference is used.
                if (_instance == null) _instance = GameObject.FindObjectOfType<DataLog>();

                return _instance;
            }
        }
        #endregion
    }
}
