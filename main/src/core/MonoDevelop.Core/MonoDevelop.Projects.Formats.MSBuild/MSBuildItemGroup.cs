//
// MSBuildItemGroup.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2014 Xamarin, Inc (http://www.xamarin.com)
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

using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;

namespace MonoDevelop.Projects.Formats.MSBuild
{
	public class MSBuildItemGroup: MSBuildObject
	{
		List<MSBuildItem> items = new List<MSBuildItem> ();

		internal override void Read (XmlReader reader, ReadContext context)
		{
			base.Read (reader, context);

			if (reader.IsEmptyElement) {
				reader.Skip ();
				return;
			}
			reader.Read ();
			while (reader.NodeType != XmlNodeType.EndElement) {
				if (reader.NodeType == XmlNodeType.Element) {
					var item = new MSBuildItem ();
					item.ParentObject = this;
					item.Read (reader, context);
					items.Add (item);
				}
				else
					reader.Read ();
			}
			reader.Read ();
		}

		internal override void Write (XmlWriter writer, WriteContext context)
		{
			writer.WriteStartElement ("ItemGroup", MSBuildProject.Schema);
			base.Write (writer, context);
			foreach (var it in items)
				it.Write (writer, context);
			writer.WriteEndElement ();
		}

		internal override IEnumerable<MSBuildObject> GetChildren ()
		{
			return items;
		}

		public bool IsImported {
			get;
			set;
		}
		
		public MSBuildItem AddNewItem (string name, string include)
		{
			var it = new MSBuildItem (name);
			it.ParentObject = this;
			it.Include = include;
			items.Add (it);
			if (Project != null)
				Project.NotifyChanged ();
			return it;
		}

		public void AddItem (MSBuildItem item)
		{
			items.Add (item);
			if (Project != null)
				Project.NotifyChanged ();
		}

		public IEnumerable<MSBuildItem> Items {
			get {
				return items;
			}
		}

		internal void RemoveItem (MSBuildItem item)
		{
			if (items.Contains (item)) {
				item.RemoveIndent ();
				items.Remove (item);
				NotifyChanged ();
			}
		}
	}
	
}
