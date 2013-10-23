using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiNG.DevTool.CodeGenerator
{
	/// <summary>
	/// Specification:
	/// [ACS] [INS] [Type] PropertyName[:CodeName] [{ [[ACS] get;] [[ACS] set;] }]
	/// </summary>
	class PropertyProcessor : IDisposable
	{
		private static readonly String DefaultGetterTemplate = "return this[\"{0}\"];";
		private static readonly String DefaultSetterTemplate = "this[\"{0}\"] = value;";

		private readonly PropertyReader reader;

		private String propertyAccess;
		private String propertyStatic;
		private String propertyType;
		private String propertyName;
		private String propertyCode;
		private Boolean propertyGettable;
		private String propertyGetAccess;
		private Boolean propertySettable;
		private String propertySetAccess;

		public DefaultSettings Default { get; set; }

		public String GetterTemplate { get; set; }
		public String SetterTemplate { get; set; }

		public String Document { get; set; }

		public PropertyProcessor(String line)
		{
			this.reader = new PropertyReader(line);
		}

		public void Process()
		{
			this.propertyAccess = this.reader.ReadAccessibility(this.Default.PropertyAccess);
			this.propertyStatic = this.reader.ReadStatic();
			this.propertyType = this.reader.ReadType(this.Default.PropertyType);
			this.propertyName = this.reader.ReadName();
			var code = (this.Default.CodeNameMappingHandler != null) ? this.Default.CodeNameMappingHandler(propertyName) : propertyName;
			this.propertyCode = this.reader.ReadCode(code);

			this.reader.ReadAccessorBlock();
			this.propertyGettable = this.reader.ReadGetterAccessibility(out this.propertyGetAccess, this.Default.PropertyGetAccess);
			this.propertySettable = this.reader.ReadSetterAccessibility(out this.propertySetAccess, this.Default.PropertySetAccess);

			if (!this.propertyGettable && !this.propertySettable)
			{
				if (!this.Default.PropertyGettable && !this.Default.PropertySettable)
				{
					this.propertyGettable = true;
					this.propertySettable = true;
				} else
				{
					this.propertyGettable = this.Default.PropertyGettable;
					this.propertySettable = this.Default.PropertySettable;
				}
			}

			if (this.GetterTemplate == null)
			{
				this.GetterTemplate = DefaultGetterTemplate;
			}
			if (this.SetterTemplate == null)
			{
				this.SetterTemplate = DefaultSetterTemplate;
			}
		}

		public void WriteProperty(IndentStreamWriter writer)
		{
			this.WriteDocument(writer);
			var declare = new String[]
			{
				this.propertyAccess,
				this.propertyStatic,
				this.propertyType, 
				this.propertyName,
			};
			var declareLine = String.Join(" ", declare.Where(w => w != PropertyReader.Missed));

			writer.WriteLineIndent(declareLine);
			using (new BraceWriter(writer))
			{
				if (this.propertyGettable)
				{
					writer.WriteIndent(String.Empty);
					if (!String.IsNullOrWhiteSpace(this.propertyGetAccess))
					{
						writer.Write("{0} ", this.propertyGetAccess);
					}
					writer.WriteLine("get");
					using (new BraceWriter(writer))
					{
						writer.WriteLineIndent(this.GetterTemplate, this.propertyCode);
					}
				}
				if (this.propertySettable)
				{
					writer.WriteIndent(String.Empty);
					if (!String.IsNullOrWhiteSpace(this.propertySetAccess))
					{
						writer.Write("{0} ", this.propertySetAccess);
					}
					writer.WriteLine("set");
					using (new BraceWriter(writer))
					{
						writer.WriteLineIndent(this.SetterTemplate, this.propertyCode);
					}
				}
			}
			writer.WriteLine();
		}

		private void WriteDocument(IndentStreamWriter writer)
		{
			if (String.IsNullOrWhiteSpace(this.Document))
			{
				return;
			}
			writer.WriteLineIndent("/// <summary>");
			using (var docReader = new StringReader(this.Document))
			{
				while (true)
				{
					var line = docReader.ReadLine();
					if (line == null)
					{
						break;
					}
					writer.WriteLineIndent("/// {0}", line);
				}
			}
			writer.WriteLineIndent("/// </summary>");
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

		public struct DefaultSettings
		{
			public String PropertyAccess { get; set; }
			public String PropertyType { get; set; }
			public Func<String, String> CodeNameMappingHandler { get; set; }
			public Boolean PropertyGettable { get; set; }
			public String PropertyGetAccess { get; set; }
			public Boolean PropertySettable { get; set; }
			public String PropertySetAccess { get; set; }
		}

	}
}
