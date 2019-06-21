using MMRando.Models.Rom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMRando.Utils
{
    public class ActorUtils
    {
        // TODO: Move this over to SceneUtil
        public static int[] Permutation(int n)
        {
            Random RNG = new Random();
            List<int> candidates = new List<int>();
            for (int i = 0; i < n; i++)
            {
                candidates.Add(i);
            }
            int[] perm = new int[n];
            int j = 0;
            while (candidates.Count > 0)
            {
                int k = RNG.Next(candidates.Count);
                perm[j++] = candidates[k];
                candidates.RemoveAt(k);
            }
            return perm;
        }
    }
}
