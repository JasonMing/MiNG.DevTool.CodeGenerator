using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiNG.DevTool.CodeGenerator
{
	class LineReader : IDisposable
	{
		private const Int32 EOF = -1;

		//private static readonly Char[] DELIMITER = { '{', '}', ';' };

		private readonly StringReader reader;

		private readonly Char[] delimiters;

		private String nextWord;

		public LineReader(String line, params Char[] delimiters)
		{
			this.reader = new StringReader(line);
			this.delimiters = delimiters ?? new Char[0];
		}

		public String PeekWord()
		{
			if (this.nextWord == null)
			{
				this.nextWord = this.ReadWord();
			}
			return this.nextWord;
		}

		public String ReadWord()
		{
			if (this.nextWord != null)
			{
				var next = this.nextWord;
				this.nextWord = null;
				return next;
			}

			var sb = new StringBuilder();
			this.SkipWhite();
			while (true)
			{
				var i = this.reader.Peek();
				if (i == EOF)
				{
					break;
				}
				var c = (Char)i;
				if (Char.IsWhiteSpace(c))
				{
					//this.reader.Read();
					break;
				} else if (this.delimiters.Contains(c))
				{
					if (sb.Length == 0)
					{	// If the first char, assume it as the word.
						this.reader.Read();
						return c.ToString();
					} else
					{
						break;
					}
				} else
				{
					this.reader.Read();
					sb.Append(c);
				}
			}

			return sb.Length == 0 ? null : sb.ToString();
		}

		public String ReadToEnd()
		{
			this.SkipWhite();
			return this.reader.ReadToEnd();
		}

		private void SkipWhite()
		{
			while (true)
			{
				var c = this.reader.Peek();
				if (c == EOF || !Char.IsWhiteSpace((Char)c))
				{
					return;
				} else
				{
					this.reader.Read();
				}
			}
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

	}
}
