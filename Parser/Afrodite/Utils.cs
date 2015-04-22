//
// Utils.cs
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
	/// Wrapper class for Afrodite.Utils namespace
	/// </summary>
	internal static class Utils
	{
		/// <summary>
		/// Get a list of vapi files for a given package
		/// </summary>
		public static List<string> GetPackagePaths (string package)
		{
			List<string> list = new List<string> ();
			IntPtr paths = afrodite_utils_get_package_paths (package, IntPtr.Zero, null);
			if (IntPtr.Zero != paths)
				list = new ValaList (paths).ToTypedList (delegate(IntPtr item){ return Marshal.PtrToStringAuto (item); });

			return list;
		}

		public static string GetMemberType (int memberType)
		{
			return Marshal.PtrToStringAuto (afrodite_utils_symbols_get_symbol_type_description (memberType));
		}

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_utils_get_package_paths (string package, IntPtr codeContext, string[] vapiDirs);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_utils_symbols_get_symbol_type_description (int memberType);
	}
}

