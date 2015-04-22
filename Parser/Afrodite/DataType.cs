//
// DataType.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Represents an Afrodite symbol data type
	/// </summary>
	internal class DataType
	{
		public DataType (IntPtr instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Get the raw name of this datatype
		/// </summary>
		public string Name {
			get{ return Marshal.PtrToStringAuto (afrodite_data_type_get_name (instance)); }
		}

		/// <summary>
		/// Get the descriptive type name (ref Gee.List<string>[]?) for this datatype
		/// </summary>
		public string TypeName {
			get {
				StringBuilder text = new StringBuilder ();

				// prefix out/ref
				if (IsOut) {
					text.Append ("out ");
				} else if (IsRef) {
					text.Append ("ref ");
				}

				text.Append (Marshal.PtrToStringAuto (afrodite_data_type_get_type_name (instance)));

				if (IsGeneric) {
					text.Append ("<");
					List<DataType> parameters = GenericTypes;
					if (parameters != null && parameters.Count > 0) {
						text.Append (parameters[0].TypeName);
						for (int i = 0; i < parameters.Count; i++) {
							text.AppendFormat (",{0}", parameters[i].TypeName);
						}
					}
					text.Append (">");
				}

				if (IsArray) { text.Append ("[]"); }
				if (IsNullable){ text.Append ("?"); }
				if (IsPointer){ text.Append ("*"); }

				return text.ToString ();
			}
		}

		/// <summary>
		/// Get the symbol for this datatype
		/// </summary>
		public Symbol Symbol {
			get {
				IntPtr symbol = afrodite_data_type_get_symbol (instance);
				return (IntPtr.Zero == symbol)? null: new Symbol (symbol);
			}
		}

		/// <summary>
		/// Whether this datatype is an array
		/// </summary>
		public bool IsArray {
			get{ return afrodite_data_type_get_is_array (instance); }
		}

		/// <summary>
		/// Whether this datatype is a pointer
		/// </summary>
		public bool IsPointer {
			get{ return afrodite_data_type_get_is_pointer (instance); }
		}

		/// <summary>
		/// Whether this datatype is nullable
		/// </summary>
		public bool IsNullable {
			get{ return afrodite_data_type_get_is_nullable (instance); }
		}

		/// <summary>
		/// Whether this is an out datatype
		/// </summary>
		public bool IsOut {
			get{ return afrodite_data_type_get_is_out (instance); }
		}

		/// <summary>
		/// Whether this is a ref datatype
		/// </summary>
		public bool IsRef {
			get{ return afrodite_data_type_get_is_ref (instance); }
		}

		/// <summary>
		/// Whether this datatype is generic
		/// </summary>
		public bool IsGeneric {
			get{ return afrodite_data_type_get_is_generic (instance); }
		}

		/// <summary>
		/// Type list for generic datatypes (e.g. HashMap<KeyType,ValueType>)
		/// </summary>
		public List<DataType> GenericTypes {
			get {
				List<DataType> list = new List<DataType> ();
				IntPtr types = afrodite_data_type_get_generic_types (instance);

				if (IntPtr.Zero != types) {
					list = new ValaList (types).ToTypedList (item => new DataType (item));
				}

				return list;
			}
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_data_type_get_type_name (IntPtr instance);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_data_type_get_name (IntPtr instance);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_data_type_get_symbol (IntPtr instance);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_data_type_get_generic_types (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_array (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_pointer (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_nullable (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_out (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_ref (IntPtr instance);

		[DllImport("libafrodite")]
		static extern bool afrodite_data_type_get_is_generic (IntPtr instance);


		#endregion
	}
}

