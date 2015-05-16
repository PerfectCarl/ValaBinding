//
// Author:
//       cran <>
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
//  Marcos David Marin Amador <MarcosMarin@gmail.com>
//

// TODO -g 
// library types
// Warnings
// extra parameters
// remove compiler in project file
// build included c files

using System;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Addins;

using MonoDevelop.Core;
 
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.ValaBinding.Parser;

namespace MonoDevelop.ValaBinding
{
	[Extension ("/ValaBinding/Compilers")]
	public class ValaCompiler : ICompiler
	{
		protected string compilerCommand;
		protected string gccCompilerCommand;

		bool compilerFound;
		bool appsChecked;
		List<string> gccArgs = new List<string> ();
		ValaProject project;
		ValaProjectConfiguration projectConfiguration;

		string gccLibs = "";
		string gccPkgConfig = "";

		public ValaCompiler ()
		{
			compilerCommand = "valac";
			gccCompilerCommand = "gcc";
		}

		public string Name {
			get{ return "valac"; }
		}

		public string CompilerCommand {
			get { return compilerCommand; }
		}

		/// <summary>
		/// Compile the project
		/// </summary>
		public BuildResult Compile (ValaProject project,
		                            ConfigurationSelector solutionConfiguration,
		                            IProgressMonitor monitor)
		{
			var projectFiles = project.Files;
			var packages = project.Packages;
			this.projectConfiguration = (ValaProjectConfiguration)project.GetConfiguration (solutionConfiguration);
			this.project = project; 
			if (!appsChecked) {
				// Check for compiler
				appsChecked = true;
				compilerFound = CheckApp (compilerCommand);
			}

			if (!compilerFound) {
				// No compiler!
				BuildResult cres = new BuildResult ();
				cres.AddError ("Compiler not found: " + compilerCommand);
				return cres;
			}

			// Build compiler params string
			string compilerArgs = GetCompilerFlags (projectConfiguration) + " " +
			                      GeneratePkgCompilerArgs (packages, solutionConfiguration);
			
			// Build executable name
			string outputName = Path.Combine (
				                    FileService.RelativeToAbsolutePath (projectConfiguration.SourceDirectory, projectConfiguration.OutputDirectory),
				                    projectConfiguration.CompiledOutputName);
			
			monitor.BeginTask (GettextCatalog.GetString ("Compiling source"), 1);

			CompilerResults cr = new CompilerResults (new TempFileCollection ());
			bool success = DoCompilation (projectFiles, compilerArgs, outputName, monitor, cr);

			GenerateDepfile (projectConfiguration, packages);

			if (success) {
				monitor.Step (1);
			}
			monitor.EndTask ();

			return new BuildResult (cr, "");
		}

		string ICompiler.GetCompilerFlags (ValaProjectConfiguration configuration)
		{
			return GetCompilerFlags (configuration);
		}

