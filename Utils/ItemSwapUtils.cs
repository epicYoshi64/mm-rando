﻿using MMRando.Attributes;
using MMRando.Constants;
using MMRando.Extensions;
using MMRando.GameObjects;
using MMRando.Models.Rom;
using System.Collections.Generic;

namespace MMRando.Utils
{
    public static class ItemSwapUtils
    {
        const int BOTTLE_CATCH_TABLE = 0xCD7C08;
        static int cycle_repeat = 0;
        static int cycle_repeat_count_address = 0xC72D16;
        static ushort cycle_repeat_count = 0x74;
        static int GET_ITEM_TABLE = 0;

        public static void ReplaceGetItemTable(string ModsDir)
        {
            ResourceUtils.ApplyHack(ModsDir + "replace-gi-table");
            int last_file = RomData.MMFileList.Count - 1;
            GET_ITEM_TABLE = RomUtils.AddNewFile(ModsDir + "gi-table");
            ReadWriteUtils.WriteToROM(0xBDAEAC, (uint)last_file + 1);
            ResourceUtils.ApplyHack(ModsDir + "update-chests");
            RomUtils.AddNewFile(ModsDir + "chest-table");
            ReadWriteUtils.WriteToROM(0xBDAEA8, (uint)last_file + 2);
            ResourceUtils.ApplyHack(ModsDir + "standing-hearts");
            ResourceUtils.ApplyHack(ModsDir + "fix-item-checks");
            cycle_repeat = 0xC72DF4;
            SceneUtils.ResetSceneFlagMask();
            SceneUtils.UpdateSceneFlagMask(0x5B); // red potion
            SceneUtils.UpdateSceneFlagMask(0x91); // chateau romani
            SceneUtils.UpdateSceneFlagMask(0x92); // milk
            SceneUtils.UpdateSceneFlagMask(0x93); // gold dust
        }

        private static void InitGetBottleList()
        {
            RomData.BottleList = new Dictionary<int, BottleCatchEntry>();
            int f = RomUtils.GetFileIndexForWriting(BOTTLE_CATCH_TABLE);
            int baseaddr = BOTTLE_CATCH_TABLE - RomData.MMFileList[f].Addr;
            var fileData = RomData.MMFileList[f].Data;
            foreach (var getBottleItemIndex in ItemUtils.AllGetBottleItemIndices())
            {
                int offset = getBottleItemIndex * 6 + baseaddr;
                RomData.BottleList[getBottleItemIndex] = new BottleCatchEntry
                {
                    ItemGained = fileData[offset + 3],
                    Index = fileData[offset + 4],
                    Message = fileData[offset + 5]
                };
            }
        }

        private static void InitGetItemList()
        {
            RomData.GetItemList = new Dictionary<int, GetItemEntry>();
            int f = RomUtils.GetFileIndexForWriting(GET_ITEM_TABLE);
            int baseaddr = GET_ITEM_TABLE - RomData.MMFileList[f].Addr;
            var fileData = RomData.MMFileList[f].Data;
            foreach (var getItemIndex in ItemUtils.AllGetItemIndices())
            {
                int offset = (getItemIndex - 1) * 8 + baseaddr;
                RomData.GetItemList[getItemIndex] = new GetItemEntry
                {
                    ItemGained = fileData[offset],
                    Flag = fileData[offset + 1],
                    Index = fileData[offset + 2],
                    Type = fileData[offset + 3],
                    Message = (short)((fileData[offset + 4] << 8) | fileData[offset + 5]),
                    Object = (short)((fileData[offset + 6] << 8) | fileData[offset + 7])
                };
            }
        }

        public static void InitItems()
        {
            InitGetItemList();
            InitGetBottleList();
        }

