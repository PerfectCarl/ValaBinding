//
// ProjectInformation.cs
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
//
// Copyright (C) 2008 Levi Bard
// Based on CBinding by Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.ValaBinding.Parser.Afrodite;

namespace MonoDevelop.ValaBinding.Parser
{
	/// <summary>
	/// Class to obtain parse information for a project
	/// </summary>
	public class ProjectInformation
	{
		private Afrodite.CompletionEngine engine;

		private Echo.Project echoProject;

		static readonly string[] topContainers = new string[]{ "namespace", "enum", "class", "struct", "interface" };

		public Project Project{ get; set; }

		public TextCompletion Completion { get; private set ; }

		public ProjectInformation (Project project)
		{
			this.Project = project;
			string projectName = (null == project)? "NoExistingProject": project.Name;

			echoProject = new Echo.Project ();
			if (CompletionEngine.DepsInstalled) {
				engine = new CompletionEngine (projectName);
			}
			Completion = new TextCompletion (this, engine); 
		}
		

		/// <summary>
		/// Adds a file to be parsed
		/// </summary>
		public void AddFile (string filename)
		{

			LoggingService.LogDebug ("Adding file {0}", filename);
			if( engine != null )
				engine.QueueSourcefile (filename, filename.EndsWith (".vapi", StringComparison.OrdinalIgnoreCase), false);
			if (echoProject != null)
				echoProject.AddFile (filename);
		}// AddFile

		/// <summary>
		/// Removes a file from the parse list
		/// </summary>
		public void RemoveFile (string filename)
		{
			// Not currently possible with Afrodite completion engine
		}// RemoveFile

		/// <summary>
		/// Adds a package to be parsed
		/// </summary>
		public void AddPackage (string packagename)
		{
			// if (!CompletionEngine.DepsInstalled){ return; }
			
			if ("glib-2.0".Equals (packagename, StringComparison.Ordinal)) {
				LoggingService.LogDebug ("AddPackage: Skipping {0} for afrodite", packagename);
				// Echo needs all the packages
				if (echoProject != null)
					echoProject.AddExternalPackage (packagename);
				return;
			} else {
				LoggingService.LogDebug ("AddPackage: Adding package {0}", packagename);
			}
			
			foreach (string path in Afrodite.Utils.GetPackagePaths (packagename)) {
				LoggingService.LogDebug ("AddPackage: Queueing {0} for package {1}", path, packagename);
				if( engine != null ) 
					engine.QueueSourcefile (path, true, false);
				if (echoProject != null)
					echoProject.AddExternalPackage (path);
			}
		}// AddPackage


		internal Afrodite.Symbol GetFunction (string name, string filename, int line, int column)
		{
			// if (!DepsInstalled){ return null; }
			if( engine != null )
			using (Afrodite.CodeDom parseTree = engine.TryAcquireCodeDom ()) {
				if (null != parseTree) {
					LoggingService.LogDebug ("GetFunction: Looking up symbol at {0}:{1}:{2}", filename, line, column);
					Afrodite.Symbol symbol = parseTree.GetSymbolForNameAndPath (name, filename, line, column);
					LoggingService.LogDebug ("GetFunction: Got {0}", (null == symbol)? "null": symbol.Name);
					return symbol;
				} else {
					LoggingService.LogDebug ("GetFunction: Unable to acquire codedom");
				}
			}

			return null;
		}

		/// <summary>
		/// Get the type of a given expression
		/// </summary>
		public string GetExpressionType (string symbol, string filename, int line, int column)
		{

			// if (!DepsInstalled){ return symbol; }
			if( engine != null )
				using (Afrodite.CodeDom parseTree = engine.TryAcquireCodeDom ()) {
					if (null != parseTree) {
						LoggingService.LogDebug ("GetExpressionType: Looking up symbol at {0}:{1}:{2}", filename, line, column);
						Afrodite.Symbol sym = parseTree.LookupSymbolAt (filename, line, column);
						if (null != sym) {
							LoggingService.LogDebug ("Got {0}", sym.SymbolType.TypeName);
							return sym.SymbolType.TypeName;
						}
					} else {
						LoggingService.LogDebug ("GetExpressionType: Unable to acquire codedom");
					}
				}

			return symbol;
		}// GetExpressionType
		
		/// <summary>
		/// Get overloads for a method
		/// </summary>
		internal List<Afrodite.Symbol> GetOverloads (string name, string filename, int line, int column)
		{
			List<Afrodite.Symbol> overloads = new List<Afrodite.Symbol> ();
			// if (!DepsInstalled){ return overloads; }
			if( engine != null )
				using (Afrodite.CodeDom parseTree = engine.TryAcquireCodeDom ()) {
					if (null != parseTree) {
						Afrodite.Symbol symbol = parseTree.GetSymbolForNameAndPath (name, filename, line, column);
						overloads = new List<Afrodite.Symbol> (){ symbol };
					} else {
						LoggingService.LogDebug ("GetOverloads: Unable to acquire codedom");
					}
				}
			
			return overloads;
		}// GetOverloads

		internal List<Afrodite.Symbol> GetRootSymbolsForFile (string file)
		{
			var symbols = GetSymbolsForFile (file /*topContainers*/);
			var result = new List<Symbol>();
			foreach (var symbol in symbols) {
				if (symbol.IsRoot)
					result.Add (symbol);
			}
			return result;
		}

		bool projectUpdated = false ; 

		internal List<Echo.Symbol> GetRootSymbolsForFileEcho (string file)
		{
			// HACK: be smarter, threaded thing
			if (!projectUpdated) {
				echoProject.UpdateSync ();
				projectUpdated = true; 
			}
			var symbols = echoProject.GetSymbolsForFile (file);
			var result = new List<Echo.Symbol>();
			foreach (var symbol in symbols) {
				result.Add (symbol);
			}
			return result;
		}

		/// <summary>
		/// Get a list of symbols declared in a given file
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>: The file to check
		/// </param>
		/// <param name="desiredTypes">
		/// A <see cref="IEnumerable<System.String>"/>: The types of symbols to allow. If null or missing all the symbols are allowed
		/// </param>
		internal List<Afrodite.Symbol> GetSymbolsForFile (string file, IEnumerable<string> desiredTypes = null )
		{
			List<Afrodite.Symbol> symbols = null;
			List<Afrodite.Symbol> classes = new List<Afrodite.Symbol> ();
			
			if (engine == null){ return classes; }

			using (Afrodite.CodeDom parseTree = engine.TryAcquireCodeDom ()) {
				if (null != parseTree){
					Afrodite.SourceFile sourceFile = parseTree.LookupSourceFile (file);
					if (null != sourceFile) {
						symbols = sourceFile.Symbols;
						if (null != symbols) {
							foreach (Afrodite.Symbol symbol in symbols) {
									if (desiredTypes == null )
									classes.Add (symbol);
								else {
									foreach (string containerType in desiredTypes) {
										if (containerType.Equals (symbol.MemberType, StringComparison.OrdinalIgnoreCase))
											classes.Add (symbol);
									}
								}
							}
						}
					} else {
						LoggingService.LogDebug ("GetClassesForFile: Unable to lookup source file {0}", file);
					}
				} else {
					LoggingService.LogDebug ("GetClassesForFile: Unable to acquire codedom");
				}
				
			}
			
			return classes;
		}
	}
}
