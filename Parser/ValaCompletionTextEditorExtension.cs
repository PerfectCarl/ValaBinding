//
// ValaTextEditorExtension.cs
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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeCompletion;

using MonoDevelop.Components;

using MonoDevelop.ValaBinding.Parser;
using Mono.TextEditor;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.ValaBinding.Parser.Echo;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace MonoDevelop.ValaBinding
{
	public class ValaCompletionTextEditorExtension : CompletionTextEditorExtension, IPathedDocument
	{
		// Allowed chars to be next to an identifier
		private static char[] allowedChars = new char[] { ' ', '\t', '\r', '\n', 
			':', '=', '*', '+', '-', '/', '%', ',', '&',
			'|', '^', '{', '}', '[', ']', '(', ')', '\n', '!', '?', '<', '>'
		};

		private static char[] operators = new char[] {
			'=', '+', '-', ',', '&', '|',
			'^', '[', '!', '?', '<', '>', ':'
		};

		private ProjectInformation ProjectInfo {
			get {
				ValaProject project = Document.Project as ValaProject;
				return (null == project) ? null : ProjectInformationManager.Instance.Get (project);
			}
		}

		private TextCompletion Completion {
			get {
				return ProjectInfo.Completion; 
			}
		}

		protected Mono.TextEditor.TextEditorData textEditorData { get; set; }

		public override void Initialize ()
		{
			base.Initialize ();
			textEditorData = Document.Editor;
			UpdatePath (null, null);
			textEditorData.Caret.PositionChanged += UpdatePath;
			Document.DocumentParsed += delegate {
				UpdatePath (null, null);
			};
		}

		public override bool KeyPress (Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			string lineText = Editor.GetLineText (Editor.Caret.Line);

			// smart formatting strategy
			if (Document.Editor.Options.IndentStyle == IndentStyle.Smart ||
			    Document.Editor.Options.IndentStyle == IndentStyle.Virtual) {
				if (key == Gdk.Key.Return) {
					if (lineText.TrimEnd ().EndsWith ("{")) {
						Editor.InsertAtCaret ("\n" + Document.Editor.Options.IndentationString + Editor.Document.GetLineIndent (Editor.Caret.Line));
						return false;
					}
				}
				// TODO: The '}' is invisible before next key press
				/*else if (keyChar == '}' && AllWhiteSpace(lineText)
                  && lineText.StartsWith(Document.Editor.Options.IndentationString))
                {
                    if (lineText.Length > 0)
                        lineText = lineText.Substring(Document.Editor.Options.IndentationString.Length);
                    var lineSegment = Editor.Document.GetLine(Editor.Caret.Line);
                    Editor.Replace(lineSegment.Offset, lineSegment.Length, lineText + "}");
                    return false;
                }*/
			}

			return base.KeyPress (key, keyChar, modifier);
		}

		/// <summary>
		/// Expression to match instance construction/initialization
		/// </summary>
		private static Regex initializationRegex = new Regex (@"(((?<typename>\w[\w\d\.<>]*)\s+)?(?<variable>\w[\w\d]*)\s*=\s*)?new\s*(?<constructor>\w[\w\d\.<>]*)?", RegexOptions.Compiled);

		public override ICompletionDataList CodeCompletionCommand (
			CodeCompletionContext completionContext)
		{
			if (null == (Document.Project as ValaProject)) {
				return null;
			}

			/*int pos = completionContext.TriggerOffset;
			int triggerWordLength = completionContext.TriggerWordLength;

			ICompletionDataList list = HandleCodeCompletion (completionContext, Editor.GetCharAt (pos), ref triggerWordLength);
			if (null == list) {
				list = GlobalComplete (completionContext);
			}
			return list; */
		
			int i = 0;
			return HandleCodeCompletion (completionContext, '\0', ref i);

		}

		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			//ProjectInformation parser = ProjectInfo;
			var loc = Editor.Document.OffsetToLocation (completionContext.TriggerOffset);
			int line = loc.Line, column = loc.Column;
			string originalLine = Editor.GetLineText (line); 
			if (column > originalLine.Length) {
				column = originalLine.Length;
			}
			// lineText = lineText.Substring (0, column - 1);
			
			var result = new ValaCompletionDataList ();

			//ThreadPool.QueueUserWorkItem (delegate {
			//}); 
			//return result; 
			//var character = originalLine.Substring (column - 2, 1);
			if (completionChar == '\0')
				completionChar = document.Editor.GetCharAt (completionContext.TriggerOffset - 1);
			switch (completionChar) {
			case '.': // foo.[complete]
				Completion.Complete (result, Document.FileName, 
					originalLine, completionChar, line, column - 1);
				//return result;
				break;
			case '\t':
			case ' ':
				var lineText = originalLine;
				if (0 == lineText.Length) {
					return null;
				}
				if (column > lineText.Length) {
					column = lineText.Length;
				}
				lineText = lineText.Substring (0, column - 1).Trim ();

				if (lineText.EndsWith ("new")) {
					Completion.Complete (result, Document.FileName, originalLine, completionChar, line, column);
					//return result;    
					//return CompleteConstructor(lineText, line, column);
				} else if (lineText.EndsWith ("is")) {
					/*ValaCompletionDataList list = new ValaCompletionDataList ();
					ThreadPool.QueueUserWorkItem (delegate {
						Completion.GetTypesVisibleFrom (Document.FileName, line, column, list);
					});*/
					Completion.Complete (result, Document.FileName, originalLine, completionChar, line, column);
					//return result; 
				} /*else if (0 < lineText.Length) {
					char lastNonWS = lineText [lineText.Length - 1];
					if (0 <= Array.IndexOf (operators, lastNonWS) ||
					    (1 == lineText.Length && 0 > Array.IndexOf (allowedChars, lastNonWS))) {
						return GlobalComplete (completionContext);
					}
				}*/

				break;
			/*default:
				if (0 <= Array.IndexOf (operators, completionChar)) {
					return GlobalComplete (completionContext);
				}
				break;*/
			}

			return result.Count != 0 ? result : null;
		}

		/*static string GetTrailingSymbol (string text)
		{
			// remove the trailing '.'
			if (text.EndsWith (".", StringComparison.Ordinal))
				text = text.Substring (0, text.Length - 1);

			int nameStart = text.LastIndexOfAny (allowedChars);
			return text.Substring (nameStart + 1).Trim ();
		}*/

		/// <summary>
		/// Perform constructor-specific completion
		/// </summary>
		/*private ValaCompletionDataList CompleteConstructor (string lineText, int line, int column)
		{
			//ProjectInformation parser = ProjectInfo;
			Match match = initializationRegex.Match (lineText);
			var list = new ValaCompletionDataList ();

			ThreadPool.QueueUserWorkItem (delegate {
				if (match.Success) {
					// variable initialization
					if (match.Groups ["typename"].Success || "var" != match.Groups ["typename"].Value) {
						// simultaneous declaration and initialization
						Completion.GetConstructorsForType (match.Groups ["typename"].Value, Document.FileName, line, column, list);
					} else if (match.Groups ["variable"].Success) {
						// initialization of previously declared variable
						Completion.GetConstructorsForExpression (match.Groups ["variable"].Value, Document.FileName, line, column, list);
					}
					if (0 == list.Count) {
						// Fallback to known types
						Completion.GetTypesVisibleFrom (Document.FileName, line, column, list);
					}
				}
			});

			return list;
		}*/
		// CompleteConstructor

		/// <summary>
		/// Get the members of a symbol
		/// </summary>
		/*private ValaCompletionDataList GetMembersOfItem (string itemFullName, int line, int column)
		{
			//ProjectInformation info = ProjectInfo;
			if (null == ProjectInfo) {
				return null;
			}

			ValaCompletionDataList list = new ValaCompletionDataList ();
			ThreadPool.QueueUserWorkItem (delegate {
				ProjectInfo.Completion.Complete (itemFullName, Document.FileName, line, column, list);
			});
			return list;
		}
*/
		/// <summary>
		/// Complete all symbols visible from a given location
		/// </summary>
		/*private ValaCompletionDataList GlobalComplete (CodeCompletionContext context)
		{
			//ProjectInformation info = ProjectInfo;
			if (null == ProjectInfo) {
				return null;
			}

			ValaCompletionDataList list = new ValaCompletionDataList ();
			var loc = Editor.Document.OffsetToLocation (context.TriggerOffset);
			/*ThreadPool.QueueUserWorkItem (delegate {
				ProjectInfo.Completion.GetSymbolsVisibleFrom (Document.FileName, loc.Line + 1, loc.Column + 1, list);
			});*/

		/* return list;
		} */

		public override MonoDevelop.Ide.CodeCompletion.ParameterDataProvider HandleParameterCompletion (
			CodeCompletionContext completionContext, char completionChar)
		{
			if (completionChar != '(')
				return null;

			//ProjectInformation info = ProjectInfo;
			if (null == ProjectInfo) {
				return null;
			}

			int position = Editor.Document.GetLine (Editor.Caret.Line).Offset;
			string lineText = Editor.GetTextBetween (position, Editor.Caret.Offset - 1).TrimEnd ();
			string functionName = string.Empty;

			Match match = initializationRegex.Match (lineText);
			if (match.Success && match.Groups ["constructor"].Success) {
				string[] tokens = match.Groups ["constructor"].Value.Split ('.');
				string overload = tokens [tokens.Length - 1];
				string typename = (match.Groups ["typename"].Success ? match.Groups ["typename"].Value : null);
				int index = 0;

				if (1 == tokens.Length || null == typename) {
					// Ideally if typename is null and token length is longer than 1, 
					// we have an expression like: var w = new x.y.z(); and 
					// we would check whether z is the type or if y.z is an overload for type y
					typename = overload;
				} else if ("var".Equals (typename, StringComparison.Ordinal)) {
					typename = match.Groups ["constructor"].Value;
				} else {
					// Foo.Bar bar = new Foo.Bar.blah( ...
					for (string[] typeTokens = typename.Split ('.'); index < typeTokens.Length && index < tokens.Length; ++index) {
						if (!typeTokens [index].Equals (tokens [index], StringComparison.Ordinal)) {
							break;
						}
					}
					List<string> overloadTokens = new List<string> ();
					for (int i = index; i < tokens.Length; ++i) {
						overloadTokens.Add (tokens [i]);
					}
					overload = string.Join (".", overloadTokens.ToArray ());
				}

				// HACK: Generics
				if (0 < (index = overload.IndexOf ("<", StringComparison.Ordinal))) {
					overload = overload.Substring (0, index);
				}
				if (0 < (index = typename.IndexOf ("<", StringComparison.Ordinal))) {
					typename = typename.Substring (0, index);
				}

				// Console.WriteLine ("Constructor: type {0}, overload {1}", typename, overload);
				return new ParameterDataProvider (Document, ProjectInfo, typename, overload);
			}

			int nameStart = lineText.LastIndexOfAny (allowedChars) + 1;
			functionName = lineText.Substring (nameStart).Trim ();
			return (string.IsNullOrEmpty (functionName) ? null : new ParameterDataProvider (Document, ProjectInfo, functionName));
		}
		/*
		private bool AllWhiteSpace (string lineText)
		{
			foreach (char c in lineText)
				if (!char.IsWhiteSpace (c))
					return false;

			return true;
		}
*/

		#region IPathedDocument implementation

		public event EventHandler<DocumentPathChangedEventArgs> PathChanged;

		public Gtk.Widget CreatePathWidget (int index)
		{
			PathEntry[] path = CurrentPath;
			if (null == path || 0 > index || path.Length <= index) {
				return null;
			}

			object tag = path [index].Tag;
			DropDownBoxListWindow.IListDataProvider provider = null;
			if (tag is ParsedDocument) {
				provider = new CompilationUnitDataProvider (Document);
			} else {
				provider = new DataProvider (Document, tag, GetAmbience ());
			}

			DropDownBoxListWindow window = new DropDownBoxListWindow (provider);
			window.SelectItem (tag);
			return window;
		}

		public PathEntry[] CurrentPath {
			get;
			private set;
		}

		protected virtual void OnPathChanged (DocumentPathChangedEventArgs args)
		{
			if (null != PathChanged) {
				PathChanged (this, args);
			}
		}



		// Yoinked from C# binding
		void UpdatePath (object sender, DocumentLocationEventArgs e)
		{
			if (ProjectInfo == null)
				return;
			// TODO:

			/*var unit = Document.CompilationUnit;
			if (unit == null)
				return;
*/
			var loc = textEditorData.Caret.Location;

			//IType type = unit.GetTypeAt (loc.Line, loc.Column);
			List<PathEntry> result = new List<PathEntry> ();
			//Ambience amb = GetAmbience ();
			//IMember member = null;
			//INode node = (INode)unit;
			if (loc == null) {
				LoggingService.LogDebug ("location null");
			} else if (FileName == null) {
				LoggingService.LogDebug ("Filename null");
			} else {
				Symbol node = ProjectInfo.GetEnclosingSymbolAtPosition (FileName.FullPath, loc.Line, loc.Column);

				PathEntry entry;

				while ((node != null) /*&& ((node is IBlockNode) || (node is DEnumValue))*/) {
					// var icon = DIcons.GetNodeIcon(node as DNode);
					Xwt.Drawing.Image icon = null; 
					if (!string.IsNullOrEmpty (node.Icon))
						icon = ImageService.GetIcon (node.Icon);
					entry = new PathEntry (icon, 
						node.DisplayText /*+ DParameterDataProvider.GetNodeParamString (node)*/);
					entry.Position = EntryPosition.Left;
					entry.Tag = node;
					//do not include the module in the path bar
					if ((node.Parent != null) /*&& !((node is DNode) && (node as DNode).IsAnonymous)*/)
						result.Insert (0, entry);
					node = node.Parent;
				}

				var prev = CurrentPath;
				CurrentPath = result.ToArray ();
				OnPathChanged (new DocumentPathChangedEventArgs (prev));
			}
		}

		#endregion

	}
}
