using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace Infrastructure
{
    public static class FileIOHelper
    {

        /// <summary>
        /// This is used to determine whether or not a particular file has been imported into the database on a previous occasion
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static string GenerateFileHashCode(string fullFileName)
        {
            string hashCode = String.Empty;
            string fileContent = File.ReadAllText(fullFileName);
            hashCode = MD5Helper.GetMD5TextHashCode(fileContent);
            return hashCode;
        }

        public static int GetFileLineCount(string fullFileName)
        {
            int dataFileLineCount = 0;
            List<string> dataFileLines = new List<string>();
            try
            {
                dataFileLines = File.ReadAllLines(fullFileName).ToList();
                dataFileLineCount = dataFileLines.Count;
                return dataFileLineCount;
            }
            catch (Exception)
            {
                throw new Exception(String.Format("Unable to open {0}.", fullFileName));
            }
        }

        public static int GetImportableLineCount(string fullFileName, bool dataFileIcludesColumnHeadings, int firstLineToImport)
        {
            int importableLineCount = 0;
            importableLineCount = GetFileLineCount(fullFileName);

            if (dataFileIcludesColumnHeadings)
            {
                // if there are 20000 lines and the first line is the column header
                // only 19999 lines are considered importable.
                // 20000 - 2 + 1 = 19999
                importableLineCount = importableLineCount - firstLineToImport + 1;
            }
            return importableLineCount;
        }

        public static string ExtractPathFromFullFileName(string fullFileName)
        {
            return Path.GetDirectoryName(fullFileName);
        }

        public static string ExtractFileNameFromPath(string fullFileName)
        {
            return Path.GetFileName(fullFileName);
        }

        public static bool CanOpenFilesFromDirectory(string directoryPath)
        {
            return HasDirectoryAccess(FileSystemRights.Read, directoryPath);
        }

        public static bool HasDirectoryAccess(FileSystemRights fileSystemRights, string directoryPath)
        {
            DirectorySecurity directorySecurity = Directory.GetAccessControl(directoryPath);

            foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
            {
                if ((rule.FileSystemRights & fileSystemRights) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsFileLocked(string fullFileName)
        {
            FileInfo info = new FileInfo(fullFileName);
            return IsFileLocked(info);
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            //file is not locked
            return false;
        }

        public static string GenerateNetworkAccessiblePathFromLocalPath(string directoryPath)
        {
            string pcName = Environment.MachineName;
            string prefix = String.Format(@"\\{0}\", pcName.Trim());
            return directoryPath.Replace(directoryPath.Substring(0, 3), prefix);
        }
    }
}
