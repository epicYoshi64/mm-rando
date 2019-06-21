using MMRando.Constants;

namespace MMRando.Utils
{
    public static class ItemUtils
    {
        public static bool IsAreaOrOther(int itemId)
        {
            return (itemId >= Items.AreaSouthAccess && itemId <= Items.AreaInvertedStoneTowerNew)
                || (itemId >= Items.OtherOneMask && itemId <= Items.AreaMoonAccess);
        }

        public static bool IsOutOfRange(int itemId)
        {
            return itemId > Items.FairyStoneTowerWindFunnel;
        }

        public static bool IsShopItem(int itemIndex)
        {
            return (itemIndex >= Items.ShopItemTradingPostRedPotion
                    && itemIndex <= Items.ShopItemZoraRedPotion)
                    || itemIndex == Items.ItemBombBag
                    || itemIndex == Items.UpgradeBigBombBag
                    || itemIndex == Items.MaskAllNight;
        }

        public static bool IsFakeItem(int itemId)
        {
            return IsAreaOrOther(itemId) || IsOutOfRange(itemId);
        }

        public static bool IsTemporaryItem(int itemId)
        {
            return IsTradeItem(itemId) || IsKey(itemId) || itemId == Items.ItemGoldDust;
        }

        public static bool IsKey(int itemId)
        {
            return itemId == Items.ItemWoodfallKey1
                || (itemId >= Items.ItemSnowheadKey1
                    && itemId <= Items.ItemSnowheadKey3)
                || itemId == Items.ItemGreatBayKey1
                || (itemId >= Items.ItemStoneTowerKey1
                    && itemId <= Items.ItemStoneTowerKey4);
        }

        private static bool IsTradeItem(int itemId)
        {
            return itemId >= Items.TradeItemMoonTear
                   && itemId <= Items.TradeItemMamaLetter;
        }

        public static int AddItemOffset(int itemId)
        {
            if (itemId >= Items.AreaSouthAccess)
            {
                itemId += Items.NumberOfAreasAndOther;
            }
            if (itemId >= Items.OtherOneMask)
            {
                itemId += 5;
            }
            return itemId;
        }

        public static int SubtractItemOffset(int itemId)
        {
            if (itemId >= Items.OtherOneMask)
            {
                itemId -= 5;
            }
            if (itemId >= Items.AreaSouthAccess)
            {
                itemId -= Items.NumberOfAreasAndOther;
            }
            return itemId;
        }

        public static bool IsDungeonItem(int itemIndex)
        {
            return itemIndex >= Items.ItemWoodfallMap
                    && itemIndex <= Items.ItemStoneTowerKey4;
        }

        public static bool IsDungeonItem(int itemIndex, int dungeonIndex)
        {
            if( dungeonIndex == 0)
            {
                return itemIndex >= Items.ItemWoodfallMap && itemIndex <= Items.ItemWoodfallKey1 
                    || itemIndex >= Items.FairyWoodfallLobby && itemIndex <= Items.FairyWoodfallBoe 
                    || itemIndex == Items.ItemBow;
            }
            if (dungeonIndex == 1)
            {
                return itemIndex >= Items.FairySnowheadWhiteRoom && itemIndex <= Items.FairySnowheadInvisibleStaircase
                    || itemIndex >= Items.ItemWoodfallMap && itemIndex <= Items.ItemSnowheadKey3
                    || itemIndex == Items.ItemFireArrow;
            }
            if (dungeonIndex == 2)
            {
                return itemIndex >= Items.FairyGreatBayBioBaba && itemIndex <= Items.FairyGreatBayLobby
                    || itemIndex >= Items.ItemGreatBayMap && itemIndex <= Items.ItemGreatBayKey1
                    || itemIndex == Items.ItemIceArrow;
            }
            if (dungeonIndex == 3)
            {
                return itemIndex >= Items.FairyInvertedTowerLobby && itemIndex <= Items.FairyStoneTowerWindFunnel
                    || itemIndex >= Items.ItemStoneTowerMap && itemIndex <= Items.ItemStoneTowerKey4
                    || itemIndex == Items.ItemLightArrow || itemIndex == Items.MaskGiant;
            }
            return itemIndex >= Items.ItemWoodfallMap
                    && itemIndex <= Items.ItemStoneTowerKey4;
        }

        public static bool IsBottleCatchContent(int itemIndex)
        {
            return itemIndex >= Items.BottleCatchFairy
                   && itemIndex <= Items.BottleCatchMushroom;
        }

        public static bool IsMoonItem(int itemIndex)
        {
            return itemIndex >= Items.HeartPieceDekuTrial && itemIndex <= Items.MaskFierceDeity;
        }

        public static bool IsOtherItem(int itemIndex)
        {
            return itemIndex >= Items.ChestLensCaveRedRupee && itemIndex <= Items.IkanaScrubGoldRupee;
        }

        internal static bool IsDeed(int item)
        {
            return item >= Items.TradeItemLandDeed
                    && item <= Items.TradeItemOceanDeed;
        }

        internal static bool IsStrayFairy(int item)
        {
            return item >= Items.FairyWoodfallLobby
                    && item <= Items.FairyStoneTowerWindFunnel;
        }

        public static bool IsHeartPiece(int itemIndex)
        {
            return (itemIndex >= Items.HeartPieceNotebookMayor && itemIndex <= Items.HeartPieceKnuckle)
                || (itemIndex >= Items.HeartPieceSouthClockTown && itemIndex <= Items.HeartContainerStoneTower)
                || (itemIndex >= Items.HeartPieceDekuTrial && itemIndex <= Items.HeartPieceLinkTrial)
                || itemIndex == Items.ChestSecretShrineHeartPiece
                || itemIndex == Items.HeartPieceBank;
        }

        public static bool IsStartingItem(int itemIndex)
        {
            return itemIndex == Items.MaskDeku || itemIndex == Items.SongHealing;
        }
    }
}
