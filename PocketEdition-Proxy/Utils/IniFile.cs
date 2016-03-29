using System.Runtime.InteropServices;
using System.Text;

namespace PocketProxy.Utils
{
    public class IniFile
    {
        public string Path;

        private string Exe => System.IO.Path.GetFileNameWithoutExtension(Path);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile(string iniPath)
        {
            Path = iniPath;
        }

        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? Exe, key, "", retVal, 255, Path);
            return retVal.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? Exe, key, value, Path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? Exe);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? Exe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}