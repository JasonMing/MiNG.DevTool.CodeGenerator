using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace MiNG.DevTool.CodeGenerator
{
	[Guid(GuidList.DelegatingPocoGenerator)]
	public class CSharpDataObjectGenerator : TemplatedCodeGenerator
	{
		private static readonly String GeneratedIdentifier = ".Generated";

		protected override Byte[] GenerateCode(String inputFileName, String inputFileContent)
		{
			try
			{
				return this.GenerateCodeInternal(inputFileName, inputFileContent);
			} catch (FormatException e)
			{
				return Encoding.UTF8.GetBytes(e.Message);
			}
		}

		public override String GetDefaultExtension()
		{
			return GeneratedIdentifier + base.GetDefaultExtension();
		}

		internal/*For test*/ Byte[] GenerateCodeInternal(String inputFileName, String inputFileContent)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new IndentStreamWriter(stream, Encoding.UTF8))
				{
					using (var reader = new ContentReader(inputFileContent))
					{
						var headerProcessor = new HeaderProcessor
						{
							Namespace = this.FileNamespace,
							ClassName = inputFileName.Split('/', '\\').Last().Split('.').First(),
						};
						var headerOver = false;
						while (true)
						{
							var line = reader.ReadLine();
							if (line == null)
							{	// EOL
								break;
							}

							if (line.StartsWith("@") && !headerOver)
							{	// Processing headers.
								headerOver = !headerProcessor.Process(line);
								if (headerOver)
								{
									headerProcessor.WriteHeader(writer);
								}
							} else
							{	// End processing headers.
								if (!headerOver)
								{	// Header is just over last line.
									headerOver = true;
									headerProcessor.WriteHeader(writer);
								}
								using (var propertyProcessor = new PropertyProcessor(line))
								{
									propertyProcessor.Default = headerProcessor.PropertyDefaultSettings;
									propertyProcessor.Process();
									propertyProcessor.WriteProperty(writer);
								}
							}
						}	// End loop.
					}	// End ContentgReader.
					writer.Flush();
				}	// End IndentStreamWriter.
				stream.Flush();
				return stream.ToArray();
			}
		}

	}
}