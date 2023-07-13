using LoggerTool;

namespace DirectoryFileManagerTool;

class DirectoryFileManager
{
    private string path;
    private string[]? arrayOfFolders, arrayOfFiles;
    private bool returnDirectory;
    public DirectoryFileManager()
    {
        path = Directory.GetCurrentDirectory();
        Folders();
        Files();
    }
    public string Path => path;
    public string[] ArrayOfFolders { get { return arrayOfFolders != null ? arrayOfFolders : new string[0]; } }
    public string[] ArrayOfFiles { get { return arrayOfFiles != null ? arrayOfFiles : new string[0]; } }
    public int CountOfFolders { get { return arrayOfFolders != null ? arrayOfFolders.Length : 0; } }
    public int CountOfFiles { get { return arrayOfFiles != null ? arrayOfFiles.Length : 0; } }
    public bool ReturnDirectory { get { return returnDirectory; } }
    public void ChangeFolder(string direction)
    {
        try
        {
            if (direction.Substring(direction.Length - 2, 2) == ":\\") Directory.SetCurrentDirectory(direction);
            else Directory.SetCurrentDirectory(path + "\\" + direction);
            path = Directory.GetCurrentDirectory();
            Folders();
            Files();
        }
        catch (Exception e)
        {
            Logger.SaveLog(e.Message);
        }
    }
    public void ChangeFolderBack()
    {
        try
        {
            Directory.SetCurrentDirectory("..");
            path = Directory.GetCurrentDirectory();
            Folders();
            Files();
        }
        catch (Exception e)
        {
            Logger.SaveLog(e.Message);
        }
    }
    private void Folders()
    {
        var temp = Directory.GetDirectories(path).ToList();
        List<string> tempOutPut = new List<string>();
        DirectoryInfo dirInfo;
        for (int i = 0; i < temp.Count; i++)
        {
            dirInfo = new DirectoryInfo(temp[i]);
            if (dirInfo.Attributes.HasFlag(FileAttributes.System)) continue;
            if (path.Length == 3) temp[i] = temp[i].Remove(0, path.Length);
            else temp[i] = temp[i].Remove(0, path.Length + 1);
            tempOutPut.Add(temp[i]);
        }
        if (!(path.Length == 3))
        {
            returnDirectory = true;
            tempOutPut.Insert(0, "..");
        }
        else
        {
            returnDirectory = false;
            List<string> drives = System.IO.Directory.GetLogicalDrives().ToList();
            drives.Remove(Path.Substring(0, 3));
            tempOutPut = drives.Concat(tempOutPut).ToList();
        }
        arrayOfFolders = tempOutPut.ToArray();
    }
    public void Files()
    {
        string[] extensions = { "wav", "mp3" };
        var musicFiles = new DirectoryInfo(path).GetFiles("*.*").Where(file => extensions.Contains(System.IO.Path.GetExtension(file.FullName).TrimStart('.').ToLowerInvariant()));
        List<string> output = new List<string>();
        foreach (FileInfo musicFile in musicFiles) output.Add(musicFile.Name);
        arrayOfFiles = output.ToArray();
    }
}