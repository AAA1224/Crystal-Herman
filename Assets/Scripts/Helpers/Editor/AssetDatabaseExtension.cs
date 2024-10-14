using System.Collections.Generic;
using UnityEditor;

namespace KoboldTools
{
    public static class AssetDatabaseExtension
    {
        /// Returns first assets from all given paths
        /// </summary>
        /// <typeparam name="T">Type of asset (basetype UnityEngine.Object)</typeparam>
        /// <param name="paths">Collection of paths to load the assets from</param>
        /// <returns>Collection of loaded assets, null where no asset found</returns>
        public static List<T> LoadAssetsFromMultiplePaths<T>(IList<string> paths) where T : UnityEngine.Object
        {
            var ret = new List<T>(paths.Count);
            for(int i = 0; i < paths.Count; i++)
            {
                ret.Add(AssetDatabase.LoadAssetAtPath<T>(paths[i]));
            }
            return ret;
        }
    }
}