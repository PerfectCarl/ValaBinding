//
// Utils.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	public class Utils
	{

		public static List<string> GetPackagePaths (string package)
		{
			List<string> list = new List<string> ();
			IntPtr paths = echo_utils_get_package_paths (package, IntPtr.Zero, null);
			if (IntPtr.Zero != paths)
				list = new GeeList (paths).ToTypedList (delegate(IntPtr item) {
					return Marshal.PtrToStringAuto (item);
				});

			return list;
		}

		public static string GetTypeDescription (SymbolType type)
		{
			switch (type) {
			case SymbolType.FILE:
				return "File";
			case SymbolType.NAMESPACE:
				return "Namespace";
			case SymbolType.CLASS:
				return "Class";
			case SymbolType.CONSTRUCTOR:
				return "Constructor";
			case SymbolType.DESTRUCTOR:
				return "Destructor";
			case SymbolType.ENUM:
				return "Enum";
			case SymbolType.INTERFACE:
				return "Interface";
			case SymbolType.METHOD:
				return "Method";
			case SymbolType.STRUCT:
				return "Struct";
			case SymbolType.PROPERTY:
				return "Property";
			case SymbolType.FIELD:
				return "Field";
			case SymbolType.SIGNAL:
				return "Signal";
			default:
				return type.ToString ();
			}
		}

		[DllImport ("libecho")]
		static extern IntPtr echo_utils_get_package_paths (string package, IntPtr codeContext, string[] vapiDirs);

	}

}

