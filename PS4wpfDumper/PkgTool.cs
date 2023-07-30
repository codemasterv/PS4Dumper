using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS4wpfDumper.GP4;
using PS4wpfDumper.PFS;
using PS4wpfDumper.PKG;
using PS4wpfDumper.SFO;
using PS4wpfDumper.Util;
using PS4wpfDumper.Rif;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static PS4wpfDumper.MainWindow;

namespace PS4wpfDumper
{

    public class PkgTool
    {

        

        public static void makeBaseGP4(string outputDir, string sourceDir, string cid)
        {
            System.Console.WriteLine("makeBaseGP4(string outputDir, string sourceDir, string cid)   " + outputDir+"  TITTTTTZZZ   "+sourceDir + "  ASSSSS   "+ cid + "BAAAAAAALLLZZZZ");

            //Gp4Creator.CreateProjectFromDirectory(outputDir, sourceDir, cid);

        }

        public bool makeFPKG(MainWindow mainWindow, string inputProjectPath, string outputDirectory)
        {
            var project = Gp4Project.ReadFrom(File.OpenRead(inputProjectPath));
            var validation = Gp4Validator.ValidateProject(project, Path.GetDirectoryName(inputProjectPath));
            bool ok = true;
            project.SetType(VolumeType.pkg_ps4_app);
            if (validation.Count > 0)
            {
                System.Console.WriteLine("Found {0} issue(s) with GP4 project:", validation.Count);
            }

            foreach (ValidateResult v in validation)
            {
                if (v.Type == ValidateResult.ResultType.Fatal)
                {
                    ok = false;
                }
                System.Console.WriteLine("{0}: {1}",
                    v.Type == ValidateResult.ResultType.Fatal ? "FATAL ERROR" : "WARNING    ",
                    v.Message);
            }

            if (!ok)
            {
                System.Console.WriteLine("Cannot build PKG due to fatal errors.");
                return false;
            }

            var props = PkgProperties.FromGp4(project, Path.GetDirectoryName(inputProjectPath));
            System.Console.WriteLine(props);
            new PkgBuilder(props).Write(Path.Combine(outputDirectory, $"{project.volume.Package.ContentId}{type}.pkg"));

            //$"test.pkg"));


            return true;
        }
        
        //static void Main(string[] args)
        //{
        //    Verb.Run(Verbs, args, AppDomain.CurrentDomain.FriendlyName);
        // }
        private static VolumeType ContentTypeToVolumeType(ContentType t)
        {
            switch (t)
            {
                case ContentType.GD:
                    return VolumeType.pkg_ps4_app;
                case ContentType.DP:
                    return VolumeType.pkg_ps4_patch;
                case ContentType.AC:
                    return VolumeType.pkg_ps4_ac_data;
                case ContentType.AL:
                    return VolumeType.pkg_ps4_ac_nodata;
                default:
                    return 0;
            }
        }

        public static void CheckSFO(MainWindow mainWindow, string sfoFilename)
        {
            //mainWindow.Dispatcher.Invoke(() =>
            //{
            //    MainWindow.WBDs("Checking the mofkn sfo");
            //});

            if (File.Exists(sfoFilename))
            {
                using (var f = File.OpenRead(sfoFilename))
                {
                    var sfo = ParamSfo.FromStream(f);
                    
                    //mainWindow.Dispatcher.Invoke(() =>
                    //{
                    //    MainWindow.WBDs($"Entry Name : Entry Type(Size / Max Size) = Entry Value");
                    //
                    //});
                    

                    foreach (var x in sfo.Values)
                    {
                        

                        //mainWindow.Dispatcher.Invoke(() =>
                        //{
                        MainWindow.WBDs($"{x.Name} : {x.Type}({x.Length}/{x.MaxLength}) = {x.ToString()}");

                        //});
                        string filePath = $@"ext\sfoOutput.text";

                        if (x.Name == "APP_TYPE")
                        {
                            string patternToRemove = @"Integer\(\+d\/\+d\) = 0x0000000";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            APP_TYPE = fixedLine;//.Substring(0, fixedLine.Length - 1);
                                                 // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("APP_TYPE: " + APP_TYPE);
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{APP_TYPE}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });
                            
                        }

                        if (x.Name == "APP_VER")
                        {
                            string patternToRemove = @"Utf8\(\+d\/\+d\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            APP_VER = fixedLine;//.Substring(0, fixedLine.Length - 1);
                                                 // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("APP_VER: " + APP_VER);
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{APP_VER}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });
                            
                        }


