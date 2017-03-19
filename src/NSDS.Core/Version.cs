﻿using System;
using System.Globalization;
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
	}

	public class DateVersion : BaseVersion
	{
		private DateTime dateTime;

		public override string Version
		{
			get => base.Version;
			set 
			{
				base.Version = value;
				this.dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", new DateTimeFormatInfo());
			}
		}

		public DateVersion()
		{
		}

		public DateVersion(string version)
		{
			this.Version = version;
		}

		public override int CompareTo(BaseVersion other)
		{
			if (other is DateVersion)
			{
				return DateTime.Compare(this.dateTime, ((DateVersion)other).dateTime);
			}
			throw new ArgumentException($"Cannot compare types of {typeof(DateVersion).FullName} and {other.GetType().FullName}");
		}
	}

	public class NumericVersion : BaseVersion
	{
		private Version version;

		public NumericVersion(string version)
		{
			this.version = new Version(version);
		}

		public NumericVersion(int major, int minor, int build = 0, int revision = 0)
		{
			this.version = new Version(major, minor, build, revision);
		}

		public override int CompareTo(BaseVersion other)
		{
			if (other is NumericVersion)
			{
				var numVersion = other as NumericVersion;
				if (this.version > numVersion.version)
				{
					return 1;
				}
				else if (this.version < numVersion.version)
				{
					return -1;
				}
				return 0;
			}
			throw new ArgumentException($"Cannot compare types of {typeof(NumericVersion).FullName} and {other.GetType().FullName}");
		}
	}
}