        public static void WriteNewBottle(Item location, Item item)
        {
            System.Diagnostics.Debug.WriteLine($"Writing {item.Name()} --> {location.Location()}");

            int f = RomUtils.GetFileIndexForWriting(BOTTLE_CATCH_TABLE);
            int baseaddr = BOTTLE_CATCH_TABLE - RomData.MMFileList[f].Addr;
            var fileData = RomData.MMFileList[f].Data;

            foreach (var index in location.GetBottleItemIndices())
            {
                var offset = index * 6 + baseaddr;
                var newBottle = RomData.BottleList[item.GetBottleItemIndices()[0]];
                var data = new byte[]
                {
                    newBottle.ItemGained,
                    newBottle.Index,
                    newBottle.Message,
                };
                ReadWriteUtils.Arr_Insert(data, 0, data.Length, fileData, offset + 3);
            }
        }

        public static void WriteNewItem(Item location, Item item, List<MessageEntry> newMessages, bool updateShop, bool preventDowngrades, bool updateChest, ChestTypeAttribute.ChestType? overrideChestType, bool isExtraStartingItem)
        {
            System.Diagnostics.Debug.WriteLine($"Writing {item.Name()} --> {location.Location()}");

            int f = RomUtils.GetFileIndexForWriting(GET_ITEM_TABLE);
            int baseaddr = GET_ITEM_TABLE - RomData.MMFileList[f].Addr;
            var getItemIndex = location.GetItemIndex().Value;
            int offset = (getItemIndex - 1) * 8 + baseaddr;
            var newItem = isExtraStartingItem 
                ? Items.RecoveryHeart // Warning: this will not work well for starting with Bottle contents (currently impossible), because you'll first have to acquire the Recovery Heart before getting the bottle-less version. Also may interfere with future implementation of progressive upgrades.
                : RomData.GetItemList[item.GetItemIndex().Value];
            var fileData = RomData.MMFileList[f].Data;

            var data = new byte[]
            {
                newItem.ItemGained,
                newItem.Flag,
                newItem.Index,
                newItem.Type,
                (byte)(newItem.Message >> 8),
                (byte)(newItem.Message & 0xFF),
                (byte)(newItem.Object >> 8),
                (byte)(newItem.Object & 0xFF),
            };
            ReadWriteUtils.Arr_Insert(data, 0, data.Length, fileData, offset);
            
            // todo use Logic Editor to handle which locations should be repeatable and which shouldn't.
            if ((item.IsCycleRepeatable() && location != Item.HeartPieceNotebookMayor) || (item.Name().Contains("Rupee") && location.IsRupeeRepeatable()))
            {
                ReadWriteUtils.WriteToROM(cycle_repeat, (ushort)getItemIndex);
                cycle_repeat += 2;
                cycle_repeat_count += 2;

                ReadWriteUtils.WriteToROM(cycle_repeat_count_address, cycle_repeat_count);
            }

            var isRepeatable = item.IsRepeatable() || (!preventDowngrades && item.IsDowngradable());
            if (!isRepeatable)
            {
                SceneUtils.UpdateSceneFlagMask(getItemIndex);
            }

            if (item == Item.ItemBottleWitch)
            {
                ReadWriteUtils.WriteToROM(0xB4997E, (ushort)getItemIndex);
                ReadWriteUtils.WriteToROM(0xC72B42, (ushort)getItemIndex);
            }

            if (item == Item.ItemBottleMadameAroma)
            {
                ReadWriteUtils.WriteToROM(0xB4998A, (ushort)getItemIndex);
                ReadWriteUtils.WriteToROM(0xC72B4E, (ushort)getItemIndex);
            }

            if (item == Item.ItemBottleAliens)
            {
                ReadWriteUtils.WriteToROM(0xB49996, (ushort)getItemIndex);
                ReadWriteUtils.WriteToROM(0xC72B5A, (ushort)getItemIndex);
            }
            
            if (item == Item.ItemBottleGoronRace)
            {
                ReadWriteUtils.WriteToROM(0xB499A2, (ushort)getItemIndex);
                ReadWriteUtils.WriteToROM(0xC72B66, (ushort)getItemIndex);
            }

            if (updateChest)
            {
                UpdateChest(location, item, overrideChestType);
            }

            if (location != item)
            {
                if (updateShop)
                {
                    UpdateShop(location, item, newMessages);
                }

                if (location == Item.StartingSword)
                {
                    ResourceUtils.ApplyHack(Values.ModsDirectory + "fix-sword-song-of-time");
                }

                if (location == Item.MundaneItemSeahorse)
                {
                    ResourceUtils.ApplyHack(Values.ModsDirectory + "fix-fisherman");
                }

                if (location == Item.MaskFierceDeity)
                {
                    ResourceUtils.ApplyHack(Values.ModsDirectory + "fix-fd-mask-reset");
                }
            }
        }

