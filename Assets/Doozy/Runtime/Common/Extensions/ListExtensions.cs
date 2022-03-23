// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Doozy.Runtime.Common.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandomItem<T>(this IList<T> target) =>
            target[Random.Range(0, target.Count)];

        public static void Shuffle<T>(this IList<T> target)
        {
            for (int i = target.Count - 1; i > 1; i--)
            {
                int j = Random.Range(0, i + 1);
                (target[j], target[i]) = (target[i], target[j]);
            }
        }

        /// <summary> Remove null entries from list </summary>
        /// <param name="target"> Target list </param>
        public static void RemoveNulls<T>(this IList<T> target)
        {
            for (int i = target.Count - 1; i > 1; i--)
                if (target[i] == null)
                    target.RemoveAt(i);
        }

    }
}
