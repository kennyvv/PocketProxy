using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace PocketProxy.Utils
{
    public class PcSkin
    {
        private static readonly byte[] SteveSkin = DownloadSkin("steve");
        private static readonly Dictionary<string, byte[]> PeSkinCache = new Dictionary<string, byte[]>();

        public static byte[] GetPeSkin(string username)
        {
            if (!PeSkinCache.ContainsKey(username)) return SteveSkin;
            return PeSkinCache[username];
        }

        public static void AddSkinToCache(string username, byte[] data)
        {
            if (PeSkinCache.ContainsKey(username)) return;
            PeSkinCache.Add(username, data);
        }

        public static void RemoveSkinFromCache(string username)
        {
            if (!PeSkinCache.ContainsKey(username)) return;
            PeSkinCache.Remove(username);
        }

        public struct SkinValue
        {
            [JsonProperty("timestamp")]
            public string Timestamp { get; set; }

            [JsonProperty("profileId")]
            public string ProfileUUID { get; set; }

            [JsonProperty("profileName")]
            public string ProfileName { get; set; }

            [JsonProperty("isPublic")]
            public bool IsPublic { get; set; }

            [JsonProperty("textures")]
            public SkinCape Textures { get; set; }

            public SkinValue(string uuid, string username, string skinUrl)
            {
                Timestamp = "0";
                ProfileUUID = uuid;
                ProfileName = username;
                IsPublic = true;
                Textures = new SkinCape(skinUrl, skinUrl);
            }
        }

        public struct SkinCape
        {
            [JsonProperty("SKIN")]
            public UrlStruct SkinURL { get; set; }

            [JsonProperty("CAPE")]
            public UrlStruct CapeURL { get; set; }

            public struct UrlStruct
            {
                [JsonProperty("url")]
                public string Url { get; set; }
            }

            public SkinCape(string skinurl, string capeurl)
            {
                SkinURL = new UrlStruct() {Url = skinurl};
                CapeURL = new UrlStruct() {Url = capeurl};
            }
        }

        public static byte[] DownloadSkin(string username)
        {
            if (username.Length > 16) username = username.Substring(0, 16);

            var data = new WebClient().DownloadData(string.Format("https://minotar.net/skin/{0}", username));
            using (MemoryStream ms = new MemoryStream(data))
            {
                Bitmap bitmap = new Bitmap(ms);
                byte[] bytes = new byte[bitmap.Height*bitmap.Width*4];
                int i = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        bytes[i++] = color.R;
                        bytes[i++] = color.G;
                        bytes[i++] = color.B;
                        bytes[i++] = color.A;
                    }
                }
                return bytes;
            }
        }
    }
}