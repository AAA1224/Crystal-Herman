using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace KoboldTools.Helpers
{
    public static class Enum35
    {
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            return TryParse<TEnum>(value, false, out result);
        }
        public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
        {
#if NET35
            result = default(TEnum);
            try
            {
                result = (TEnum)System.Enum.Parse(typeof(TEnum), value, ignoreCase);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
#else
            return Enum.TryParse(value, ignoreCase, out result);
#endif
        }
    }

    public static class Directory35
    {
        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return EnumerateFiles(path, String.Empty, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
#if NET35
            return Directory.GetFiles(path, searchPattern, searchOption);
#else
            return Directory.EnumerateFiles(path, searchPattern, searchOption);
#endif
        }

        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            return EnumerateDirectories(path, String.Empty, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
#if NET35
            return Directory.GetDirectories(
path, searchPattern, searchOption);
#else
            return Directory.EnumerateDirectories(path, searchPattern, searchOption);
#endif
        }
    }

    public static class Path35
    {
        public static string Combine(params string[] paths)
        {
#if NET35
            var path = "";
            for(int i = 0; i < paths.Length; i++)
            {
                path = Path.Combine(path, paths[i]);
            }
            return path;
#else
            return Path.Combine(paths);
#endif
        }
    }
}