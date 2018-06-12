public delegate void ProgressDelegate(long fileSize, long processSize);

public class FileChangeInfo
{
   
    // Fields
    public string inpath;
    public string outpath;
    public ProgressDelegate progressPercentDelegate;
    public ProgressDelegate progressDelegate;

    // Methods
    public FileChangeInfo() { }
}


