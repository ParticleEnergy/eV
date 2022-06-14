// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;
using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Model;
namespace eV.Tool.ExcelToJson.Core;

public static class File
{
    public static List<ExcelInfo> GetFiles(string path)
    {
        List<ExcelInfo> result = new();
        DirectoryInfo directoryInfo = new(path);

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            string[] f = file.Name.Split(".");
            string fileName = f[0];
            string type = f[1];
            if (!(type.Equals("xlsx") || type.Equals("xlx")))
            {
                Logger.Error($"{file.FullName} File type is not excel");
                continue;
            }
            if (!CheckFileName(fileName))
            {
                Logger.Error($"{file.FullName} File names can only consist of letters");
                continue;
            }
            result.Add(new ExcelInfo
            {
                FileName = fileName,
                FileType = type,
                FilePath = file.FullName
            });
        }

        return result;
    }

    private static bool CheckFileName(string name)
    {
        return Regex.IsMatch(name, "^\\S[a-zA-Z]*$");
    }

    public static void InitOutJsonPath(string path)
    {
        DirectoryInfo dir = new(path);
        FileInfo[] files = dir.GetFiles();
        try
        {
            foreach (var item in files)
            {
                System.IO.File.Delete(item.FullName);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message ,e);
        }
    }

    public static void InitOutClassPath(string path)
    {
        DirectoryInfo dir = new(path);
        FileInfo[] files = dir.GetFiles();
        try
        {
            foreach (var item in files)
            {
                System.IO.File.Delete(item.FullName);
            }

            DirectoryInfo objectDir = new(path);
            FileInfo[] objectFiles = objectDir.GetFiles();
            foreach (var i in objectFiles)
            {
                System.IO.File.Delete(i.FullName);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message ,e);
        }
    }

    public static void Write(string file, string data)
    {
        System.IO.File.WriteAllText(file, data, new UTF8Encoding(false));
    }
}