		/// <summary>
		/// Generates compiler args for the current settings
		/// </summary>
		/// <param name="configuration">
		/// Project configuration
		/// <see cref="ValaProjectConfiguration"/>
		/// </param>
		/// <returns>
		/// A compiler-interpretable string
		/// <see cref="System.String"/>
		/// </returns>
		public string GetCompilerFlags (ValaProjectConfiguration configuration)
		{
			List<string> args = new List<string> ();
			gccArgs.Clear ();

			ValaCompilationParameters cp =
				(ValaCompilationParameters)configuration.CompilationParameters;

			var outputDir = FileService.RelativeToAbsolutePath (configuration.SourceDirectory,
				                configuration.OutputDirectory);
			var outputNameWithoutExt = Path.GetFileNameWithoutExtension (configuration.Output);

			// args.Add(string.Format("-d \"{0}\"", outputDir));

			if (configuration.DebugMode)
				args.Add ("-g");

			switch (configuration.CompileTarget) {
			case ValaBinding.CompileTarget.Bin:
				if (cp.EnableMultithreading) {
					args.Add ("--thread");
				}
				break;
			case ValaBinding.CompileTarget.SharedLibrary:
				if (Platform.IsWindows) {
					args.Add (string.Format ("--Xcc=\"-shared\" --Xcc=-I\"{0}\" --header \"{1}.h\" --vapi \"{1}.vapi\" --library \"{1}\"", outputDir, outputDir + "/" + outputNameWithoutExt));
					gccArgs.Add (string.Format ("-shared\" -I\"{0}\" ", outputDir));
				} else {
					args.Add (string.Format ("--Xcc=\"-shared\" --Xcc=\"-fPIC\" --Xcc=\"-I'{0}'\" --header \"{1}.h\" --vapi \"{1}.vapi\" --library \"{1}\"", outputDir, outputDir + "/" + outputNameWithoutExt));    
					gccArgs.Add (string.Format ("\"-shared\" \"-fPIC\" \"-I'{0}'\"", outputDir));
				}
				break;
			}

			// Valac will get these sooner or later			
			//			switch (cp.WarningLevel)
			//			{
			//			case WarningLevel.None:
			//				args.Append ("-w ");
			//				break;
			//			case WarningLevel.Normal:
			//				// nothing
			//				break;
			//			case WarningLevel.All:
			//				args.Append ("-Wall ");
			//				break;
			//			}
			//			
			//			if (cp.WarningsAsErrors)
			//				args.Append ("-Werror ");
			//			
			if (0 < cp.OptimizationLevel) {
				args.Add ("--Xcc=\"-O" + cp.OptimizationLevel + "\"");
				gccArgs.Add ("\"-O" + cp.OptimizationLevel + "\"");

			}

			// global extra compiler arguments
			string globalExtraCompilerArgs = PropertyService.Get ("ValaBinding.ExtraCompilerOptions", "");
			if (!string.IsNullOrEmpty (globalExtraCompilerArgs)) {
				args.Add (globalExtraCompilerArgs.Replace (Environment.NewLine, " "));
			}

			// extra compiler arguments specific to project
			if (cp.ExtraCompilerArguments != null && cp.ExtraCompilerArguments.Length > 0) {
				args.Add (cp.ExtraCompilerArguments.Replace (Environment.NewLine, " "));
			}

			if (cp.DefineSymbols != null && cp.DefineSymbols.Length > 0) {
				args.Add (ProcessDefineSymbols (cp.DefineSymbols));
			}

			if (configuration.Includes != null) {
				foreach (string inc in configuration.Includes) {
					var includeDir = FileService.RelativeToAbsolutePath (configuration.SourceDirectory, inc);
					args.Add ("--vapidir \"" + includeDir + "\"");
				}
			}

			if (configuration.Libs != null) {
				foreach (string lib in configuration.Libs) {
					args.Add ("--pkg \"" + lib + "\"");
					gccPkgConfig += " " + lib;
				}
			}
			return string.Join (" ", args.ToArray ());
		}

		/// <summary>
		/// Generates compiler args for depended packages
		/// </summary>
		/// <param name="packages">
		/// The collection of packages for this project 
		/// <see cref="ProjectPackageCollection"/>
		/// </param>
		/// <returns>
		/// The string needed by the compiler to reference the necessary packages
		/// <see cref="System.String"/>
		/// </returns>
		public string GeneratePkgCompilerArgs (ProjectPackageCollection packages,
		                                       ConfigurationSelector solutionConfiguration)
		{
			gccLibs = ""; 
			if (packages == null || packages.Count < 1)
				return string.Empty;

			StringBuilder libs = new StringBuilder ();
			StringBuilder gccProjectlibs = new StringBuilder ();
			StringBuilder gccPackagelibs = new StringBuilder ();

			foreach (ProjectPackage p in packages) {
				if (p.IsProject) {
					var proj = p.GetProject ();
					var projectConfiguration = (ValaProjectConfiguration)proj.GetConfiguration (solutionConfiguration);
					var outputDir = FileService.RelativeToAbsolutePath (projectConfiguration.SourceDirectory,
						                projectConfiguration.OutputDirectory);
					var outputNameWithoutExt = Path.GetFileNameWithoutExtension (projectConfiguration.Output);
					var vapifile = Path.Combine (outputDir, outputNameWithoutExt + ".vapi");
					libs.AppendFormat (" --Xcc=-I\"{0}\" --Xcc=-L\"{0}\" --Xcc=-l\"{1}\" \"{2}\" ",
						outputDir, outputNameWithoutExt, vapifile);
					gccProjectlibs.AppendFormat (" -I\"{0}\" -L\"{0}\" -l\"{1}\" \"{2}\" ",
						outputDir, outputNameWithoutExt, vapifile);
				} else {
					libs.AppendFormat (" --pkg \"{0}\" ", p.Name);
					gccPackagelibs.Append (" " + p.Name);
				}
			}
			gccLibs = gccProjectlibs.ToString ();
			gccPkgConfig = gccPackagelibs.ToString ();
            
			return libs.ToString ();
		}

