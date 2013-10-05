using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiNG.DevTool.CodeGenerator
{
	class BraceWriter : IDisposable
	{
		private readonly IndentStreamWriter writer;

		public BraceWriter(IndentStreamWriter writer)
		{
			this.writer = writer;
			this.writer.WriteOpenBrace();
		}

		public void Dispose()
		{
			this.writer.WriteCloseBrace();
		}

	}
}
