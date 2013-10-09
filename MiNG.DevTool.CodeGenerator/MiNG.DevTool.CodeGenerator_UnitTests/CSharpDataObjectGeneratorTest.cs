using MiNG.DevTool.CodeGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.IO;

namespace MiNG.DevTool.CodeGenerator.UnitTests
{


	/// <summary>
	///这是 CSharpDataObjectGeneratorTest 的测试类，旨在
	///包含所有 CSharpDataObjectGeneratorTest 单元测试
	///</summary>
	[TestClass()]
	public class CSharpDataObjectGeneratorTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///获取或设置测试上下文，上下文提供
		///有关当前测试运行及其功能的信息。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 附加测试特性
		// 
		//编写测试时，还可使用以下特性:
		//
		//使用 ClassInitialize 在运行类中的第一个测试前先运行代码
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//使用 ClassCleanup 在运行完类中的所有测试后再运行代码
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//使用 TestInitialize 在运行每个测试前先运行代码
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//使用 TestCleanup 在运行完每个测试后运行代码
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		private String CallGenerateCodeInternal(String templateFileName)
		{
			var generator = new CSharpDataObjectGenerator();
			var file = new FileInfo(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\TestTemplates\\" + templateFileName);
			using (var fs = file.OpenRead())
			using (var reader = new StreamReader(fs))
			{
				var generated = generator.GenerateCodeInternal(file.FullName, reader.ReadToEnd());
				using (var ms = new MemoryStream(generated))
				using (var genReader = new StreamReader(ms))
				{
					return genReader.ReadToEnd();
				}
			}
		}


		[TestMethod()]
		public void Test_GenerateCodeInternal_PropertyOnly()
		{
			var code = this.CallGenerateCodeInternal("PropertyOnly.poco");
			this.TestContext.WriteLine("{0}", code);
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_PropertyOnly_WithPropertytAccessibility()
		{
			var code = this.CallGenerateCodeInternal("PropertyOnly_WithPropertytAccessibility.poco");
			this.TestContext.WriteLine("{0}", code);
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_PropertyOnly_WithNameMapping()
		{
			var code = this.CallGenerateCodeInternal("PropertyOnly_WithNameMapping.poco");
			this.TestContext.WriteLine("{0}", code);
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_PropertyOnly_WithAccessor()
		{
			var code = this.CallGenerateCodeInternal("PropertyOnly_WithAccessor.poco");
			this.TestContext.WriteLine("{0}", code);
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_Namespace()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_Namespace.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("namespace Test"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_Base()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_Base.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("class WithHeader_Base : PocoBase"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_Base_WithPlaceHolder()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_Base_WithPlaceHolder.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("class WithHeader_Base_WithPlaceHolder : PocoBase<WithHeader_Base_WithPlaceHolder>"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_BaseImpl_WithPlaceHolder()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_BaseImpl_WithPlaceHolder.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("class WithHeader_BaseImpl_WithPlaceHolder : PocoBase<WithHeader_BaseImpl_WithPlaceHolder>, ISerializable, ISignable<WithHeader_BaseImpl_WithPlaceHolder>"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_Impl_WithPlaceHolder()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_Impl_WithPlaceHolder.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("class WithHeader_Impl_WithPlaceHolder : ISerializable, ISignable<WithHeader_Impl_WithPlaceHolder>"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_DefaultAccess()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_DefaultAccess.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("public String Name"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_DefaultType()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_DefaultType.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("public Int32 Name"));
		}

		[TestMethod()]
		public void Test_GenerateCodeInternal_WithHeader_Using()
		{
			var code = this.CallGenerateCodeInternal("WithHeader_Using.poco");
			this.TestContext.WriteLine("{0}", code);
			Assert.IsTrue(code.Contains("using System;"));
			Assert.IsTrue(code.Contains("using System.Linq;"));
		}

	}
}
