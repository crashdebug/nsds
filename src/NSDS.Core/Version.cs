using System;
using System.Globalization;

namespace NSDS.Core
{
	public abstract class BaseVersion : IComparable<BaseVersion>
	{
		public virtual string Version { get; set; }

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
}