		/// <summary>
		/// Generates compilers flags for selected defines
		/// </summary>
		/// <param name="configuration">
		/// Project configuration
		/// <see cref="ValaProjectConfiguration"/>
		/// </param>
		/// <returns>
		/// A compiler-interpretable string
		/// <see cref="System.String"/>
		/// </returns>
		public string GetDefineFlags (ValaProjectConfiguration configuration)
		{
			string defines = ((ValaCompilationParameters)configuration.CompilationParameters).DefineSymbols;
			return ProcessDefineSymbols (defines);
		}

		/// <summary>
		/// Determines whether a file needs to be compiled
		/// </summary>
		/// <param name="file">
		/// The file in question
		/// <see cref="ProjectFile"/>
		/// </param>
		/// <returns>
		/// true if the file needs to be compiled
		/// <see cref="System.Boolean"/>
		/// </returns>
		private bool NeedsCompiling (ProjectFile file)
		{
			return true;
		}

		/// <summary>
		/// Executes a build command
		/// </summary>
		/// <param name="command">
		/// The executable to be launched
		/// <see cref="System.String"/>
		/// </param>
		/// <param name="args">
		/// The arguments to command
		/// <see cref="System.String"/>
		/// </param>
		/// <param name="baseDirectory">
		/// The directory in which the command will be executed
		/// <see cref="System.String"/>
		/// </param>
		/// <param name="monitor">
		/// The progress monitor to be used
		/// <see cref="IProgressMonitor"/>
		/// </param>
		/// <param name="errorOutput">
		/// Error output will be stored here
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// The exit code of the command
		/// <see cref="System.Int32"/>
		/// </returns>
		int ExecuteCommand (string command, string args, string baseDirectory, IProgressMonitor monitor, out string errorOutput)
		{
			errorOutput = string.Empty;
			int exitCode = -1;
			
			StringWriter swError = new StringWriter ();
			LogTextWriter chainedError = new LogTextWriter ();
			chainedError.ChainWriter (monitor.Log);
			chainedError.ChainWriter (swError);
			
			monitor.Log.WriteLine ("{0} {1}", command, args);
			
			AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor (monitor);
			
			try {
				ProcessWrapper p = Runtime.ProcessService.StartProcess (command, args, baseDirectory, monitor.Log, chainedError, null);
				operationMonitor.AddOperation (p); //handles cancellation
				
				p.WaitForOutput ();
				errorOutput = swError.ToString ();
				exitCode = p.ExitCode;
				p.Dispose ();
				// Log error in the output
				if (exitCode != 0) {
					var errMsg = "";
					if (string.IsNullOrEmpty (errorOutput))
						errMsg = string.Format ("Return code {0}. No error message provided", exitCode);
					else
						errMsg = errorOutput;
					monitor.Log.WriteLine (errMsg);
				}
				if (monitor.IsCancelRequested) {
					monitor.Log.WriteLine (GettextCatalog.GetString ("Build cancelled"));
					monitor.ReportError (GettextCatalog.GetString ("Build cancelled"), null);
					if (exitCode == 0)
						exitCode = -1;
				}
			} finally {
				chainedError.Close ();
				swError.Close ();
				operationMonitor.Dispose ();
			}
			
			return exitCode;
		}

