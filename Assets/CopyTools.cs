using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public static class CopyTools {

   public static void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            Directory.CreateDirectory(srcPath);
        }
        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }
        CopyFile(srcPath, tarPath);
        string[] directionName = Directory.GetDirectories(srcPath);
        foreach (string dirPath in directionName)
        {
            string directionPathTemp = tarPath + "/" + dirPath.Substring(srcPath.Length + 1);
            CopyFolder(dirPath, directionPathTemp);
        }
    }
    public static void CopyFile(string srcPath, string tarPath)
    {
        string[] filesList = Directory.GetFiles(srcPath);
        foreach (string f in filesList)
        {
            string fTarPath = tarPath + "/" + f.Substring(srcPath.Length + 1);
            if (File.Exists(fTarPath))
            {
                File.Copy(f, fTarPath, true);
            }
            else
            {
                File.Copy(f, fTarPath);
            }
        }
    }

}
