using Rhino;
using Rhino.Collections;
using Rhino.FileIO;
using Rhino.PlugIns;
using System;

namespace AutoPlan.AutoPlan
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class AutoPlanPlugin : Rhino.PlugIns.PlugIn
    {
        private const int MAJOR = 1;
        private const int MINOR = 0;
        public ArchivableDictionary Dictionary { get; private set; }
        public AutoPlanPlugin()
        {
            Instance = this;
            Dictionary = new ArchivableDictionary(MAJOR, "SampleCsDictionary");
        }

        ///<summary>Gets the only instance of the AutoPlanPlugin plug-in.</summary>
        public static AutoPlanPlugin Instance { get; private set; }
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            // Add an event handler so we know when documents are closed.
            RhinoDoc.CloseDocument += OnCloseDocument;
            return LoadReturnCode.Success;

        }
        /// <summary>
        /// OnCloseDocument event handler.
        /// </summary>
        private void OnCloseDocument(object sender, DocumentEventArgs e)
        {
            // When the document is closed, clear our 
            // document user data containers.
            // Dictionary.Clear();
        }
        /// <summary>
        /// Called whenever a Rhino is about to save a .3dm file.  If you want to save
        //  plug-in document data when a model is saved in a version 5 .3dm file, then
        //  you must override this function to return true and you must override WriteDocument().
        /// </summary>
        protected override bool ShouldCallWriteDocument(FileWriteOptions options)
        {
            return !options.WriteGeometryOnly && !options.WriteSelectedObjectsOnly;
        }

        /// <summary>
        /// Called when Rhino is saving a .3dm file to allow the plug-in to save document user data.
        /// </summary>
        protected override void WriteDocument(RhinoDoc doc, BinaryArchiveWriter archive, FileWriteOptions options)
        {
            // Write the version of our document data
            archive.Write3dmChunkVersion(MAJOR, MINOR);
            // Write the dictionary
            archive.WriteDictionary(Dictionary);
        }
        /// <summary>
        /// Called whenever a Rhino document is being loaded and plug-in user data was
        /// encountered written by a plug-in with this plug-in's GUID.
        /// </summary>
        protected override void ReadDocument(RhinoDoc doc, BinaryArchiveReader archive, FileReadOptions options)
        {
            archive.Read3dmChunkVersion(out var major, out var minor);
            if (MAJOR == major && MINOR == minor)
            {
                // Always read user data even though you might not use it.

                var dictionary = archive.ReadDictionary();

                if (!options.ImportMode && !options.ImportReferenceMode)
                {
                    if (null != dictionary && dictionary.Count > 0)
                        Dictionary = dictionary;
                }
            }
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
    }
}