﻿using MMRando.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MMRando.Models
{
    public class Spoiler
    {
        public string Version { get; set; }

        public string SettingsString { get; set; }

        public bool RandomizeDungeonEntrances { get; set; }

        public int Seed { get; set; }

        public List<ItemObject> ItemList { get; set; }

        public ReadOnlyCollection<string> ITEM_NAMES { get; set; }

        public int[] NewEntranceIndices { get; set; }

        public string[] destinations = new string[] 
        {
            "Woodfall", "Snowhead", "Inverted Stone Tower", "Great Bay"
        };

        
    }
}

