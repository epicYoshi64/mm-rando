using MMRando.Models.Rom;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MMRando.Utils
{
    public class ActorUtils
    {
        public static List<Actor> GetSceneActorsByNumber(int SceneNumber, int ActorNumber)
        {
            List<Actor> SearchActor = new List<Actor>();
            List<Actor> SceneActors;
            // search the scene list starting from the requested scene looking backwards
            // SceneList follows the internal entrance list but skips some
            // so the desired scene should be found at or before the requested index
            for (int i = Math.Min(RomData.SceneList.Count, SceneNumber); i >= 0; i--)
            {
                if (RomData.SceneList[i].Number == SceneNumber)
                {
                    for (int MapNumber = 0; MapNumber < RomData.SceneList[i].Maps.Count;MapNumber++)
                    {
                        SceneActors = RomData.SceneList[i].Maps[MapNumber].Actors;
                        for (int a = 0; a < SceneActors.Count; a++)
                        {
                            if (SceneActors[a].n == ActorNumber)
                            {
                                SearchActor.Add(SceneActors[a]);
                            }
                        }
                    }
                }
            }
            return SearchActor;
        }
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
