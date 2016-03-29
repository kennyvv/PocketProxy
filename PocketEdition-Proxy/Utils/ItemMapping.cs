using System;

namespace PocketProxy.Utils
{
    public class ItemMapping
    {
        public static Mapping Pe2Pc(short itemid)
        {
            return Pe2Pc(itemid, 0);
        }

        public static Mapping Pe2Pc(short itemid, short metadata)
        {
            if (itemid == 325 && metadata == 8) return new Mapping(326);
            if (itemid == 325 && metadata == 10) return new Mapping(327);
            if (itemid == 373) //Potions...
            {  
                switch (metadata)
                {
                    case 7: //Invisibility 3600 0
                        break;
                    case 8: //Invisibility 9600 0
                        break;
                    case 9: //JumpBoost 3600 0
                        break;
                    case 10: //JumpBoost 9600 0
                        break;
                    case 11: //JumpBoost 1800 1
                        break;
                    case 12: //Fire Resistance 3600 0
                        metadata = 8195;
                        break;
                    case 13: //Fire Resistance 9600 0 
                        metadata = 8259;
                        break;
                    case 14: //Speed 3600, 0
                        metadata = 8194;
                        break;
                    case 15: //Speed 9600 0
                        metadata = 8258;
                        break;
                    case 16: //Speed 1800, 1
                        metadata = 8226;
                        break;
                    case 17: //Slowness 3600 0
                        metadata = 8202;
                        break;
                    case 18: //Slowness 4800 0
                        metadata = 8266;
                        break;
                    case 19: //Water Breathing 3600 0
                        break;
                    case 20: //Water Breathing 9600 0
                        break;
                    case 21: //InstantHealth 0 0
                        break;
                    case 22: //InstantHealth 0 1
                        break;
                    case 23: //Instant damage 0 0
                        break;
                    case 24: //Instant damage 0 1
                        break;
                    case 25: //Poison 900 0
                        metadata = 8196;
                        break;
                    case 26: //Posion 2400 0
                        metadata = 8260;
                        break;
                    case 27: //Poison 440 1
                        metadata = 8228;
                        break;
                    case 28: //Regeneration 900 0
                        metadata = 8257;
                        break;
                    case 29: //Regeneration 2400 0
                        metadata = 8193;
                        break;
                    case 30: //Regeneration 440 1
                        metadata = 8225;
                        break;
                    case 31: //Strength 3600 0
                        metadata = 8201;
                        break;
                    case 32: //Strength 9600 0
                        metadata = 8265;
                        break;
                    case 33: //Strength 1800 1
                        metadata = 8233;
                        break;
                    case 34: //Weakness 1800 0
                        metadata = 8200;
                        break;
                    case 35: //Weakness 4800 0
                        metadata = 8264;
                        break;
                    default:
                        Console.WriteLine("Potion: " + metadata);
                        metadata = 0;
                        break;
                }
            }
            return new Mapping(itemid, metadata);
        }
    }

    public struct Mapping
    {
        public short Itemid;
        public short Metadata;

        public Mapping(short id, short meta)
        {
            Itemid = id;
            Metadata = meta;
        }

        public Mapping(short id)
        {
            Itemid = id;
            Metadata = 0;
        }
    }
}
