using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiNG.DevTool.CodeGenerator
{
	class ContentReader : IDisposable
	{
		private const String DocumentPrefix = "///";

		private readonly StringReader reader;

		private String nextLine;

		public ContentReader(String content)
		{
			this.reader = new StringReader(content);
		}

		public String ReadDocument()
		{
			var sb = new StringBuilder();
			while (true)
			{
				var line = this.PeekLine().TrimStart();
				if (line.StartsWith(DocumentPrefix))
				{
					line = line.Remove(0, 3);
					sb.AppendLine(line);
				} else
				{
					break;
				}
			}
			return sb.ToString();
		}

		public String ReadLine()
		{
			this.SkipWhiteLine();

			if (this.nextLine != null)
			{
				var next = this.nextLine;
				this.nextLine = null;
				return next;
			}

			return this.reader.ReadLine();
		}

		public String PeekLine()
		{
			if (this.nextLine == null)
			{
				this.nextLine = this.ReadLine();
			}
			return this.nextLine;
		}

		private void SkipWhiteLine()
		{
			if (!String.IsNullOrWhiteSpace(this.nextLine))
			{
				return;
			}
			while (true)
			{
				var line = this.reader.ReadLine();
				if (line == null || !String.IsNullOrWhiteSpace(line))
				{
					this.nextLine = line;
					return;
				}
			}
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

	}
}