        private static void UpdateShop(Item location, Item item, List<MessageEntry> newMessages)
        {
            var newItem = RomData.GetItemList[item.GetItemIndex().Value];

            var shopRooms = location.GetAttributes<ShopRoomAttribute>();
            foreach (var shopRoom in shopRooms)
            {
                ReadWriteUtils.WriteToROM(shopRoom.RoomObjectAddress, (ushort)newItem.Object);
            }

            var shopInventories = location.GetAttributes<ShopInventoryAttribute>();
            foreach (var shopInventory in shopInventories)
            {
                ReadWriteUtils.WriteToROM(shopInventory.ShopItemAddress, (ushort)newItem.Object);
                var index = newItem.Index > 0x7F ? (byte)(0xFF - newItem.Index) : (byte)(newItem.Index - 1);
                ReadWriteUtils.WriteToROM(shopInventory.ShopItemAddress + 0x03, index);

                var shopTexts = item.ShopTexts();
                string description;
                switch (shopInventory.Keeper)
                {
                    case ShopInventoryAttribute.ShopKeeper.WitchShop:
                        description = shopTexts.WitchShop;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.TradingPostMain:
                        description = shopTexts.TradingPostMain;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.TradingPostPartTimer:
                        description = shopTexts.TradingPostPartTimer;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.CuriosityShop:
                        description = shopTexts.CuriosityShop;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.BombShop:
                        description = shopTexts.BombShop;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.ZoraShop:
                        description = shopTexts.ZoraShop;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.GoronShop:
                        description = shopTexts.GoronShop;
                        break;
                    case ShopInventoryAttribute.ShopKeeper.GoronShopSpring:
                        description = shopTexts.GoronShopSpring;
                        break;
                    default:
                        description = null;
                        break;
                }
                if (description == null)
                {
                    description = shopTexts.Default;
                }

                var messageId = ReadWriteUtils.ReadU16(shopInventory.ShopItemAddress + 0x0A);
                newMessages.Add(new MessageEntry
                {
                    Id = messageId,
                    Header = null,
                    Message = MessageUtils.BuildShopDescriptionMessage(item.Name(), 20, description)
                });

                newMessages.Add(new MessageEntry
                {
                    Id = (ushort)(messageId + 1),
                    Header = null,
                    Message = MessageUtils.BuildShopPurchaseMessage(item.Name(), 20, item)
                });
            }
        }

        private static void UpdateChest(Item location, Item item, ChestTypeAttribute.ChestType? overrideChestType)
        {
            var chestType = item.GetAttribute<ChestTypeAttribute>().Type;
            if (overrideChestType.HasValue)
            {
                chestType = overrideChestType.Value;
            }
            var chestAttribute = location.GetAttribute<ChestAttribute>();
            if (chestAttribute != null)
            {
                foreach (var address in chestAttribute.Addresses)
                {
                    var chestVariable = ReadWriteUtils.Read(address);
                    chestVariable &= 0x0F; // remove existing chest type
                    var newChestType = ChestAttribute.GetType(chestType, chestAttribute.Type);
                    newChestType <<= 4;
                    chestVariable |= newChestType;
                    ReadWriteUtils.WriteToROM(address, chestVariable);
                }
            }

            var grottoChestAttribute = location.GetAttribute<GrottoChestAttribute>();
            if (grottoChestAttribute != null)
            {
                foreach (var address in grottoChestAttribute.Addresses)
                {
                    var grottoVariable = ReadWriteUtils.Read(address);
                    grottoVariable &= 0x1F; // remove existing chest type
                    var newChestType = (byte)chestType;
                    newChestType <<= 5;
                    grottoVariable |= newChestType; // add new chest type
                    ReadWriteUtils.WriteToROM(address, grottoVariable);
                }
            }
        }

    }

}