//
// LanguageItemNodeBuilder.cs
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
//
// Copyright (C) 2009 Levi Bard
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
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.ValaBinding.Parser.Echo;

namespace MonoDevelop.ValaBinding.Navigation
{
	/// <summary>
	/// Class pad node builder for all Vala language items
	/// </summary>
	public class LanguageItemNodeBuilder: TypeNodeBuilder
	{
		//// <value>
		/// Sort order for nodes
		/// </value>
		private static string[] types = {
			"namespace",
			"class",
			"struct",
			"interface",
			"property",
			"method",
			"signal",
			"field",
			"constant",
			"enum",
			"other"
		};

		public override Type NodeDataType {
			get { return typeof(Symbol); }
		}

		public override Type CommandHandlerType {
			get { return typeof(LanguageItemCommandHandler); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return ((Symbol)dataObject).Name;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			Symbol symbol = (Symbol)dataObject;
			var displayText = symbol.DisplayText;
			displayText = GLib.Markup.EscapeText (displayText);
			nodeInfo.Label = displayText;
			nodeInfo.Icon = Context.GetIcon (symbol.Icon);
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			// bool publicOnly = treeBuilder.Options["PublicApiOnly"];
			Symbol thisSymbol = (Symbol)dataObject;

			foreach (Symbol child in thisSymbol.Children) {
				// We don't display code blocks like if/then/else in the ClassPad
				if (child.MemberType != "Block" && child.MemberType != "Creation Method")
					treeBuilder.AddChild (child);
			}
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			Symbol symbol = (Symbol)dataObject;
			if (symbol.Children == null)
				return false; 
			if (symbol.Children.Count == 0)
				return false; 
			// We check that the children are not simple code blocks that we 
			// don't want to display in the ClassPad
			foreach (var child in symbol.Children) {
				if (child.MemberType != "Block" && child.MemberType != "Creation Method")
					return true;
			}
			return false;
		}

		public override int CompareObjects (ITreeNavigator thisNode, ITreeNavigator otherNode)
		{
			if (null != thisNode && null != otherNode) {
				Symbol thisCN = thisNode.DataItem as Symbol,
				otherCN = otherNode.DataItem as Symbol;
	
				if (null != thisCN && null != otherCN) {
					return Array.IndexOf<string> (types, thisCN.MemberType) -
					Array.IndexOf<string> (types, otherCN.MemberType);
				}
			}

			return -1;
		}
	}
}
