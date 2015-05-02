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

using MonoDevelop.Projects;
using MonoDevelop.Core;

// using MonoDevelop.ValaBinding.Parser.Afrodite;
using MonoDevelop.ValaBinding.Parser;
using MonoDevelop.Ide.Tasks;

namespace MonoDevelop.ValaBinding.Parser
{
	/// <summary>
	/// Class to obtain parse information for a project
	/// </summary>
	public class ProjectInformation
	{
		private Echo.Project echoProject;

		public Project Project{ get; set; }

		public TextCompletion Completion { get; private set ; }

		public ProjectInformation (Project project)
		{
			this.Project = project;
			echoProject = new Echo.Project (project.Name);
			Completion = new TextCompletion (this, echoProject); 
		}


		/// <summary>
		/// Adds a file to be parsed
		/// </summary>
		public void AddFile (string filename)
		{
			if (echoProject != null)
				echoProject.AddFile (filename);
		}
		// AddFile

		/// <summary>
		/// Removes a file from the parse list
		/// </summary>
		public void RemoveFile (string filename)
		{
			// Not currently possible with Afrodite completion engine
		}
		// RemoveFile

		/// <summary>
		/// Adds a package to be parsed
		/// </summary>
		public void AddPackage (string packagename)
		{
			// if (!CompletionEngine.DepsInstalled){ return; }
			
			/*if ("glib-2.0".Equals (packagename, StringComparison.Ordinal)) {
				LoggingService.LogDebug ("AddPackage: Skipping {0} for afrodite", packagename);
				// Echo needs all the packages
				if (echoProject != null)
					echoProject.AddExternalPackage (packagename);
				return;
			} else {
				LoggingService.LogDebug ("AddPackage: Adding package {0}", packagename);
			}*/

			foreach (string path in Echo.Utils.GetPackagePaths (packagename)) {
				LoggingService.LogDebug ("AddPackage: Queueing {0} for package {1}", path, packagename);
				if (echoProject != null)
					echoProject.AddExternalPackage (packagename);
			}
		}

		internal Echo.Symbol GetEnclosingSymbolAtPosition (string fileFullPath, int line, int column)
		{
			/*if( engine != null )
				using (Afrodite.CodeDom parseTree = engine.TryAcquireCodeDom ()) {
					if (null != parseTree) {
						LoggingService.LogDebug ("GetFunction: Looking up symbol at {0}:{1}:{2}", filename, line, column);
						Afrodite.Symbol symbol = parseTree.GetSymbolForNameAndPath (name, filename, line, column);
						LoggingService.LogDebug ("GetFunction: Got {0}", (null == symbol)? "null": symbol.Name);
						return symbol;
					} else {
						LoggingService.LogDebug ("GetFunction: Unable to acquire codedom");
					}
				}*/
			if (!projectUpdated) {
				echoProject.UpdateSync ();
				HandleParsingErrors (fileFullPath); 
				projectUpdated = true; 
			}

			var result = echoProject.GetEnclosingSymbolAtPosition (fileFullPath, line, column);
			//HandleParsingErrors (fileFullPath); 
			return result;

			//return null;
		}

		bool projectUpdated = false;
		const string OWNER = "Vala.Parsing";

		static public void ClearParsingErrors ()
		{

			//var enumerator = TaskService.Errors.
			//var tasks = TaskService.Errors.GetOwnerTasks (OWNER);
			var tasks = new List<Task> (); 
			foreach (var task in TaskService.Errors.GetOwnerTasks (OWNER)) {
				tasks.Add (task);
			}

			foreach (var task in tasks) {
				//for (int i = TaskService.Errors.Count - 1; i >= 0; i--) {
				//	var task = TaskService.Errors. [i];
				//if (task.FileName == file && task.Owner.ToString () == OWNER) {
				TaskService.Errors.Remove (task);
				//}
			}

		}

		void HandleParsingErrors (string file)
		{
			ClearParsingErrors ();
			foreach (Echo.ParsingError err in echoProject.GetParsingErrors()) {
				// LoggingService.LogDebug ("Parsing error " + err.ToString ());
				//if (err.FileFullPath == file) {
				var task = new Task (err.FileFullPath, err.Message, err.Column, err.Line, err.Severity, TaskPriority.Normal, Project, OWNER);
				TaskService.Errors.Add (task);
				//}
			}

		}

		internal List<Echo.Symbol> GetRootSymbolsForFile (string fileFullPath)
		{
			// HACK: be smarter, threaded thing
			if (!projectUpdated) {
				echoProject.UpdateSync ();
				HandleParsingErrors (fileFullPath); 
				projectUpdated = true; 
			}
			var symbols = echoProject.GetSymbolsForFile (fileFullPath);
			var result = new List<Echo.Symbol> ();
			// FIXME OVERDOING Really necessary 
			foreach (var symbol in symbols) {
				result.Add (symbol);
			}
			//HandleParsingErrors (fileFullPath);
			/*
			TaskService.Errors.Add (new Task (file.FilePath, err.ErrorText, err.Column, err.Line,
				err.IsWarning? TaskSeverity.Warning : TaskSeverity.Error,
				TaskPriority.Normal, file.Project.ParentSolution, file));
*/

			return result;
		}
	}
}
