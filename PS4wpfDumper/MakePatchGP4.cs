using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PS4wpfDumper
{
    class MakePatchGP4
    {
        public class FileProperties
        {
            public string Path { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime LastAccessTime { get; set; }
            public DateTime LastWriteTime { get; set; }
            public string AbsolutePath
            {
                get
                {
                    return new FileInfo(Path).FullName;
                }
            }
        }

        public static void makePatchGP4(string patchdirectoryPath, string CONTENT_ID, string BASE_PKG)
        {
            string[] filePatternsToIgnore = { "icon0.png", "disc_info.dat", "right.sprx", "icon0.dds", "license.dat", "license.info", "pic0.dds", "pic1.dds", "playgo-chunk.dat", "playgo-chunk.sha", "playgo-manifest.xml", "psreserved.dat", "sce_discmap.plt", "sce_discmap_patch.plt", "npcommid.dat", "pic0.png", "pic1.png", "playgo-chunk.dat", "playgo-chunk.sha", "playgo-manifest.xml", "psreserved.dat",  };//"keystone",

            //if(Directory.Exists($@"{patchdirectoryPath}\sce_sys\about"))
            //{
            //    Directory.Delete($@"{patchdirectoryPath}\sce_sys\about", true);
            //}
            if (Directory.Exists($@"{patchdirectoryPath}\sce_sys\app"))
            {
                Directory.Delete($@"{patchdirectoryPath}\sce_sys\app", true);
            }
            if(File.Exists($@"{patchdirectoryPath}\sce_sys\libc.prx"))
            {
                File.Delete($@"{patchdirectoryPath}\sce_sys\libc.prx");
            }
            if (File.Exists($@"{patchdirectoryPath}\sce_sys\libSceFios2.prx"))
            {
                File.Delete($@"{patchdirectoryPath}\sce_sys\libSceFios2.prx");
            }
            if (File.Exists($@"{patchdirectoryPath}\sce_sys\libSceNpToolkit2.prx"))
            {
                File.Delete($@"{patchdirectoryPath}\sce_sys\libSceNpToolkit2.prx");
            }

            IEnumerable<string> filePaths = Directory.GetFiles(patchdirectoryPath, "*", SearchOption.AllDirectories);

            List<FileProperties> directoryFiles = new List<FileProperties>();

            foreach (var file in filePaths)
            {
                FileProperties fileProperty = new FileProperties();
                fileProperty.Path = file;
                fileProperty.CreationTime = File.GetCreationTime(file);
                fileProperty.LastAccessTime = File.GetLastAccessTime(file);
                fileProperty.LastWriteTime = File.GetLastWriteTime(file);
                directoryFiles.Add(fileProperty);
            }

            XElement psproject = new XElement("psproject",
                new XAttribute("fmt", "gp4"),
                new XAttribute("version", "1000"),
                new XElement("volume",
                    new XElement("volume_type", "pkg_ps4_patch"),
                    new XElement("volume_id", "PS4VOLUME"),
                    new XElement("volume_ts", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("package",
                        new XAttribute("content_id", $@"{CONTENT_ID}"),
                        new XAttribute("passcode", "00000000000000000000000000000000"),
                        new XAttribute("storage_type", "digital25"),
                        new XAttribute("app_type", "full"),
                        new XAttribute("app_path", $@"{BASE_PKG}")),
                    new XElement("chunk_info",
                        new XAttribute("chunk_count", "1"),
                        new XAttribute("scenario_count", "1"),
                        new XElement("chunks",
                            new XElement("chunk",
                                new XAttribute("id", "0"),
                                new XAttribute("layer_no", "0"),
                                new XAttribute("label", "Chunk #0"))),
                        new XElement("scenarios", new XAttribute("default_id", "0"),
                            new XElement("scenario",
                                new XAttribute("id", "0"),
                                new XAttribute("type", "sp"),
                                new XAttribute("initial_chunk_count", "1"),
                                new XAttribute("label", "Scenario #0"), "0")))),
                    new XElement("files",
                        new XAttribute("img_no", "0"),
                        (from file in directoryFiles
                         where file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path.Contains("eboot.bin")
                         where !filePatternsToIgnore.Any(pattern => file.Path.Contains(pattern))
                         orderby file.Path
                         select new XElement("file",
                             new XAttribute("targ_path", file.Path.Replace(patchdirectoryPath, "").TrimStart('\\').Replace('\\', '/')),
                             new XAttribute("orig_path", file.AbsolutePath),
                             (file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path == patchdirectoryPath || file.Path.EndsWith("eboot.bin"))
                             ? null : new XAttribute("pfs_compression", "enable")))
                        .Union(from file in directoryFiles
                               where !(file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path.Contains("eboot.bin"))
                               where !filePatternsToIgnore.Any(pattern => file.Path.Contains(pattern))
                               orderby file.Path
                               select new XElement("file",
                                   new XAttribute("targ_path", file.Path.Replace(patchdirectoryPath, "").TrimStart('\\').Replace('\\', '/')),
                                   new XAttribute("orig_path", file.AbsolutePath),
                                   (file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path == patchdirectoryPath || file.Path.EndsWith("eboot.bin"))
                                   ? null : new XAttribute("pfs_compression", "enable")))),
                    new XElement("rootdir", CreateDirectoryTree(patchdirectoryPath))
                );

            XDocument doc = new XDocument(new XDeclaration("1.1", "utf-8", "yes"), psproject);
            doc.Save(Path.Combine(patchdirectoryPath, @"..\patch_output.xml"));

            XElement root = doc.Root;
            XElement rootdir = root.Element("rootdir");

            if (rootdir.Elements("dir").Any())
            {
                XElement lastDir = rootdir.Elements("dir").Last();

                if (lastDir != null)
                {
                    foreach (XElement child in lastDir.Elements())
                    {
                        rootdir.Add(child);
                    }
                    lastDir.Remove();
                }
            }
            //doc.Declaration = new XDeclaration("1.1", "utf-8", "yes");
            doc.Save(Path.Combine(patchdirectoryPath, @"..\patch_output.gp4"));
            if (File.Exists(Path.Combine(patchdirectoryPath, @"..\patch_output.xml")))
            {
                File.Delete(Path.Combine(patchdirectoryPath, @"..\patch_output.xml"));
            }

            string filePath = @"ext\patch_output.gp4"; // path to your file

            // Read all text from file
            string input = File.ReadAllText(filePath);
            //string patternsPKG = "mypkg"; // regex pattern to match " />"
            //string replacementPKG = $@"{BASE_PKG}"; // what to replace it with
            //string result = Regex.Replace(input, patternsPKG, replacementPKG);

            string patterns = "\\s+/>"; // regex pattern to match " />"
            string replacement = "/>"; // what to replace it with

            // Use Regex.Replace to replace the string
            String result = Regex.Replace(input, patterns, replacement);

            // Additional replacement
            //result = result.Replace("<?xml version=\"1.0\"", "<?xml version=\"1.1\"");

            string pattern2 = @"\d{2}:\d{2}:\d{2}</volume_ts>"; // regex pattern to match timestamp before </volume_ts>
            string replacement2 = "00:00:00</volume_ts>"; // what to replace it with

            // Use Regex.Replace to replace the timestamp
            result = Regex.Replace(result, pattern2, replacement2);

            // Write the result back to the file
            File.WriteAllText(filePath, result);

        }

        static XElement CreateDirectoryTree(string path)
        {
            var info = new DirectoryInfo(path);
            var element = new XElement("dir");

            foreach (var directory in info.GetDirectories())
            {
                element.Add(CreateSubDirectoryTree(directory));
            }

            return element;
        }

        static XElement CreateSubDirectoryTree(DirectoryInfo info)
        {
            var element = new XElement("dir", new XAttribute("targ_name", info.Name));

            foreach (var directory in info.GetDirectories())
            {
                element.Add(CreateSubDirectoryTree(directory));
            }

            return element;
        }
    }
}