		int ExecuteCommandForOutput (string command, string args, string baseDirectory, IProgressMonitor monitor, out string output, out string errorOutput)
		{
			errorOutput = string.Empty;
			int exitCode = -1;

			StringWriter swError = new StringWriter ();
			StringWriter swOuput = new StringWriter ();
			LogTextWriter chainedError = new LogTextWriter ();
			chainedError.ChainWriter (monitor.Log);
			chainedError.ChainWriter (swError);

			monitor.Log.WriteLine ("{0} {1}", command, args);

			AggregatedOperationMonitor operationMonitor = new AggregatedOperationMonitor (monitor);

			try {
				// See http://stackoverflow.com/q/29848761/740464
				const string paths = "/usr/lib/pkgconfig:/usr/share/pkgconfig:/usr/lib/x86_64-linux-gnu/pkgconfig:/usr/local/pkgconfig";
				//Runtime.ProcessService.EnvironmentVariableOverrides.Add ("PKG_CONFIG_DEBUG_SPEW", "true");
				Runtime.ProcessService.EnvironmentVariableOverrides ["PKG_CONFIG_PATH"] = paths;
				ProcessWrapper p = Runtime.ProcessService.StartProcess (command, args, baseDirectory, swOuput, chainedError, null);

				operationMonitor.AddOperation (p); //handles cancellation

				p.WaitForOutput ();
				errorOutput = swError.ToString ();
				output = swOuput.ToString ();
				monitor.Log.WriteLine (output);
				exitCode = p.ExitCode;
				p.Dispose ();
				// Log error in the output
				if (exitCode != 0) {
					var errMsg = "";
					if (string.IsNullOrEmpty (errorOutput))
						errMsg = string.Format ("Return code {0}. No error message provided", exitCode);
					else
						errMsg = errorOutput;
					monitor.Log.WriteLine (errMsg);
				}
				if (monitor.IsCancelRequested) {
					monitor.Log.WriteLine (GettextCatalog.GetString ("Build cancelled"));
					monitor.ReportError (GettextCatalog.GetString ("Build cancelled"), null);
					if (exitCode == 0)
						exitCode = -1;
				}
			} finally {
				chainedError.Close ();
				swError.Close ();
				operationMonitor.Dispose ();
			}

			return exitCode;
		}

		/// <summary>
		/// Transforms a whitespace-delimited string of 
		/// symbols into a set of compiler flags
		/// </summary>
		/// <param name="symbols">
		/// A whitespace-delimited string of symbols, 
		/// e.g., "DEBUG MONODEVELOP"
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private static string ProcessDefineSymbols (string symbols)
		{
			return (string.IsNullOrEmpty (symbols)) ?
				string.Empty :
				"-D " + Regex.Replace (symbols, " +", " -D ");
		}

		/// <summary>
		/// Compiles the project
		/// </summary>
		private bool DoCompilation (ProjectFileCollection projectFiles, string args,
		                            string outputName,
		                            IProgressMonitor monitor,
		                            CompilerResults cr)
		{
			Directory.CreateDirectory (projectConfiguration.IntermediateOutputDirectory);
			//var buildDate = new DateTime ();
			var result = GenerateCFiles (projectFiles, args, monitor, cr);
			if (result)
				// Only if files get generated properly
				CompileCFiles (projectFiles, outputName, monitor, cr);
			return result;
		}

		private string ProcessPkgConfig (
			IProgressMonitor monitor,
			CompilerResults cr)
		{
			var config = gccPkgConfig.Trim (); 

			if (config == "")
				return ""; 

			var args = "--cflags --libs " + gccPkgConfig; 
			string errorOutput = string.Empty;
			string output = string.Empty;

			int exitCode = ExecuteCommandForOutput ("pkg-config", args, project.BaseIntermediateOutputPath, monitor, out output, out errorOutput);

			if (exitCode != 0 && cr.Errors.Count == 0) {
				// error isn't recognized but cannot ignore it because exitCode != 0
				var errMsg = "";
				if (string.IsNullOrEmpty (errorOutput))
					errMsg = string.Format ("Return code {0}. No error message provided", exitCode);
				else
					errMsg = errorOutput;
				cr.Errors.Add (new CompilerError () { ErrorText = errMsg });
			}

			return output;
		}

