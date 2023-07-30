
using System;

namespace PS4wpfDumper.PKG
{
  /// <summary>
  /// Collection of data to use when building a PKG.
  /// </summary>
  public class PkgProperties
  {
        /// <summary>
        /// The volume type (should be one of the pkg_* types)
        /// </summary>
        public GP4.VolumeType VolumeType;
//public string VolumeType = MainWindow.type;
    /// <summary>
    /// 36 Character ID for the PKG
    /// </summary>
    public string ContentId;

        public string StorageType;
    /// <summary>
    /// 32 Character Passcode
    /// </summary>
        public string Passcode = "00000000000000000000000000000000";

    /// <summary>
    /// The volume timestamp.
    /// </summary>
    public DateTime TimeStamp;

    /// <summary>
    /// 32 Hex Character Entitlement Key (For AC, AL only)
    /// </summary>
    public string EntitlementKey;

    /// <summary>
    /// The creation date/time. Leave as default(DateTime) to disable
    /// </summary>
    public DateTime CreationDate;

    /// <summary>
    /// Set to true to use the creation time in addition to the date
    /// </summary>
    public bool UseCreationTime;


    /// <summary>
    /// The root of the directory tree for the PFS image.
    /// </summary>
    //string rootdir = $@"ext\{MainWindow.TITLE_ID}{MainWindow.type}";
        public PFS.FSDir RootDir;

        //public PFS.FSDir RootDir;

        public static PkgProperties FromGp4(GP4.Gp4Project project, string projDir)
    {
            //project = $@"etc\{MainWindow.CUSA}{MainWindow.type}\test.gp4";
      DateTime CreationDate;
      bool UseCreationTime = false;
      if ((project.volume.Package.CreationDate ?? "") == "")
      {
        CreationDate = default;
      }
      else if (project.volume.Package.CreationDate == "actual_datetime")
      {
        CreationDate = default;
        UseCreationTime = true;
      }
      else
      {
        var split = project.volume.Package.CreationDate.Split(' ');
        UseCreationTime = split.Length == 2; // Date and time specified
        CreationDate = DateTime.Parse(project.volume.Package.CreationDate).ToUniversalTime();
      }
            return new PkgProperties
            {
                VolumeType = GP4.VolumeType.pkg_ps4_app,
                ContentId = MainWindow.CONTENT_ID,
                TimeStamp = project.volume.TimeStamp,
                StorageType = MainWindow.PUBTOOLINFO_st_type,

          Passcode = project.volume.Package.Passcode,
          CreationDate = CreationDate,

          EntitlementKey = project.volume.Package.EntitlementKey,
          UseCreationTime = UseCreationTime,


          RootDir = PFS.PfsProperties.BuildFSTree(project, $@"etc\{MainWindow.CUSA}{MainWindow.type}")
      };
    }
  }
}
