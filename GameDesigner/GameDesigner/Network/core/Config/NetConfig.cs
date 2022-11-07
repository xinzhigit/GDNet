using System;
using System.Collections.Generic;
using System.IO;

namespace Net.Config
{
    public class Config 
    {
        private static bool init;
        private static bool useMemoryStream = true;
        /// <summary>
        /// 使用内存流进行缓存? 默认是文件流缓存, 速度会比较慢, 运行内存占用比较小!
        /// 使用内存流缓存速度会比较快, 但运行内存占用比较大
        /// </summary>
        [Obsolete("文件流已废弃, 统一内存流")]
        public static bool UseMemoryStream
        {
            get 
            {
                Init();
                return useMemoryStream;
            }
            set 
            {
                useMemoryStream = value;
                Save();
            }
        }
        private static int baseCapacity = 1024;
        /// <summary>
        /// 内存接收缓冲区基础容量 默认1024
        /// </summary>
        public static int BaseCapacity
        {
            get
            {
                Init();
                return baseCapacity;
            }
            set
            {
                baseCapacity = value;
                Save();
            }
        }

        public static string BasePath;

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#else
        [Net.Share.RuntimeInitializeOnLoadMethod]
#endif
        public static void InitBasePath()
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
#if UNITY_STANDALONE || UNITY_WSA
            var streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
                Directory.CreateDirectory(streamingAssetsPath);
            var path = streamingAssetsPath;
#else
            var path = UnityEngine.Application.persistentDataPath;
#endif
#else
            var path = AppDomain.CurrentDomain.BaseDirectory;
#endif
            BasePath = path;
        }

        private static void Init()
        {
            if (init)
                return;
            init = true;
            var configPath = BasePath + "/network.config";
            if (File.Exists(configPath))
            {
                var textRows = File.ReadAllLines(configPath);
                foreach (var item in textRows)
                {
                    if (item.Contains("{"))//旧版本json存储
                    {
                        Save();
                        break;
                    }
                    var texts = item.Split('=');
                    var key = texts[0].Trim().ToLower();
                    var value = texts[1].Split('#')[0].Trim();
                    switch (key)
                    {
                        case "usememorystream":
                            useMemoryStream = bool.Parse(value);
                            break;
                        case "basecapacity":
                            BaseCapacity = int.Parse(value);
                            break;
                    }
                }
            }
            else
            {
                Save();
            }
        }

        private static void Save()
        {
            var list = new List<string>();
            var text = $"useMemoryStream={useMemoryStream}#使用运行内存作为缓冲区? 否则使用文件流作为缓冲区";
            list.Add(text);
            text = $"baseCapacity={baseCapacity}#当客户端连接时分配的初始缓冲区大小";
            list.Add(text);
            var configPath = BasePath + "/network.config";
            File.WriteAllLines(configPath, list);
        }
    }
}
