using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiNG.DevTool.CodeGenerator
{
	class HeaderProcessor
	{
		private readonly List<String> _fullUsings = new List<String>();
		private readonly List<String> _shortUsings = new List<String>();
		private String _namespace;
		private String _base;
		private readonly List<String> _implements = new List<String>();

		private PropertyProcessor.DefaultSettings _propertyDefaultSettings;
		public PropertyProcessor.DefaultSettings PropertyDefaultSettings
		{
			get
			{
				return this._propertyDefaultSettings;
			}
		}

		public String Namespace { get; set; }
		public String ClassName { get; set; }

		public HeaderProcessor()
		{
			this._propertyDefaultSettings = new PropertyProcessor.DefaultSettings();
			this._propertyDefaultSettings.PropertyAccess = "public";
			this._propertyDefaultSettings.PropertyType = "String";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <returns>Continuable.</returns>
		public Boolean Process(String line)
		{
			if (String.IsNullOrWhiteSpace(this.ClassName))
			{
				throw new InvalidOperationException("ClassName must be set.");
			}

			using (var reader = new LineReader(line))
			{
				var instruction = reader.ReadWord();
				switch (instruction)
				{
					case "@end":
						return false;
					case "@using":
						var @using = reader.ReadToEnd();
						if (String.IsNullOrEmpty(@using))
						{
							throw new FormatException("@using not allow empty definition.");
						}
						if (@using.StartsWith("."))
						{
							this._shortUsings.Add(@using.Remove(0, 1));
						} else
						{
							this._fullUsings.Add(@using);
						}
						break;
					case "@ns":
					case "@namespace":
						if (this._namespace != null)
						{
							throw new FormatException("Duplicated @namespace.");
						}
						var ns = reader.ReadToEnd();
						if (String.IsNullOrEmpty(ns))
						{
							throw new FormatException("@namespace not allow empty definition.");
						}
						this._namespace = ns;
						break;
					case "@base":
						if (this._base != null)
						{
							throw new FormatException("Duplicated @base.");
						}
						var @base = reader.ReadToEnd();
						if (String.IsNullOrEmpty(@base))
						{
							throw new FormatException("@base not allow empty definition.");
						}
						this._base = @base.Replace("$class", this.ClassName);
						break;
					case "@impl":
					case "@implement":
						var impl = reader.ReadToEnd();
						if (String.IsNullOrEmpty(impl))
						{
							throw new FormatException("@using not allow empty definition.");
						}
						this._implements.Add(impl.Replace("$class", this.ClassName));
						break;
					case "@defaultAccess":
						var defPropAccess = reader.ReadToEnd();
						this._propertyDefaultSettings.PropertyAccess = defPropAccess;
						break;
					case "@defaultType":
						var defPropType = reader.ReadToEnd();
						this._propertyDefaultSettings.PropertyType = defPropType;
						break;
					default:
						// Ignore unknown instruction.
						break;
				}
				return true;
			}
		}

		public void WriteHeader(IndentStreamWriter writer)
		{
			if (this._fullUsings.Count == 0)
			{
				writer.WriteLine("using System;");
			} else
			{
				foreach (var @using in this._fullUsings.OrderBy(key => key))
				{
					writer.WriteLine("using {0};", @using);
				}
			}
			writer.WriteLine();

			writer.WriteLine("namespace {0}", this._namespace ?? this.Namespace);
			writer.WriteOpenBrace();
			foreach (var @using in this._shortUsings.OrderBy(key => key))
			{
				writer.WriteLineIndent("using {0};", @using);
			}
			writer.WriteLine();
			writer.WriteIndent("public partial class {0}", this.ClassName);
			if (this._base != null)
			{
				writer.Write(" : {0}", this._base);
			}
			if (this._implements.Count != 0)
			{
				writer.Write(this._base == null ? " : {0}" : ", {0}", String.Join(", ", this._implements));
			}
			writer.WriteLine();
			writer.WriteOpenBrace();
		}

	}
}