		// Handle:
		//  - application
		//  - libraries
		//  - project depending on other projects
		private bool CompileCFiles (ProjectFileCollection projectFiles, string outputName,
		                            IProgressMonitor monitor,
		                            CompilerResults cr)
		{
			StringBuilder filelist = new StringBuilder ();

			string pkgargs = null;

			var globalExitCode = 0; 
			var compiledFileCount = 0; 

			foreach (ProjectFile f in projectFiles) { 
				if (f.Subtype != Subtype.Directory && f.BuildAction == BuildAction.Compile) {
					var outputDir = projectConfiguration.IntermediateOutputDirectory;
					//var filePath = f.FilePath.ChangeExtension ("c");
					var prefixPath = f.FilePath.ToString ().Substring (project.BaseDirectory.ToString ().Length);
					prefixPath = prefixPath.Substring (0, prefixPath.Length - f.FilePath.FileName.Length - 1);
					var filename = f.FilePath.FileNameWithoutExtension + ".c";
					var cFilePath = outputDir + prefixPath + "/" + filename;
					var oFilePath = outputDir + prefixPath + "/" + f.FilePath.FileNameWithoutExtension + ".o";
					filelist.AppendFormat ("\"{0}\" ", oFilePath);
					if (File.GetCreationTime (cFilePath) <= File.GetCreationTime (oFilePath) && File.Exists (oFilePath)) { 
						//monitor.Log.WriteLine ("Skipping {0}", filename );
					} else {
						// filelist.AppendFormat ("\"{0}\" ", listFilePath);

						// *
						// * Build only the recently modified files
						// * 
						// .o files are generated in obj/

						if (pkgargs == null) {
							pkgargs = ProcessPkgConfig (monitor, cr);
							monitor.Log.WriteLine ("Build .c files");
							monitor.Log.WriteLine ("---");
						}
						monitor.Log.WriteLine ("Building {0}", "." + prefixPath + "/" + filename);
						string args = string.Join (" ", gccArgs.ToArray ());
						string compilerArgs = string.Format ("{0} -c {1} {2} {3} ", /*-o \"{4}\"",*/
							                      args, gccLibs, cFilePath, pkgargs);

						string errorOutput = string.Empty;
						int exitCode = ExecuteCommand (gccCompilerCommand, compilerArgs, outputDir + "/" + prefixPath, monitor, out errorOutput);
						globalExitCode += exitCode;
						compiledFileCount++;
						ParseCompilerOutput (errorOutput, cr, projectFiles);

						if (exitCode != 0 && cr.Errors.Count == 0) {
							// error isn't recognized but cannot ignore it because exitCode != 0
							var errMsg = "";
							if (string.IsNullOrEmpty (errorOutput))
								errMsg = string.Format ("Return code {0}. No error message provided", exitCode);
							else
								errMsg = errorOutput;
							cr.Errors.Add (new CompilerError () { ErrorText = errMsg });
						}
					}
				}
			}

			// *
			// * Build only the recently modified files
			// * 
			// .o files are generated in obj/
			if (globalExitCode == 0 && File.Exists (outputName)) {
				if (compiledFileCount == 0) {
					monitor.Log.WriteLine ("Everything is up to date");
				} else {
					monitor.Log.WriteLine ("Linking .o files");
					monitor.Log.WriteLine ("---");
					if (pkgargs == null)
						pkgargs = ProcessPkgConfig (monitor, cr);
					var linkArgs = string.Join (" ", gccArgs.ToArray ());
					string linkingArgs = string.Format ("{0} {1} {2} {3} -o \"{4}\"",
						                     linkArgs, gccLibs, filelist.ToString (), pkgargs, Path.GetFileName (outputName));

					string linkErrorOutput = string.Empty;
					int linkExitCode = ExecuteCommand (gccCompilerCommand, linkingArgs, Path.GetDirectoryName (outputName), 
						                   monitor, out linkErrorOutput);

					ParseCompilerOutput (linkErrorOutput, cr, projectFiles);

					if (linkExitCode != 0 && cr.Errors.Count == 0) {
						// error isn't recognized but cannot ignore it because exitCode != 0
						var errMsg = "";
						if (string.IsNullOrEmpty (linkErrorOutput))
							errMsg = string.Format ("Return code {0}. No error message provided", linkExitCode);
						else
							errMsg = linkErrorOutput;
						cr.Errors.Add (new CompilerError () { ErrorText = errMsg });
					}
					globalExitCode += linkExitCode;
				}
			}
			var compilationSuccess = globalExitCode == 0;

			// * 
			// *  Link all the .o files if needed
			// * 

			return compilationSuccess;
		}

