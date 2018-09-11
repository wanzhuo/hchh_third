using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace ZRui.Web.Common
{
    public static class FileUtils
    {
        public static void CreateDirectory(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        public static void CopyDirectory(string fromDir, string toDir, bool overwrite = false, bool ignoreError = false)
        {
            if (!Directory.Exists(fromDir)) return;

            //如果源文件夹不存在，则创建
            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }

            string[] files = Directory.GetFiles(fromDir);
            foreach (string formFileName in files)
            {
                string fileName = Path.GetFileName(formFileName);
                string toFileName = Path.Combine(toDir, fileName);
                try
                {
                    File.Copy(formFileName, toFileName, overwrite);
                }
                catch (Exception ex)
                {
                    if (!ignoreError)
                    {
                        throw ex;
                    }
                }
            }
            string[] fromDirs = Directory.GetDirectories(fromDir);
            foreach (string fromDirName in fromDirs)
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDirectory(fromDirName, toDirName, overwrite);
            }
        }

    }
}
