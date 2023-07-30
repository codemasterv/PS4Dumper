using PS4wpfDumper.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PS4wpfDumper.PKG
{
  public class PkgValidator
  {
    public enum ValidationType
    {
      Hash,
      Signature,
      Limit
    }
    public enum ValidationResult
    {
      /// <summary>
      /// The hash or signature was successfully validated
      /// </summary>
      Ok,
      /// <summary>
      /// The hash or signature was invalid
      /// </summary>
      Fail,
      /// <summary>
      /// The hash or signature could not be verified due to lack of keys
      /// </summary>
      NoKey,
    }
    public class Validation
    {
      /// <summary>
      /// The type of validation being checked.
      /// </summary>
      public readonly ValidationType Type;
      /// <summary>
      /// Short name of the digest
      /// </summary>
      public readonly string Name;
      /// <summary>
      /// Human-readable description of this validation step.
      /// </summary>
      public readonly string Description;
      /// <summary>
      /// The offset of the final hash or digest in the PKG file;
      /// </summary>
      public readonly long Location;
      public System.Func<ValidationResult> Validate;
      public Validation(ValidationType t, string name, string desc, long location)
      {
        Type = t;
        Name = name;
        Description = desc;
        Location = location;
      }
      public override bool Equals(object obj)
      => obj is Validation v ? v.Type == Type && v.Name == Name && v.Location == Location : false;
      public override int GetHashCode()
      => Util.Crypto.CombineHashCodes(
          Type.GetHashCode(),
          Name.GetHashCode(),
          Description.GetHashCode(),
          Location.GetHashCode());
    }

    private static ValidationResult CheckHashes(byte[] expected, byte[] actual)
    {
      if(expected.SequenceEqual(actual))
      {
        return ValidationResult.Ok;
      }
      else
      {
        return ValidationResult.Fail;
      }
    }

    private Pkg pkg;
    public PkgValidator(Pkg pkg)
    {
      this.pkg = pkg;
    }

    private Dictionary<GeneralDigest, System.Tuple<string, string>> GeneralDigests =
      new Dictionary<GeneralDigest, System.Tuple<string, string>> {
        { GeneralDigest.ContentDigest,
          System.Tuple.Create("Content Digest",
            "A hash of the Content ID, DRM type, Content Type, PFS Image digest, and Major Param digest") },
        { GeneralDigest.GameDigest,
          System.Tuple.Create("Game Digest",
            "The PFS image digest") },
        { GeneralDigest.HeaderDigest,
          System.Tuple.Create("Header Digest",
            "A hash of the first 64 bytes and the 128 bytes at 0x400 of the PKG") },
        { GeneralDigest.SystemDigest,
          System.Tuple.Create("System Digest",
            "???") },
        { GeneralDigest.MajorParamDigest,
          System.Tuple.Create("Major Param Digest",
            "A hash of the ATTRIBUTE, CATEGORY, FORMAT, and PUBTOOLVER param.sfo entries") },
        { GeneralDigest.ParamDigest,
          System.Tuple.Create("Param Digest",
            "A hash of the entire param.sfo file (entry)") },
        { GeneralDigest.PlaygoDigest,
          System.Tuple.Create("Playgo Digest",
            "???") },
        { GeneralDigest.TrophyDigest,
          System.Tuple.Create("Trophy Digest",
            "???") },
        { GeneralDigest.ManualDigest,
          System.Tuple.Create("Manual Digest",
            "???") },
        { GeneralDigest.KeymapDigest,
          System.Tuple.Create("Keymap Digest",
            "???") },
        { GeneralDigest.OriginDigest,
          System.Tuple.Create("Origin Digest",
            "???") },
        { GeneralDigest.TargetDigest,
          System.Tuple.Create("Target Digest",
            "???") },
        { GeneralDigest.OriginGameDigest,
          System.Tuple.Create("Origin Game Digest",
            "???") },
        { GeneralDigest.TargetGameDigest,
          System.Tuple.Create("Target Game Digest",
            "???") },
      };

    public List<Validation> Validations(Stream pkgStream)
    {
      var validations = new List<Validation>();

      if (pkg.Header.content_type != ContentType.AL)
      {
        validations.Add(new Validation(ValidationType.Limit, "PKG Size", "The PKG should be aligned to 0x8000 and have a minimum size of 1MB", 0x418)
        {
          Validate = () => pkg.Header.mount_image_size % 0x8000 == 0 && pkg.Header.mount_image_size >= 0x100000 ? ValidationResult.Ok : ValidationResult.Fail
        });
      }

      // Create validators for PKG general digests
      foreach(var d in pkg.GeneralDigests.Digests)
      {
        if (d.Key == GeneralDigest.DigestMetaData) continue;
        if (!pkg.GeneralDigests.set_digests.HasFlag(d.Key)) continue;
        var meta = GeneralDigests[d.Key];
        var generalDigestsLoc = pkg.GeneralDigests.meta.DataOffset;
        var validation = new Validation(
          ValidationType.Hash, 
          meta.Item1, 
          meta.Item2, 
          generalDigestsLoc + ((int)System.Math.Log((int)d.Key,2) * 32));
        switch (d.Key)
        {
          case GeneralDigest.ContentDigest:
            validation.Validate = () =>
              CheckHashes(pkg.GeneralDigests.Digests[GeneralDigest.ContentDigest], pkg.ComputeContentDigest());
            break;
          case GeneralDigest.GameDigest:
            validation.Validate = () =>
              CheckHashes(pkg.GeneralDigests.Digests[GeneralDigest.GameDigest], pkg.Header.pfs_image_digest);
            break;
          case GeneralDigest.HeaderDigest:
            validation.Validate = () =>
              CheckHashes(pkg.GeneralDigests.Digests[GeneralDigest.HeaderDigest], pkg.ComputeHeaderDigest());
            break;
          case GeneralDigest.MajorParamDigest:
            validation.Validate = () =>
              CheckHashes(pkg.GeneralDigests.Digests[GeneralDigest.MajorParamDigest], pkg.ComputeMajorParamDigest());
            break;
          case GeneralDigest.ParamDigest:
            validation.Validate = () =>
              CheckHashes(pkg.GeneralDigests.Digests[GeneralDigest.ParamDigest], Util.Crypto.Sha256(pkg.ParamSfo.ParamSfo.Serialize()));
            break;
          default:
            // Don't know how to compute other hashes, so skip them.
            continue;
        }
        validations.Add(validation);
      }

      var digests = pkg.Digests;
      var digestsOffset = pkg.Metas.Metas.Where(m => m.id == EntryId.DIGESTS).First().DataOffset;
      for (var i = 1; i < pkg.Metas.Metas.Count; i++)
      {
        var meta = pkg.Metas.Metas[i];
        var j = i;
        validations.Add(new Validation(ValidationType.Hash,
          $"{meta.id} digest",
          $"Hash of the {meta.id} entry",
          digests.meta.DataOffset + (32 * j))
        {
          Validate = () =>
            CheckHashes(
              digests.FileData.Skip(32 * j).Take(32).ToArray(),
              Util.Crypto.Sha256(pkgStream, meta.DataOffset, meta.DataSize)),
        });
      }

      validations.Add(new Validation(
        ValidationType.Hash,
        "PFS Signed Digest",
        "A hash of the first 0x10000 bytes of the PFS image",
        0x460)
      {
        Validate = () =>
          CheckHashes(
            pkg.Header.pfs_signed_digest,
            Util.Crypto.Sha256(pkgStream, (long)pkg.Header.pfs_image_offset, 0x10000))
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "PFS Image Digest",
        "A hash of the entire PFS image",
        0x440)
      {
        Validate = () =>
          CheckHashes(
            pkg.Header.pfs_image_digest,
            Util.Crypto.Sha256(pkgStream, (long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size))
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "Body Digest",
        "Hash of the PKG body (entry filesystem)",
        0x160)
      {
        Validate = () =>
          CheckHashes(
            pkg.Header.body_digest,
            Util.Crypto.Sha256(pkgStream, (long)pkg.Header.body_offset, (long)pkg.Header.body_size))
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "Digest Table Hash",
        "Hash of the entry digests table",
        0x140)
      {
        Validate = () =>
          CheckHashes(
            pkg.Header.digest_table_hash,
            Util.Crypto.Sha256(pkg.Digests.FileData))
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "SC Entries Hash 1",
        "Hash of 5 entries (ENTRY_KEYS, IMAGE_KEY, GENERAL_DIGESTS, METAS, DIGESTS)",
        0x100)
      {
        Validate = () =>
        {
          var ms = new MemoryStream();
          foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas, pkg.Digests })
          {
            new SubStream(pkgStream, entry.meta.DataOffset, entry.meta.DataSize).CopyTo(ms);
          }
          return CheckHashes(pkg.Header.sc_entries1_hash, Util.Crypto.Sha256(ms));
        }
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "SC Entries Hash 2",
        "Hash of 4 entries (ENTRY_KEYS, IMAGE_KEY, GENERAL_DIGESTS, METAS)",
        0x120)
      {
        Validate = () =>
        {
          var ms = new MemoryStream();
          foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas })
          {
            long size = entry.meta.DataSize;
            if (entry.Id == EntryId.METAS)
            {
              size = pkg.Header.sc_entry_count * 0x20;
            }
            new SubStream(pkgStream, entry.meta.DataOffset, size).CopyTo(ms);
          }
          return CheckHashes(pkg.Header.sc_entries2_hash, Util.Crypto.Sha256(ms));
        }
      });

      validations.Add(new Validation(
        ValidationType.Hash,
        "PKG Header Digest",
        "Hash of the first 0xFE0 bytes of the PKG",
        0xFE0)
      {
        Validate = () =>
          CheckHashes(
            pkg.HeaderDigest,
            Util.Crypto.Sha256(pkgStream, 0, 0xFE0))
      });

      validations.Add(new Validation(
        ValidationType.Signature,
        "PKG Header Signature",
        "Signed hash of the first 0x1000 bytes of the PKG",
        0x1000)
      {
        Validate = () =>
        {
          byte[] header_sha256 = Util.Crypto.Sha256(pkgStream, 0, 0x1000);
          var sig = Util.Crypto.RSA2048EncryptKey(Keys.PkgSignKey, header_sha256);
          return sig.SequenceEqual(pkg.HeaderSignature) ? ValidationResult.Ok : ValidationResult.NoKey;
        }
      });

      if(pkg.LicenseDat is Rif.LicenseDat ld)
      {
        validations.Add(new Validation(
         ValidationType.Signature,
         "Debug RIF Signature",
         "Signed hash of the debug RIF secret",
         ld.meta.DataOffset + 0x300)
        {
          Validate = () =>
          {
            using (var ms = new MemoryStream())
            {
              ld.Write(ms);
              var hash = Util.Crypto.Sha256(ms, 0, 0x300);
              return Util.Crypto.RSA2048VerifySha256(hash, ld.Signature, RSAKeyset.DebugRifKeyset)
                ? ValidationResult.Ok : ValidationResult.Fail;
            }
          }
        });
      }

      return validations;
    }

    /// <summary>
    /// Checks the hashes and signatures for this PKG.
    /// </summary>
    /// <returns>Returns a list of validation steps and their success (true) or failure (false).</returns>
    public IEnumerable<System.Tuple<Validation, ValidationResult>> Validate(Stream pkgStream)
    {
      foreach(var validation in Validations(pkgStream))
      {
        yield return System.Tuple.Create(validation, validation.Validate());
      }
    }
  }
}
