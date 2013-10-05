using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiNG.DevTool.CodeGenerator
{
	class IndentStreamWriter : StreamWriter
	{
		private const Char IndentChar = '\t';

		private Int32 indent = 0;

		private LinkedList<String> CloseIdentifiers = new LinkedList<String>();

		private LinkedList<Boolean> CloseIndents = new LinkedList<Boolean>();

		public IndentStreamWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }

		public void WriteOpenPair(String left, String right, Boolean indent = true)
		{
			this.WriteIndent();
			base.WriteLine(left ?? String.Empty);
			if (indent)
			{
				this.indent += 1;
			}
			this.CloseIndents.AddLast(indent);
			this.CloseIdentifiers.AddLast(right ?? String.Empty);
		}

		public void WriteClosePair()
		{
			var indent = this.CloseIndents.Last.Value;
			this.CloseIndents.RemoveLast();
			var right = this.CloseIdentifiers.Last.Value;
			this.CloseIdentifiers.RemoveLast();

			if (indent && this.indent != 0)
			{
				this.indent -= 1;
			}
			this.WriteIndent();
			base.WriteLine(right ?? String.Empty);
		}

		public void WriteIndent(String format, params Object[] arg)
		{
			this.WriteIndent();
			base.Write(format, arg);
		}

		public void WriteLineIndent(String format, params Object[] arg)
		{
			this.WriteIndent();
			base.WriteLine(format, arg);
		}

		public void WriteOpenBrace()
		{
			this.WriteOpenPair("{", "}");
		}

		public void WriteCloseBrace()
		{
			this.WriteClosePair();
		}

		private void WriteIndent()
		{
			for (int i = 0; i < indent; i++)
			{
				base.Write(IndentChar);
			}
		}

		protected override void Dispose(Boolean disposing)
		{
			var length = this.CloseIndents.Count;
			for (Int32 i = 0; i < length; i++)
			{
				this.WriteClosePair();
			}
			base.Dispose(disposing);
		}

	}
}
