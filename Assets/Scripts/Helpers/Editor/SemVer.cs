using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KoboldTools {
	public enum ParseMode {
		Normal = 1,
		Strict = 2,
		Permissive = 4,
	}

	[Serializable]
	public class SemVer {
		private static Regex semverEx = new Regex(
			@"^(?<major>\d+)" +
			@"(\.(?<minor>\d+))?" +
			@"(\.(?<patch>\d+))?" +
			@"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
			@"(\+(?<build>[0-9A-Za-z\-\.]+))?" +
			@"(?<ign>.*)$",
			RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture
		);
		public uint Major;
		public uint Minor;
		public uint Patch;
		public string Prerelease;
		public string BuildMetadata;

		public SemVer() {
			this.Major = 0;
			this.Minor = 0;
			this.Patch = 0;
			this.Prerelease = "";
			this.BuildMetadata = "";
		}

		public SemVer(uint major, uint minor, uint patch, string prerelease = "", string buildMetadata = "") {
			this.Major = major;
			this.Minor = minor;
			this.Patch = patch;
			this.Prerelease = prerelease ?? "";
			this.BuildMetadata = buildMetadata ?? "";
		}

		public static SemVer Parse(string version, ParseMode mode) {
			Match match = SemVer.semverEx.Match(version);
			if (match.Success) {
				uint major = uint.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);
				uint minor = 0;
				Group minorMatch = match.Groups["minor"];
				if (minorMatch.Success) {
					minor = uint.Parse(minorMatch.Value, CultureInfo.InvariantCulture);
				} else if (mode == ParseMode.Strict) {
					throw new ArgumentException("The version string is missing a minor version", "version");
				}

				uint patch = 0;
				Group patchMatch = match.Groups["patch"];
				if (patchMatch.Success) {
					patch = uint.Parse(patchMatch.Value, CultureInfo.InvariantCulture);
				} else if (mode == ParseMode.Strict) {
					throw new ArgumentException("The version string is missing a patch version", "version");
				}

				string prerelease = match.Groups["pre"].Value;
				string buildMetadata = match.Groups["build"].Value;

				bool superfluousParts = match.Groups["ign"].Success;
				if (superfluousParts && mode == ParseMode.Normal) {
					return null;
				} else if (superfluousParts && mode == ParseMode.Strict) {
					throw new ArgumentException("The version string contains superfluous information", "version");
				}

				return new SemVer(major, minor, patch, prerelease, buildMetadata);
			} else if (mode == ParseMode.Strict) {
				throw new ArgumentException("Unrecognized version convention", "version");
			}

			return null;
		}

		public string Public() {
			return String.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
		}

		public string Internal() {
			return String.Format("v{0}.{1}.{2}b{3}", this.Major, this.Minor, this.Patch, this.BuildMetadata);
		}

		public string Ios() {
			uint builds = 0;
			if (uint.TryParse(this.BuildMetadata, out builds)) {
				return String.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Patch, builds);
			} else {
				return null;
			}
		}

		public int Android() {
			int num;
			if (int.TryParse(this.Ios().Replace(".", ""), out num)) {
				return num;
			} else {
				return -1;
			}
		}

		public string Canonical() {
			string p = String.IsNullOrEmpty(this.Prerelease) ? "" : String.Format("-{0}", this.Prerelease);
			string b = String.IsNullOrEmpty(this.BuildMetadata) ? "" : String.Format("+{0}", this.BuildMetadata);
			return String.Format("{0}.{1}.{2}{3}{4}", this.Major, this.Minor, this.Patch, p, b);
		}

		public override string ToString() {
			return this.Internal();
		}
	}
}