//
// CodeDomDumper.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Utility class for dumping a CodeDOM to Console.Out
	/// </summary>
	internal class CodeDomDumper
	{
		public CodeDomDumper ()
		{
			instance = afrodite_ast_dumper_new ();
		}

		public void Dump (CodeDom codeDom, string filterSymbol)
		{
			afrodite_ast_dumper_dump (instance, codeDom.Instance, filterSymbol);
		}

		#region P/Invokes

		IntPtr instance;

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_ast_dumper_new ();

		[DllImport("libafrodite")]
		static extern void afrodite_ast_dumper_dump (IntPtr instance, IntPtr codeDom, string filterSymbol);

		#endregion
	}
}

