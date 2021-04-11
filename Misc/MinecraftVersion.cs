using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.Data
{
    public struct MinecraftVersion
    {
        private static class VersionInfo
        {
            public static readonly VersionKey[] Versions;
            static VersionInfo()
            {
                List<VersionKey> VersionList = new List<VersionKey>();
                string[] lineSplit = Resources.VersionData.Split('\n');
                foreach (string versionLine in lineSplit)
                {
                    string[] tabSplit = versionLine.Split('\t');
                    VersionKey vk = new VersionKey();
                    vk.VersionName = tabSplit[0];
                    vk.ProtocolVersion = int.Parse(tabSplit[1]);
                    if (tabSplit.Length == 3)
                    {
                        vk.BaseVersionName = tabSplit[2];
                    } else
                    {
                        vk.BaseVersionName = vk.VersionName;
                    }
                    VersionList.Add(vk);
                    //Console.WriteLine(vk.ProtocolVersion + " " + vk.VersionName + " " + vk.BaseVersionName);
                }
                Versions = VersionList.ToArray();
            }
            public struct VersionKey
            {
                public string VersionName;
                public int ProtocolVersion;
                public string BaseVersionName;
            }
        }

        private VersionInfo.VersionKey versionKey;
        private bool isInvalid;
        private bool createdFromProtocolVersion;
        public MinecraftVersion(int ProtocolVersion)
        {
            createdFromProtocolVersion = true;
            versionKey = new VersionInfo.VersionKey();
            isInvalid = true;
            foreach (VersionInfo.VersionKey version in VersionInfo.Versions)
            {
                if (version.ProtocolVersion == ProtocolVersion)
                {
                    versionKey = version;
                    isInvalid = false;
                    break;
                }
            }
        }
        public MinecraftVersion(string VersionName)
        {
            createdFromProtocolVersion = false;
            versionKey = new VersionInfo.VersionKey();
            isInvalid = true;
            foreach (VersionInfo.VersionKey version in VersionInfo.Versions)
            {
                if (version.VersionName == VersionName)
                {
                    versionKey = version;
                    isInvalid = false;
                    break;
                }
            }
        }
        public string VersionName
        {
            get
            {
                if (createdFromProtocolVersion)
                {
                    return versionKey.BaseVersionName;
                } else
                {
                    return versionKey.VersionName;
                }
            }
        }
        public int ProtocolVersion
        {
            get
            {
                return versionKey.ProtocolVersion;
            }
        }
        public IEnumerable<string> GetAllCompatibleVersionNames()
        {
            foreach (VersionInfo.VersionKey version in VersionInfo.Versions)
            {
                if (version.ProtocolVersion == versionKey.ProtocolVersion)
                {
                    yield return version.VersionName;
                }
            }
        }
        public static bool operator >(MinecraftVersion a, MinecraftVersion b)
        {
            return a.versionKey.ProtocolVersion > b.versionKey.ProtocolVersion;
        }
        public static bool operator <(MinecraftVersion a, MinecraftVersion b)
        {
            return a.versionKey.ProtocolVersion < b.versionKey.ProtocolVersion;
        }
    }
}
