// Guids.cs
// MUST match guids.h
using System;

namespace MiNG.DevTool.CodeGenerator
{
	static class GuidList
	{
		public const string guidMiNG_DevTool_CodeGeneratorPkgString = "a3d9bd45-ab54-4946-af71-c31afa4d669a";
		public const string guidMiNG_DevTool_CodeGeneratorCmdSetString = "42fa81fc-1c97-4973-8e57-0f0198fdaf1b";

		internal const string DelegatingPocoGenerator = "9cd55b43-ab30-4283-9b0c-8b461004e22e";

		public static readonly Guid guidMiNG_DevTool_CodeGeneratorCmdSet = new Guid(guidMiNG_DevTool_CodeGeneratorCmdSetString);
	};
}