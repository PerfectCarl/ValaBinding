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
				list = new ValaList (paths).ToTypedList (delegate(IntPtr item) {
					return Marshal.PtrToStringAuto (item);
				});

			return list;
		}

		public static string GetTypeDescription (SymbolType type)
		{
			/*switch (type) {
			case MemberType.NONE:
				return "None";
			case MemberType.VOID:
				return "Void";
			case MemberType.CONSTANT:
				return "Constant";
			case MemberType.ENUM:
				return "Enum";
			case MemberType.ENUM_VALUE:
				return "Enum Value";
			case MemberType.FIELD:
				return "Field";
			case MemberType.PROPERTY:
				return "Property";
			case MemberType.LOCAL_VARIABLE:
				return "Variable";
			case MemberType.SIGNAL:
				return "Signal";
			case MemberType.CREATION_METHOD:
				return "Creation Method";
			case MemberType.CONSTRUCTOR:
				return "Constructor";
			case MemberType.DESTRUCTOR:
				return "Destructor";
			case MemberType.METHOD:
				return "Method";
			case MemberType.DELEGATE:
				return "Delegate";
			case MemberType.PARAMETER:
				return "Parameter";
			case MemberType.ERROR_DOMAIN:
				return "Error Domain";
			case MemberType.ERROR_CODE:
				return "Error Code";
			case MemberType.NAMESPACE:
				return "Namespace";
			case MemberType.STRUCT:
				return "Struct";
			case MemberType.CLASS:
				return "Class";
			case MemberType.INTERFACE:
				return "Interface";
			case MemberType.SCOPED_CODE_NODE:
				return "Block";
			default:
				string des = type.to_string ().up ();
				warning ("Undefined description for symbol type: %s", des);
				return des;
			}*/
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

