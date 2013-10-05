using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MiNG.DevTool.CodeGenerator
{
	class PropertyReader : IDisposable
	{
		public const String Missed = null;

		private static readonly Char[] Delimiters = ":{;}".ToCharArray();

		private static readonly Regex NamePatern = new Regex(@"@?[a-zA-Z_][\da-zA-Z_]*");

		private static readonly HashSet<String> KwdAccessibility = new HashSet<String> { "public", "private", "protected", "internal" };
		private static readonly HashSet<String> KwdStatic = new HashSet<String> { "static" };
		private static readonly HashSet<String> KwdGetSet = new HashSet<String> { "get", "set" };

		private readonly LineReader reader;

		private String name;

		private String getAccessibility;
		private String setAccessibility;

		public PropertyReader(String line)
		{
			this.reader = new LineReader(line, Delimiters);
		}

		public String ReadAccessibility(String @default = Missed)
		{
			var accessibility = this.reader.PeekWord();

			if (KwdAccessibility.Contains(accessibility))
			{
				this.reader.ReadWord();
				var accessibility2 = this.reader.PeekWord();
				if ((accessibility == "protected" && accessibility2 == "internal")
					|| (accessibility == "internal" && accessibility2 == "protected"))
				{
					this.reader.ReadWord();
					return "protected internal";
				} else
				{
					return accessibility;
				}
			} else
			{
				return @default ?? Missed;
			}
		}

		public String ReadStatic()
		{
			var @static = this.reader.PeekWord();

			if (KwdStatic.Contains(@static))
			{
				this.reader.ReadWord();
				return @static;
			} else
			{
				return Missed;
			}
		}

		public String ReadType(String @default = Missed)
		{
			var word = this.reader.ReadWord();
			var next = this.reader.PeekWord();
			if (next == ":" || next == "{" || next == null)
			{	// word is name
				this.name = word;
				return @default ?? Missed;
			} else
			{	// word is type
				// Not neccessary to check.
				return word;
			}
		}

		public String ReadName()
		{
			if (this.name != null)
			{	// name has already been parsed in ReadType.
				return this.name;
			}

			var name = this.reader.PeekWord();

			if (NamePatern.IsMatch(name))
			{
				this.reader.ReadWord();
				return name;
			} else
			{
				throw new FormatException("Property name invalid.");
			}
		}

		public String ReadCode(String @default = Missed)
		{
			var colon = this.reader.PeekWord();
			if (colon == ":")
			{
				this.reader.ReadWord();
				var code = this.reader.PeekWord();
				if (code == "{" && code == ";")
				{
					return @default ?? Missed;
				} else
				{
					return code;
				}
			} else
			{
				return @default ?? Missed;
			}
		}

		public void ReadAccessorBlock()
		{
			if (this.reader.PeekWord() == "{")
			{
				this.reader.ReadWord();
				while (this.reader.PeekWord() != "}")
				{
					var accessibility = this.ReadAccessibility();
					var accessor = this.reader.ReadWord();
					switch (accessor)
					{
						case "get":
							this.getAccessibility = accessibility ?? String.Empty /*Empty means getter exist*/;
							break;

						case "set":
							this.setAccessibility = accessibility ?? String.Empty /*Empty means setter exist*/;
							break;

						default:
							throw new FormatException("Invalid accessor.");
					}
					var semicolon = this.reader.ReadWord();
					if (semicolon != ";")
					{
						throw new FormatException("Accessor must be followed with a semicolon.");
					}
				}
			}
		}

		public Boolean ReadGetterAccessibility(out String accessibility, String @default = Missed)
		{
			if (this.getAccessibility == null)
			{
				accessibility = null;
				return false;
			} else
			{
				accessibility = (this.getAccessibility != String.Empty) ? this.getAccessibility : @default;
				return true;
			}
		}

		public Boolean ReadSetterAccessibility(out String accessibility, String @default = Missed)
		{
			if (this.setAccessibility == null)
			{
				accessibility = null;
				return false;
			} else
			{
				accessibility = (this.setAccessibility != String.Empty) ? this.setAccessibility : @default;
				return true;
			}
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

	}
}
