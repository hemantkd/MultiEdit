using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace MultiPointEdit
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class MultiPointEditFilterProvider : IVsTextViewCreationListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("MultiEditLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition multiEditAdornmentLayer = null;

        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);

            if (textView != null)
                AddCommandFilter(textViewAdapter, textView, new MultiPointEditCommandFilter(textView));
        }

        private void AddCommandFilter(IVsTextView viewAdapter, IWpfTextView textView, MultiPointEditCommandFilter commandFilter)
        {
            if (commandFilter.Added == false)
            {
                IOleCommandTarget next;
                int result = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (result == VSConstants.S_OK)
                {
                    commandFilter.Added = true;
                    textView.Properties.AddProperty(typeof(MultiPointEditCommandFilter), commandFilter);

                    if (next != null)
                    {
                        commandFilter.NextTarget = next;
                    }
                }
            }
        }
    }
}