                        if (x.Name == "ATTRIBUTE")
                        {
                            string patternToRemove = @"Integer\(\+d\/\+d\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            ATTRIBUTE = fixedLine;//.Substring(0, fixedLine.Length - 1);
                                                // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("ATTRIBUTE: "+ ATTRIBUTE);
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{ATTRIBUTE}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });
                           
                        }

                        if (x.Name == "ATTRIBUTE2")
                        {
                            string patternToRemove = @"Integer\(\+d\/\+d\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            ATTRIBUTE2 = fixedLine;//.Substring(0, fixedLine.Length - 1);
                                                  // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("ATTRIBUTE2: " + ATTRIBUTE2);
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{ATTRIBUTE2}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });
                            
                        }

                        if (x.Name == "CATEGORY")
                        {
                            string patternToRemove = @"Utf8\(\+d\/\+d\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            CATEGORY = fixedLine;//.Substring(0, fixedLine.Length - 1);
                                                   // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("CATEGORY: " + CATEGORY);
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{CATEGORY}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });

                        }

                        if (x.Name == "PUBTOOLINFO")
                        {
                            string patternToMatch = @"(?<=c_date=)\d+";
                            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);

                            if (m.Success) // if match found
                            {
                                string c_date = m.Value;
                                // Open the file to append new text 
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    // Write the new c_date to the file
                                    PUBTOOLINFO_c_Date = c_date;
                                    sw.WriteLine("PUBTOOLINFO c_date: " + c_date);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO c_date: " + c_date}");
                                    //System.Console.WriteLine($@"{CONTENT_ID}");
                                });
                            }
                        }


                        if (x.Name == "PUBTOOLINFO")
                        {
                            string patternToMatch = @"(?<=sdk_ver=)\d+";
                            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);

                            if (m.Success) // if match found
                            {
                                string sdk_ver = m.Value;
                                // Open the file to append new text 
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    // Write the new sdk_ver to the file
                                    PUBTOOLINFO_sdk_ver = sdk_ver;
                                    sw.WriteLine("PUBTOOLINFO sdk_ver: " + sdk_ver);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO sdk_ver: " + sdk_ver}");
                                    //System.Console.WriteLine($@"{CONTENT_ID}");
                                });
                            }
                        }


                        if (x.Name == "PUBTOOLINFO")
                        {
                            string patternToMatch = @"(?<=st_type=)[a-zA-Z0-9]+";
                            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);

                            if (m.Success) // if match found
                            {
                                string st_type = m.Value;
                                // Open the file to append new text 
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    // Write the new st_type to the file
                                    PUBTOOLINFO_st_type = st_type;
                                    sw.WriteLine("PUBTOOLINFO st_type: " + st_type);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO st_type: " + st_type}");
                                    //System.Console.WriteLine($@"{CONTENT_ID}");
                                });
                            }
                        }

                        if (x.Name == "PUBTOOLINFO")
                        {
                            // Pattern to match "img0_l0_size=7834"
                            string patternToMatch = @"(?<=img0_l0_size=)\d+";
                            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);
                            if (m.Success)
                            {
                                string img0_l0_size = m.Value;
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    PUBTOOLINFO_img0_l0_size = img0_l0_size;

                                    sw.WriteLine("PUBTOOLINFO img0_l0_size: " + img0_l0_size);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO img0_l0_size: " + img0_l0_size}");
                                });
                            }

                            // Pattern to match "img0_l1_size=0"
                            patternToMatch = @"(?<=img0_l1_size=)\d+";
                            m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);
                            if (m.Success)
                            {
                                string img0_l1_size = m.Value;
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    PUBTOOLINFO_img0_l1_size = img0_l1_size;

                                    sw.WriteLine("PUBTOOLINFO img0_l1_size: " + img0_l1_size);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO img0_l1_size: " + img0_l1_size}");
                                });
                            }

                            // Pattern to match "img0_sc_ksize=9216"
                            patternToMatch = @"(?<=img0_sc_ksize=)\d+";
                            m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);
                            if (m.Success)
                            {
                                string img0_sc_ksize = m.Value;
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    PUBTOOLINFO_img0_sc_ksize = img0_sc_ksize;
                                    sw.WriteLine("PUBTOOLINFO img0_sc_ksize: " + img0_sc_ksize);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO img0_sc_ksize: " + img0_sc_ksize}");
                                });
                            }

                            // Pattern to match "img0_pc_ksize=6080"
                            patternToMatch = @"(?<=img0_pc_ksize=)\d+";
                            m = System.Text.RegularExpressions.Regex.Match(x.ToString(), patternToMatch);
                            if (m.Success)
                            {
                                string img0_pc_ksize = m.Value;
                                using (StreamWriter sw = File.AppendText(filePath))
                                {
                                    PUBTOOLINFO_img0_pc_ksize = img0_pc_ksize;

                                    sw.WriteLine("PUBTOOLINFO img0_pc_ksize: " + img0_pc_ksize);
                                }
                                mainWindow.Dispatcher.Invoke(() =>
                                {
                                    MainWindow.WBDs($@"{"PUBTOOLINFO img0_pc_ksize: " + img0_pc_ksize}");
                                });
                            }
                        }


                        if (x.Name == "TITLE_ID")
                        {
                            string patternToRemove = @"TITLE_ID : Utf8\(\d+\/\d+\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            TITLE_ID = fixedLine;//.Substring(0, fixedLine.Length - 1);
                            //mainWindow.Dispatcher.Invoke(() =>
                            //{
                            //    MainWindow.WBDs($@"{TITLE_ID}");
                            //System.Console.WriteLine($@"{TITLE_ID}");
                            // });

                            // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine("TITLE_ID: "+ TITLE_ID);
                            }

                        }

                        if (x.Name == "CONTENT_ID")
                        {
                            string patternToRemove = @"CONTENT_ID : Utf8\(\d+\/\d+\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            string string1 = x.ToString();
                            int position = string1.IndexOf("=");
                            position += 1;
                            string string2 = string1.Substring(position);
                            string2 = string2.Trim();

                            CONTENT_ID = string2;//.Substring(0, fixedLine.Length - 1);
                                                 // Open the file to append new text 
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine($@"CONTENT_ID: {CONTENT_ID}");
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"CONTENT_ID: {CONTENT_ID}");
                                //System.Console.WriteLine($@"{CONTENT_ID}");
                            });
                            
                        }

                        if (x.Name == "VERSION")
                        {
                            string patternToRemove = @"Utf8\(\d+\/\d+\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            VERSION = fixedLine;
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine($@"VERSION: {VERSION}");
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"{VERSION}");
                                //System.Console.WriteLine($@"{VERSION}");
                            });

                        }

                        if (x.Name == "TITLE" && !x.Name.Contains("TITLE_ID") && !x.Name.Contains("TITLE_01"))
                        {
                            string patternToRemove = @"TITLE : Utf8\(\d+\/\d+\) = ";
                            string fixedLine = System.Text.RegularExpressions.Regex.Replace(x.ToString(), patternToRemove, "");
                            TITLE = fixedLine;
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                // Write the new TITLE_ID to the file
                                sw.WriteLine($@"TITLE: {TITLE}");
                            }
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                MainWindow.WBDs($@"TITLE: {TITLE}");
                                //System.Console.WriteLine(TITLE);
                            });
                            f.Close();
                        }                        
                    }
                }
            }
            else
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    MainWindow.WBDs("File not found: " + sfoFilename);

                });
            }
        }

        public static void pfs_extract(string pfsPath, string outPath)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.WBDs($"ExtractInParallel");
            });
            using (var mmf = MemoryMappedFile.CreateFromFile(pfsPath))
            using (var acc = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                var pfs = new PfsReader(acc);
                ExtractInParallel(pfs, outPath);
            }
        }

        public void createfPKG(string gp4, string fpkgOutDir)
        {
            var project = Gp4Project.ReadFrom(File.OpenRead(gp4));
            var validation = Gp4Validator.ValidateProject(project, Path.GetDirectoryName(gp4));
            bool ok = true;
            if (validation.Count > 0)
            {
                MainWindow.WBDs("Found {0} issue(s) with GP4 project:" + validation.Count);
            }
            foreach (ValidateResult v in validation)
            {
                if (v.Type == ValidateResult.ResultType.Fatal)
                {
                    ok = false;
                }
                System.Console.WriteLine("{0}: {1}",
                  v.Type == ValidateResult.ResultType.Fatal ? "FATAL ERROR" : "WARNING    ",
                  v.Message);
            }
            if (!ok)
            {
                System.Console.WriteLine("Cannot build PKG due to fatal errors.");
                System.Environment.Exit(1);
                return;
            }
            var props = PkgProperties.FromGp4(project, Path.GetDirectoryName(gp4));
            var outputPath = fpkgOutDir;
            new PkgBuilder(props).Write(Path.Combine(outputPath, $"{project.volume.Package.ContentId}{type}.pkg"));
        }


        private static void ExtractInParallel(PfsReader inner, string outPath)
        {
            System.Console.WriteLine("Extracting in parallel...");
            Parallel.ForEach(
                inner.GetAllFiles(),
                () => new byte[0x10000],
                (f, _, buf) =>
                {
                    var size = f.size;
                    long pos = 0;
                    var view = f.GetView();
                    var fullName = f.FullName;
                    var path = Path.Combine(outPath, fullName.Replace('/', Path.DirectorySeparatorChar).Substring(1));
                    var dir = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

                    Directory.CreateDirectory(dir);
                    using (var file = File.OpenWrite(path))
                    {
                        file.SetLength(size);
                        while (size > 0)
                        {
                            var toRead = (int)System.Math.Min(size, buf.Length);
                            view.Read(pos, buf, 0, toRead);
                            file.Write(buf, 0, toRead);
                            pos += toRead;
                            size -= toRead;
                        }
                    }
                    return buf;
                },
                x => { });
        }
    }
}
