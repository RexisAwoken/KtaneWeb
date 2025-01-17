﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KtaneWeb.Puzzles;
using RT.Json;
using RT.Serialization;
using RT.Util.ExtensionMethods;

namespace KtaneWeb
{
    sealed class KtaneWebConfig
    {
#pragma warning disable 0649 // Field is never assigned to, and will always have its default value

        // User/password file for editing
        public string UsersFile;
        public string JavaScriptFile;
        public string CssFile;

        public string BaseDir = @"C:\Sites\KTANE\Public";
        public string[] DocumentDirs = new[] { "HTML", "PDF" };
        public string PdfDir = "PDF";
        public string[] OriginalDocumentIcons = new[] { "HTML/img/html_manual.png", "HTML/img/pdf_manual.png" };
        public string[] ExtraDocumentIcons = new[] { "HTML/img/html_manual_embellished.png", "HTML/img/pdf_manual_embellished.png" };
        public string PdfTempPath = null;

        public string ChromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        public string Archive7zPath = null;

        public string LogfilesDir = @"C:\Sites\KTANE\Logfiles";
        public string MergedPdfsDir = @"C:\Sites\KTANE\MergedPdfs";

#pragma warning restore 0649 // Field is never assigned to, and will always have its default value

        [ClassifyNotNull]
        public PuzzleInfo Puzzles = new PuzzleInfo();

        /// <summary>Maps from SessionID to Username.</summary>
        [ClassifyNotNull]
        public Dictionary<string, string> Sessions = new Dictionary<string, string>();

        public JsonList EnumerateSheetUrls(string moduleFileName, string[] notModuleNames)
        {
            if (moduleFileName == null)
                throw new ArgumentNullException(nameof(moduleFileName));

            var list = new HashSet<string>();
            for (int i = 0; i < DocumentDirs.Length; i++)
            {
                var dirInfo = new DirectoryInfo(Path.Combine(BaseDir, DocumentDirs[i]));
                var ext = DocumentDirs[i].ToLowerInvariant();
                foreach (var inf in dirInfo.EnumerateFiles($"{moduleFileName}.{ext}").Select(f => new { File = f, Icon = 2 * i }).Concat(dirInfo.EnumerateFiles($"{moduleFileName} *.{ext}").Select(f => new { File = f, Icon = 2 * i + 1 })))
                    if (!notModuleNames.Any(inf.File.Name.StartsWith))
                    {
                        list.Add($"{Path.GetFileNameWithoutExtension(inf.File.Name).Substring(moduleFileName.Length)}|{inf.File.Extension.Substring(1)}|{inf.Icon}");
                        if (ext == "html" && !inf.File.Name.Contains("interactive"))
                            list.Add($"{Path.GetFileNameWithoutExtension(inf.File.Name).Substring(moduleFileName.Length)}|pdf|{inf.Icon + 2}");
                    }
            }
            return list.OrderBy(x => x.Substring(0, x.IndexOf('|'))).ToJsonList();
        }
    }
}
