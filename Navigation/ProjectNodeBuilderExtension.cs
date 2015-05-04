//
// ProjectNodeBuilderExtension.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
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
using System.IO;
using System.Collections.Generic;

using MonoDevelop.Projects;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;

using MonoDevelop.ValaBinding;
using MonoDevelop.ValaBinding.Parser;


namespace MonoDevelop.ValaBinding.Navigation
{
	public class ProjectNodeBuilderExtension : NodeBuilderExtension
	{
		public ClassPadEventHandler finishedBuildingTreeHandler;

		public override bool CanBuildNode (Type dataType)
		{
			return typeof(ValaProject).IsAssignableFrom (dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof(ProjectNodeBuilderExtensionHandler); }
		}

		protected override void Initialize ()
		{
			finishedBuildingTreeHandler = (ClassPadEventHandler)DispatchService.GuiDispatch (new ClassPadEventHandler (OnFinishedBuildingTree));
		}

		public override void Dispose ()
		{
		}

		public static void CreatePadTree (object o)
		{
			ValaProject project = o as ValaProject;
			if (o == null)
				return;
			ProjectInformation projectInfo = ProjectInformationManager.Instance.Get (project);
			
			try {
				foreach (ProjectFile f in project.Files) {
					if (f.BuildAction == BuildAction.Compile)
						projectInfo.AddFile (f.FilePath);
				}
				foreach (ProjectPackage package in project.Packages) {
					if (!package.IsProject) {
						projectInfo.AddPackage (project.Name);
					}
				}
			} catch (IOException) {
				return;
			}
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
		}
		// Used for ClassPad
		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{			
			ValaProject project = dataObject as ValaProject;
			if (project == null)
				return;
			
			// bool nestedNamespaces = builder.Options["NestedNamespaces"];
			
			ProjectInformation info = ProjectInformationManager.Instance.Get (project);
			//var added = new List<String> ();
			// Namespaces

			foreach (var child in info.GetRootSymbols ()) {
				builder.AddChild (child);
			}
			/*foreach (ProjectFile file in project.Files) {
				if (file.BuildAction == BuildAction.Compile)
					foreach (var child in info.GetRootSymbolsForFile (file.FilePath.FullPath)) {
						var name = child.Name;
						//if (added.IndexOf (name) == -1 /*&& child.Parent.Name == null*/ // ) { 
			// builder	: MonoDevelop.Ide.Gui.Components.ExtensibleTreeView.TreeBuilder
			// builder.options : 
			//			builder.AddChild (child);
			//	added.Add (name);
			//}
			/*		}
			}*/
		}

		/*
		 * public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{			
			ValaProject p = dataObject as ValaProject;
			if (p == null) return;
			
			// bool nestedNamespaces = builder.Options["NestedNamespaces"];
			
			ProjectInformation info = ProjectInformationManager.Instance.Get (p);
			var added = new List<String> () ;
			// Namespaces
			foreach (ProjectFile file in p.Files) {
				foreach (Symbol child in info.GetRootSymbolsForFile (file.FilePath.FullPath)) {
					var name = child.Name ;
					if( added.IndexOf (name) == -1 /*&& child.Parent.Name == null*/
		/*) { 
			builder.AddChild (child);
			added.Add(name) ;
		}
	}
}
}*/
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		private void OnFinishedBuildingTree (ClassPadEventArgs e)
		{
			ITreeBuilder builder = Context.GetTreeBuilder (e.Project);
			if (null != builder)
				builder.UpdateChildren ();
		}
	}

	public class ProjectNodeBuilderExtensionHandler : NodeCommandHandler
	{
		[CommandHandler (ValaProjectCommands.UpdateClassPad)]
		public void UpdateClassPad ()
		{
			ProjectNodeBuilderExtension.CreatePadTree (CurrentNode.DataItem);
		}
	}
}
