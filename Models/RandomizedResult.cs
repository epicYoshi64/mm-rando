﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMRando.Models
{
    public class RandomizedResult
    {
        public Settings Settings { get; private set; }
        public Random Random { get; private set; }
        public List<ItemObject> ItemList { get; set; }
        public int[] EntranceList { get; set; }
        public int[] ShuffledEntranceList { get; set; }
        public List<string> EntranceSpoilers { get; set; }
        public int[] OwlStatueList { get; set; }
        public List<string> GossipQuotes { get; set; }
        public List<ItemLogic> Logic { get; set; }

        public int[] NewEntrances = new int[] { -1, -1, -1, -1 };
        public int[] NewExits = new int[] { -1, -1, -1, -1 };

        public int[] NewExitIndices = new int[] { -1, -1, -1, -1 };
        public int[] NewDCFlags = new int[] { -1, -1, -1, -1 };
        public int[] NewDCMasks = new int[] { -1, -1, -1, -1 };
        public int[] NewDestinationIndices = new int[] { -1, -1, -1, -1 };

        public RandomizedResult(Settings settings, Random random)
        {
            Settings = settings;
            Random = random;
        }

    }
}
