using Rhino;
using Rhino.Collections;
using Rhino.FileIO;
using Rhino.PlugIns;

namespace SiteBuilding
{
  /// <summary>
  /// SampleCsUserDataPlugIn plug-in class
  /// </summary>
  public class SiteBuilding : PlugIn
  {
    private const int MAJOR = 1;
    private const int MINOR = 0;

    // Even more document user data
    public ArchivableDictionary Dictionary { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public SiteBuilding()
    {
      Instance = this;
      Dictionary = new ArchivableDictionary(MAJOR, "SampleCsDictionary");
    }

    /// <summary>
    /// Gets the only instance of the SampleCsUserDataPlugIn plug-in.
    /// </summary>
    public static SiteBuilding Instance { get; private set; }

    /// <summary>
    /// Called when the plug-in is being loaded.
    /// </summary>
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
  }
}