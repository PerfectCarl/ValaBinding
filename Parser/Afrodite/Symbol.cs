﻿//
// Symbol.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	// From afrodite.vapi
	public enum SymbolAccessibility
	{
		Private = 0x1,
		Internal = 0x2,
		Protected = 0x4,
		Public = 0x8,
		Any = 0x10
	}

	/// <summary>
	/// Represents a Vala symbol
	/// </summary>
	internal class Symbol
	{
		public Symbol (IntPtr instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Children of this symbol
		/// </summary>
		public List<Symbol> Children {
			get {
				List<Symbol> list = new List<Symbol> ();
				IntPtr children = afrodite_symbol_get_children (instance);

				if (IntPtr.Zero != children) {
					list = new ValaList (children).ToTypedList (item => new Symbol (item));
				}

				return list;
			}
		}

		/// <summary>
		/// The type of this symbol
		/// </summary>
		public DataType SymbolType {
			get { 
				IntPtr datatype = afrodite_symbol_get_symbol_type (instance);
				return (IntPtr.Zero == datatype) ? null : new DataType (afrodite_symbol_get_symbol_type (instance));
			}
		}

		/// <summary>
		/// The return type of this symbol, if applicable
		/// </summary>
		public DataType ReturnType {
			get { 
				IntPtr datatype = afrodite_symbol_get_return_type (instance);
				return (IntPtr.Zero == datatype) ? null : new DataType (afrodite_symbol_get_return_type (instance));
			}
		}

		/// <summary>
		/// The name of this symbol
		/// </summary>
		public string Name {
			get{ return Marshal.PtrToStringAuto (afrodite_symbol_get_display_name (instance)); }
		}

		public bool IsRoot { 
			get { 
				if (Parent == null)
					return true;
				return Parent.Name == null; 
			}
		}

		/// <summary>
		/// The fully qualified name of this symbol
		/// </summary>
		public string FullyQualifiedName {
			get { return Marshal.PtrToStringAuto (afrodite_symbol_get_fully_qualified_name (instance)); }
		}

		/// <summary>
		/// The parent of this symbol
		/// </summary>
		public Symbol Parent {
			get {
				IntPtr parent = afrodite_symbol_get_parent (instance);
				return (IntPtr.Zero == parent) ? null : new Symbol (parent);
			}
		}

		/// <summary>
		/// The places where this symbol is declared/defined
		/// </summary>
		public List<SourceReference> SourceReferences {
			get {
				List<SourceReference> list = new List<SourceReference> ();
				IntPtr refs = afrodite_symbol_get_source_references (instance);

				if (IntPtr.Zero != refs) {
					list = new ValaList (refs).ToTypedList (item => new SourceReference (item));
				}

				return list;
			}
		}

		/// <summary>
		/// The symbol type (class, method, ...) of this symbol
		/// </summary>
		public string MemberType {
			get{ return Utils.GetMemberType (afrodite_symbol_get_member_type (instance)); }
		}

		/// <summary>
		/// The accessibility (public, private, ...) of this symbol
		/// </summary>
		public SymbolAccessibility Accessibility {
			get{ return (SymbolAccessibility)afrodite_symbol_get_access (instance); }
		}

		/// <summary>
		/// The parameters this symbol accepts, if applicable
		/// </summary>
		public List<DataType> Parameters {
			get {
				List<DataType> list = new List<DataType> ();
				IntPtr parameters = afrodite_symbol_get_parameters (instance);

				if (IntPtr.Zero != parameters) {
					list = new ValaList (parameters).ToTypedList (delegate (IntPtr item) {
						return new DataType (item);
					});
				}

				return list;
			}
		}

		/// <summary>
		/// The icon to be used for this symbol
		/// </summary>
		public string Icon {
			get{ return GetIconForType (MemberType, Accessibility); }
		}


		public string GetParameterDisplayText (bool includeNames)
		{
			StringBuilder text = new StringBuilder ();
			List<DataType> parameters = Parameters;
			if (0 < parameters.Count) {
				if (includeNames)
					text.AppendFormat ("({0} {1}", parameters [0].TypeName, Parameters [0].Name);
				else
					text.AppendFormat ("({0}", parameters [0].TypeName);
				for (int i = 1; i < parameters.Count; i++) {
					if (includeNames)
						text.AppendFormat (", {0} {1}", parameters [i].TypeName, Parameters [i].Name);
					else
						text.AppendFormat (", {0}", parameters [i].TypeName);
				}
				text.AppendFormat (")");
			}
			if (null != ReturnType && !string.IsNullOrEmpty (ReturnType.TypeName)) {
				text.AppendFormat (": {0}", ReturnType.TypeName);
			}
			return text.ToString ();
		}

		/// <summary>
		/// Descriptive text for this symbol
		/// </summary>
		public string DisplayText {
			get {
				StringBuilder text = new StringBuilder (Name);
				List<DataType> parameters = Parameters;
				if (0 < parameters.Count) {
					text.AppendFormat ("({0} {1}", parameters [0].TypeName, Parameters [0].Name);
					for (int i = 1; i < parameters.Count; i++) {
						text.AppendFormat (", {0} {1}", parameters [i].TypeName, Parameters [i].Name);
					}
					text.AppendFormat (")");
				}
				if (null != ReturnType && !string.IsNullOrEmpty (ReturnType.TypeName)) {
					text.AppendFormat (": {0}", ReturnType.TypeName);
				}

				return text.ToString ();
			}
		}

		#region Icons

		private static Dictionary<string,string> publicIcons = new Dictionary<string, string> () {
			{ "namespace", Stock.NameSpace },
			{ "class", Stock.Class },
			{ "struct", Stock.Struct },
			{ "enum", Stock.Enum },
			{ "error domain", Stock.Enum },
			{ "field", Stock.Field },
			{ "method", Stock.Method },
			{ "constructor", Stock.Method },
			{ "creationmethod", Stock.Method },
			{ "property", Stock.Property },
			{ "constant", Stock.Literal },
			{ "enum value", Stock.Literal },
			{ "error code", Stock.Literal },
			{ "signal", Stock.Event },
			{ "delegate", Stock.Delegate },
			{ "interface", Stock.Interface },
			{ "other", Stock.Delegate }
		};

		private static Dictionary<string,string> privateIcons = new Dictionary<string, string> () {
			{ "namespace", Stock.NameSpace },
			{ "class", Stock.PrivateClass },
			{ "struct", Stock.PrivateStruct },
			{ "enum", Stock.PrivateEnum },
			{ "error domain", Stock.PrivateEnum },
			{ "field", Stock.PrivateField },
			{ "method", Stock.PrivateMethod },
			{ "constructor", Stock.PrivateMethod },
			{ "creationmethod", Stock.PrivateMethod },
			{ "property", Stock.PrivateProperty },
			{ "constant", Stock.Literal },
			{ "enum value", Stock.Literal },
			{ "error code", Stock.Literal },
			{ "signal", Stock.PrivateEvent },
			{ "delegate", Stock.PrivateDelegate },
			{ "interface", Stock.PrivateInterface },
			{ "other", Stock.PrivateDelegate }
		};

		private static Dictionary<string,string> protectedIcons = new Dictionary<string, string> () {
			{ "namespace", Stock.NameSpace },
			{ "class", Stock.ProtectedClass },
			{ "struct", Stock.ProtectedStruct },
			{ "enum", Stock.ProtectedEnum },
			{ "error domain", Stock.ProtectedEnum },
			{ "field", Stock.ProtectedField },
			{ "method", Stock.ProtectedMethod },
			{ "constructor", Stock.ProtectedMethod },
			{ "creationmethod", Stock.ProtectedMethod },
			{ "property", Stock.ProtectedProperty },
			{ "constant", Stock.Literal },
			{ "enum value", Stock.Literal },
			{ "error code", Stock.Literal },
			{ "signal", Stock.ProtectedEvent },
			{ "delegate", Stock.ProtectedDelegate },
			{ "interface", Stock.ProtectedInterface },
			{ "other", Stock.ProtectedDelegate }
		};

		private static Dictionary<SymbolAccessibility,Dictionary<string,string>> iconTable = new Dictionary<SymbolAccessibility, Dictionary<string, string>> () {
			{ SymbolAccessibility.Public, publicIcons },
			{ SymbolAccessibility.Internal, publicIcons },
			{ SymbolAccessibility.Private, privateIcons },
			{ SymbolAccessibility.Protected, protectedIcons }
		};

		public static string GetIconForType (string nodeType, SymbolAccessibility visibility)
		{
			string icon = null;
			iconTable [visibility].TryGetValue (nodeType.ToLower (), out icon);
			return icon;
		}

		#endregion

		#region P/Invokes

		IntPtr instance;

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_type_name (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_display_name (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_children (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_parent (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_fully_qualified_name (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_source_references (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern int afrodite_symbol_get_access (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_parameters (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern int afrodite_symbol_get_member_type (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_symbol_type (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_symbol_get_return_type (IntPtr instance);

		#endregion
	}

}

