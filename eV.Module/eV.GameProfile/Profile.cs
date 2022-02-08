// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using eV.GameProfile.Attributes;
using eV.GameProfile.Interface;
using log4net;
namespace eV.GameProfile
{
    public static class Profile
    {
        public static event Action? OnLoad;
        public static Dictionary<string, object> Config { get; private set; } = new();

        private static readonly ILog s_logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        private static DirectoryInfo? s_configRoot;
        private static string? s_nsName;
        private static string? s_configPath;
        private static IConfigParser? s_configParser;

        public static void Init(string nsName, string path,IConfigParser configParser, bool monitoringChange = false)
        {
            s_nsName = nsName;
            s_configPath = path;
            s_configRoot = new DirectoryInfo(s_configPath);
            s_configParser = configParser;

            Load();

            if (!monitoringChange)
                return;
            Monitoring();
        }

        private static void Load()
        {
            if (s_configParser == null)
            {
                s_logger.Error("GameProfile not init");
                return;
            }

            var configType = RegisterConfig();
            var configJsonString = LoadConfig();

            Config = s_configParser.Parser(configType, configJsonString);
            OnLoad?.Invoke();

            s_logger.Info("Game profile load");
        }

        public static void AssignmentConfigObject<T>(T co)
        {
            foreach (PropertyInfo propertyInfo in co?.GetType().GetProperties()!)
            {
                if (!Config.TryGetValue(propertyInfo.Name, out object? value))
                    continue;
                propertyInfo.SetValue(co, value);
            }
        }

        private static Dictionary<string, string> LoadConfig()
        {
            Dictionary<string, string> result = new();
            if (s_configRoot == null)
            {
                s_logger.Error("GameProfile not init");
            }
            else
            {
                foreach (FileInfo file in s_configRoot.GetFiles())
                {
                    try
                    {
                        string text = File.ReadAllText(file.FullName);
                        result.Add(file.Name.Split('.')[0], text);
                    }
                    catch (Exception e)
                    {
                        s_logger.Error(e.Message, e);
                    }
                }
            }
            return result;
        }

        private static  Dictionary<string, Type>  RegisterConfig()
        {
            Dictionary<string, Type> result = new();
            if (s_nsName == null)
            {
                s_logger.Error("GameProfile not init");
            }
            else
            {
                Type[] allTypes = Assembly.Load(s_nsName).GetExportedTypes();

                foreach (Type type in allTypes)
                {
                    object[] profileAttributes = type.GetCustomAttributes(typeof(ProfileStructureAttribute), true);

                    if (profileAttributes.Length <= 0)
                        continue;

                    result.Add(type.Name, type);
                }
            }
            return result;
        }

        private static void Monitoring()
        {
            if (s_configPath == null)
            {
                s_logger.Error("GameProfile not init");
                return;
            }

            FileSystemWatcher watcher = new(s_configPath);
            watcher.Filter = "*.json";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += (_, _) =>
            {
                Load();
            };
            watcher.Created += (_, _) =>
            {
                Load();
            };
            watcher.Deleted +=  (_, _) =>
            {
                Load();
            };
            watcher.Renamed +=  (_, _) =>
            {
                Load();
            };
            watcher.Error += (_, args) =>
            {
                s_logger.Error(args.GetException().Message, args.GetException());
            };
        }
    }
}
