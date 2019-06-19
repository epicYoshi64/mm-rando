using MMRando.Models.Rom;
using System;
using System.Collections.Generic;

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

        public static void UpdateSceneByNumber(int SceneNumber)
        {
            for (int i = Math.Min(RomData.SceneList.Count, SceneNumber); i >= 0; i--)
            {
                if (RomData.SceneList[i].Number == SceneNumber)
                {
                    UpdateScene(RomData.SceneList[i]);
                }
            }
        }

        public static void WriteShuffledDungeonChests(int[][] ChestShuffle)
        {
            SceneUtils.ReadSceneTable();
            SceneUtils.GetMaps();
            SceneUtils.GetMapHeaders();
            SceneUtils.GetActors();
            // WF, SH, GB, ST, IST
            int[] DungeonScene = new int[] { 0x1B, 0x21, 0x49, 0x16, 0x18 };
            // WF
            // spinning flower fairy, first room fairy, Compass, Small Key, Map, Bow, BK, dark room fairy
            // SH
            // Compass Fairy, Compass, Freezard Bridge Key, Push block fairy, push block key
            // Lava Fairy, BK, Deku Flower Fairy, Freezard Stalagmite Fairy, Fire Arrows
            // Ice Stalactite Fairy, Ice Stalactite Key, Invisible Stair Platform Fairy, Map
            // GB
            // Map, Bio Baba Fairy, BK, Key, Compass, Ice Arrows, Water Wheel Fairy, Hidden Torch Fairy, 
            // Seesaw Fairy, Yellow/Green Reservoir Fairy, First Room Fairy
            // ST ... oh god ... I'll suffix inverted temple fairy chests with (i)
            // Remains Platform (i), Lower First Room, Upper First Room, Eyegore, Eyegore Sunblock,
            // Giant, Eyegore Key, Elegy Key, Wind Elevator (i), Frozen Eye Switch (i), Underwater Sun Switch [have to fact check if this is the one tied to the sun switch]
            // Beetle Guarded Key, Compass, Wizzrobe (i), Wizzrobe Key, Wizzrobe, Map [assuming that's the one in the back]
            // Mirror Shield Sun Switch, Sunblock Room, Post Light Arrows Room, Fire Ring, Rupee Nook, Light Arrows
            int DungeonSceneNumber;
            for (int d = 0; d < DungeonScene.Length; d++)
            {
                DungeonSceneNumber = DungeonScene[d];
                List<Actor> TempleChests = ActorUtils.GetSceneActorsByNumber(DungeonSceneNumber, 0x0006);
                List<short> ChestContents = new List<short>();
                ushort GetItemMask = 0xFF0, ChestFlagMask = 0xF;
                int ChestFlagBits = 4;
                int ChestTypeBits = 12;
                byte ChestType = 0;
                foreach (Actor OldChest in TempleChests)
                {
                    short GetItem = (short)((OldChest.v & GetItemMask) >> ChestFlagBits);
                    ChestContents.Add(GetItem);
                }

                // this let's us apply the same shuffling to IST that ST received
                int s = Math.Min(d, ChestShuffle.Length - 1);

                for (int i = 0; i < ChestShuffle[s].Length; i++)
                {
                    int ShuffledChest = ChestShuffle[s][i];
                    TempleChests[i].v = (ChestType << ChestTypeBits) + (ChestContents[ShuffledChest] << ChestFlagBits) + (TempleChests[i].v & (0xF000 | ChestFlagMask));
                }
                SceneUtils.UpdateSceneByNumber(DungeonScene[d]);
            }
        }

    }

}