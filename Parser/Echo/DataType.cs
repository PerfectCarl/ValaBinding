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

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	/// <summary>
	/// Represents an Echo symbol data type
	/// </summary>
	public class DataType
	{
		public DataType (IntPtr instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Get the raw name of this datatype
		/// </summary>
		public string Name {
			get{ return Marshal.PtrToStringAuto (echo_data_type_get_name (instance)); }
		}

		/// <summary>
		/// Get the descriptive type name (ref Gee.List<string>[]?) for this datatype
		/// </summary>
		public string TypeName {
			get {
				StringBuilder text = new StringBuilder ();
				// Not wanted anymore
				// prefix out/ref
				/*if (IsOut) {
					text.Append ("out ");
				} else if (IsRef) {
					text.Append ("ref ");
				}*/

				text.Append (Marshal.PtrToStringAuto (echo_data_type_get_type_name (instance)));

				/*if (IsGeneric) {
					text.Append ("<");
					List<DataType> parameters = GenericTypes;
					if (parameters != null && parameters.Count > 0) {
						text.Append (parameters [0].TypeName);
						for (int i = 0; i < parameters.Count; i++) {
							text.AppendFormat (",{0}", parameters [i].TypeName);
						}
					}
					text.Append (">");
				}*/
				// Not needed as echo does the trick
				/*if (IsArray) {
					text.Append ("[]");
				}
				if (IsNullable) {
					text.Append ("?");
				}
				if (IsPointer) {
					text.Append ("*");
				}*/

				return text.ToString ();
			}
		}

		/// <summary>
		/// Get the symbol for this datatype
		/// </summary>
		public Symbol Symbol {
			get {
				IntPtr symbol = echo_data_type_get_symbol (instance);
				return (IntPtr.Zero == symbol) ? null : new Symbol (symbol);
			}
		}

		/// <summary>
		/// Whether this datatype is an array
		/// </summary>
		public bool IsArray {
			get{ return echo_data_type_get_is_array (instance); }
		}

		/// <summary>
		/// Whether this datatype is a pointer
		/// </summary>
		public bool IsPointer {
			get{ return echo_data_type_get_is_pointer (instance); }
		}

		/// <summary>
		/// Whether this datatype is nullable
		/// </summary>
		public bool IsNullable {
			get{ return echo_data_type_get_is_nullable (instance); }
		}

		/// <summary>
		/// Whether this is an out datatype
		/// </summary>
		public bool IsOut {
			get{ return echo_data_type_get_is_out (instance); }
		}

		/// <summary>
		/// Whether this is a ref datatype
		/// </summary>
		public bool IsRef {
			get{ return echo_data_type_get_is_ref (instance); }
		}

		/// <summary>
		/// Whether this datatype is generic
		/// </summary>
		public bool IsGeneric {
			get{ return echo_data_type_get_is_generic (instance); }
		}

		/// <summary>
		/// Type list for generic datatypes (e.g. HashMap<KeyType,ValueType>)
		/// </summary>
		public List<DataType> GenericTypes {
			get {
				List<DataType> list = new List<DataType> ();
				IntPtr types = echo_data_type_get_generic_types (instance);

				if (IntPtr.Zero != types) {
					list = new GeeList (types).ToTypedList (item => new DataType (item));
				}

				return list;
			}
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport ("libecho")]
		static extern IntPtr echo_data_type_get_type_name (IntPtr instance);

		[DllImport ("libecho")]
		static extern IntPtr echo_data_type_get_name (IntPtr instance);

		[DllImport ("libecho")]
		static extern IntPtr echo_data_type_get_symbol (IntPtr instance);

		[DllImport ("libecho")]
		static extern IntPtr echo_data_type_get_generic_types (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_array (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_pointer (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_nullable (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_out (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_ref (IntPtr instance);

		[DllImport ("libecho")]
		static extern bool echo_data_type_get_is_generic (IntPtr instance);


		#endregion
	}
}