		private bool GenerateCFiles (ProjectFileCollection projectFiles, string args,
		                             IProgressMonitor monitor,
		                             CompilerResults cr)
		{
			monitor.Log.WriteLine ("Compiling vala files into .c");
			monitor.Log.WriteLine ("---");
			StringBuilder filelist = new StringBuilder ();
			foreach (ProjectFile f in projectFiles) { 
				if (f.Subtype != Subtype.Directory && f.BuildAction == BuildAction.Compile) {
					filelist.AppendFormat ("\"{0}\" ", f.FilePath);
				}
			}

			// Just generate the c code.
			string compiler_args = string.Format ("{0} {1} --ccode --basedir {2} --directory {3} ",
				                       args, filelist.ToString (), project.BaseDirectory, projectConfiguration.IntermediateOutputDirectory);
			
			string errorOutput = string.Empty;
			int exitCode = ExecuteCommand (compilerCommand, compiler_args, projectConfiguration.IntermediateOutputDirectory, monitor, out errorOutput);
			
			ParseCompilerOutput (errorOutput, cr, projectFiles);

			if (exitCode != 0 && cr.Errors.Count == 0) {
				// error isn't recognized but cannot ignore it because exitCode != 0
				var errMsg = "";
				if (string.IsNullOrEmpty (errorOutput))
					errMsg = string.Format ("Return code {0}. No error message provided", exitCode);
				else
					errMsg = errorOutput;
				cr.Errors.Add (new CompilerError () { ErrorText = errMsg });
			}

			return exitCode == 0;
		}

		/// <summary>
		/// Cleans up intermediate files
		/// </summary>
		/// <param name="projectFiles">
		/// The project's files
		/// <see cref="ProjectFileCollection"/>
		/// </param>
		/// <param name="configuration">
		/// Project configuration
		/// <see cref="ValaProjectConfiguration"/>
		/// </param>
		/// <param name="monitor">
		/// The progress monitor to be used
		/// <see cref="IProgressMonitor"/>
		/// </param>
		public void Clean (ProjectFileCollection projectFiles, ValaProjectConfiguration configuration, IProgressMonitor monitor)
		{
			// Clean up intermediate files
			// These should only be generated for libraries, but we'll check for them in all cases
			foreach (ProjectFile file in projectFiles) {
				if (file.BuildAction == BuildAction.Compile) {
					string cFile = Path.Combine (
						               FileService.RelativeToAbsolutePath (configuration.SourceDirectory, configuration.OutputDirectory),
						               Path.GetFileNameWithoutExtension (file.Name) + ".c");
					if (File.Exists (cFile)) {
						File.Delete (cFile);
					}

					string hFile = Path.Combine (
						               FileService.RelativeToAbsolutePath (configuration.SourceDirectory, configuration.OutputDirectory),
						               Path.GetFileNameWithoutExtension (file.Name) + ".h");
					if (File.Exists (hFile)) {
						File.Delete (hFile);
					}
				}
			}

			string vapiFile = Path.Combine (
				                  FileService.RelativeToAbsolutePath (configuration.SourceDirectory, configuration.OutputDirectory),
				                  configuration.Output + ".vapi");
			if (File.Exists (vapiFile)) {
				File.Delete (vapiFile);
			}

			var output = configuration.IntermediateOutputDirectory;
			if (Directory.Exists (output))
				Directory.Delete (output, true);

			ProjectInformation.ClearParsingErrors ();
		}

		/// <summary>
		/// Parses a compiler output string into CompilerResults
		/// </summary>
		/// <param name="errorString">
		/// The string output by the compiler
		/// <see cref="System.String"/>
		/// </param>
		/// <param name="cr">
		/// The CompilerResults into which to parse errorString
		/// <see cref="CompilerResults"/>
		/// </param>
		protected void ParseCompilerOutput (string errorString, CompilerResults cr, ProjectFileCollection projectFiles)
		{
			TextReader reader = new StringReader (errorString);
			string next;

			while ((next = reader.ReadLine ()) != null) {
				CompilerError error = CreateErrorFromErrorString (next, projectFiles);
				// System.Console.WriteLine ("Creating error from string \"{0}\"", next);
				if (error != null) {
					cr.Errors.Insert (0, error);
					// System.Console.WriteLine ("Adding error");
				}
			}

			reader.Close ();
		}

