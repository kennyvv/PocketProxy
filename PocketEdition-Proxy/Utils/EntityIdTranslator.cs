namespace PocketProxy.Utils
{
    public class EntityIdTranslator
    {
        public static EntityData GetEntityId(int id)
        {
            bool translated = true;
            bool isMob = false;
            byte entityType = (byte)id;
            switch (entityType)
            {
                case 81: //Snowball
                    entityType = 61;
                    break;
                case 65: //Primed TNT
                    entityType = 50;
                    break;
                case 66: //Falling Block
                    entityType = 70;
                    break;
                case 80: //Shot Arrow
                    entityType = 60;
                    break;
                case 12: //Pig
                    entityType = 90;
                    isMob = true;
                    break;
                case 11: //Chicken
                    entityType = 93;
                    isMob = true;
                    break;
                case 10: //Cow
                    entityType = 92;
                    isMob = true;
                    break;
                case 13: //Sheep
                    entityType = 91;
                    isMob = true;
                    break;
                case 14: //Wolf
                    entityType = 95;
                    isMob = true;
                    break;
                case 15: //Villager
                    entityType = 120;
                    isMob = true;
                    break;
                case 16: //Mooshroom
                    entityType = 96;
                    isMob = true;
                    break;
                case 17: //Squid
                    entityType = 94;
                    isMob = true;
                    break;
                case 18: //Rabbit
                    entityType = 101;
                    isMob = true;
                    break;
                case 19: //Bat
                    entityType = 65;
                    isMob = true;
                    break;
                case 20: //Iron Golem
                    entityType = 99;
                    isMob = true;
                    break;
                case 21: //Snow Golem
                    entityType = 97;
                    isMob = true;
                    break;
                case 22: //Ocelot
                    entityType = 98;
                    isMob = true;
                    break;
                case 32: //Zombie
                    entityType = 54;
                    isMob = true;
                    break;
                case 33: //Creeper
                    entityType = 50;
                    isMob = true;
                    break;
                case 34: //Skeleton
                    entityType = 51;
                    isMob = true;
                    break;
                case 35: //Spider
                    entityType = 52;
                    isMob = true;
                    break;
                case 36: //Zombie Pigman
                    entityType = 57;
                    isMob = true;
                    break;
                case 37: //Slime
                    entityType = 55;
                    isMob = true;
                    break;
                case 38: //Enderman
                    entityType = 58;
                    isMob = true;
                    break;
                case 39: //Silverfish
                    entityType = 60;
                    isMob = true;
                    break;
                case 40: //Cave Spider
                    entityType = 59;
                    isMob = true;
                    break;
                case 41: //Ghast
                    entityType = 56;
                    isMob = true;
                    break;
                case 42: //Magma Cube
                    entityType = 62;
                    isMob = true;
                    break;
                case 43: //Blaze
                    entityType = 61;
                    isMob = true;
                    break;
                case 44: //Zombie Villager
                    entityType = 54;
                    isMob = true;
                    break;
                default:
                    entityType = (byte) id;
                    translated = false;
                    break;
            }

            return new EntityData()
            {
                Id = entityType,
                IsMob = isMob,
                Translated = translated
            };
        }

        public struct EntityData
        {
            public byte Id { get; set; }
            public bool IsMob { get; set; }
            public bool Translated { get; set; }
        }
    }
}
