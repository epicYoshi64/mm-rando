using MMRando.Constants;
using MMRando.Models;
using MMRando.Models.Rom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MMRando.Utils
{

    public static class EntranceUtils
    {
        private static int GetEntranceAddr(int ent)
        {
            int offset = ((ent >> 9) * 12) + (Addresses.ExternalEntranceTable + 0x4);
            int f = RomUtils.GetFileIndexForWriting(offset);

            offset -= RomData.MMFileList[f].Addr;
            uint p1 = ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, offset);
            offset = ((ent >> 4) & 0x1F) * 4;

            p1 = (uint)((p1 & 0xFFFFFF) + 0xA96540 - RomData.MMFileList[f].Addr);
            p1 = ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, (int)(p1 + offset));
            p1 = (uint)((p1 & 0xFFFFFF) + 0xA96540 - RomData.MMFileList[f].Addr);

            offset = (ent & 0xF) << 2;
            return (int)p1 + offset;
        }

        public static void SetAllEntranceMusicContinue()
        {
            // for each internal scene
            for( int i = 0; i< 113; i++)
            {
                int offset = Addresses.ExternalEntranceTable + (i*0x0C);
                int f = RomUtils.GetFileIndexForWriting(offset);
                offset -= RomData.MMFileList[f].Addr;
                uint entranceCount = ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, offset);
                int entranceTableAddress = (int)ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, offset + 4);
                Debug.WriteLine($"{entranceCount} @ {entranceTableAddress}");
                f = RomUtils.GetFileIndexForWriting(entranceTableAddress);
                for (int j = 0; j < entranceCount; j++)
                {
                    offset = entranceTableAddress - RomData.MMFileList[f].Addr + (j * 0x04);
                    Debug.WriteLine($"{ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, offset)}");
                }
            }
        }

        public static void WriteEntrances(int[] olde, int[] newe)
        {
            int f = RomUtils.GetFileIndexForWriting(Addresses.ExternalEntranceTable + 0x4);
            uint[] data = new uint[newe.Length];

            for (int i = 0; i < newe.Length; i++)
            {
                data[i] = ReadWriteUtils.Arr_ReadU32(RomData.MMFileList[f].Data, GetEntranceAddr(newe[i]));
            }

            for (int i = 0; i < newe.Length; i++)
            {
                ReadWriteUtils.Arr_WriteU32(RomData.MMFileList[f].Data, GetEntranceAddr(olde[i]), data[i]);
            }
        }

    }

}