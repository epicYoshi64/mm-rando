using MMRando.Models;
using MMRando.Models.Rom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMRando.Utils
{

    public class SceneUtils
    {
        const int SCENE_TABLE = 0xC5A1E0;
        const int SCENE_FLAG_MASKS = 0xC5C500;
        public static void ResetSceneFlagMask()
        {
            ReadWriteUtils.WriteToROM(SCENE_FLAG_MASKS, (uint)0);
            ReadWriteUtils.WriteToROM(SCENE_FLAG_MASKS + 0xC, (uint)0);
        }

        public static void UpdateSceneFlagMask(int num)
        {
            int offset = num >> 3;
            int mod = offset % 16;
            if (mod < 4)
            {
                offset += 8;
            }
            else if (mod < 12)
            {
                offset -= 4;
            }

            int bit = 1 << (num & 7);
            int f = RomUtils.GetFileIndexForWriting(SCENE_FLAG_MASKS);
            int addr = SCENE_FLAG_MASKS - RomData.MMFileList[f].Addr + offset;
            RomData.MMFileList[f].Data[addr] |= (byte)bit;
        }

        public static void ReadSceneTable()
        {
            RomData.SceneList = new List<Scene>();
            int f = RomUtils.GetFileIndexForWriting(SCENE_TABLE);
            int _SceneTable = SCENE_TABLE - RomData.MMFileList[f].Addr;
            int i = 0;
            while (true)
            {
                Scene s = new Scene();
                uint saddr = ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, _SceneTable + i);
                if (saddr > 0x4000000)
                {
                    break;
                }
                if (saddr != 0)
                {
                    s.File = RomUtils.AddrToFile((int)saddr);
                    s.Number = i >> 4;
                    RomData.SceneList.Add(s);
                }
                i += 16;
            }
        }

        public static void GetMaps()
        {
            for (int i = 0; i < RomData.SceneList.Count; i++)
            {
                int f = RomData.SceneList[i].File;
                RomUtils.CheckCompressed(f);
                int j = 0;
                while (true)
                {
                    byte cmd = RomData.MMFileList[f].Data[j];
                    if (cmd == 0x04)
                    {
                        byte mapcount = RomData.MMFileList[f].Data[j + 1];
                        int mapsaddr = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, j + 4) & 0xFFFFFF;
                        for (int k = 0; k < mapcount; k++)
                        {
                            Map m = new Map();
                            m.File = RomUtils.AddrToFile((int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, mapsaddr));
                            RomData.SceneList[i].Maps.Add(m);
                            mapsaddr += 8;
                        }
                        break;
                    }
                    if (cmd == 0x14)
                    {
                        break;
                    }
                    j += 8;
                }
            }
        }

        public static void GetMapHeaders()
        {
            for (int i = 0; i < RomData.SceneList.Count; i++)
            {
                int maps = RomData.SceneList[i].Maps.Count;
                for (int j = 0; j < maps; j++)
                {
                    int f = RomData.SceneList[i].Maps[j].File;
                    RomUtils.CheckCompressed(f);
                    int k = 0;
                    int setupsaddr = -1;
                    int nextlowest = -1;
                    while (true)
                    {
                        byte cmd = RomData.MMFileList[f].Data[k];
                        if (cmd == 0x18)
                        {
                            setupsaddr = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, k + 4) & 0xFFFFFF;
                        }
                        else if (cmd == 0x14)
                        {
                            break;
                        }
                        else
                        {
                            if (RomData.MMFileList[f].Data[k + 4] == 0x03)
                            {
                                int p = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, k + 4) & 0xFFFFFF;
                                if (((p < nextlowest) || (nextlowest == -1)) && ((p > setupsaddr) && (setupsaddr != -1)))
                                {
                                    nextlowest = p;
                                }
                            }
                        }
                        k += 8;
                    }
                    if ((setupsaddr == -1) || (nextlowest == -1))
                    {
                        continue;
                    }
                    for (k = setupsaddr; k < nextlowest; k += 4)
                    {
                        byte s = RomData.MMFileList[f].Data[k];
                        if (s != 0x03)
                        {
                            break;
                        }
                        int p = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, k) & 0xFFFFFF;
                        Map m = new Map();
                        m.File = f;
                        m.Header = p;
                        RomData.SceneList[i].Maps.Add(m);
                    }
                }
            }
        }

        private static List<Actor> ReadMapActors(byte[] Map, int Addr, int Count)
        {
            List<Actor> Actors = new List<Actor>();
            for (int i = 0; i < Count; i++)
            {
                Actor a = new Actor();
                ushort an = ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16));
                a.m = an & 0xF000;
                a.n = an & 0x0FFF;
                a.p.x = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 2);
                a.p.y = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 4);
                a.p.z = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 6);
                a.r.x = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 8);
                a.r.y = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 10);
                a.r.z = (short)ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 12);
                a.v = ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 16) + 14);
                Actors.Add(a);
            }
            return Actors;
        }

        private static List<int> ReadMapObjects(byte[] Map, int Addr, int Count)
        {
            List<int> Objects = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                Objects.Add(ReadWriteUtils.Arr_ReadU16(Map, Addr + (i * 2)));
            }
            return Objects;
        }

        private static void WriteMapActors(byte[] Map, int Addr, List<Actor> Actors)
        {
            for (int i = 0; i < Actors.Count; i++)
            {
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16), (ushort)(Actors[i].m | Actors[i].n));
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 2, (ushort)Actors[i].p.x);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 4, (ushort)Actors[i].p.y);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 6, (ushort)Actors[i].p.z);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 8, (ushort)Actors[i].r.x);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 10, (ushort)Actors[i].r.y);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 12, (ushort)Actors[i].r.z);
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 16) + 14, (ushort)Actors[i].v);
            }
        }

        private static void WriteMapObjects(byte[] Map, int Addr, List<int> Objects)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                ReadWriteUtils.Arr_WriteU16(Map, Addr + (i * 2), (ushort)Objects[i]);
            }
        }

        private static void UpdateMap(Map M)
        {
            WriteMapActors(RomData.MMFileList[M.File].Data, M.ActorAddr, M.Actors);
            WriteMapObjects(RomData.MMFileList[M.File].Data, M.ObjAddr, M.Objects);
        }

        public static void UpdateScene(Scene scene)
        {
            for (int i = 0; i < scene.Maps.Count; i++)
            {
                UpdateMap(scene.Maps[i]);
            }
        }

        public static void GetActors()
        {
            for (int i = 0; i < RomData.SceneList.Count; i++)
            {
                for (int j = 0; j < RomData.SceneList[i].Maps.Count; j++)
                {
                    int f = RomData.SceneList[i].Maps[j].File;
                    RomUtils.CheckCompressed(f);
                    int k = RomData.SceneList[i].Maps[j].Header;
                    while (true)
                    {
                        byte cmd = RomData.MMFileList[f].Data[k];
                        if (cmd == 0x01)
                        {
                            byte ActorCount = RomData.MMFileList[f].Data[k + 1];
                            int ActorAddr = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, k + 4) & 0xFFFFFF;
                            RomData.SceneList[i].Maps[j].ActorAddr = ActorAddr;
                            RomData.SceneList[i].Maps[j].Actors = ReadMapActors(RomData.MMFileList[f].Data, ActorAddr, ActorCount);
                        }
                        if (cmd == 0x0B)
                        {
                            byte ObjectCount = RomData.MMFileList[f].Data[k + 1];
                            int ObjectAddr = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, k + 4) & 0xFFFFFF;
                            RomData.SceneList[i].Maps[j].ObjAddr = ObjectAddr;
                            RomData.SceneList[i].Maps[j].Objects = ReadMapObjects(RomData.MMFileList[f].Data, ObjectAddr, ObjectCount);
                        }
                        if (cmd == 0x14)
                        {
                            break;
                        }
                        k += 8;
                    }
                }
            }
        }

        public static List<Actor> GetSceneActorsByNumber(int sceneNumber, int actorNumber)
        {
            List<Actor> searchActors = new List<Actor>();
            Scene searchScene = RomData.SceneList.Single(s => s.Number == sceneNumber);
            foreach (Map map in searchScene.Maps)
            {
                searchActors.AddRange(map.Actors.Where(a => a.n == actorNumber));
            }
            return searchActors;
        }

        public static void UpdateSceneByNumber(int sceneNumber)
        {
            Scene sceneToUpdate = RomData.SceneList.Single(s => s.Number == sceneNumber);
            UpdateScene(sceneToUpdate);
        }

        public static void WriteDungeonFairiesToChests(List<ItemObject> itemList)
        {
            ReadSceneTable();
            GetMaps();
            GetMapHeaders();
            GetActors();
            // WF, SH, GB, ST, IST
            int[] dungeonScenes = new int[] { 0x1B, 0x21, 0x49, 0x16 };
            // WF
            // spinning flower fairy, first room fairy, Compass, Small Key, Map, Bow, BK, dark room fairy
            List<List<int>> dungeonItemIndices = new List<List<int>>() {
                new List<int>(){
                    Items.FairyWoodfallFlower, Items.FairyWoodfallLobby, Items.ItemWoodfallCompass, Items.ItemWoodfallKey1,
                    Items.ItemWoodfallMap, Items.ItemBow, Items.ItemWoodfallBossKey, Items.FairyWoodfallBoe
                },
                new List<int>()
                {
                    Items.FairySnowheadWhiteRoom, Items.ItemSnowheadCompass, Items.ItemSnowheadKey3,
                    Items.FairySnowheadYellowRoom, Items.ItemSnowheadKey1, Items.FairySnowheadBottom,
                    Items.ItemSnowheadBossKey, Items.FairySnowheadDeku, Items.FairySnowheadGreenRoom,
                    Items.ItemFireArrow, Items.FairySnowheadIceStalactite, Items.ItemSnowheadKey2,
                    Items.FairySnowheadInvisibleStaircase, Items.ItemSnowheadMap
                },
                new List<int>()
                {
                    Items.ItemGreatBayMap, Items.FairyGreatBayBioBaba, Items.ItemGreatBayBossKey,
                    Items.ItemGreatBayKey1, Items.ItemGreatBayCompass, Items.ItemIceArrow,
                    Items.FairyGreatBayWaterWheel, Items.FairyGreatBayAlcove, Items.FairyGreatBaySeesaw,
                    Items.FairyGreatBayReservoirs, Items.FairyGreatBayLobby
                },
                new List<int>()
                {
                    Items.FairyInvertedTowerLobby, Items.FairyStoneTowerLowerLobby, Items.FairyStoneTowerUpperLobby,
                    Items.FairyStoneTowerEyegore, Items.MaskGiant, Items.ItemStoneTowerKey2, Items.ItemStoneTowerKey4,
                    Items.FairyStoneTowerWaterSunblock, Items.FairyInvertedTowerWindFunnel, Items.FairyInvertedTowerFrozenEyeSwitch,
                    Items.FairyStoneTowerUnderwaterSunSwitch, Items.ItemStoneTowerKey3, Items.ItemStoneTowerCompass,
                    Items.FairyInvertedTowerWizzrobe, Items.ItemStoneTowerKey1, Items.FairyStoneTowerWizzrobe,
                    Items.ItemStoneTowerMap, Items.FairyStoneTowerMirrorSunSwitch, Items.FairyStoneTowerMirrorRoom,
                    Items.FairyStoneTowerSpikeRollers, Items.FairyStoneTowerTimedFireRing, Items.FairyStoneTowerWindFunnel,
                    Items.ItemLightArrow, Items.ItemStoneTowerBossKey
                }
            };
            int d = 0, i, j;
            List<Actor> invertedStoneTowerChests = GetSceneActorsByNumber(0x18, 0x006);
            List<ItemObject> fairyChests, itemChests;
            ItemObject displacedItem;
            Dictionary<int, int> itemUpdates = new Dictionary<int, int>();
            short itemValue = 0x00, fairyValue = 0x00;
            ushort getItemMask = 0xFE0, chestFlagMask = 0x1F;
            int chestFlagBits = 5;
            foreach (int dungeonSceneNumber in dungeonScenes)
            {
                List<Actor> templeChests = GetSceneActorsByNumber(dungeonSceneNumber, 0x0006);
                // these chests are set to have an item in them
                itemChests = itemList.Where(item => ItemUtils.IsStrayFairy(item.ID) && ItemUtils.IsDungeonItem(item.ReplacesItemId,d) && !ItemUtils.IsStrayFairy(item.ReplacesItemId)).ToList();
                // these chests are set to have fairies in them
                fairyChests = itemList.Where(item => ItemUtils.IsStrayFairy(item.ReplacesItemId) && ItemUtils.IsDungeonItem(item.ReplacesItemId, d) && !ItemUtils.IsStrayFairy(item.ID)).ToList();
                foreach (ItemObject fairy in fairyChests)
                {
                    if (itemChests.Count > 0)
                    {
                        displacedItem = itemChests[0];
                        itemChests.RemoveAt(0);
                        i = dungeonItemIndices[d].FindIndex(c => c == fairy.ReplacesItemId);
                        j = dungeonItemIndices[d].FindIndex(c => c == displacedItem.ReplacesItemId);

                        if (i != -1 && j != -1)
                        {
                            fairyValue = (short)((templeChests[i].v & getItemMask) >> chestFlagBits);
                            itemValue = (short)((templeChests[j].v & getItemMask) >> chestFlagBits);

                            templeChests[i].v = (itemValue << chestFlagBits) + (templeChests[i].v & (0xF000 | chestFlagMask));
                            templeChests[j].v = (fairyValue << chestFlagBits) + (templeChests[j].v & (0xF000 | chestFlagMask));
                            if (d == 3)
                            {
                                fairyValue = (short)((invertedStoneTowerChests[i].v & getItemMask) >> chestFlagBits);
                                itemValue = (short)((invertedStoneTowerChests[j].v & getItemMask) >> chestFlagBits);

                                invertedStoneTowerChests[i].v = (itemValue << chestFlagBits) + (invertedStoneTowerChests[i].v & (0xF000 | chestFlagMask));
                                invertedStoneTowerChests[j].v = (fairyValue << chestFlagBits) + (invertedStoneTowerChests[j].v & (0xF000 | chestFlagMask));
                            }
                        }
                    }
                }
                UpdateSceneByNumber(dungeonScenes[d]);
                d++;
            }
        }

        public static void ShuffleSceneActorsInPlace(ushort[] sceneNumbers, int[] actorTypes, ushort[] actorMasks)
        {
            ReadSceneTable();
            GetMaps();
            GetMapHeaders();
            GetActors();
            List<Actor> actorPool = new List<Actor>();
            Dictionary<int, List<ushort>> actorContents;
            ushort contents;
            int type;
            foreach (ushort scene in sceneNumbers)
            {
                type = 0;
                actorContents = new Dictionary<int, List<ushort>>();
                foreach (int actorType in actorTypes)
                {
                    foreach (Actor actor in GetSceneActorsByNumber(scene, actorType))
                    {
                        actorPool.Add(actor);
                        contents = (ushort)(actor.v & actorMasks[type]);
                        if (!actorContents.ContainsKey(actorType))
                        {
                            actorContents.Add(actorType, new List<ushort>());
                        }
                        actorContents[actorType].Add(contents);
                        System.Diagnostics.Debug.WriteLine(actor.v.ToString("X4"));
                    }
                    type++;
                }
                System.Diagnostics.Debug.WriteLine("");

                if (actorContents.Count > 0)
                {
                    Random RNG = new Random();
                    int i, m;
                    foreach (Actor actor in actorPool)
                    {
                        m = actorTypes.ToList().FindIndex(a => a == actor.n);
                        if (actorContents.ContainsKey(actor.n) && actorContents[actor.n].Count > 0 && m != -1 && m >= 0 && m < actorMasks.Length)
                        {
                            i = RNG.Next(actorContents[actor.n].Count);
                            actor.v = (actor.v & ~actorMasks[m]) + actorContents[actor.n][i];
                            actor.v = 0x0011;
                            actorContents[actor.n].RemoveAt(i);
                            System.Diagnostics.Debug.WriteLine(actor.v.ToString("X4"));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Couldn't place");
                        }
                    }
                    UpdateSceneByNumber(scene);
                }
            }
        }

        public static void UpdateSingleActor(ushort scene, int room, int i, int actor, int variable, int mask)
        {
            ReadSceneTable();
            GetMaps();
            GetMapHeaders();
            GetActors();
            Actor a = RomData.SceneList.Find(s => scene == s.Number).Maps[room].Actors[i];
            a.n = actor;
            a.v = (~mask & a.v) + (variable & mask);
            UpdateSceneByNumber(scene);
        }
    }
}