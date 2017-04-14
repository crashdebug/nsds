using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace NSDS.Core
{
	public abstract class BaseVersion : IComparable<BaseVersion>
	{
		[JsonProperty("version")]
		public virtual string Version { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		public abstract int CompareTo(BaseVersion other);

		protected BaseVersion(string version = null)
		{
			this.Version = version;
			this.Created = DateTime.UtcNow;
		}
	}

	public class VersionResource
	{
		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("pathQuery")]
		public string PathQuery { get; set; }
	}

	public interface IVersionParser
	{
		[JsonIgnore]
		Regex Pattern { get; }

		BaseVersion Parse(string input);
	}

	public class DateVersion : BaseVersion, IVersionParser
	{
		private DateTime? dateTime;

		/*public override string Version
		{
			get => base.Version;
			set 
			{
				base.Version = value;
				this.dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", new DateTimeFormatInfo());
			}
		}*/

		private static Regex _pattern = new Regex(@"\d{4}-\d{2}-\d{2}(Z|\s+)(\d{2}:\d{2}:\d{2}(\.\d+)?)?", RegexOptions.Compiled | RegexOptions.Singleline);
		public Regex Pattern => _pattern;

		public DateVersion()
		{
		}

		public DateVersion(string version) : base(version)
		{
		}

		private	DateTime GetDateTime()
		{
			if (!this.dateTime.HasValue)
			{
				this.dateTime = DateTime.ParseExact(this.Version, "yyyy-MM-dd HH:mm:ss", new DateTimeFormatInfo());
			}
			return this.dateTime.Value;
		}

		public override int CompareTo(BaseVersion other)
		{
			if (other == null)
			{
				return 1;
			}
			if (other is DateVersion)
			{
				return DateTime.Compare(this.GetDateTime(), ((DateVersion)other).GetDateTime());
			}
			throw new ArgumentException($"Cannot compare types of {typeof(DateVersion).FullName} and {other.GetType().FullName}");
		}

		public BaseVersion Parse(string input)
		{
			return new DateVersion(input);
		}
	}

	public class NumericVersion : BaseVersion, IVersionParser
	{
		private static Regex _pattern = new Regex(@"\d+(\.\d+){1,3}", RegexOptions.Compiled | RegexOptions.Singleline);
		public Regex Pattern => _pattern;

		private Version version;

		/*public override string Version
		{
			get => this.version?.ToString();
			set => this.version = new Version(value);
		}*/

		public NumericVersion()
		{
		}

		public NumericVersion(string version) : base(version)
		{
			this.version = new Version(version);
		}

		public NumericVersion(int major, int minor, int build = 0, int revision = 0)
		{
			this.version = new Version(major, minor, build, revision);
		}

		private Version GetVersion()
		{
			if (this.version == null)
			{
				this.version = new Version(this.Version);
			}
			return this.version;
		}

		public override int CompareTo(BaseVersion other)
		{
			if (other == null)
			{
				return 1;
			}
			if (other is NumericVersion)
			{
				var numVersion = other as NumericVersion;
				if (this.GetVersion() > numVersion.GetVersion())
				{
					return 1;
				}
				else if (this.GetVersion() < numVersion.GetVersion())
				{
					return -1;
				}
				return 0;
			}
			throw new ArgumentException($"Cannot compare types of {typeof(NumericVersion).FullName} and {other.GetType().FullName}");
		}

		public BaseVersion Parse(string input)
		{
			return new NumericVersion(input);
		}
	}
}