		/// Error regex for valac
		/// Sample output: "/home/user/project/src/blah.vala:23.5-23.5: error: syntax error, unexpected }, expecting ;"
		private static Regex errorRegex = new Regex (
			                                  @"^\s*(?<file>.*):(?<line>\d*)\.(?<column>\d*)-\d*\.\d*: (?<level>[^:]*): (?<message>.*)",
			                                  RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		private static Regex gccRegex = new Regex (
			                                @"^\s*(?<file>.*\.c):(?<line>\d*):((?<column>\d*):)?\s*(?<level>[^:]*):\s(?<message>.*)",
			                                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		/// Error regex for gnu linker - this could still be pertinent for vala
		private static Regex linkerRegex = new Regex (
			                                   @"^\s*(?<file>[^:]*):(?<line>\d*):\s*(?<message>[^:]*)",
			                                   RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		/// <summary>
		/// Creates a compiler error from an output string
		/// </summary>
		/// <param name="errorString">
		/// The error string to be parsed
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A newly created CompilerError
		/// <see cref="CompilerError"/>
		/// </returns>
		private CompilerError CreateErrorFromErrorString (string errorString, ProjectFileCollection projectFiles)
		{
			Match errorMatch = null;
			foreach (Regex regex in new Regex[]{errorRegex, gccRegex})
				if ((errorMatch = regex.Match (errorString)).Success)
					break;

			if (!errorMatch.Success) {
				return null;
			}

			CompilerError error = new CompilerError ();
			foreach (ProjectFile pf in projectFiles) {
				if (Path.GetFileName (pf.Name) == errorMatch.Groups ["file"].Value) {
					error.FileName = pf.FilePath;
					break;
				}
			}// check for fully pathed file
			if (string.Empty == error.FileName) {
				error.FileName = errorMatch.Groups ["file"].Value;
			}// fallback to exact match
			error.Line = int.Parse (errorMatch.Groups ["line"].Value);
			if (errorMatch.Groups ["column"].Success)
				error.Column = int.Parse (errorMatch.Groups ["column"].Value);
			error.IsWarning = !errorMatch.Groups ["level"].Value.Equals (GettextCatalog.GetString ("error"), StringComparison.Ordinal) &&
			!errorMatch.Groups ["level"].Value.StartsWith ("fatal error");
			error.ErrorText = errorMatch.Groups ["message"].Value;
			
			return error;
		}

		/// <summary>
		/// Parses linker output into compiler results
		/// </summary>
		/// <param name="errorString">
		/// The linker output to be parsed
		/// <see cref="System.String"/>
		/// </param>
		/// <param name="cr">
		/// Results will be stored here
		/// <see cref="CompilerResults"/>
		/// </param>
		protected void ParseLinkerOutput (string errorString, CompilerResults cr)
		{
			TextReader reader = new StringReader (errorString);
			string next;
			
			while ((next = reader.ReadLine ()) != null) {
				CompilerError error = CreateLinkerErrorFromErrorString (next);
				if (error != null)
					cr.Errors.Add (error);
			}
			
			reader.Close ();
		}

		/// <summary>
		/// Creates a linker error from an output string
		/// </summary>
		/// <param name="errorString">
		/// The error string to be parsed
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A newly created LinkerError
		/// <see cref="LinkerError"/>
		/// </returns>
		private CompilerError CreateLinkerErrorFromErrorString (string errorString)
		{
			CompilerError error = new CompilerError ();
			
			Match linkerMatch = linkerRegex.Match (errorString);
			
			if (linkerMatch.Success) {
				error.FileName = linkerMatch.Groups ["file"].Value;
				error.Line = int.Parse (linkerMatch.Groups ["line"].Value);
				error.ErrorText = linkerMatch.Groups ["message"].Value;
				
				return error;
			}
			
			return null;
		}

		/// <summary>
		/// Check to see if we have a given app
		/// </summary>
		/// <param name="app">
		/// The app to check
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// true if the app is found
		/// <see cref="System.Boolean"/>
		/// </returns>
		bool CheckApp (string app)
		{
			try {
				ProcessWrapper p = Runtime.ProcessService.StartProcess (app, "--version", null, null);
				p.WaitForOutput ();
				return true;
			} catch {
				return false;
			}
		}

		public void GenerateDepfile (ValaProjectConfiguration configuration, ProjectPackageCollection packages)
		{
			try {
				if (configuration.CompileTarget != CompileTarget.SharedLibrary) {
					return;
				}

				using (StreamWriter writer = new StreamWriter (Path.Combine (
					                             FileService.RelativeToAbsolutePath (configuration.SourceDirectory, configuration.OutputDirectory),
					                             Path.ChangeExtension (configuration.Output, ".deps")))) {
					foreach (ProjectPackage package in packages) {
						writer.WriteLine (package.Name);
					}
				}
			} catch { /* Don't care */
			}
		}
	}
}
