using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PS4wpfDumper
{
    class MakeAppGP4
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

        public static void makeAppGP4(string appdirectoryPath, string CONTENT_ID)
        {
            string[] filePatternsToIgnore = {"icon0.dds", "license.dat", "license.info", "pic0.dds", "pic1.dds", "playgo-chunk.dat", "playgo-chunk.sha", "playgo-manifest.xml", "psreserved.dat", "sce_discmap.plt", "disc_info.dat" };//"keystone", "right.sprx"
            if (File.Exists($@"{appdirectoryPath}\sce_sys\disc_info.dat"))
            {
                File.Delete($@"{appdirectoryPath}\sce_sys\disc_info.dat");
            }
            if (File.Exists($@"{appdirectoryPath}\sce_sys\keystone"))
            {
                File.Move($@"{appdirectoryPath}\sce_sys\keystone", $@"{appdirectoryPath}\sce_sys\keystone.bak");
            }
            if (File.Exists($@"{appdirectoryPath}\sce_sys\about\right.sprx"))
            {
                //File.Delete($@"{appdirectoryPath}\sce_sys\about\right.sprx");
                
            }
            if (Directory.Exists($@"{appdirectoryPath}\sce_sys\app"))
            {
                Directory.Delete($@"{appdirectoryPath}\sce_sys\app", true);
            }
            if (Directory.Exists($@"{appdirectoryPath}\sce_sys\changeinfo"))
            {
                Directory.Delete($@"{appdirectoryPath}\sce_sys\changeinfo", true);
            }

            IEnumerable<string> filePaths = Directory.GetFiles(appdirectoryPath, "*", SearchOption.AllDirectories);

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
                    new XElement("volume_type", $@"pkg_ps4_app"),//sets the app type from the main window. pkg_ps4_app, pkg_ps4_patch, pkg_ps4_ac
                    new XElement("volume_id", "PS4VOLUME"),
                    new XElement("volume_ts", DateTime.Now.ToString("yyyy-MM-dd" + " 00:00:00")),
                    new XElement("package",
                        new XAttribute("content_id", $@"{CONTENT_ID}"),
                        new XAttribute("passcode", "00000000000000000000000000000000"),
                        new XAttribute("storage_type", $@"{MainWindow.PUBTOOLINFO_st_type}"),//Pulls from the SFO file digital50
                        new XAttribute("app_type", "full"),
                        new XAttribute("c_date", DateTime.Now.ToString("yyyy-MM-dd")),
                        new XAttribute("c_time", "00:00:00")),
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
                                new XAttribute("label", "Scenario #0"), "0")))),//Finished with the header.
                    new XElement("files", new XAttribute("img_no", "0"),//This starts the files
                        (from file in directoryFiles
                         where file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path.Contains("eboot.bin")
                         where !filePatternsToIgnore.Any(pattern => file.Path.Contains(pattern))
                         orderby file.Path
                         select new XElement("file",
                             new XAttribute("targ_path", file.Path.Replace(appdirectoryPath, "").TrimStart('\\').Replace('\\', '/')),
                             new XAttribute("orig_path", file.AbsolutePath),
                             (file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path == appdirectoryPath || file.Path.EndsWith("eboot.bin"))
                             ? null : new XAttribute("pfs_compression", "enable")))
                        .Union(from file in directoryFiles
                               where !(file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path.Contains("eboot.bin"))
                               where !filePatternsToIgnore.Any(pattern => file.Path.Contains(pattern))
                               orderby file.Path
                               select new XElement("file",
                                   new XAttribute("targ_path", file.Path.Replace(appdirectoryPath, "").TrimStart('\\').Replace('\\', '/')),
                                   new XAttribute("orig_path", file.AbsolutePath),
                                   (file.Path.Contains("\\sce_sys\\") || file.Path.Contains("\\sce_module\\") || file.Path == appdirectoryPath || file.Path.EndsWith("eboot.bin"))
                                   ? null : new XAttribute("pfs_compression", "enable")))),
                    new XElement("rootdir", CreateDirectoryTree(appdirectoryPath))
                );

            XDocument doc = new XDocument(new XDeclaration("1.1", "utf-8", "yes"), psproject);
            doc.Save(Path.Combine(appdirectoryPath, $@"test.gp4"));
            
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
            Task.Delay(TimeSpan.FromSeconds(5));
            string xmlFilePath = Path.Combine(appdirectoryPath, $@"test.gp4"); // Specify the path to your XML file
            XDocument xDoc = XDocument.Load(xmlFilePath); // Load the XML file

            // Previous XML operations...

            // Get the root <rootdir> element
            XElement rootdirElement = xDoc.Descendants("rootdir").FirstOrDefault();

            // Get the first child <dir> element
            XElement firstDirElement = rootdirElement?.Element("dir");

            if (firstDirElement != null)
            {
                // Move all child elements of the first <dir> to <rootdir>
                foreach (var child in firstDirElement.Elements().ToList())
                {
                    child.Remove();
                    rootdirElement.Add(child);
                }

                // Remove the first <dir>
                firstDirElement.Remove();
            }

            // Save the XML file
            xDoc.Save(xmlFilePath);

            if (File.Exists(xmlFilePath))
            {

                // Read the entire XML content
                string xmlContent = File.ReadAllText(xmlFilePath);




                // Replace all occurrences of " />" with "/>"
                string updatedXmlContent = Regex.Replace(xmlContent, " />", "/>");

                // Write the updated XML content back to the file
                File.WriteAllText($@"ext\test.gp4", updatedXmlContent);
            }
            if(File.Exists($@"ext\test.gp4"))
            {
                string filePath6 = @"ext\test.gp4";
                string currentDir6 = Environment.CurrentDirectory + $@"\ext\";
                // Add double backslashes to the current directory path for the regex pattern
                string pattern6 = Regex.Escape(currentDir6);
                string replacement6 = "";
                // Read all the file content
                string fileContent6 = File.ReadAllText(filePath6);
                // Use Regex to replace the pattern with the replacement
                string updatedContent6 = Regex.Replace(fileContent6, pattern6, replacement6);
                // Write the updated content back to the file
                File.WriteAllText(filePath6, updatedContent6);
                Console.WriteLine("Hello");
                if (File.Exists($@"ext\test.gp4"))
                {
                   // File.Copy($@"ext\test.gp4", $@"ext\{MainWindow.CUSA}{MainWindow.type}\test.gp4", true);
                }
            }

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