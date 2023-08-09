namespace DirectoryFileManagerTool;

class DirectoryFileManager
{
    private string[]? arrayOfFolders, arrayOfFiles;
    private string path;
    public DirectoryFileManager(string? startPath)
    {
        try
        {
            Directory.SetCurrentDirectory(startPath != null ? startPath : AppContext.BaseDirectory);
        }
        catch (Exception)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }
        this.path = Directory.GetCurrentDirectory();
        Folders();
        Files();
    }
    public string[] ArrayOfFolders => arrayOfFolders != null ? arrayOfFolders : new string[0];
    public string[] ArrayOfFiles => arrayOfFiles != null ? arrayOfFiles : new string[0];
    public string Path => path;
    public int CountOfFolders => arrayOfFolders != null ? arrayOfFolders.Length : 0;
    public int CountOfFiles => arrayOfFiles != null ? arrayOfFiles.Length : 0;
    public void ChangeFolder(string direction)
    {
        try
        {
            if (direction == "..") Directory.SetCurrentDirectory("..");
            else if (direction.Substring(direction.Length - 2, 2) == ":\\") Directory.SetCurrentDirectory(direction);
            else Directory.SetCurrentDirectory(path + "\\" + direction);
        }
        catch (Exception)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }
        path = Directory.GetCurrentDirectory();
        Folders();
        Files();
    }
    private void Folders()
    {
        List<string> folders = Directory.GetDirectories(path).ToList();
        List<string> outFolders = new List<string>();
        DirectoryInfo folderInfo;
        for (int i = 0; i < folders.Count; i++)
        {
            folderInfo = new DirectoryInfo(folders[i]);
            if (folderInfo.Attributes.HasFlag(FileAttributes.System)) continue;
            if (path.Length == 3) folders[i] = folders[i].Remove(0, path.Length);
            else folders[i] = folders[i].Remove(0, path.Length + 1);
            outFolders.Add(folders[i]);
        }
        if (!(path.Length == 3)) outFolders.Insert(0, "..");
        else
        {
            List<string> drives = Directory.GetLogicalDrives().ToList();
            drives.Remove(path.Substring(0, 3));
            outFolders = drives.Concat(outFolders).ToList();
        }
        arrayOfFolders = outFolders.ToArray();
    }
    private void Files()
    {
        string[] extensions = { "wav", "mp3" };
        IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles("*.*").Where(file => extensions.Contains(System.IO.Path.GetExtension(file.FullName).TrimStart('.').ToLowerInvariant()));
        List<string> outFiles = new List<string>();
        foreach (FileInfo file in files) outFiles.Add(file.Name);
        arrayOfFiles = outFiles.ToArray();
    }
    public bool Refresh()
    {
        bool changedFolder = false;
        if (!Directory.Exists(path))
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            path = Directory.GetCurrentDirectory();
            changedFolder = true;
            Folders();
        }
        Files();
        return changedFolder;
    }
}