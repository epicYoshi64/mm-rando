﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace MMRando.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class HtmlSpoiler : HtmlSpoilerBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"<html>
<head>
<style>
	th{ text-align:left }
	.spoiler{ background-color:black }
	.spoiler:hover { background-color: white;  }
	[data-content]:before { content: attr(data-content); }

	.show-highlight .unavailable .newlocation { background-color: #FFDDDD; }
	.show-highlight .acquired .newlocation { background-color: #DDFFDD; }
	.show-highlight .available .newlocation { background-color: #DDDDFF; }
</style>
</head>
<label><b>Version: </b></label><span>");
            this.Write(this.ToStringHelper.ToStringWithCulture(spoiler.Version));
            this.Write("</span><br/>\r\n<label><b>Settings String: </b></label><span>");
            this.Write(this.ToStringHelper.ToStringWithCulture(spoiler.SettingsString));
            this.Write("</span><br/>\r\n<label><b>Seed: </b></label><span>");
            this.Write(this.ToStringHelper.ToStringWithCulture(spoiler.Seed));
            this.Write("<span><br/>\n");
 if (spoiler.CustomItemListString != null) { 
            this.Write("<label><b>Custom Item List: </b></label><span>");
            this.Write(this.ToStringHelper.ToStringWithCulture(spoiler.CustomItemListString));
            this.Write("<span><br/>\n");
 } 
            this.Write("\n<br/>\r\n");
 if (spoiler.RandomizeDungeonEntrances) { 

            this.Write("<h2>Dungeon Entrance Replacements</h2>\r\n<table border=\"1\">\r\n\t<tr>\r\n\t\t<th>Entrance" +
                    "</th>\r\n\t\t<th>New Destination</th>\r\n\t</tr>\r\n");
		 for (int i = 0; i < 4; i++) { 
			int newEntranceIndex = spoiler.NewDestinationIndices[i]; 
			string destination = spoiler.Destinations[i];
			string newDestination = spoiler.Destinations[newEntranceIndex];
            this.Write("\t<tr>\r\n\t\t<td>");
            this.Write(this.ToStringHelper.ToStringWithCulture(destination));
            this.Write("</td>\r\n\t\t<td class=\"spoiler\"><span data-content=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(newDestination));
            this.Write("\"></span></td>\r\n\t</tr>\r\n");
 } 
            this.Write("</table>\r\n");
 } 
            this.Write("<h2>Item Replacements</h2>\r\n<input type=\"checkbox\" id=\"highlight-checks\"/> Highli" +
                    "ght available checks\r\n<table border=\"1\" id=\"item-replacements\">\r\n <tr>\r\n     <th" +
                    ">Location</th>\r\n\t <th></th>\r\n     <th>Item</th>\r\n </tr>\r\n");
 foreach (var item in spoiler.ItemList.OrderBy(item => item.NewLocationId)) {

            this.Write(" <tr data-id=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.Id));
            this.Write("\" data-newlocationid=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.NewLocationId));
            this.Write("\" class=\"unavailable\">\r\n\t<td class=\"newlocation\">");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.NewLocationName));
            this.Write("</td>\r\n\t<td><input type=\"checkbox\"/></td>\r\n\t<td class=\"spoiler itemname\"> <span d" +
                    "ata-content=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.Name));
            this.Write("\"></span></td>\r\n </tr>\r\n");
 } 
            this.Write("</table>\r\n<h2>Item Locations</h2>\r\n<table border=\"1\" id=\"item-locations\">\r\n <tr>\r" +
                    "\n     <th>Item</th>\r\n\t <th></th>\r\n     <th>Location</th>\r\n </tr>\r\n");
 foreach (var item in spoiler.ItemList.Where(item => !item.IsJunk)) {

            this.Write(" <tr data-id=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.Id));
            this.Write("\" data-newlocationid=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.NewLocationId));
            this.Write("\">\r\n\t<td>");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.Name));
            this.Write("</td>\r\n\t<td><input type=\"checkbox\"/></td>\r\n\t<td class=\"spoiler newlocation\"> <spa" +
                    "n data-content=\"");
            this.Write(this.ToStringHelper.ToStringWithCulture(item.NewLocationName));
            this.Write("\"></span></td>\r\n </tr>\r\n");
 } 
            this.Write("</table>\r\n<script>\r\n\tvar logic = ");
            this.Write(this.ToStringHelper.ToStringWithCulture(spoiler.LogicJson));
            this.Write("\r\n\r\n\tfunction all(list, predicate) {\r\n\t\tfor (var i = 0; i < list.length; i++) {\r\n" +
                    "\t\t\tif (!predicate(list[i])) {\r\n\t\t\t\treturn false;\r\n\t\t\t}\r\n\t\t}\r\n\t\treturn true;\r\n\t}\r" +
                    "\n\r\n\tfunction any(list, predicate) {\r\n\t\tfor (var i = 0; i < list.length; i++) {\r\n" +
                    "\t\t\tif (predicate(list[i])) {\r\n\t\t\t\treturn true;\r\n\t\t\t}\r\n\t\t}\r\n\t\treturn false;\r\n\t}\r\n" +
                    "\r\n\tfunction recalculateItems() {\r\n\t\tvar recalculate = false;\r\n\t\tfor (var i = 0; " +
                    "i < logic.length; i++) {\r\n\t\t\tvar item = logic[i];\r\n\t\t\titem.IsAvailable = \r\n\t\t\t\t(" +
                    "item.RequiredItemIds === null || all(item.RequiredItemIds, function(id) { return" +
                    " logic[id].Acquired; }))\r\n\t\t\t\t&& \r\n\t\t\t\t(item.ConditionalItemIds === null || any(" +
                    "item.ConditionalItemIds, function(conditionals) { return all(conditionals, funct" +
                    "ion(id) { return logic[id].Acquired; }); }));\r\n            \r\n\t\t\tif (!item.Acquir" +
                    "ed && item.IsFakeItem && item.IsAvailable) {\r\n\t\t\t\titem.Acquired = true;\r\n\t\t\t\trec" +
                    "alculate = true;\r\n\t\t\t}\r\n\t\t\tif (item.Acquired && item.IsFakeItem && !item.IsAvail" +
                    "able) {\r\n\t\t\t\titem.Acquired = false;\r\n\t\t\t\trecalculate = true;\r\n\t\t\t}\r\n        \r\n\t\t" +
                    "\tvar locationRow = document.querySelector(\"#item-replacements tr[data-newlocatio" +
                    "nid=\'\" + item.ItemId + \"\']\");\r\n\t\t\tif (locationRow) {\r\n\t\t\t\tlocationRow.className " +
                    "= \"\";\r\n\t\t\t\tlocationRow.classList.add(item.IsAvailable ? \"available\" : \"unavailab" +
                    "le\");\r\n\t\t\t\tvar itemName = locationRow.querySelector(\".itemname\");\n              " +
                    "  var checkbox = locationRow.querySelector(\"input\");\n                checkbox.ch" +
                    "ecked = item.Checked;\r\n\t\t\t\tif (item.Checked) {\r\n\t\t\t\t\titemName.classList.remove(\"" +
                    "spoiler\");\r\n\t\t\t\t} else {\r\n\t\t\t\t\titemName.classList.add(\"spoiler\");\r\n\t\t\t\t}\r\n\t\t\t}\r\n" +
                    "        \r\n\t\t\tvar itemRow = document.querySelector(\"#item-locations tr[data-id=\'\"" +
                    " + item.ItemId + \"\']\");\r\n\t\t\tif (itemRow) {\r\n\t\t\t\tvar itemName = itemRow.querySele" +
                    "ctor(\".newlocation\");\n                var checkbox = itemRow.querySelector(\"inpu" +
                    "t\");\n                checkbox.checked = item.Acquired;\r\n\t\t\t\tif (item.Acquired) {" +
                    "\r\n\t\t\t\t\titemName.classList.remove(\"spoiler\");\r\n\t\t\t\t} else {\r\n\t\t\t\t\titemName.classL" +
                    "ist.add(\"spoiler\");\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t\tif (recalculate) {\r\n\t\t\trecalculateItems" +
                    "();\r\n\t\t}\r\n\t}\r\n\r\n\tlogic[0].Checked = true;\r\n\tlogic[document.querySelector(\"tr[dat" +
                    "a-newlocationid=\'0\']\").dataset.id].Acquired = true;\r\n\tdocument.querySelector(\"tr" +
                    "[data-newlocationid=\'0\'] input\").checked = true;\r\n\r\n\tlogic[90].Checked = true;\r\n" +
                    "\tlogic[document.querySelector(\"tr[data-newlocationid=\'90\']\").dataset.id].Acquire" +
                    "d = true;\r\n\tdocument.querySelector(\"tr[data-newlocationid=\'90\'] input\").checked " +
                    "= true;\r\n\r\n\trecalculateItems();\r\n\r\n\tvar rows = document.querySelectorAll(\"tr\");\r" +
                    "\n\tfor (var i = 1; i < rows.length; i++) {\r\n\t\tvar row = rows[i];\r\n\t\tvar checkbox " +
                    "= row.querySelector(\"input\");\r\n\t\tif (checkbox) {\r\n\t\t\tcheckbox.addEventListener(\"" +
                    "click\", function(e) {\r\n\t\t\t\tvar row = e.target.closest(\"tr\");\n                var" +
                    " rowId = parseInt(row.dataset.id);\r\n\t\t\t\tvar newLocationId = parseInt(row.dataset" +
                    ".newlocationid);\r\n\t\t\t\tlogic[newLocationId].Checked = e.target.checked;\n         " +
                    "       logic[rowId].Acquired = e.target.checked;\r\n\t\t\t\trecalculateItems();\r\n\t\t\t})" +
                    ";\r\n\t\t}\r\n\t}\r\n\r\n\tdocument.querySelector(\"#highlight-checks\").addEventListener(\"cli" +
                    "ck\", function(e) {\r\n\t\tdocument.querySelector(\"table#item-replacements\").classNam" +
                    "e = e.target.checked ? \"show-highlight\" : \"\";\r\n\t});\r\n</script>\r\n</html>");
            return this.GenerationEnvironment.ToString();
        }
    }
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class HtmlSpoilerBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
