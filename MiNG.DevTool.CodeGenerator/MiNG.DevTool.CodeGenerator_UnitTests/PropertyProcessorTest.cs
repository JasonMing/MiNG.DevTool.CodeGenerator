using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiNG.DevTool.CodeGenerator.UnitTests
{
	[TestClass]
	public class PropertyProcessorTest
	{
		private String CallProcessLine(String line)
		{
			using (var stream = new MemoryStream())
			using (var writer = new IndentStreamWriter(stream, Encoding.UTF8))
			using (var propertyProcessor = new PropertyProcessor(line))
			{
				propertyProcessor.WriteProperty(writer);
				writer.Flush();
				stream.Flush();
				var processed = Encoding.UTF8.GetString(stream.ToArray());
				Console.WriteLine(processed);
				Console.WriteLine();
				return processed;
			}
		}

		[TestMethod]
		public void Test_ProcessLine_Simple()
		{
			this.CallProcessLine("Name");
		}

		[TestMethod]
		public void Test_ProcessLine_PropertyAccessibility()
		{
			this.CallProcessLine("protected Name");
			this.CallProcessLine("private Name");
			this.CallProcessLine("public Name");
			this.CallProcessLine("internal Name");
			this.CallProcessLine("internal protected Name");
			this.CallProcessLine("protected internal Name");
		}

		[TestMethod]
		public void Test_ProcessLine_Static()
		{
			this.CallProcessLine("static Name");
			this.CallProcessLine("protected static Name");
			this.CallProcessLine("private static Name");
			this.CallProcessLine("public Name");
			this.CallProcessLine("internal static Name");
			this.CallProcessLine("internal protected static Name");
			this.CallProcessLine("protected internal static Name");
		}

		[TestMethod]
		public void Test_ProcessLine_NameMapping()
		{
			this.CallProcessLine("Name:cc_name");
			this.CallProcessLine("internal Name:cc_name");
			this.CallProcessLine("internal protected static Name:cc_name");
		}

		[TestMethod]
		public void Test_ProcessLine_Accessor()
		{
			var a = this.CallProcessLine("Name { get; private set; }");
			var b = this.CallProcessLine("Name { private set; get; }");
			Assert.AreEqual(a, b);
			this.CallProcessLine("Name { get; }");
			this.CallProcessLine("Name { private get; }");
		}

		[TestMethod]
		public void Test_ProcessLine_Accessor_Format()
		{
			var openBrace_1 = this.CallProcessLine("Name{ get; }");
			var openBrace_2 = this.CallProcessLine("Name {get; }");
			var openBrace_3 = this.CallProcessLine("Name{get; }");
			Assert.IsTrue((openBrace_1 == openBrace_2)
				&& (openBrace_1 == openBrace_3));

			var semicolon_1 = this.CallProcessLine("Name { get; set; }");
			var semicolon_2 = this.CallProcessLine("Name { get ; set; }");
			var semicolon_3 = this.CallProcessLine("Name { get ;set; }");
			var semicolon_4 = this.CallProcessLine("Name { get;set; }");
			Assert.IsTrue((semicolon_1 == semicolon_2)
				&& (semicolon_1 == semicolon_3)
				&& (semicolon_1 == semicolon_4));
		}
	}
}
