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
            Directory.SetCurrentDirectory(path + "\\" + direction);
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
        else returnDirectory = false;
        arrayOfFolders = tempOutPut.ToArray();
    }
    private void Files()
    {
        var tempWav = Directory.GetFiles(path, "*.wav");
        var tempMp3 = Directory.GetFiles(path, "*.mp3");
        string[] temp = new string[tempWav.Length + tempMp3.Length];
        tempWav.CopyTo(temp, 0);
        tempMp3.CopyTo(temp, tempWav.Length);
        for (int i = 0; i < temp.Length; i++) temp[i] = temp[i].Remove(0, path.Length + 1);
        arrayOfFiles = temp;
    }
}