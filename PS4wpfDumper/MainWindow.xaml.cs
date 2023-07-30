using System;
using System.IO;
using System.Net;
using FluentFTP;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FluentFTP.Exceptions;
using System.Linq;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Media;
using PS4wpfDumper.PKG;
using System.Collections;
using PS4wpfDumper.SFO;
using System.Xml;
using System.Xml.Linq;
using PS4wpfDumper.GP4;
using PS4wpfDumper.Rif;


namespace PS4wpfDumper
{
    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        public static MainWindow Instance { get; private set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        List<int> inode_offsets = new List<int>();
        private SendPKG sendPKG = new SendPKG();
        private DispatcherTimer loadingTimer;
        public PkgBuilder pkgBuilder;
        public PkgTool pkgTool = new PkgTool();
        public string MyMessage { get; set; }
        public static bool debug = false;
        public static bool resume = false;
        public static int unknownCount = 0;
        public long totalSize;
        private int loadingIndex;

        public static String TITLE_ID;
        public static String CONTENT_ID;
        public static String VERSION;
        public static String TITLE;
        public static String APP_VER;
        public static String PUBTOOLINFO_st_type;
        public static String PUBTOOLINFO_sdk_ver;
        public static String PUBTOOLINFO_c_Date;
        public static String PUBTOOLINFO_img0_pc_ksize;
        public static String PUBTOOLINFO_img0_sc_ksize;
        public static String PUBTOOLINFO_img0_l1_size;
        public static String PUBTOOLINFO_img0_l0_size;
        public static String CATEGORY;
        public static String ATTRIBUTE2;
        public static String ATTRIBUTE;
        public static String APP_TYPE;
        public static String CUSA;
        public static String port;
        public static String ip;
        public static String pfsPath;
        public static String outPath;
        public static String type;
        public static String WBD; //{ get; set; }
        public static String apptype;

        String stringOutput = "";
        String host;
        String username;
        String password;
        String GameDir;
        String iconPath;
        String newIconPath;
        String PFS_PATH;
        String appPKG;
        String TRP;
        String error;
        String localDirectoryPath;
        String debugMessage;
        String appPath;
        String BASE_PKG;
        String gp4path;
        String gamepath;
   
        int TMR;
        int TMR2;
        int superroot_inode_index;
        int block_size;
        const int INODE_SIZE = 168;

        public static void APPtype(string appType)
        {
            appType = apptype;
        }
       
        public static void PUBTOOLINFO_img0_pc_ksizes(string pc_ksizes)
        {
            Console.WriteLine("This is the string " + pc_ksizes);
            PUBTOOLINFO_img0_pc_ksize = String.Format(pc_ksizes);
        }
        public static void PUBTOOLINFO_img0_sc_ksizes(string img0_sc_ksizes)
        {
            Console.WriteLine("This is the string " + img0_sc_ksizes);
            PUBTOOLINFO_img0_sc_ksize = String.Format(img0_sc_ksizes);
        }
        public static void PUBTOOLINFO_img0_l0_sizes(string img0_l0_sizes)
        {
            Console.WriteLine("This is the string " + img0_l0_sizes);
            PUBTOOLINFO_img0_l0_size = String.Format(img0_l0_sizes);
        }
        public static void PUBTOOLINFO_img0_l1_sizes(string img0_l1_sizes)
        {
            Console.WriteLine("This is the string " + img0_l1_sizes);
            PUBTOOLINFO_img0_l1_size = String.Format(img0_l1_sizes);
        }
        public static void APP_TYPEs(string app_type)
        {
            Console.WriteLine("This is the string " + app_type);
            APP_TYPE = String.Format(app_type);
        }
        public static void ATTRIBUTEs(string ATT)
        {
            Console.WriteLine("This is the string " + ATT);
            ATTRIBUTE = String.Format(ATT);
        }
        public static void ATTRIBUTE2s(string ATT2)
        {
            Console.WriteLine("This is the string " + ATT2);
            ATTRIBUTE2 = String.Format(ATT2);
        }
        public static void CATEGORYs(string CAT)
        {
            Console.WriteLine("This is the string " + CAT);
            CATEGORY = String.Format(CAT);
        }
        public static void PUBTOOLINFO_c_Dates(string c_Dates)
        {
            Console.WriteLine("This is the string " + c_Dates);
            PUBTOOLINFO_c_Date = String.Format(c_Dates);
        }
        public static void PUBTOOLINFO_sdk_vers(string sdk_vers)
        {
            Console.WriteLine("This is the string " + sdk_vers);
            PUBTOOLINFO_sdk_ver = String.Format(sdk_vers);
        }
        public static void PUBTOOLINFO_st_types(string st_types)
        {
            Console.WriteLine("This is the string " + st_types);
            PUBTOOLINFO_st_type = String.Format(st_types);
        }
        public static void types(string types)
        {
            Console.WriteLine("This is the string" + types);
            type = String.Format(types);
        }
        public static void WBDs(string wbd)
        {
            Console.WriteLine("This is the string" + wbd);
            //WBD = String.Format(wbd);
        }
        public static void tid(string tid)
        {
            Console.WriteLine("This is the string" + tid);
            TITLE_ID = String.Format(tid);
        }
        public static void cid(string cid)
        {
            Console.WriteLine("This is the string" + cid);
            CONTENT_ID = String.Format(cid);
        }
        public static void ver(string ver)
        {
            Console.WriteLine("This is the string" + ver);
            VERSION = String.Format(ver);
        }
        public static void tit(string tit)
        {
            Console.WriteLine("This is the string" + tit);
            TITLE = String.Format(tit);
        }
        public static void apver(string apver)
        {
            Console.WriteLine("This is the string" + apver);
            APP_VER = String.Format(apver);
        }
        public MainWindow()
        {
            InitializeComponent();
            port = portTextBox.Text;
            ip = ipTextBox.Text;
            WBD = "Welcome to PS4 Dumper!";
            aniList.Add("-");
            host = String.Format(ipTextBox.Text);
            username = "root";
            password = "root";
            port = String.Format(portTextBox.Text);
            GameDir = "";
            error = $"Unable to connect to the PS4 with the saved information.";
            // Initialize the loading animation timer
            loadingTimer = new DispatcherTimer();
            loadingTimer.Interval = TimeSpan.FromSeconds(0.1);  // Adjust the interval as needed
            loadingTimer.Tick += LoadingTimer_Tick;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyPath = @"libs\" + new System.Reflection.AssemblyName(args.Name).Name + ".dll";
                if (File.Exists(assemblyPath))
                {
                    return System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
                return null;
            };

            string[] files = Directory.GetFiles("bins", "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                comboBox.Items.Add(file);
            }

            // Add event handler for ListBox selection change
            outputListBox.SelectionChanged += outputListBox_SelectionChanged;
            //pkgBuilder.Logger?.Invoke("This is a log message");
            if (patchRadioButton.IsChecked == true)
            {
                type = "-app0";
            }
            if (patchRadioButton.IsChecked == true)
            {
                type = "-patch0";
            }
            startClean();
        }

        private void startClean()
        {
            string path = $@"ext\";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // Delete files in the directory.
            foreach (var file in Directory.EnumerateFiles(path))
            {
                File.Delete(file);
            }

            // Delete directories that do not end with "-patch" or "-app".
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Name.EndsWith("-patch0") && !dirInfo.Name.EndsWith("-app0"))
                {
                    Directory.Delete(directory, true); // The second parameter "true" allows recursive deletion of sub-directories and files.
                }
            }
        }

        // Method to be executed when button is clicked
        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            //Null out data before each start labels and image box
            if (baseRadioButton.IsChecked == true)
            {
                image.Source = null;
                keystoneLabel.Content = "";
                npLabel.Content = "";
                cusaLabel.Content = "";
                gameLabel.Content = "";
                cidLabel.Content = "";
                appVerLabel.Content = "";
                verLabel.Content = "";
                button.IsEnabled = false;

                StartLoadingAnimation();
                WBDs("Downloading App Files");

                type = "-app0";
                await unloadPic();

                if (Directory.Exists(@"ext\"))
                {
                    await CleanUpFiles();
                }
                await Task.Delay(TimeSpan.FromSeconds(2));

                if(portTextBox.Text == "1337")
                {
                    await SendDECRYPTToFtpServerAsync();
                }

                await dlAPPs();
                await movePFSuroot();

                if (File.Exists($@"ext\{MainWindow.CUSA}{MainWindow.type}\sce_sys\keystone"))
                {
                    File.Delete($@"ext\{MainWindow.CUSA}{MainWindow.type}\sce_sys\keystone");
                }

                string outDir = $@"FPKG\{CONTENT_ID}\";
                string gamePath = $@"ext\{TITLE_ID}{type}";
                if (checkBox.IsChecked == true)
                {
                    await gp4BASE(gamePath, outDir, CONTENT_ID);
                    await pkgBASE(this);
                    await baseVER();//adds base version to the name of the finished pkg file
                }
                
                this.Dispatcher.Invoke(() =>
                {        
                    button.IsEnabled = true;
                });
                //string sourceDirectory = $@"ext\{TITLE_ID}{type}";
                //string targetDirectory = $@"FPKG\{CONTENT_ID}\{TITLE_ID}{type}";// trying to decide if I want to move the raw files with the FPKG. Leaving this out for now
                //await DirectoryCopy(sourceDirectory, targetDirectory, true);
               
                if (portTextBox.Text == "1337")
                {
                    await SendDECRYPTToFtpServerAsync();
                }

                if (Directory.Exists(@"ext\"))
                {
                    await CleanUpFiles();
                }

                WBDs("Done!");
                StopLoadingAnimation();
                loadingLabel.Content = "";
                prcnt.Content = "";
                await DoneSon();
            }

            if (patchRadioButton.IsChecked == true)
            {
                image.Source = null;
                keystoneLabel.Content = "";
                npLabel.Content = "";
                cusaLabel.Content = "";
                gameLabel.Content = "";
                cidLabel.Content = "";
                appVerLabel.Content = "";
                verLabel.Content = "";
                button.IsEnabled = false;

                StartLoadingAnimation();
                WBDs("Downloading Patch Files");
                type = "-patch0";
                await unloadPic();
                await Task.Delay(TimeSpan.FromSeconds(1));
                await dlAPPs();//download game files and orginize them

                if (portTextBox.Text == "1337")
                {
                    await SendDECRYPTToFtpServerAsync();//turn server decryption off
                }
                if (checkBox.IsChecked == true)
                {
                    await gp4PATCH();
                    await patchBASE(this);
                    await patchVER();
                }
                //string sourceDirectory = $@"ext\{TITLE_ID}{type}";
                //string targetDirectory = $@"FPKG\{CONTENT_ID}\{TITLE_ID}{type}";
                //await DirectoryCopy(sourceDirectory, targetDirectory, true);
                this.Dispatcher.Invoke(() =>
                {
                    button.IsEnabled = true;
                });

                WBDs("Done!");
                StopLoadingAnimation();
                loadingLabel.Content = "";
                prcnt.Content = "";
                await DoneSon();

            }
            if (acRadioButton.IsChecked == true)
            {
    
                await Task.Delay(TimeSpan.FromSeconds(10)); // wait for 5 seconds
            }

            if (allRadioButton.IsChecked == true)
            {
                baseRadioButton.IsChecked = true;

                //Null out data before each start labels and image box
                if (baseRadioButton.IsChecked == true)
                {
                    image.Source = null;
                    keystoneLabel.Content = "";
                    npLabel.Content = "";
                    cusaLabel.Content = "";
                    gameLabel.Content = "";
                    cidLabel.Content = "";
                    appVerLabel.Content = "";
                    verLabel.Content = "";
                    button.IsEnabled = false;

                    StartLoadingAnimation();
                    WBDs("Downloading App Files");

                    type = "-app0";
                    await unloadPic();

                    if (Directory.Exists(@"ext\"))
                    {
                        await CleanUpFiles();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));

                    if (portTextBox.Text == "1337")
                    {
                        await SendDECRYPTToFtpServerAsync();
                    }

                    await dlAPPs();
                    await movePFSuroot();

                    if (checkBox.IsChecked == true)
                    {
                        string outDir = $@"FPKG\{CONTENT_ID}\";
                        string gamePath = $@"ext\{TITLE_ID}{type}";
                        await gp4BASE(gamePath, outDir, CONTENT_ID);
                        await pkgBASE(this);
                        await baseVER();//adds base version to the name of the finished pkg file
                    }

                    WBDs("Done! With App Files Moving to Patch Files");

                    if (portTextBox.Text == "1337")
                    {
                        await SendDECRYPTToFtpServerAsync();
                    }
                    patchRadioButton.IsChecked = true;

                    if (patchRadioButton.IsChecked == true)
                    {
                        image.Source = null;
                        keystoneLabel.Content = "";
                        npLabel.Content = "";
                        cusaLabel.Content = "";
                        gameLabel.Content = "";
                        cidLabel.Content = "";
                        appVerLabel.Content = "";
                        verLabel.Content = "";
                        button.IsEnabled = false;

                        StartLoadingAnimation();
                        WBDs("Downloading Patch Files");
                        type = "-patch0";
                        await unloadPic();
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        await dlAPPs();//download game files and orginize them


                        if (portTextBox.Text == "1337")
                        {
                            await SendDECRYPTToFtpServerAsync();//turn server decryption off
                        }
                        if (checkBox.IsChecked == true)
                        {
                            await gp4PATCH();
                            await patchBASE(this);
                            await patchVER();
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            button.IsEnabled = true;
                        });

                        WBDs("Done!");
                        StopLoadingAnimation();
                        loadingLabel.Content = "";
                        prcnt.Content = "";
                        await DoneSon();
                    }
                }


                
            }

        }

        public async Task DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    await DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public async Task loadPic()
        {
            await unloadPic();
            string newIconPath = $@"libs\icon0.png";
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(newIconPath, UriKind.Relative);
            bitmap.EndInit();

            image.Source = bitmap;
        }

        public async Task unloadPic()
        {
            image.Source = null;

            image.Source = new BitmapImage();

            Dispatcher.Invoke(() =>
            {
                image.Source = null;
            });
        }

        public async Task uroot()//This part of uroot task handles the move of the extracted PFS Image
        {
            if (Directory.Exists($@"ext\uroot"))
            {
                if(File.Exists($@"ext\uroot\eboot.bin"))
                {
                    File.Delete($@"ext\uroot\eboot.bin");
                }
                if (File.Exists($@"ext\uroot\sce_discmap.plt"))
                {
                    File.Delete($@"ext\uroot\sce_discmap.plt");
                }
                if (Directory.Exists($@"ext\uroot\sce_module"))
                {
                    Directory.Delete($@"ext\uroot\sce_module", true);
                }
                if (Directory.Exists($@"ext\uroot\sce_sys"))
                {
                    Directory.Delete($@"ext\uroot\sce_sys", true);
                }
            }
            await movePFSuroot();//moves the other files to proper directory with minor cleaning.
        }

        public async Task pfstool(MainWindow mainWindow, string pfsPath, string outPath)
        {
            await Task.Run(() => PkgTool.pfs_extract(pfsPath, outPath));//extract the pfs image to get the uroot directory
        }

        public void StartLoadingAnimation()
        {
            loadingIndex = 0;
            loadingTimer.Start();
        }

        public void StopLoadingAnimation()
        {
            loadingTimer.Stop();
        }

        private List<string> stringList = new List<string>
        {
            ".       ",
            "        ",
            ".       ",
            "..      ",
            "...     ",
            "....    ",
            "...     ",
            "..      ",
            ".       ",
            "        "
        };

        private int currentIndex = 0;
        private DispatcherTimer stringTimer;
        private static System.Collections.ArrayList aniList = new ArrayList();
       
        public void LoadingTimer_Tick(object sender, EventArgs e)
        {
            const string loadingSequence = "▁▂▃▄▅▆▇█▇▆▅▄▃▂▁";
            string currentString = stringList[currentIndex];

            loadingLabel.Content = String.Format(loadingSequence[loadingIndex].ToString() + $" {WBD}{currentString}");
            // Increment the loading index
            currentIndex = (currentIndex + 1) % stringList.Count;
            loadingIndex = (loadingIndex + 1) % loadingSequence.Length;                          
        }

        private void CheckKeystone(string filePath)
        {
            // Expected string
            var expected = "294a5ed06db170618f2eed8c424b9d828879c080cc66fbc4864f69e974deb856";

            // Read the specific bytes from the file
            byte[] fileBytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Position = 32;
                fileBytes = new byte[32];
                fileStream.Read(fileBytes, 0, 32);
            }
            // Convert the bytes to a hex string
            var hexString = BitConverter.ToString(fileBytes).Replace("-", "").ToLower();

            // Check if the read string is as expected
            if (hexString == expected)
            {
                WBDs("Keystone is not original.");
                keystoneLabel.Content = "Keystone is not original.";
            }
            if (hexString != expected)
            {
                WBDs("Keystone is original.");
                keystoneLabel.Content = "Keystone is original.";
            }
        }

        public static void DebugMessage(string message)
        {
            string debugMessage = $"Debug: {message}";
            WBDs(debugMessage);
        }

        public static int ReadIntBe(string fileName, int offset)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(offset, SeekOrigin.Begin);

                using (var reader = new BinaryReader(stream))
                {
                    byte[] bytes = reader.ReadBytes(4); // Assuming you want to read an int32 (4 bytes)

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bytes);

                    return BitConverter.ToInt32(bytes, 0);
                }
            }
        }

        public static string ReadString(string fileName, int offset)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(offset, SeekOrigin.Begin);

                using (var reader = new BinaryReader(stream))
                {
                    var bytes = new List<byte>();
                    byte b;
                    while ((b = reader.ReadByte()) != 0)
                        bytes.Add(b);

                    return Encoding.UTF8.GetString(bytes.ToArray());
                }
            }
        }

        public static void ExtractPkgFile(string pkgFileName, int offset, int size, string fileName)
        {
            using (FileStream sourceStream = new FileStream(pkgFileName, FileMode.Open, FileAccess.Read))
            {
                sourceStream.Seek(offset, SeekOrigin.Begin);

                byte[] bytes = new byte[size];
                int numBytesToRead = size;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = sourceStream.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                using (FileStream targetStream = new FileStream(fileName, FileMode.Create))
                {
                    targetStream.Write(bytes, 0, numBytesRead);
                }
            }
        }

        public static void WarningMessage(string message)
        {
            WBDs($"Warning: {message}");
        }

        public void netCat()//for future use. Thought about the code and didnt want to forget.
        {
            //place holder variables until text boxes are set up
            string ncIP = String.Format(ipTextBox.Text);//ip text box
            string ncPort = String.Format(portTextBox.Text);//port text box
            string payloadPath = String.Format(comboBox.Text);//payload path textbox

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            Process process = Process.Start(psi);

            StreamWriter sw = process.StandardInput;
            System.IO.StreamReader sr = process.StandardOutput;

            sw.WriteLine($@"bins\nc64.exe -d -w 3 {ncIP} {ncPort} < {payloadPath}");  // replace with your target IP and port
            sw.WriteLine("exit");

            string output = sr.ReadToEnd();
            this.Dispatcher.Invoke(() =>
            {
              loadingLabel.Content = $@"bins\nc64.exe -w 3 {ncIP} {ncPort} < {payloadPath}";
            });

            process.WaitForExit();

            WBDs($@"bins\nc64.exe -d 3 {ncIP} {ncPort} < {payloadPath}");

        }

        public async Task baseVER()
        {           
            if (File.Exists($@"FPKG\{CONTENT_ID}\{CONTENT_ID}{type}.pkg"))
            {
                if (File.Exists($@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-base.pkg"))
                {
                    await Task.Run(() => File.Delete($@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-base.pkg"));
                }
                await Task.Run(() => File.Move($@"FPKG\{CONTENT_ID}\{CONTENT_ID}{type}.pkg", $@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-base.pkg"));
            }           
        }

        public async Task patchVER()
        {
            if (File.Exists($@"FPKG\{CONTENT_ID}\{CONTENT_ID}{type}.pkg"))
            {
                if (File.Exists($@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-patch.pkg"))
                {
                    await Task.Run(() => File.Delete($@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-patch.pkg"));
                }
                await Task.Run(() => File.Move($@"FPKG\{CONTENT_ID}\{CONTENT_ID}{type}.pkg", $@"FPKG\{CONTENT_ID}\{CONTENT_ID}-v{APP_VER}-patch.pkg"));
            }
        }

        public void Prcnt(string info)
        {
                // UI updates must be dispatched to the main UI thread
            this.Dispatcher.Invoke(() => 
            {
                prcnt.Content = "Working: "+ info;
            });

        }
        public async Task movePFSuroot()
        {
            if (patchRadioButton.IsChecked == true)
            {
                type = "-app0";
                
            }
            if (patchRadioButton.IsChecked == true)
            {
                type = "-patch0";
                
            }

            if (!Directory.Exists($@"FPKG\{CONTENT_ID}"))
            {
                Directory.CreateDirectory($@"FPKG\{CONTENT_ID}");
            }

            //string oldPath = $@"ext\uroot";
            string newPath = $@"ext\{CUSA}{type}\";

            //Maybe use uroot keystone???
            if(Directory.Exists($@"{newPath}sce_module"))
            {
                Directory.Delete($@"{newPath}sce_module", true);
                Directory.Move($@"ext\decrypted\sce_module", $@"{newPath}sce_module");
            }

            if (Directory.Exists($@"ext\uroot\sce_sys"))
            {
                Directory.Delete($@"ext\uroot\sce_sys", true);
            }

            if(File.Exists($@"ext\uroot\eboot.bin"))
            {
                File.Delete($@"ext\uroot\eboot.bin");
                File.Copy($@"ext\decrypted\eboot.bin", $@"{newPath}eboot.bin", true);
            }

            /*if (Directory.Exists(oldPath))
            {
                // Create the new directory if it doesn't exist
                Directory.CreateDirectory(newPath);

                foreach (var filePath in Directory.EnumerateFiles(oldPath, "*", SearchOption.AllDirectories))
                {
                    // Calculate the destination path for the file
                    var relativePath = filePath.Substring(oldPath.Length + 1);
                    var destinationPath = Path.Combine(newPath, relativePath);

                    // Create the directory structure for the new file
                    var directoryPath = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Copy the file
                    File.Copy(filePath, destinationPath, true);
                    WBDs("copied i think?" + filePath + destinationPath);
                }

                // Delete the old directory
                //Directory.Delete(oldPath, true);

                WBDs($"Files from {oldPath} have been copied to {newPath} and the old directory has been removed.");
            }
            else
            {
                WBDs($"The directory {oldPath} does not exist.");
            }*/

            if(File.Exists($@"{newPath}sce_sys\npbind.dat"))
            {
                File.Delete($@"{newPath}sce_sys\npbind.dat");
                File.Copy($@"ext\decrypted\sce_sys\npbind.dat", $@"{newPath}sce_sys\npbind.dat", true);
            }
            if (File.Exists($@"{newPath}sce_sys\nptitle.dat"))
            {
                File.Delete($@"{newPath}sce_sys\nptitle.dat");
                File.Copy($@"ext\decrypted\sce_sys\nptitle.dat", $@"{newPath}sce_sys\nptitle.dat", true);
            }
            if(Directory.Exists($@"{newPath}sce_sys\app"))
            {
                Directory.Delete($@"{newPath}sce_sys\app", true);
            }
            if (Directory.Exists($@"{newPath}sce_sys\changeinfo"))
            {
                Directory.Delete($@"{newPath}sce_sys\changeinfo", true);
            }
            if (Directory.Exists($@"ext\meta"))
            {
                Directory.Delete($@"ext\meta", true);
            }
            if (Directory.Exists($@"ext\trophy"))
            {
                Directory.Delete($@"ext\trophy", true);
            }           
        }

        public async Task extractAPP(string appPath)//working and used to get proper original sfo
        {
            if(baseRadioButton.IsChecked == true)
            {
                type = "-app0";
            }
            if(patchRadioButton.IsChecked == true)
            {
                type = "-patch0";
            }
            string fileName = appPath;
            var fileTableOffset = ReadIntBe(fileName, 0x18);
            var fileCount = ReadIntBe(fileName, 0x10);
            var offset = fileTableOffset;
            int? filenameTableOffset = null;

            for (var i = 0; i < fileCount; i++)
            {
                var id = ReadIntBe(fileName, offset);
                var filenameOffset = ReadIntBe(fileName, offset + 4);
                var dataOffset = ReadIntBe(fileName, offset + 16);
                var size = ReadIntBe(fileName, offset + 20);

                if (id == 512)
                {
                    filenameTableOffset = dataOffset;
                }

                if (debug)
                {
                    DebugMessage($"ID: {id:X}");
                    DebugMessage($"File name offset: {filenameOffset}");
                    DebugMessage($"Data offset: {dataOffset}");
                    DebugMessage($"Size: {size}");
                }

                if (id >= 1024)
                {
                    string filename = null;

                    if (id < 4096)
                    {
                        switch (id)
                        {
                            case 1024: filename = "license.dat"; break;
                            case 1025: filename = "license.info"; break;
                            case 1026: filename = "nptitle.dat"; break;
                            case 1027: filename = "npbind.dat"; break;
                            case 1028: filename = "selfinfo.dat"; break;
                            case 1030: filename = "imageinfo.dat"; break;
                            case 1031: filename = "target-deltainfo.dat"; break;
                            case 1032: filename = "origin-deltainfo.dat"; break;
                            case 1033: filename = "psreserved.dat"; break;
                            default:
                                filename = $"UNKNOWN_{unknownCount++}";
                                Console.Error.WriteLine($"WARNING: No file name known for file ID \"{id:X}\".");
                                Console.Error.WriteLine($"         Using file name \"{filename}\".");
                                break;
                        }
                    }

                    else if (filenameTableOffset.HasValue && filenameOffset > 0)
                    {
                        filename = ReadString(fileName, filenameTableOffset.Value + filenameOffset);
                    }

                    if (debug)
                    {
                        DebugMessage($"File name: \"{filename}\"");
                    }

                    string temppath = $@"ext\{CUSA}{type}\sce_sys\";
                    if (resume || !File.Exists(Path.Combine(temppath, filename)))
                    {
                        WBDs($@"I made it this far: {fileName}, {dataOffset}, {size}, {temppath}{filename}");
                        ExtractPkgFile(fileName, dataOffset, size, temppath + filename);

                    }
                    else
                    {
                        WarningMessage($"File already exists: \"{filename}\" (skipped).");
                    }
                }

                offset += 32;
            }

        }

        public async Task CleanUpFiles()
        {
            string path = $@"ext\";
            // Delete files in the directory.
            foreach (var file in Directory.EnumerateFiles(path))
            {
                File.Delete(file);
            }

            // Delete directories that do not end with "-patch" or "-app".
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Name.EndsWith("-patch0") && !dirInfo.Name.EndsWith("-app0"))
                {
                    Directory.Delete(directory, true); // The second parameter "true" allows recursive deletion of sub-directories and files.
                }
            }
        }

        public string GetGameFile(string directoryPath)//gets the base package
        {
            try
            {
                string pattern = $"{CONTENT_ID}-app0.pkg";
                Regex rgx = new Regex(pattern);

                var allFilePaths = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                foreach (var filePath in allFilePaths)
                {
                    if (rgx.IsMatch(Path.GetFileName(filePath)))
                    {
                        string absolutePath = Path.GetFullPath(filePath);
                        WBDs("Dumped base: " + absolutePath);
                        BASE_PKG = absolutePath;
                        return absolutePath;
                    }
                }

                throw new Exception("No game file found matching the pattern");
            }
            catch (Exception ex)
            {
                WBDs($"An error occurred while searching for the game file: {ex.Message}");
                return string.Empty;
            }
        }


        public void baseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void patchRadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void acRadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ipTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            host = ipTextBox.Text;
        }

        private void portTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            port = portTextBox.Text;
        }

        private void outputListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get the selected item in the ListBox
            FtpListItem selectedItem = outputListBox.SelectedItem as FtpListItem;

            if (selectedItem == null)
            {
                // No item was selected
                return;
            }


            if (!Directory.Exists($@"ext/{selectedItem.Name}"))
            {
                Directory.CreateDirectory($@"ext/{selectedItem.Name}");
            }

            GameDir = selectedItem.Name;
            // Download the directory
            localDirectoryPath = "ext" + "/" + GameDir;
        }

        private void allRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private List<Task> tasks = new List<Task>();
        //private bool errorFlag = false;

        public void ReadIntLe(ref int result, string fileName, int offset, int byteCount)
        {
            try
            {
                byte[] buffer = new byte[byteCount];

                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    fileStream.Position = offset;
                    fileStream.Read(buffer, 0, byteCount);
                }

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

                result = BitConverter.ToInt32(buffer, 0);
            }
            catch (Exception ex)
            {
                //errorFlag = true;
                Console.WriteLine($"An error occurred while reading the integer: {ex.Message}");
            }
        }

        public async Task checkSFO(MainWindow mainWindow, string path)
        {

            //string path = $@"CUSA29249-app\sce_sys\param.sfo";
            await Task.Run(() => PkgTool.CheckSFO(mainWindow, path));
            cusaLabel.Content = TITLE_ID;
            cidLabel.Content = CONTENT_ID;
            verLabel.Content = VERSION;
            appVerLabel.Content = APP_VER;
            gameLabel.Content = TITLE;

            //These are for the right side labels not below the icon
            sfoTITLELabel.Content = TITLE;
            APP_VERLabel.Content = APP_VER;
            ATTRIBUTELabel.Content = ATTRIBUTE;
            ATTRIBUTE2Label.Content = ATTRIBUTE2;
            CATEGORYLabel.Content = CATEGORY;
            PUBTOOLINFOcLabel.Content = PUBTOOLINFO_c_Date;
            PUBTOOLINFOsdkLabel.Content = PUBTOOLINFO_sdk_ver;
            PUBTOOLINFOstLabel.Content = PUBTOOLINFO_st_type;
            PUBTOOLINFOl0Label.Content = PUBTOOLINFO_img0_l0_size;
            PUBTOOLINFOl1Label.Content = PUBTOOLINFO_img0_l1_size;
            PUBTOOLINFOscLabel.Content = PUBTOOLINFO_img0_sc_ksize;
            PUBTOOLINFOpcLabel.Content = PUBTOOLINFO_img0_pc_ksize;
            VERSIONsLabel.Content = VERSION;
        }

        public async Task moveAPPtoSCE()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            string oldPath = $@"ext\temp\sce_sys";
            string newPath = $@"FPKG\{CONTENT_ID}-patch\sce_sys";

            if (Directory.Exists(oldPath))
            {
                // Create the new directory if it doesn't exist
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                foreach (var filePath in Directory.EnumerateFiles(oldPath, "*", SearchOption.AllDirectories))
                {
                    // Calculate the destination path for the file
                    var relativePath = filePath.Substring(oldPath.Length + 1);
                    var destinationPath = Path.Combine(newPath, relativePath);

                    // Create the directory structure for the new file
                    var directoryPath = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Copy the file
                    File.Copy(filePath, destinationPath, true);
                }

                Console.WriteLine($"Files from {oldPath} have been copied to {newPath} and the old directory has been removed.");
            }
            else
            {
                Console.WriteLine($"The directory {oldPath} does not exist.");
            }
        }

        public async Task pkgBASE(MainWindow mainWindow)
        {

            WBDs($@"Creating Base Game PKG");
            var outdir = $@"FPKG\{CONTENT_ID}";
            Console.WriteLine("pkgBASE Content ID: "+ outdir);
            var gp4 = $@"ext\{CUSA}{type}\test.gp4";

            await Task.Run(() => pkgTool.createfPKG(gp4, $@"{outdir}")); 

            GetGameFile(outdir);
            if (File.Exists($@"ext\param.sfo"))
            {
                File.Copy($@"ext\param.sfo", $@"{outdir}\param.sfo", true);
                File.Copy($@"ext\icon0.png", $@"{outdir}\icon0.png", true);
                //File.Copy($@"ext\param.sfo", $@"libs\param.sfo", true);
                //File.Copy($@"ext\icon0.png", $@"libs\icon0.png", true);
            }

        }

        public async Task patchBASE(MainWindow mainWindow)
        {
            
            WBDs($@"Creating Patch and Marrying it to the Base Game PKG");
            Console.WriteLine(CONTENT_ID+"JUST WORK PATCH FILE YOU FUCK");
            var outdir = $@"FPKG\{CONTENT_ID}";
            var gp4 = $@"ext\{CUSA}{type}\test.gp4";

            await Task.Run(() => pkgTool.createfPKG(gp4, $@"{outdir}"));

            //GetGameFile(outdir);
            if (File.Exists($@"ext\param.sfo"))
            {
                File.Copy($@"ext\param.sfo", $@"{outdir}\param.sfo", true);
                File.Copy($@"ext\icon0.png", $@"{outdir}\icon0.png", true);
                //File.Copy($@"ext\param.sfo", $@"libs\param.sfo", true);
                //File.Copy($@"ext\icon0.png", $@"libs\icon0.png", true);
            }

        }
        //async task to make gp4 and base pkg
        public async Task gp4BASE(string gamePath, string outDir, string tid)
        {
            WBDs($@"Creating GP4 File");
            type = "-app0";
            
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            MakeAppGP4.makeAppGP4(gamePath, tid);
            //MakeAppGP4.makeAppGP4($@"ext\CUSA20085-app0", $@"UP0001-CUSA20085_00-SCOTTPILGRIMGAME");
    
        }
        //async task to make gp4 and patch 
        public async Task gp4PATCH()
        {
            type = "-patch0";
            GetGameFile($@"FPKG\{CONTENT_ID}");
            var outdir = $@"FPKG\{CONTENT_ID}";
            var gp4 = $@"ext\patch_output.gp4";
            MakePatchGP4.makePatchGP4($@"ext\{CUSA}{type}", $@"{CONTENT_ID}", $@"{BASE_PKG}");//$@"FPKG\UP0001-CUSA20085_00-SCOTTPILGRIMGAME\UP0001-CUSA20085_00-SCOTTPILGRIMGAME-app0.pkg");//BASE_PKG);
        }

        public async Task ftpList()
        {
            var token = new CancellationToken();
            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port))
            {
                try
                {
                    using (var conn = new AsyncFtpClient(host, username, password, int.Parse(port)))
                    {
                        //Console.WriteLine("Connecting");
                        WBDs("Connecting to PS4 with: " + host + ":" + port);
                        await conn.Connect(token);
                        // Get the directory listing
                        var listing = await conn.GetListing("/mnt/sandbox/pfsmnt/");
                        this.Title = "Connected to PS4 with: " + host + ":" + port;
                        WBDs("Connected to PS4 with: " + host + ":" + port);

                        if (baseRadioButton.IsChecked == true)
                        {
                            // Stop the loading animation
                            outputListBox.ItemsSource = null;
                            outputListBox.Items.Clear();


                            var filteredListing = listing
                            .Where(item => !item.Name.EndsWith("-union") && !item.Name.EndsWith("-nest") && !item.Name.EndsWith("-ac") && !item.Name.EndsWith("-patch0"))
                            .ToList();

                            // Update the UI on the UI thread
                            Dispatcher.Invoke(() =>
                            {
                                //SystemSounds.Beep.Play();

                                outputListBox.ItemsSource = filteredListing;
                            });                         

                            // Download the directory and all its files &sub - folders
                            if (listing.Any()) // check if listing has any items
                            {
                                try
                                {
                                    if (outputListBox.SelectedItem == null)
                                    {
                                        try
                                        {
                                            loadingLabel.Content = "No item is selected in the list box.";

                                        }
                                        catch (System.NullReferenceException ex)
                                        {
                                            outputListBox.ItemsSource = "Is ftp enabled on the ps4? " + ex.Message;

                                        }

                                        return;
                                    }

                                    FtpListItem firstItem = outputListBox.SelectedItem as FtpListItem;

                                    if (firstItem?.Name == null)
                                    {
                                        loadingLabel.Content = $@"No Base Game Loaded.";
                                        return;
                                    }

                                    string dlPath = "ext/" + firstItem.Name;

                                    // Get the directory listing
                                    string cusa = $@"{firstItem.Name.Substring(0, firstItem.Name.Length - 5)}";
                                    CUSA = cusa;
                                    cusaLabel.Content = cusa;
                                    await conn.Disconnect();
                                }
                                catch (System.NullReferenceException ex)
                                {

                                    outputListBox.ItemsSource = "Is ftp enabled on the ps4? " + ex.Message;
                                }


                            }
                        }

                        if (patchRadioButton.IsChecked == true)
                        {
                            // Stop the loading animation
                            outputListBox.ItemsSource = null;
                            outputListBox.Items.Clear();


                            var filteredListing = listing
                            .Where(item => !item.Name.EndsWith("-union") && !item.Name.EndsWith("-nest") && !item.Name.EndsWith("-ac") && !item.Name.EndsWith("-app0"))
                            .ToList();

                            // Update the UI on the UI thread
                            Dispatcher.Invoke(() =>
                            {
                                //SystemSounds.Beep.Play();

                                outputListBox.ItemsSource = filteredListing;
                            });

                            // Create a new progress reporting object
                            var progress = new Progress<FtpProgress>(x =>
                            {
                                WBDs($"Transferred: {x.Progress}%");
                            });

                            // Download the directory and all its files &sub - folders
                            if (listing.Any()) // check if listing has any items
                            {

                                outputListBox.SelectedIndex = 0;
                                FtpListItem firstItem = outputListBox.SelectedItem as FtpListItem; // get the first item
                                string dlPath = @"ext\" + firstItem.Name;

                                if (firstItem == null)
                                {
                                    loadingLabel.Content = $@"No Base Game Loaded.";
                                }

                                // Get the directory listing
                                string cusa = $@"{firstItem.Name.Substring(0, firstItem.Name.Length - 7)}";
                                CUSA = cusa;
                                cusaLabel.Content = cusa;
                                await conn.Disconnect();


                            }
                        }
                        if (allRadioButton.IsChecked == true)
                        {
                            // Stop the loading animation
                            outputListBox.ItemsSource = null;
                            outputListBox.Items.Clear();


                            var filteredListing = listing
                            .Where(item => !item.Name.EndsWith("-union") && !item.Name.EndsWith("-nest") && !item.Name.EndsWith("-ac"))
                            .ToList();

                            // Update the UI on the UI thread
                            Dispatcher.Invoke(() =>
                            {
                                //SystemSounds.Beep.Play();

                                outputListBox.ItemsSource = filteredListing;
                            });

                            // Create a new progress reporting object
                            var progress = new Progress<FtpProgress>(x =>
                            {
                                WBDs($"Transferred: {x.Progress}%");
                            });

                            // Download the directory and all its files &sub - folders
                            if (listing.Any()) // check if listing has any items
                            {

                                outputListBox.SelectedIndex = 0;
                                FtpListItem firstItem = outputListBox.SelectedItem as FtpListItem; // get the first item
                                string dlPath = "ext/" + firstItem.Name;

                                if (firstItem == null)
                                {
                                    loadingLabel.Content = $@"No Base Game Loaded.";
                                }

                                // Get the directory listing
                                string cusa = $@"{firstItem.Name.Substring(0, firstItem.Name.Length - 5)}";
                                CUSA = cusa;
                                cusaLabel.Content = cusa;
                                await conn.Disconnect();


                            }
                        }
                    }
                }
            
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"You need to fill out the IP and Port information to continue.\n\nIP: {host}\nPort: {port}\n\nCheck your IP and Port if you do not see it above.", $@"{error}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public async Task DoneSon()
        {
            Thread soundThread = new Thread(() =>
            {
                while (true)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Thread.Sleep(1000); // Pause for a second
                }
            });

            soundThread.Start();
            MessageBoxResult result = MessageBox.Show($@"Your Dumped Files Are In (ext\{CUSA}{type})", "All Done!", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                soundThread.Abort(); // Stop the sound when OK is clicked
            }
        }

        private void MyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                portTextBox.Text = String.Format("9090");
                button1.Content = "Send";
            });
        }

        private void MyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                portTextBox.Text = String.Format("2121");
                button1.Content = "Connect";

            });
        }
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            if (sendPayloadCheckBox.IsChecked == true)
            {
                netCat();
                WBDs("Payload Sent");

            }
            //
            if (sendPayloadCheckBox.IsChecked == false)
            {
                await ftpList();// lists the check radio box "base, update, dlc " thats currently loaded in the sandbox on the ps4.
            }
        }

        public async Task dlAPPs()
        {
            MainWindow.WBDs($@"{TITLE_ID}");
            if (baseRadioButton.IsChecked == true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if(baseRadioButton.IsChecked == true)
                    {
                        type = "-app0";                       

                    }
                    if (patchRadioButton.IsChecked == true)
                    {
                        type = "-patch0";

                    }
                });
            }

            using (var conn = new AsyncFtpClient(host, username, password, int.Parse(port)))
            {
                string filterAC = "-";
                string filterNest = "";
                string filterUnion = "";
                string filterPkgT = "";
                string cusa = "";


                if (baseRadioButton.IsChecked == true)
                {
                    filterAC = "-ac";
                    filterNest = "-nest";
                    filterUnion = "-union";
                    filterPkgT = "-patch0";
                    appPKG = "app.pkg";


                }
                if (patchRadioButton.IsChecked == true)
                {
                    filterAC = "-ac";
                    filterNest = "-nest";
                    filterUnion = "-union";
                    filterPkgT = "-app0";
                    appPKG = "patch.pkg";

                }

                var token = new CancellationToken();
                await conn.Connect(token);

                // Get the directory listing
                var listing = await conn.GetListing("/mnt/sandbox/pfsmnt/");

                // Stop the loading animation
                this.Dispatcher.Invoke(() =>
                {
                    outputListBox.ItemsSource = null;
                    outputListBox.Items.Clear();

                });
                try
                {
                    var filteredListing = listing.Where(item => !item.Name.EndsWith(filterUnion) && !item.Name.EndsWith(filterNest) && !item.Name.EndsWith(filterAC) && !item.Name.EndsWith(filterPkgT))
                .ToList();

                    //double totalSize = filteredListing.Where(item => item.Type == FtpObjectType.File).Sum(item => item.Size);

                    this.Title = "Connected to PS4 with: " + host + ":" + port;
                    WBDs("Connected to PS4 with: " + host + ":" + port);

                    //SystemSounds.Beep.Play();
                    if (filteredListing == null)
                    {
                        loadingLabel.Content = "Error: Is ftp and your game running on your PS4?";
                        outputListBox.ItemsSource = String.Format("No Content To Load");
                    }
                    outputListBox.ItemsSource = filteredListing;
                }
                catch
                {
                    
                }
  
                long totalDownloadedBytes = 0;
                var progress = new Progress<FtpProgress>(p =>
                {
                    totalDownloadedBytes += p.TransferredBytes;
                    double totalDownloadedBytesInMB = totalDownloadedBytes / 1024.0 / 1024.0;

                    if (p.Progress != 0)
                    {
                        double totalSizeInMB = totalSize / 1024.0 / 1024.0;
                        double totalSizeInMBRounded = Math.Floor(totalSizeInMB); // Declared and assigned totalSizeInMBRounded

                        double overallProgress = totalDownloadedBytesInMB / totalSizeInMB * 100;////////////////////////////////////////////This whole thing is fuuuuuuuucked except the total download ammount......
                        string fileName = Path.GetFileName(p.RemotePath);
                        double transferredBytesInMB = p.TransferredBytes / 1024.0 / 1024.0;
                        double transferredBytesInMBRounded = Math.Floor(transferredBytesInMB);
                        double totalDownloadedBytesInMBRounded = Math.Floor(totalDownloadedBytesInMB);
                        int overallProgress1 = Convert.ToInt32(overallProgress / 100);
                        int totalDownloadedBytesInMBRounded1 = Convert.ToInt32(totalDownloadedBytesInMBRounded / 100);
                        double total = overallProgress1;
                        double percentageToDeduct = 8; // Replace with your actual percentage
                        double deductedTotal = total - (total * percentageToDeduct / 100);
                        WBDs($"Downloading file {fileName}, {transferredBytesInMBRounded}MB {p.Progress}% ");
                        Prcnt($"Overall progress: {deductedTotal}%,  Transfered: {totalDownloadedBytesInMBRounded1}MB of {totalSizeInMBRounded}MB");
                        Console.WriteLine($"Downloading file {fileName}, File progress: {p.Progress}%, Overall progress: {overallProgress1}%, Transferred MB: {transferredBytesInMBRounded}, Total downloaded MB: {totalDownloadedBytesInMBRounded1}, Total MB: {totalSizeInMBRounded}");
                    }
                });

                if (listing.Any()) // check if listing has any items
                {
                    outputListBox.SelectedIndex = 0;
                    FtpListItem firstItem = outputListBox.SelectedItem as FtpListItem; // get the first item

                    if (firstItem == null)
                    {
                        loadingLabel.Content=$@"No game started on the ps4.";
                    }
                    else
                    {
                        // Get the directory listing
                        int l = 0;
                        if (baseRadioButton.IsChecked == true)
                        {
                            l = 5;
                        }
                        // Get the directory listing
                        if (patchRadioButton.IsChecked == true)
                        {
                            l = 7;                            
                        }
                        cusa = $@"{firstItem.Name.Substring(0, firstItem.Name.Length - l)}";
                        CUSA = cusa;
                        
                        string dlPath = $@"ext\{CUSA}{type}";
                        WBDs(dlPath);

                        //This is where the downloads start
                        if (firstItem != null)
                        {
                            //USE THIS IF YOU WANT TO USE THE PFS IMAGE FILES AND NOT INDIVIDUAL FILES
                            /*WBDs($@"Downloading pfs_image.dat package");
                            await conn.DownloadFile($@"ext\pfs_image.dat", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}-nest/pfs_image.dat", FtpLocalExists.Overwrite, FtpVerify.None);
                            string pfsPath = $@"ext\pfs_image.dat";
                            string outPath = $@"ext\";

                            WBDs($@"Extracting pfs_image.dat package");
                            await pfstool(this, pfsPath, outPath);
                            string pfsP = $@"ext\pfs_image.dat";*/
                            /*WBDs("delete pfs");
                            if (File.Exists(pfsP))
                            {
                                File.Delete(pfsP);
                                WBDs("deleted pfs");
                            }*/

                            WBDs($@"Calculating Download Size for {CUSA}");
                            Console.WriteLine(dlPath);
                            totalSize = await GetDirectorySizeAsync(conn, firstItem.FullName);

                            await conn.DownloadDirectory(dlPath, firstItem.FullName, FtpFolderSyncMode.Update, FtpLocalExists.Overwrite, FtpVerify.Retry, null, progress);

                            WBDs($@"Download of {firstItem.Name} Finished. Total Size: {totalSize}");
                            totalDownloadedBytes = 0;

                            if (!Directory.Exists($@"ext\{CUSA}{type}\sce_sys\about\"))
                            {
                                Directory.CreateDirectory($@"ext\{CUSA}{type}\sce_sys\about\");
                                WBDs($@"Calculating Download Size for right.prx package");
                                totalSize = await GetDirectorySizeAsync(conn, $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/sce_sys/about/");
                                Console.WriteLine(dlPath);

                                WBDs($@"Downloading right.prx package");
                                totalSize = await GetDirectorySizeAsync(conn, $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/sce_sys/about/");

                                await conn.DownloadFile($@"ext\{CUSA}{type}\sce_sys\about\right.sprx", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/sce_sys/about/right.sprx", FtpLocalExists.Overwrite, FtpVerify.None);
                            }
                            totalDownloadedBytes = 0;
                            string keyStn = $@"ext\uroot\sce_sys\keystone";
                            if (File.Exists(keyStn))
                            {
                                File.Copy(keyStn, $@"libs\keystone", true);
                                File.Copy(keyStn, $@"ext\{CUSA}{type}\sce_sys\keystone", true);
                                WBDs("copied keystone to libs");
                            }
                        }

                        totalDownloadedBytes = 0;
                        //Download app0
                        if (firstItem != null)
                        {
                            WBDs($@"Downloading app package");
                            string appPKGPath = $@"{dlPath}\sce_sys\{appPKG}";
                            // Get the directory listing
                            string p_app = "";
                            if (baseRadioButton.IsChecked == true)
                            {
                                p_app = "app";
                            }
                            // Get the directory listing
                            if (patchRadioButton.IsChecked == true)
                            {
                                p_app = "patch";
                            }
                            totalSize = await GetDirectorySizeAsync(conn, $@"/user/{p_app}/{cusa}/");

                            await conn.DownloadFile(appPKGPath, $@"/user/{p_app}/{cusa}/{appPKG}", FtpLocalExists.Overwrite, FtpVerify.None);
                            loadingLabel.Content = $@"Dump finished: {dlPath}\sce_sys\{appPKG}";
                            appPath = appPKGPath;
                            totalDownloadedBytes = 0;
                            if (!Directory.Exists($@"ext\{CUSA}{type}\sce_sys\app"))
                            {
                                Directory.CreateDirectory($@"ext\{CUSA}{type}\sce_sys\app\");
                                Directory.CreateDirectory($@"ext\{CUSA}{type}\sce_sys\changeinfo\");
                                Directory.CreateDirectory($@"ext\{CUSA}{type}\sce_sys\trophy");
                            }
                            WBDs($@"Extracting App Package");
                            await extractAPP(appPKGPath);
                            //this.Close();
                            if (File.Exists(appPKGPath))
                            {
                                File.Copy($@"{dlPath}\sce_sys\param.sfo", $@"ext\param.sfo", true);
                                File.Delete(appPKGPath);
                                await checkSFO(this, $@"ext\param.sfo");
                            }

                            WBDs($@"Getting app meta");
                            if (firstItem != null)
                            {
                                if (!Directory.Exists($@"ext\meta"))
                                {
                                    Directory.CreateDirectory($@"ext\meta");
                                }
                                WBDs($@"Calculating Download Size for right.prx package");
                                totalSize = await GetDirectorySizeAsync(conn, $@"/user/appmeta/{CUSA}");
                                WBDs($@"getting appmeta");
                                listing = await conn.GetListing("/user/appmeta");
                                await conn.DownloadDirectory($@"ext\meta", $@"/user/appmeta/{CUSA}", FtpFolderSyncMode.Update, FtpLocalExists.Overwrite, FtpVerify.None, null, progress);
                                totalDownloadedBytes = 0;
                            }

                        }

                        if (File.Exists($@"ext\meta\icon0.png"))
                        {
                            await unloadPic();
                            File.Copy($@"ext\meta\icon0.png", $@"ext\icon0.png", true);
                            File.Copy($@"ext\meta\icon0.png", $@"libs\icon0.png", true);

                            string newIconPath = Path.Combine(Directory.GetCurrentDirectory(), @"libs\icon0.png");

                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(newIconPath, UriKind.Absolute);
                            bitmap.EndInit();

                            image.Source = bitmap;
                        }
                        totalDownloadedBytes = 0;
                        //Get decrypted files
                        if (firstItem != null)
                        {
                            WBDs($@"getting decrypted nptitle, npbind");

                            if (!Directory.Exists($@"ext\decrypted"))
                            {
                                Directory.CreateDirectory($@"ext\decrypted");
                                Directory.CreateDirectory($@"ext\decrypted\sce_sys");
                            }
                            await conn.DownloadFile($@"ext\decrypted\eboot.bin", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/eboot.bin", FtpLocalExists.Overwrite, FtpVerify.None);
                            await conn.DownloadFile($@"ext\{CUSA}{type}\eboot.bin", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/eboot.bin", FtpLocalExists.Overwrite, FtpVerify.None);

                            await conn.DownloadDirectory($@"ext\decrypted\sce_module", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/sce_module", FtpFolderSyncMode.Mirror, FtpLocalExists.Overwrite, FtpVerify.None, null, progress);
                            await conn.DownloadDirectory($@"ext\{CUSA}{type}\sce_module", $@"/mnt/sandbox/pfsmnt/{CUSA}{type}/sce_module", FtpFolderSyncMode.Mirror, FtpLocalExists.Overwrite, FtpVerify.None, null, progress);

                            await conn.DownloadFile($@"ext\decrypted\sce_sys\nptitle.dat", $@"/system_data/priv/appmeta/{CUSA}/nptitle.dat", FtpLocalExists.Overwrite, FtpVerify.None);
                            await conn.DownloadFile($@"ext\{CUSA}{type}\sce_sys\nptitle.dat", $@"/system_data/priv/appmeta/{CUSA}/nptitle.dat", FtpLocalExists.Overwrite, FtpVerify.None);

                            await conn.DownloadFile($@"ext\decrypted\sce_sys\npbind.dat", $@"/system_data/priv/appmeta/{CUSA}/npbind.dat", FtpLocalExists.Overwrite, FtpVerify.None);
                            await conn.DownloadFile($@"ext\{CUSA}{type}\sce_sys\npbind.dat", $@"/system_data/priv/appmeta/{CUSA}/npbind.dat", FtpLocalExists.Overwrite, FtpVerify.None);
                            if (type == "-patch0")
                            {
                                await conn.DownloadFile($@"ext\decrypted\sce_sys\param.sfo", $@"/system_data/priv/appmeta/{CUSA}/param.sfo", FtpLocalExists.Overwrite, FtpVerify.None);//This is where the patch sfo lives
                            }
                        }

                        if (firstItem != null)
                        {
                            WBDs($@"Checking npbind for trophy information.");
                            //This reads the npbind and creates a string for the trophy path
                            string data = File.ReadAllText($@"ext\decrypted\sce_sys\npbind.dat");
                            Regex regex = new Regex(@"NPWR[0-9]{5}_[0-9]{2}");
                            MatchCollection matches = regex.Matches(data);

                            string[] trophy_dirs = new string[matches.Count];
                            for (int i = 0; i < matches.Count; i++)
                            {
                                trophy_dirs[i] = matches[i].Value;
                            }
                            bool debug = true; // Set this as appropriate
                            if (debug)
                            {
                                debugMessage = string.Join(", ", trophy_dirs);
                            }

                            WBDs($@"Getting decrypted trophy file");

                            //Dump decrypted trophy to sce_sys/trophy
                            string debugMessage2 = debugMessage.Insert(debugMessage.Length - 2, "_");//Keeps adding 2 _
                            Console.WriteLine(debugMessage2);
                            npLabel.Content = debugMessage2; // Or handle the debug message in the way you prefer
                                                             // Get the directory listing
                            if (!Directory.Exists($@"ext\trophy"))
                            {
                                Directory.CreateDirectory($@"ext\trophy");
                            }
                            //await fixRight($@"ext\{CUSA}{type}\sce_sys\about\right.sprx");

                            listing = await conn.GetListing($@"/user/trophy/conf/{debugMessage}");
                            await conn.DownloadDirectory($@"ext\trophy", $@"/user/trophy/conf/{debugMessage}", FtpFolderSyncMode.Update, FtpLocalExists.Overwrite, FtpVerify.None, null, progress);
                            //Fix trophy directory name
                            string trophyPath = $@"/user/trophy/conf/{debugMessage}/TROPHY.TRP";

                            loadingLabel.Content = $@"/user/trophy/conf/{debugMessage}";
                            //gets original trophy name from the extracted app directory and renames
                            await TrpNameAsync($@"ext\{CUSA}-app0\sce_sys\trophy\");

                            File.Move($@"ext\trophy\TROPHY.TRP", $@"ext\trophy\{TRP}");
                            Console.WriteLine($@"ext\trophy\TROPHY.TRP to ext\trophy\{TRP}");

                            if (File.Exists($@"ext\trophy\{TRP}"))
                            {
                                File.Copy($@"ext\trophy\{TRP}", $@"ext\{CUSA}{type}\sce_sys\trophy\{TRP}", true);
                            }                          
                        }

                        WBDs($@"Downloading Keystone");
                        if (firstItem != null)
                        {
                            await conn.DownloadFile($@"ext\keystone", $@"/mnt/sandbox/pfsmnt/{CUSA}-app0/sce_sys/keystone", FtpLocalExists.Overwrite, FtpVerify.None);
                            CheckKeystone($@"ext\keystone");
                        }
                        WBDs($@"Done Downloading Files");
                    }
                }
            }
        }

       public async Task fixRight(string rightPath)//If the rights file comes up short on padding this will add the needed padding to the rights.sprx
        {         
            const int startPaddingPos = 0x8570;  // Changed this to 0x8570
            const int endPaddingPos = 0xBFFF;
            const byte paddingByte = 0x00;

            using (var stream = new FileStream(rightPath, FileMode.Open, FileAccess.Write))
            {
                stream.Position = startPaddingPos;

                for (int i = startPaddingPos; i < endPaddingPos; i++)
                {
                    stream.WriteByte(paddingByte);
                }
            }
        }

        //Sends request to FTP Server to self decrypt
        public async Task SendDECRYPTToFtpServerAsync()
        {
            WBDs("Sending DECRYPT command to FTP Server");
            // Get values from text boxes
            var port = portTextBox.Text;
            var ipaddr = ipTextBox.Text;
            var filepath = "/mnt";
            var command = "DECRYPT";

            FtpClient client = new FtpClient(ipaddr, int.Parse(port));
            client.Connect();

            FtpListItem[] items = client.GetListing(filepath);
            foreach (FtpListItem item in items)
            {
                Console.WriteLine(item);
            }

            FtpReply reply = client.Execute(command);
            Console.WriteLine("test: " + reply.InfoMessages);
            codeLabel.Content = "PS4 SELF decryption: " + reply.Command + reply.Code;
            Console.WriteLine("Code: " + reply.Code);
            Console.WriteLine("Message: " + reply.Command);   
        }

        public async Task<string> TrpNameAsync(string trpPath)//get proper trophy name for decrypted trophy
        {
            TRP = string.Empty;
            // Get the list of files in the directory
            var files = Directory.GetFiles(trpPath);

            foreach (var file in files)
            {
                // Check if the file name matches our pattern
                if (Path.GetExtension(file) == ".trp")
                {
                    TRP = Path.GetFileName(file);
                    break;  // exit the loop once we found the file
                }
            }

            if (string.IsNullOrEmpty(TRP))
            {
                // Handle the case when no file ending with .trp was found
                Console.WriteLine("No .trp file found.");
            }
            else
            {
                // Use TRP here
                Console.WriteLine("TRP file name: " + TRP);
            }

            return TRP;
        }

        public async Task MakePKGgp4()
        {
            await Task.Run(() => PkgTool.makeBaseGP4($@"ext\{CUSA}{type}-gp4", $@"ext\{CUSA}{type}\", CONTENT_ID));
        }

        public async Task fixmaGP4()//test function to modify GP4, not needed unless running tests
        {
            // specify the path to your XML file
            string xmlFilePath = $@"ext\test.gp4";
            //string xmlFilePath = $@"ext\{CUSA}{type}-gp4\Project.gp4";
            string xmlContent = File.ReadAllText(xmlFilePath);

            string oldHeader = "<?xml version=\"1.0\"?>";
            string newHeader = "<?xml version=\"1.1\" encoding=\"utf-8\" standalone=\"yes\"?>";
            string oldh2 = "<psproject xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" fmt=\"gp4\" version=\"1000\">";
            string newh2 = "<psproject fmt=\"gp4\" version=\"1000\">";
            string cDate = " c_date=\"2017-07-20\" ";
            string rmCdate = "";
            string chnk = "layer_no=\"0\"";
            string newchnk = " ";
            string oldfilez = "<files img_no=\"0\">";
            string newfilez = "<files>";

            xmlContent = xmlContent.Replace(oldHeader, newHeader);
            xmlContent = xmlContent.Replace(oldh2, newh2);
            xmlContent = xmlContent.Replace(cDate, rmCdate);
            xmlContent = xmlContent.Replace(chnk, newchnk);
            xmlContent = xmlContent.Replace(oldfilez, newfilez);


            string basePath = Directory.GetCurrentDirectory();
            Console.WriteLine(basePath + "This is string base path");
            string commonPath = basePath+$@"\"; // replace this with the common path prefix
            Console.WriteLine(commonPath);
            //string workin = $@"{basePath}\ext\{CUSA}{type}";
            string workin = $@"{basePath}\ext\CUSA29249-app0";
            string result = Regex.Replace(xmlContent, "(<file [^>]*?orig_path=\")([^\"]+)(\"[^>]*?>)", m =>
            {
                string prefix = m.Groups[1].Value;
                string origPath = m.Groups[2].Value;
                string suffix = m.Groups[3].Value;

                if (origPath.StartsWith(commonPath))
                {
                    origPath = origPath.Substring(commonPath.Length);
                }

                string absolutePath = Path.Combine(workin, origPath.Replace('/', Path.DirectorySeparatorChar));

                if (!origPath.Contains("sce_sys") && !origPath.Contains("sce_module") && !origPath.Contains("eboot.bin"))
                {
                    suffix = suffix.Insert(suffix.Length - 2, " pfs_compression=\"enable\"");
                }

                return prefix + absolutePath + suffix;
            });

            //result = result.Replace(" />", "/>");
            //result = Regex.Replace(result, @"\s+", " ");

            File.WriteAllText(xmlFilePath, result);
            Console.WriteLine(result);


            XDocument xmlDocument = XDocument.Load(xmlFilePath);
            XElement volumeElement = xmlDocument.Root.Element("volume");
            XElement volumeIdElement = new XElement("volume_id", "PS4VOLUME");
            volumeElement.Element("volume_type").AddAfterSelf(volumeIdElement);
            xmlDocument.Save(xmlFilePath);

            // Load your XML document
            // Get the 'volume_ts' element
            XElement volumeTsElement = xmlDocument.Root.Element("volume").Element("volume_ts");
            // Get the current value of 'volume_ts' element
            string currentVolumeTsValue = volumeTsElement.Value;
            // Extract the date part and set the time part to '00:00:00'
            string newVolumeTsValue = DateTime.Parse(currentVolumeTsValue).ToString("yyyy-MM-dd") + " 00:00:00";
            // Set the new value
            volumeTsElement.Value = newVolumeTsValue;
            // Save the changes
            xmlDocument.Save(xmlFilePath);

            // Convert the XML to string
            string xmlString = xmlDocument.ToString();

            // Replace all occurrences of " />" with "/>"
            xmlString = xmlString.Replace(" />", "/>");

            // Write the new XML content to the file
            File.WriteAllText(xmlFilePath, xmlString);

            List<string> lines = File.ReadAllLines(xmlFilePath).ToList();

            // Insert the new line at the beginning
            lines.Insert(0, newHeader);

            // Write all lines back to the file
            File.WriteAllLines(xmlFilePath, lines);

            XDocument doc = XDocument.Load(xmlFilePath); // Load the XML file
            var files = doc.Descendants("file").ToList(); // Get all 'file' elements

            // Sort the 'file' elements by 'targ_path' attribute
            var sortedFiles = files.OrderBy(x => (string)x.Attribute("targ_path")).ToList();

            // Separate the sorted 'file' elements into two lists: 
            // Those with 'pfs_compression' attribute equals to 'enable' and those without
            var filesWithPfs = sortedFiles.Where(x => ((string)x.Attribute("pfs_compression")) == "enable").ToList();
            var filesWithoutPfs = sortedFiles.Where(x => ((string)x.Attribute("pfs_compression")) != "enable").ToList();

            // Remove all 'file' elements from the document
            files.ForEach(x => x.Remove());

            // Add 'file' elements back to the document in the desired order
            var parent = doc.Descendants("files").First(); // Get the 'files' element
            filesWithoutPfs.ForEach(x => parent.Add(x));
            filesWithPfs.ForEach(x => parent.Add(x));

            // Save the updated XML document back to the file
            doc.Save(xmlFilePath);

            //File.Copy(xmlFilePath, $@"ext\{CUSA}{type}.gp4", true);
            File.Copy(xmlFilePath, $@"ext\test.gp4", true);

            string xmlFilePathz = $@"ext\test.gp4"; // Specify the path to your XML file

            var linez = File.ReadLines(xmlFilePathz).ToList(); // Read all lines of the file

            for (int i = 4; i < linez.Count; i++) // Start from the fifth line
            {
                // Replace " />" with "/>"
                linez[i] = linez[i].Replace(" />", "/>");
            }

            // Write the modified lines back to the file
            File.WriteAllLines(xmlFilePathz, linez);
            File.Copy(xmlFilePathz, $@"ext\test.gp4", true);
        }

        public static void ReorderXML(string xmlFilePath)//another test function for building the gp4. MakeAppGP4.cs is the final I am using
        {
            var xdoc = XDocument.Load(xmlFilePath);

            var orderedFiles = new[] {
            "sce_sys/",
            "sce_module/",
            "eboot.bin"
        };

            var files = xdoc.Descendants("file").Where(f => !f.Attribute("pfs_compression")?.Value.Equals("enable") ?? false).ToList();

            foreach (var file in files)
            {
                file.Remove();
            }

            foreach (var orderedFile in orderedFiles)
            {
                var matchingFiles = orderedFile.EndsWith("/") ?
                    files.Where(f => f.Attribute("targ_path").Value.StartsWith(orderedFile)).ToList() :
                    files.Where(f => f.Attribute("targ_path").Value.EndsWith(orderedFile)).ToList();

                foreach (var file in matchingFiles)
                {
                    xdoc.Root.AddFirst(file);
                    files.Remove(file);
                }
            }

            xdoc.Save(xmlFilePath);
        }



        public async Task<long> GetDirectorySizeAsync(AsyncFtpClient client, string remoteDirectoryPath)
        {
            long totalSize = 0;

            var items = await client.GetListing(remoteDirectoryPath);

            foreach (var item in items)
            {
                if (item.Type == FtpObjectType.File)
                {
                    totalSize += item.Size;
                }
                else if (item.Type == FtpObjectType.Directory)
                {
                    totalSize += await GetDirectorySizeAsync(client, item.FullName);
                }
            }

            return totalSize;
        }
        public static int SearchBytePattern(byte[] pattern, byte[] bytes, int startIndex)
        {
            int patternLen = pattern.Length;
            int totalLen = bytes.Length;
            byte firstByte = pattern[0];

            for (int i = startIndex; i <= totalLen - patternLen; i++)
            {
                if (bytes[i] != firstByte) continue;

                for (int j = patternLen - 1; j >= 1; j--)
                {
                    if (bytes[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }

            return -1;
        }
        //Below are tests being ran from button.
        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            //string xmlFilePath = "";
            //ReorderXML(xmlFilePath);
            await dlAPPs();

            //await SendDECRYPTToFtpServerAsync();
            await movePFSuroot();
            type = "-app0";
            apptype = "pkg_ps4_app";
            await checkSFO(this, $@"ext\param.sfo");

            /*//Create GP4
            type = "-app0";
            string gamePath = $@"ext\{TITLE_ID}{type}";
            string gameGp4Path = $@"ext\{TITLE_ID}{type}.gp4";
            string outDir = $@"FPKG\{CONTENT_ID}";
            string tid = $@"{CONTENT_ID}";           
            await Task.Run(async () => await TempAppGP4(gamePath));

            WBDs($@"Creating Base Game PKG");
            var pkgoutdir = $@"FPKG\{tid}\";
            Console.WriteLine("pkgBASE Content ID: " + pkgoutdir);
            await Task.Run(async () => await tempBuildFPKG(gameGp4Path, gamepath));*/
            if (!Directory.Exists($@"FPKG\{CONTENT_ID}"))
            {
                Directory.CreateDirectory($@"FPKG\{CONTENT_ID}");
            }
            string outDir = $@"FPKG\{CONTENT_ID}\";
            string gamePath = $@"ext\{TITLE_ID}{type}";
            await gp4BASE(gamePath, outDir, CONTENT_ID);
            //Gp4Creator.fixSFO(this);
            //var dumpedGame = $@"ext\CUSA07023{type}";
            //await Gp4Creator.CreateProjectFromDIRAsync($@"ext\", dumpedGame, "00000000000000000000000000000000");


            //File.Delete($@"ext\{TITLE_ID}{type}\test.gp4");
            //Gp4Creator.CreateProjectFromDirectory(this, $@"FPKG\", $@"ext\CUSA07023-app0", "UP0177-CUSA07023_00-SONICMANIA000000");// tried reusing code from unpack package and its less optimal than just making a new gp4 because the original wants to move all files to make the list. Mine creates a list in memory and stores it to xml, modifies gp4 for errors and saves the fixed formatted gp4 file.
            //await fixmaGP4();

            //string xmlFilePath = $@"ext\test.gp4";
            //ReorderXML(xmlFilePath);

            LicenseDatWriter writer = new LicenseDatWriter(null); // pass null or some Stream object if it's required for other methods
            byte[] entitlementKey = null;
            writer.CreateAndWriteToFile(CONTENT_ID, ContentType.GD, entitlementKey, $@"ext\{TITLE_ID}{type}\sce_sys\license.dat");



            //await Task.Run(() => pkgTool.makeFPKG(this, gp4, $@"{pkgoutdir}"));
            //Other test functions
            //await SendDECRYPTToFtpServerAsync();
            //PkgTool.makeFPKG(this, $@"ext\test.gp4", "UP0177-CUSA07023_00-SONICMANIA000000");
            await Task.Run(() => pkgTool.createfPKG($@"ext\test.gp4", "UP0177-CUSA07023_00-SONICMANIA000000"));
            //await gp4PATCH($@"ext\CUSA07023-app0");
            //GetGameFile($@"FPKG\UP0177-CUSA07023_00-SONICMANIA000000\");
            //await dlPATCHs();
            //await dlAPPs();
            //await checkSFO(this, $@"param.sfo");


        }
       //More tests for pkg building.
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string path = $@"ext\{CUSA}{type}\param.sfo";

            if (!File.Exists(path))
            {
                // Handle the case where the file does not exist.
                Console.WriteLine("File does not exist.");
                return;
            }

            List<byte> fileBytes = new List<byte>(File.ReadAllBytes(path));

            // Convert the byte array into a string using ASCII encoding.
            string text = Encoding.ASCII.GetString(fileBytes.ToArray());

            string searchString = "c_date=";
            int startIndex = text.IndexOf(searchString);
            if (startIndex == -1)
            {
                // Handle the case where the searchString does not exist in the file.
                Console.WriteLine("String not found in file.");
                return;
            }

            int endIndex = text.IndexOf(",", startIndex);
            if (endIndex == -1)
            {
                // Handle the case where there's no comma after the searchString.
                Console.WriteLine("No comma found after the date.");
                return;
            }

            // Prepare the string to be inserted.
            string insertString = "sdk_ver=04508000,st_type=digital50,";
            byte[] insertBytes = Encoding.ASCII.GetBytes(insertString);

            // Add 1 to the endIndex to place the insertion point after the comma.
            fileBytes.InsertRange(endIndex + 1, insertBytes);

            // Create a byte sequence for null bytes equal to the length of the insertBytes.
            byte[] nullBytes = new byte[insertBytes.Length];

            int nullIndex = SearchBytePattern(nullBytes, fileBytes.ToArray(), endIndex + 1 + insertBytes.Length);

            if (nullIndex == -1)
            {
                // Handle the case where there's no sequence of null bytes of equal length.
                Console.WriteLine("No null bytes found after the inserted string.");
                return;
            }

            // Remove the found sequence of null bytes.
            fileBytes.RemoveRange(nullIndex, insertBytes.Length);

            // Save the modified bytes back to the file.
            File.WriteAllBytes(path, fileBytes.ToArray());
        }
    }
}

