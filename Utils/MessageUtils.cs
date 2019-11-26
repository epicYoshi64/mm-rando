﻿using MMRando.Attributes;
using MMRando.Extensions;
using MMRando.GameObjects;
using MMRando.Models;
using MMRando.Models.Rom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace MMRando.Utils
{
    public static class MessageUtils
    {
        static ReadOnlyCollection<byte> MessageHeader
            = new ReadOnlyCollection<byte>(new byte[] {
                2, 0, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        });

        public static List<MessageEntry> MakeGossipQuotes(RandomizedResult randomizedResult)
        {
            if (randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Default)
                return new List<MessageEntry>();

            var randomizedItems = new List<ItemObject>();
            var competitiveHints = new List<string>();
            var itemsInRegions = new Dictionary<Region, List<ItemObject>>();
            foreach (var item in randomizedResult.ItemList)
            {
                if (item.NewLocation == null)
                {
                    continue;
                }

                if (randomizedResult.Settings.ClearHints)
                {
                    // skip free items
                    if (ItemUtils.IsStartingLocation(item.NewLocation.Value))
                    {
                        continue;
                    }
                }

                if (!item.IsRandomized)
                {
                    continue;
                }

                var itemName = item.Item.Name();
                if (randomizedResult.Settings.GossipHintStyle != GossipHintStyle.Competitive 
                    && (itemName.Contains("Heart") || itemName.Contains("Rupee"))
                    && (randomizedResult.Settings.ClearHints || randomizedResult.Random.Next(8) != 0))
                {
                    continue;
                }

                if (randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Competitive)
                {
                    var preventRegions = new List<Region> { Region.TheMoon, Region.BottleCatch, Region.Misc };
                    var itemRegion = item.NewLocation.Value.Region();
                    if (itemRegion.HasValue
                        && !preventRegions.Contains(itemRegion.Value)
                        && !randomizedResult.Settings.CustomJunkLocations.Contains(item.NewLocation.Value))
                    {
                        if (!itemsInRegions.ContainsKey(itemRegion.Value))
                        {
                            itemsInRegions[itemRegion.Value] = new List<ItemObject>();
                        }
                        itemsInRegions[itemRegion.Value].Add(item);
                    }

                    var competitiveHintInfo = item.NewLocation.Value.GetAttribute<GossipCompetitiveHintAttribute>();
                    if (competitiveHintInfo == null)
                    {
                        continue;
                    }

                    if (randomizedResult.Settings.CustomJunkLocations.Contains(item.NewLocation.Value))
                    {
                        continue;
                    }

                    if (competitiveHintInfo.Condition != null && competitiveHintInfo.Condition(randomizedResult.Settings))
                    {
                        continue;
                    }
                }

                randomizedItems.Add(item);
            }

            var unusedItems = randomizedItems.ToList();

            if (randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Competitive)
            {
                unusedItems.AddRange(randomizedItems);
                var requiredHints = new List<string>();
                var nonRequiredHints = new List<string>();
                foreach (var kvp in itemsInRegions)
                {
                    var numberOfRequiredItems = kvp.Value.Count(io => !io.Item.Name().Contains("Heart")
                        && (randomizedResult.Settings.AddSongs || !ItemUtils.IsSong(io.Item))
                        && !ItemUtils.IsStrayFairy(io.Item)
                        && !ItemUtils.IsSkulltulaToken(io.Item)
                        && randomizedResult.ItemsRequiredForMoonAccess.Contains(io.Item));
                    var numberOfImportantItems = kvp.Value.Count(io => !io.Item.Name().Contains("Heart") && randomizedResult.ImportantItems.Contains(io.Item));

                    if (numberOfRequiredItems == 0 && numberOfImportantItems > 0)
                    {
                        continue;
                    }

                    ushort soundEffectId = 0x690C; // grandma curious
                    string start = Gossip.MessageStartSentences.Random(randomizedResult.Random);

                    string sfx = $"{(char)((soundEffectId >> 8) & 0xFF)}{(char)(soundEffectId & 0xFF)}";
                    var locationMessage = kvp.Key.Name();
                    //var mid = "is";
                    //var itemMessage = numberOfRequiredItems > 0
                    //    ? "on the Way of the Hero"
                    //    : "a foolish choice";
                    List<string> list;
                    char color;
                    if (numberOfRequiredItems > 0)
                    {
                        list = requiredHints;
                        color = TextCommands.ColorYellow;
                    }
                    else
                    {
                        list = nonRequiredHints;
                        color = TextCommands.ColorSilver;
                    }

                    //list.Add($"\x1E{sfx}{start} \x01{locationMessage}\x00 {mid} \x06{itemMessage}\x00...\xBF".Wrap(35, "\x11"));

                    var mid = "has";
                    list.Add($"\x1E{sfx}{start} {TextCommands.ColorRed}{locationMessage}{TextCommands.ColorWhite} {mid} {color}{NumberToWords(numberOfImportantItems)} important item{(numberOfRequiredItems == 1 ? "" : "s")}{TextCommands.ColorWhite}...\xBF".Wrap(35, "\x11"));
                }

                var numberOfRequiredHints = 3;
                var numberOfNonRequiredHints = 2;

                for (var i = 0; i < numberOfRequiredHints; i++)
                {
                    var chosen = requiredHints.RandomOrDefault(randomizedResult.Random);
                    if (chosen != null)
                    {
                        requiredHints.Remove(chosen);
                        competitiveHints.Add(chosen);
                        competitiveHints.Add(chosen);
                    }
                }

                for (var i = 0; i < numberOfNonRequiredHints; i++)
                {
                    var chosen = nonRequiredHints.RandomOrDefault(randomizedResult.Random);
                    if (chosen != null)
                    {
                        nonRequiredHints.Remove(chosen);
                        competitiveHints.Add(chosen);
                        competitiveHints.Add(chosen);
                    }
                }
            }

            List<MessageEntry> finalHints = new List<MessageEntry>();

            foreach (var gossipQuote in Enum.GetValues(typeof(GossipQuote)).Cast<GossipQuote>().OrderBy(gq => randomizedResult.Random.Next()))
            {
                string messageText = null;
                var isMoonGossipStone = gossipQuote.ToString().StartsWith("Moon");
                if (!isMoonGossipStone && competitiveHints.Any())
                {
                    messageText = competitiveHints.Random(randomizedResult.Random);
                    competitiveHints.Remove(messageText);
                }

                if (messageText == null)
                {
                    var restrictionAttributes = gossipQuote.GetAttributes<GossipRestrictAttribute>().ToList();
                    ItemObject item = null;
                    var forceClear = false;
                    while (item == null)
                    {
                        if (restrictionAttributes.Any() && (isMoonGossipStone || randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Relevant))
                        {
                            var chosen = restrictionAttributes.Random(randomizedResult.Random);
                            var candidateItem = chosen.Type == GossipRestrictAttribute.RestrictionType.Item
                                ? randomizedResult.ItemList.Single(io => io.Item == chosen.Item)
                                : randomizedResult.ItemList.Single(io => io.NewLocation == chosen.Item);
                            if (isMoonGossipStone || unusedItems.Contains(candidateItem))
                            {
                                item = candidateItem;
                                forceClear = chosen.ForceClear;
                            }
                            else
                            {
                                restrictionAttributes.Remove(chosen);
                            }
                        }
                        else if (unusedItems.Any())
                        {
                            if (randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Competitive)
                            {
                                item = unusedItems.FirstOrDefault(io => unusedItems.Count(x => x.Item == io.Item) == 1);
                                if (item == null)
                                {
                                    item = unusedItems.GroupBy(io => io.NewLocation.Value.GetAttribute<GossipCompetitiveHintAttribute>().Priority)
                                        .OrderByDescending(g => g.Key)
                                        .First()
                                        .ToList()
                                        .Random(randomizedResult.Random);
                                }
                            }
                            else
                            {
                                item = unusedItems.Random(randomizedResult.Random);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!isMoonGossipStone)
                    {
                        unusedItems.Remove(item);
                    }

                    if (item != null)
                    {
                        ushort soundEffectId = 0x690C; // grandma curious
                        string itemName = null;
                        string locationName = null;
                        if (forceClear || randomizedResult.Settings.ClearHints)
                        {
                            itemName = item.Item.Name();
                            locationName = item.NewLocation.Value.Location();
                        }
                        else
                        {
                            if (isMoonGossipStone || randomizedResult.Settings.GossipHintStyle == GossipHintStyle.Competitive || randomizedResult.Random.Next(100) >= 5) // 5% chance of fake/junk hint if it's not a moon gossip stone or competitive style
                            {
                                itemName = item.Item.ItemHints().Random(randomizedResult.Random);
                                locationName = item.NewLocation.Value.LocationHints().Random(randomizedResult.Random);
                            }
                            else
                            {
                                if (randomizedResult.Random.Next(2) == 0) // 50% chance for fake hint. otherwise default to junk hint.
                                {
                                    soundEffectId = 0x690A; // grandma laugh
                                    itemName = item.Item.ItemHints().Random(randomizedResult.Random);
                                    locationName = randomizedItems.Random(randomizedResult.Random).Item.LocationHints().Random(randomizedResult.Random);
                                }
                            }
                        }
                        if (itemName != null && locationName != null)
                        {
                            messageText = BuildGossipQuote(soundEffectId, locationName, itemName, randomizedResult.Random);
                        }
                    }
                }

                if (messageText == null)
                {
                    messageText = Gossip.JunkMessages.Random(randomizedResult.Random);
                }

                finalHints.Add(new MessageEntry()
                {
                    Id = (ushort)gossipQuote,
                    Message = messageText,
                    Header = MessageHeader.ToArray()
                });
            }

            return finalHints;
        }
        
        private static string BuildGossipQuote(ushort soundEffectId, string locationMessage, string itemMessage, Random random)
        {
            int startIndex = random.Next(Gossip.MessageStartSentences.Count);
            int midIndex = random.Next(Gossip.MessageMidSentences.Count);
            string start = Gossip.MessageStartSentences[startIndex];
            string mid = Gossip.MessageMidSentences[midIndex];

            string sfx = $"{(char)((soundEffectId >> 8) & 0xFF)}{(char)(soundEffectId & 0xFF)}";

            return $"\x1E{sfx}{start} \x01{locationMessage}\x00 {mid} \x06{itemMessage}\x00...\xBF".Wrap(35, "\x11");
        }

        public static string BuildShopDescriptionMessage(string title, int cost, string description)
        {
            return $"\x01{title}: {cost} Rupees\x11\x00{description.Wrap(35, "\x11")}\x1A\xBF";
        }

        public static string BuildShopPurchaseMessage(string title, int cost, Item item)
        {
            return $"{title}: {cost} Rupees\x11 \x11\x02\xC2I'll buy {GetPronoun(item)}\x11No thanks\xBF";
        }

        public static string GetArticle(Item item, string indefiniteArticle = null)
        {
            var shopTexts = item.ShopTexts();
            return shopTexts.IsMultiple
                ? ""
                : shopTexts.IsDefinite
                    ? "the "
                    : indefiniteArticle ?? (Regex.IsMatch(item.Name(), "^[aeiou]", RegexOptions.IgnoreCase)
                        ? "an "
                        : "a ");
        }

        public static string GetPronoun(Item item)
        {
            var shopTexts = item.ShopTexts();
            var itemAmount = Regex.Replace(item.Name(), "[^0-9]", "");
            return shopTexts.IsMultiple && !string.IsNullOrWhiteSpace(itemAmount)
                ? "them"
                : "it";
        }

        public static string GetPronounOrAmount(Item item, string it = " It")
        {
            var shopTexts = item.ShopTexts();
            var itemAmount = Regex.Replace(item.Name(), "[^0-9]", "");
            return shopTexts.IsMultiple
                ? string.IsNullOrWhiteSpace(itemAmount)
                    ? it
                    : " " + itemAmount
                : shopTexts.IsDefinite
                    ? it
                    : " One";
        }

        public static string GetVerb(Item item)
        {
            var shopTexts = item.ShopTexts();
            var itemAmount = Regex.Replace(item.Name(), "[^0-9]", "");
            return shopTexts.IsMultiple && !string.IsNullOrWhiteSpace(itemAmount)
                ? "are"
                : "is";
        }

        public static string GetFor(Item item)
        {
            var shopTexts = item.ShopTexts();
            return shopTexts.IsDefinite
                ? "is"
                : "for";
        }

        public static string GetAlternateName(Item item)
        {
            return Regex.Replace(item.Name(), "[0-9]+ ", "");
        }

        private static string[] numberWordUnitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        private static string[] numberWordTensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                    words += numberWordUnitsMap[number];
                else
                {
                    words += numberWordTensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + numberWordUnitsMap[number % 10];
                }
            }

            return words;
        }
    }
}