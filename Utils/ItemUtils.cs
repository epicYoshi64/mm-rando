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

        public static bool IsShopItem(int itemId)
        {
            return (itemId >= Items.ShopItemTradingPostRedPotion
                    && itemId <= Items.ShopItemZoraRedPotion)
                    || itemId == Items.ItemBombBag
                    || itemId == Items.UpgradeBigBombBag
                    || itemId == Items.MaskAllNight;
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

        public static bool IsDungeonItem(int itemId)
        {
            return itemId >= Items.ItemWoodfallMap
                    && itemId <= Items.ItemStoneTowerKey4;
        }

        public static bool IsDungeonItem(int itemId, int dungeonId)
        {
            if( dungeonId == 0)
            {
                return itemId >= Items.ItemWoodfallMap && itemId <= Items.ItemWoodfallKey1 
                    || itemId >= Items.FairyWoodfallLobby && itemId <= Items.FairyWoodfallBoe 
                    || itemId == Items.ItemBow;
            }
            if (dungeonId == 1)
            {
                return itemId >= Items.FairySnowheadWhiteRoom && itemId <= Items.FairySnowheadInvisibleStaircase
                    || itemId >= Items.ItemSnowheadMap && itemId <= Items.ItemSnowheadKey3
                    || itemId == Items.ItemFireArrow;
            }
            if (dungeonId == 2)
            {
                return itemId >= Items.FairyGreatBayBioBaba && itemId <= Items.FairyGreatBayLobby
                    || itemId >= Items.ItemGreatBayMap && itemId <= Items.ItemGreatBayKey1
                    || itemId == Items.ItemIceArrow;
            }
            if (dungeonId == 3)
            {
                return itemId >= Items.FairyInvertedTowerLobby && itemId <= Items.FairyStoneTowerWindFunnel
                    || itemId >= Items.ItemStoneTowerMap && itemId <= Items.ItemStoneTowerKey4
                    || itemId == Items.ItemLightArrow || itemId == Items.MaskGiant;
            }
            return itemId >= Items.ItemWoodfallMap
                    && itemId <= Items.ItemStoneTowerKey4;
        }

        public static bool IsBottleCatchContent(int itemId)
        {
            return itemId >= Items.BottleCatchFairy
                   && itemId <= Items.BottleCatchMushroom;
        }

        public static bool IsMoonItem(int itemId)
        {
            return itemId >= Items.HeartPieceDekuTrial && itemId <= Items.MaskFierceDeity;
        }

        public static bool IsOtherItem(int itemId)
        {
            return itemId >= Items.ChestLensCaveRedRupee && itemId <= Items.IkanaScrubGoldRupee;
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

        public static bool IsHeartPiece(int itemId)
        {
            return (itemId >= Items.HeartPieceNotebookMayor && itemId <= Items.HeartPieceKnuckle)
                || (itemId >= Items.HeartPieceSouthClockTown && itemId <= Items.HeartContainerStoneTower)
                || (itemId >= Items.HeartPieceDekuTrial && itemId <= Items.HeartPieceLinkTrial)
                || itemId == Items.ChestSecretShrineHeartPiece
                || itemId == Items.HeartPieceBank;
        }

        public static bool IsStartingItem(int itemId)
        {
            return itemId == Items.MaskDeku || itemId == Items.SongHealing;
        }

        public static bool IsSong(int itemId)
        {
            return itemId >= Items.SongHealing
                && itemId <= Items.SongOath;
        }
    }
}
