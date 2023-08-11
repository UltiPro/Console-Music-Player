using System.Diagnostics;
using LoggerTool;
using DirectoryFileManagerTool;
using MusicPlayerTool;

namespace MusicConsoleTool;

class MusicConsole
{
    private DirectoryFileManager directoryFileManager;
    private MusicPlayer musicPlayer;
    private Thread threadTimer, threadAnimation, threadAnimation2;
    private char[,] console;
    private readonly char[] walls = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
    private readonly char[] arrows = { '▲', '▼' };
    private readonly char[] dots = { '●', '○' };
    private readonly char[] notes = { '♪', '♫' };
    private readonly int widthOfWindow, widthOfWindow_m1, widthOfWindow_m2;
    private readonly int widthOfWindow1_8;
    private readonly int widthOfWindow1_4, widthOfWindow1_4_m1, widthOfWindow1_4_m2, widthOfWindow1_4_p1, widthOfWindow1_4_p2;
    private readonly int widthOfWindow1_2, widthOfWindow1_2_m1, widthOfWindow1_2_m2, widthOfWindow1_2_m3, widthOfWindow1_2_m4, widthOfWindow1_2_m8, widthOfWindow1_2_m9, widthOfWindow1_2_m10, widthOfWindow1_2_m11, widthOfWindow1_2_p1;
    private readonly int widthOfWindow3_4, widthOfWindow3_4_m2, widthOfWindow3_4_p1, widthOfWindow3_4_p2;
    private readonly int widthOfWindow_m_widthOfWindow1_8_m3;
    private readonly int heightOfWindow, heightOfWindow_m1;
    private readonly int heightOfWindow1_4, heightOfWindow1_4_m1;
    private readonly int heightOfWindow3_4, heightOfWindow3_4_m1, heightOfWindow3_4_m2, heightOfWindow3_4_m3, heightOfWindow3_4_m4, heightOfWindow3_4_m6, heightOfWindow3_4_m7;
    private readonly int heightOfWindow3_4_m9, heightOfWindow3_4_m10, heightOfWindow3_4_m12, heightOfWindow3_4_p1, heightOfWindow3_4_p2, heightOfWindow3_4_p3;
    private readonly int heightOfWindow3_4_p4, heightOfWindow3_4_p5, heightOfWindow3_4_p6, heightOfWindow3_4_p7, heightOfWindow3_4_p8;
    private bool launchApp;
    public DirectoryFileManager DirectoryFileManager => directoryFileManager;
    public int currentFolderIdx, currentFileIdx, currentFileIdxMemory, cursorFileIdx;
    public bool nowPlaying;
    public MusicConsole()
    {
        console = new char[160, 40];
        Console.Title = "Console Music Player";
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.TreatControlCAsInput = true;
        Console.ForegroundColor = ConsoleColor.White;
        try
        {
            Console.SetBufferSize(console.GetLength(0), console.GetLength(1));
            Console.SetWindowSize(console.GetLength(0), console.GetLength(1));
        }
        catch (Exception e)
        {
            Logger.SaveLog(e.Message);
            System.Environment.Exit(-1);
        }
        try
        {
            using (StreamReader settingsFile = new(AppContext.BaseDirectory + "/Settings"))
            {
                directoryFileManager = new DirectoryFileManager(settingsFile.ReadLine());
                short tempVolume;
                if (short.TryParse(settingsFile.ReadLine(), out tempVolume)) musicPlayer = new MusicPlayer(this, tempVolume);
                else musicPlayer = new MusicPlayer(this);
                settingsFile.Close();
            }
        }
        catch (Exception)
        {
            directoryFileManager = new DirectoryFileManager(null);
            musicPlayer = new MusicPlayer(this);
        }
        threadTimer = new Thread(() => UpdateTimer());
        threadAnimation = new Thread(() => UpdateAnimation());
        threadAnimation2 = new Thread(() => UpdateAnimation2());
        widthOfWindow = Console.WindowWidth;
        widthOfWindow_m1 = widthOfWindow - 1;
        widthOfWindow_m2 = widthOfWindow - 2;
        widthOfWindow1_8 = Console.WindowWidth / 8;
        widthOfWindow1_4 = Console.WindowWidth / 4;
        widthOfWindow1_4_m1 = widthOfWindow1_4 - 1;
        widthOfWindow1_4_m2 = widthOfWindow1_4 - 2;
        widthOfWindow1_4_p1 = widthOfWindow1_4 + 1;
        widthOfWindow1_4_p2 = widthOfWindow1_4 + 2;
        widthOfWindow1_2 = Console.WindowWidth / 2;
        widthOfWindow1_2_m1 = widthOfWindow1_2 - 1;
        widthOfWindow1_2_m2 = widthOfWindow1_2 - 2;
        widthOfWindow1_2_m3 = widthOfWindow1_2 - 3;
        widthOfWindow1_2_m4 = widthOfWindow1_2 - 4;
        widthOfWindow1_2_m8 = widthOfWindow1_2 - 8;
        widthOfWindow1_2_m9 = widthOfWindow1_2 - 9;
        widthOfWindow1_2_m10 = widthOfWindow1_2 - 10;
        widthOfWindow1_2_m11 = widthOfWindow1_2 - 11;
        widthOfWindow1_2_p1 = widthOfWindow1_2 + 1;
        widthOfWindow3_4 = Console.WindowWidth / 4 * 3;
        widthOfWindow3_4_m2 = widthOfWindow3_4 - 2;
        widthOfWindow3_4_p1 = widthOfWindow3_4 + 1;
        widthOfWindow3_4_p2 = widthOfWindow3_4 + 2;
        widthOfWindow_m_widthOfWindow1_8_m3 = widthOfWindow - widthOfWindow1_8 - 3;
        heightOfWindow = Console.WindowHeight;
        heightOfWindow_m1 = heightOfWindow - 1;
        heightOfWindow1_4 = Console.WindowHeight / 4;
        heightOfWindow1_4_m1 = heightOfWindow1_4 - 1;
        heightOfWindow3_4 = Console.WindowHeight / 4 * 3;
        heightOfWindow3_4_m1 = heightOfWindow3_4 - 1;
        heightOfWindow3_4_m2 = heightOfWindow3_4 - 2;
        heightOfWindow3_4_m3 = heightOfWindow3_4 - 3;
        heightOfWindow3_4_m4 = heightOfWindow3_4 - 4;
        heightOfWindow3_4_m6 = heightOfWindow3_4 - 6;
        heightOfWindow3_4_m7 = heightOfWindow3_4 - 7;
        heightOfWindow3_4_m9 = heightOfWindow3_4 - 9;
        heightOfWindow3_4_m10 = heightOfWindow3_4 - 10;
        heightOfWindow3_4_m12 = heightOfWindow3_4 - 12;
        heightOfWindow3_4_p1 = heightOfWindow3_4 + 1;
        heightOfWindow3_4_p2 = heightOfWindow3_4 + 2;
        heightOfWindow3_4_p3 = heightOfWindow3_4 + 3;
        heightOfWindow3_4_p4 = heightOfWindow3_4 + 4;
        heightOfWindow3_4_p5 = heightOfWindow3_4 + 5;
        heightOfWindow3_4_p6 = heightOfWindow3_4 + 6;
        heightOfWindow3_4_p7 = heightOfWindow3_4 + 7;
        heightOfWindow3_4_p8 = heightOfWindow3_4 + 8;
        currentFolderIdx = 0;
        currentFileIdx = currentFileIdxMemory = -2;
        if (directoryFileManager.CountOfFiles > 0) cursorFileIdx = 0;
        else cursorFileIdx = -2;
        launchApp = nowPlaying = false;
    }
    public void Start()
    {
        StartMenu();
        InitMusicConsole();
        launchApp = true;
        threadTimer.Start();
        threadAnimation.Start();
        threadAnimation2.Start();
        ConsoleKeyInfo inputFromKeyboard;
        while (launchApp)
        {
            inputFromKeyboard = Console.ReadKey(true);
            if (Console.KeyAvailable) continue;
            switch (inputFromKeyboard.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentFolderIdx > 0)
                    {
                        currentFolderIdx--;
                        UpdateFolders();
                    }
                    Thread.Sleep(100);
                    break;
                case ConsoleKey.DownArrow:
                    if (currentFolderIdx < directoryFileManager.CountOfFolders - 1)
                    {
                        currentFolderIdx++;
                        UpdateFolders();
                    }
                    Thread.Sleep(100);
                    break;
                case ConsoleKey.LeftArrow:
                    if (cursorFileIdx > 0 && cursorFileIdx != -2)
                    {
                        cursorFileIdx--;
                        UpdateFiles();
                    }
                    Thread.Sleep(100);
                    break;
                case ConsoleKey.RightArrow:
                    if (cursorFileIdx < directoryFileManager.CountOfFiles - 1 && cursorFileIdx != -2)
                    {
                        cursorFileIdx++;
                        UpdateFiles();
                    }
                    Thread.Sleep(100);
                    break;
                case ConsoleKey.Enter:
                    directoryFileManager.ChangeFolder(directoryFileManager.ArrayOfFolders[currentFolderIdx]);
                    currentFolderIdx = 0;
                    currentFileIdxMemory = currentFileIdx == -2 ? currentFileIdxMemory : currentFileIdx;
                    currentFileIdx = Path.GetDirectoryName(musicPlayer.TrackPath) == directoryFileManager.Path ? currentFileIdxMemory : -2;
                    if (directoryFileManager.CountOfFiles > 0) cursorFileIdx = 0;
                    else cursorFileIdx = -2;
                    UpdatePath();
                    UpdateFolders();
                    UpdateFiles();
                    break;
                case ConsoleKey.Spacebar:
                    if (cursorFileIdx != -2)
                    {
                        currentFileIdx = cursorFileIdx;
                        musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    }
                    break;
                case ConsoleKey.F1:
                    musicPlayer.ChangeMute(true);
                    UpdateVolume();
                    break;
                case ConsoleKey.F2:
                    musicPlayer.ChangeVolume(-1);
                    UpdateVolume();
                    break;
                case ConsoleKey.F3:
                    musicPlayer.ChangeVolume(1);
                    UpdateVolume();
                    break;
                case ConsoleKey.F4:
                    musicPlayer.ChangeMute(false);
                    UpdateVolume();
                    break;
                case ConsoleKey.F5:
                    if (currentFileIdx == -2) continue;
                    if (!musicPlayer.IsPlayerJustStarted)
                    {
                        musicPlayer.RewindTrack(double.MinValue);
                        continue;
                    }
                    currentFileIdx = (currentFileIdx - 1) < 0 ? directoryFileManager.CountOfFiles - 1 : --currentFileIdx;
                    musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    Thread.Sleep(200);
                    break;
                case ConsoleKey.F6:
                    musicPlayer.RewindTrack(-10);
                    Thread.Sleep(500);
                    break;
                case ConsoleKey.F7:
                    if (nowPlaying)
                    {
                        musicPlayer.Pause();
                        nowPlaying = !nowPlaying;
                    }
                    else if (!nowPlaying && musicPlayer.IsLoaded && !musicPlayer.ErrorOccured)
                    {
                        musicPlayer.Play();
                        nowPlaying = !nowPlaying;
                    }
                    break;
                case ConsoleKey.F8:
                    musicPlayer.RewindTrack(10);
                    Thread.Sleep(500);
                    break;
                case ConsoleKey.F9:
                    if (currentFileIdx == -2) continue;
                    currentFileIdx = (currentFileIdx + 1) < directoryFileManager.CountOfFiles ? ++currentFileIdx : 0;
                    musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    Thread.Sleep(200);
                    break;
                case ConsoleKey.I:
                    try
                    {
                        Process.Start("notepad.exe", AppContext.BaseDirectory + "/Information.txt");
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.Message);
                    }
                    break;
                case ConsoleKey.Escape:
                    try
                    {
                        using (StreamWriter settingsFile = new(AppContext.BaseDirectory + "/Settings", false))
                        {
                            settingsFile.WriteLine(directoryFileManager.Path);
                            settingsFile.WriteLine(musicPlayer.PlayerVolume.ToString());
                            settingsFile.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.Message);
                    }
                    launchApp = false;
                    Thread.Sleep(300);
                    Console.Clear();
                    break;
            }
        }
    }
    private void StartMenu()
    {
        Console.Clear();
        for (int i = 0; i < 10; i++) Console.WriteLine();
        try
        {
            using (StreamReader logoFile = new(AppContext.BaseDirectory + "/Logo.txt"))
            {
                String? line;
                while ((line = logoFile.ReadLine()) != null) Console.WriteLine(line);
            }
        }
        catch (Exception)
        {
            string titleLogo = "Console Music Player";
            Console.SetCursorPosition(widthOfWindow1_2 - (titleLogo.Length / 2), 14);
            Console.Write(titleLogo);
        }
        string pressButton = "<<< Press Any Button To Continue >>>";
        Console.SetCursorPosition(widthOfWindow1_2 - (pressButton.Length / 2), 24);
        Console.Write(pressButton);
        Console.ReadKey();
        Console.Clear();
    }
    private void InitMusicConsole()
    {
        DrawBaseConsole();
        DrawConsole();
        UpdatePath();
        UpdateFolders();
        UpdateFiles();
        UpdateVolume();
    }
    private void DrawBaseConsole()
    {
        FrameDrawer(0, 0, widthOfWindow1_4_m1, heightOfWindow3_4_m1);
        FrameDrawer(widthOfWindow1_4, 0, widthOfWindow1_2_m1, heightOfWindow3_4_m1);
        FrameDrawer(widthOfWindow3_4, 0, widthOfWindow1_4_m1, heightOfWindow3_4_m1);
        FrameDrawer(0, heightOfWindow3_4, widthOfWindow_m1, heightOfWindow1_4_m1);
        WallDrawer(widthOfWindow1_4_m1, heightOfWindow3_4, heightOfWindow1_4_m1);
        WallDrawer(widthOfWindow3_4, heightOfWindow3_4, heightOfWindow1_4_m1);
        LineDrawer(0, 2, widthOfWindow1_4_m1);
        LineDrawer(0, 6, widthOfWindow1_4_m1);
        LineDrawer(0, 8, widthOfWindow1_4_m1);
        LineDrawer(widthOfWindow1_4, 2, widthOfWindow1_2_m1);
        LineDrawer(widthOfWindow1_4, heightOfWindow3_4_m3, widthOfWindow1_2_m1);
        LineDrawer(widthOfWindow3_4, 2, widthOfWindow1_4_m1);
        LineDrawer(0, heightOfWindow3_4_p2, widthOfWindow1_4_m1);
        LineDrawer(widthOfWindow3_4, heightOfWindow3_4_p2, widthOfWindow1_4_m1);
        LineDrawer(widthOfWindow3_4, heightOfWindow3_4_p4, widthOfWindow1_4_m1);
        LineDrawer(widthOfWindow1_4_m1, heightOfWindow3_4_p7, widthOfWindow1_2_p1);
        WallDrawer(widthOfWindow1_2_m1, heightOfWindow3_4_p7, 2);
        LineDrawerConnected(widthOfWindow1_4_m1, heightOfWindow3_4_p2, widthOfWindow1_2_p1);
        LineDrawerRightConnected(widthOfWindow1_4_m1, heightOfWindow3_4_p4, widthOfWindow1_2_p1);
        StringWriter("PATH", widthOfWindow1_8 - 2, 1, widthOfWindow1_4_m2, 1, false);
        StringWriter("OTHER LOCATIONS", widthOfWindow1_8 - 8, 7, widthOfWindow1_4_m2, 1, false);
        StringWriter("Choose first track...", widthOfWindow1_2_m11, 1, widthOfWindow1_2_m1, 1, false);
        StringWriter("FILES", widthOfWindow_m_widthOfWindow1_8_m3, 1, widthOfWindow1_4_m2, 1, false);
        StringWriter("FOLDER & FILE CONTROLS", widthOfWindow1_8 - 11, heightOfWindow3_4_p1, widthOfWindow1_4_m2, 1, false);
        StringWriter("Arrow Up    | Folder Up", 2, heightOfWindow3_4_p3, widthOfWindow1_4_m2, 1, false);
        StringWriter("Arrow Down  | Folder Down", 2, heightOfWindow3_4_p4, widthOfWindow1_4_m2, 1, false);
        StringWriter("Arrow Left  | File Up", 2, heightOfWindow3_4_p5, widthOfWindow1_4_m2, 1, false);
        StringWriter("Arrow Right | File Down", 2, heightOfWindow3_4_p6, widthOfWindow1_4_m2, 1, false);
        StringWriter("Enter       | Choose Folder", 2, heightOfWindow3_4_p7, widthOfWindow1_4_m2, 1, false);
        StringWriter("Space       | Choose File", 2, heightOfWindow3_4_p8, widthOfWindow1_4_m2, 1, false);
        StringWriter("TRACK CONTROLS", widthOfWindow1_2 - 7, heightOfWindow3_4_p1, widthOfWindow1_2_m1, 1, false);
        StringWriter("(F5) Previous | (F6) Back | (F7) Pause Play | (F8) Forwad | (F9) Next", widthOfWindow1_2 - 35, heightOfWindow3_4_p3, widthOfWindow1_2_m1, 1, false);
        StringWriter("♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪", widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 1, false);
        StringWriter("♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫", widthOfWindow1_4, heightOfWindow3_4_p6, widthOfWindow1_2, 1, false);
        StringWriter("(I) More Information", widthOfWindow1_4 + widthOfWindow1_8 - 10, heightOfWindow3_4_p8, widthOfWindow1_4, 1, false);
        StringWriter("(ESC) Turn Off", widthOfWindow1_2 + widthOfWindow1_8 - 7, heightOfWindow3_4_p8, widthOfWindow1_4, 1, false);
        StringWriter("VOLUME", widthOfWindow_m_widthOfWindow1_8_m3, heightOfWindow3_4_p1, widthOfWindow1_4_m2, 1, false);
        StringWriter("(F1) Mute", widthOfWindow3_4_p2, heightOfWindow3_4_p5, widthOfWindow1_4_m2, 1, false);
        StringWriter("(F2) Volume Down", widthOfWindow3_4_p2, heightOfWindow3_4_p6, widthOfWindow1_4_m2, 1, false);
        StringWriter("(F3) Volume Up", widthOfWindow3_4_p2, heightOfWindow3_4_p7, widthOfWindow1_4_m2, 1, false);
        StringWriter("(F4) Unmute", widthOfWindow3_4_p2, heightOfWindow3_4_p8, widthOfWindow1_4_m2, 1, false);
    }
    private void DrawConsole()
    {
        for (int i = 0; i < heightOfWindow_m1; i++)
        {
            for (int j = 0; j < widthOfWindow; j++) Console.Write(console[j, i]);
        }
        for (int i = 0; i < widthOfWindow_m1; i++) Console.Write(console[i, 39]);
        Console.SetCursorPosition(widthOfWindow_m2, heightOfWindow_m1);
        Console.Write(walls[5]);
        Console.MoveBufferArea(widthOfWindow_m2, heightOfWindow_m1, 1, 1, widthOfWindow_m1, heightOfWindow_m1, walls[0], Console.ForegroundColor, Console.BackgroundColor);
    }
    public void UpdatePath()
    {
        string newPath = directoryFileManager.Path;
        if (!(newPath[newPath.Length - 1] == '\\')) newPath += "\\";
        StringWriter(newPath, 1, 3, widthOfWindow1_4_m2, 3, true);
        UpdateConsoleBlock(1, 3, widthOfWindow1_4_m2, 3);
    }
    public void UpdateFolders()
    {
        ListWriter(directoryFileManager.ArrayOfFolders.Skip(currentFolderIdx / heightOfWindow3_4_m12 * heightOfWindow3_4_m12).Take(heightOfWindow3_4_m12).ToArray(), 1, 9, widthOfWindow1_4_m2, heightOfWindow3_4_m10, true);
        UpdateConsoleBlockWithHighlights(1, 9, widthOfWindow1_4_m2, heightOfWindow3_4_m10, (currentFolderIdx % heightOfWindow3_4_m12), null);
    }
    public void UpdateFiles()
    {
        int skipFilesMultiplication = cursorFileIdx / heightOfWindow3_4_m6 * heightOfWindow3_4_m6;
        ListWriter(directoryFileManager.ArrayOfFiles.Skip(skipFilesMultiplication).Take(heightOfWindow3_4_m6).ToArray(), widthOfWindow3_4_p1, 3, widthOfWindow1_4_m2, heightOfWindow3_4_m4, false);
        UpdateConsoleBlockWithHighlights(widthOfWindow3_4_p1, 3, widthOfWindow1_4_m2, heightOfWindow3_4_m4, (cursorFileIdx % heightOfWindow3_4_m6), (currentFileIdx >= skipFilesMultiplication && currentFileIdx <= skipFilesMultiplication + heightOfWindow3_4_m7) ? (currentFileIdx % heightOfWindow3_4_m6) : null);
    }
    private void UpdateVolume()
    {
        int volumeDots = musicPlayer.PlayerVolume / 10;
        string volumeDisplay = " Value: [";
        if (musicPlayer.IsPlayerMute) volumeDisplay += " Muted  ";
        else
        {
            for (int i = 0; i < volumeDots; i++) volumeDisplay += dots[0] + " ";
            for (int i = 0; i < 10 - volumeDots; i++) volumeDisplay += dots[1] + " ";
        }
        volumeDisplay = volumeDisplay.Remove(volumeDisplay.Length - 1) + "]  " + (musicPlayer.IsPlayerMute ? 0 : musicPlayer.PlayerVolume) + " %";
        if (volumeDisplay.Length < widthOfWindow1_4_m2)
        {
            int countOfWhiteSpaces = widthOfWindow1_4_m2 - volumeDisplay.Length;
            for (int i = 0; i < countOfWhiteSpaces; i++) volumeDisplay = volumeDisplay + " ";
        }
        lock (console)
        {
            Console.SetCursorPosition(widthOfWindow3_4_p1, heightOfWindow3_4_p3);
            Console.Write(volumeDisplay);
        }
    }
    public void UpdateTrack()
    {
        string trackToDisplay = "Choose new track...";
        if (currentFileIdx != -2)
        {
            trackToDisplay = directoryFileManager.ArrayOfFiles[currentFileIdx].Remove(directoryFileManager.ArrayOfFiles[currentFileIdx].Length - 4);
            if (trackToDisplay.Length > widthOfWindow1_2_m4) trackToDisplay = trackToDisplay.Substring(0, widthOfWindow1_2_m8) + " ...";
        }
        if (trackToDisplay.Length <= widthOfWindow1_2_m4)
        {
            int countOfWhiteSpaces = widthOfWindow1_2_m2 - trackToDisplay.Length;
            if (countOfWhiteSpaces % 2 == 0) countOfWhiteSpaces = (countOfWhiteSpaces / 2) - 1;
            else countOfWhiteSpaces = countOfWhiteSpaces / 2;
            for (int i = 0; i < countOfWhiteSpaces; i++) trackToDisplay = " " + trackToDisplay + " ";
        }
        lock (console)
        {
            Console.SetCursorPosition(widthOfWindow1_4_p2, 1);
            Console.Write(trackToDisplay);
        }
    }
    private void UpdateTimer()
    {
        while (launchApp)
        {
            try
            {
                lock (console)
                {
                    Console.SetCursorPosition(widthOfWindow1_2_m9, heightOfWindow3_4_m2);
                    Console.Write("<<< " + musicPlayer.TrackCurrentDuration + " | " + musicPlayer.TrackDuration + " >>>");
                }
                Thread.Sleep(200);
            }
            catch (Exception) { }
        }
    }
    private void UpdateAnimation()
    {
        Random rnd = new Random();
        string infoText = "PAUSED: Resume Or Select New Track";
        int z, endWidthOfAnimation = widthOfWindow1_4_p2 + widthOfWindow1_2_m4;
        int infoTextStartX = widthOfWindow1_2 - (infoText.Length / 2), infoTextStartY = heightOfWindow3_4 / 2;
        bool changedMode = true;
        while (launchApp)
        {
            try
            {
                if (nowPlaying)
                {
                    changedMode = true;
                    for (int i = widthOfWindow1_4_p2; i < endWidthOfAnimation; i++)
                    {
                        z = rnd.Next(8, heightOfWindow3_4_m1);
                        for (int j = heightOfWindow3_4_m4; j > 5; j--)
                        {
                            if (j > z) console[i, j] = dots[0];
                            else console[i, j] = ' ';
                        }
                    }
                    UpdateConsoleBlock(widthOfWindow1_4_p2, 6, widthOfWindow1_2_m4, heightOfWindow3_4_m9);
                }
                else if (changedMode)
                {
                    changedMode = false;
                    BoxClear(widthOfWindow1_4_p2, 6, widthOfWindow1_2_m4, heightOfWindow3_4_m9);
                    StringWriter(infoText, infoTextStartX, infoTextStartY, widthOfWindow1_2_m2, 1, false);
                    UpdateConsoleBlock(widthOfWindow1_4_p2, 6, widthOfWindow1_2_m4, heightOfWindow3_4_m9);
                }
                Thread.Sleep(200);
            }
            catch (Exception) { }
        }
    }
    private void UpdateAnimation2()
    {
        Random rnd = new Random();
        int col;
        while (launchApp)
        {
            try
            {
                if (nowPlaying)
                {
                    col = rnd.Next(0, 10) + widthOfWindow1_4_p1;
                    while (col <= widthOfWindow3_4_m2)
                    {
                        console[col, heightOfWindow3_4_p5] = notes[col % 2];
                        col += rnd.Next(0, 10);
                    }
                    col = rnd.Next(0, 10) + widthOfWindow1_4_p1;
                    while (col <= widthOfWindow3_4_m2)
                    {
                        console[col, heightOfWindow3_4_p6] = notes[col % 2];
                        col += rnd.Next(0, 10);
                    }
                    UpdateConsoleBlock(widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 2);
                    BoxClear(widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 2);
                }
                Thread.Sleep(200);
            }
            catch (Exception) { }
        }
    }
    private void BoxClear(int startX, int startY, int lengthOfBlock, int heightOfBlock)
    {
        for (int i = startY; i < heightOfBlock + startY; i++)
        {
            for (int j = startX; j < lengthOfBlock + startX; j++) console[j, i] = ' ';
        }
    }
    private void LineDrawer(int startX, int startY, int countX)
    {
        for (int i = startX; i < countX + startX; i++) console[i, startY] = walls[0];
        console[startX, startY] = walls[6];
        console[startX + countX, startY] = walls[7];
    }
    private void LineDrawerConnected(int startX, int startY, int countX)
    {
        for (int i = startX; i < countX + startX; i++) console[i, startY] = walls[0];
        console[startX, startY] = walls[10];
        console[startX + countX, startY] = walls[10];
    }
    private void LineDrawerRightConnected(int startX, int startY, int countX)
    {
        for (int i = startX; i < countX + startX; i++) console[i, startY] = walls[0];
        console[startX, startY] = walls[6];
        console[startX + countX, startY] = walls[10];
    }
    private void WallDrawer(int startX, int startY, int countY)
    {
        for (int i = startY; i < countY + startY; i++) console[startX, i] = walls[1];
        console[startX, startY] = walls[8];
        console[startX, startY + countY] = walls[9];
    }
    private void FrameDrawer(int startX, int startY, int countX, int countY)
    {
        int sXcX = startX + countX, sYcY = startY + countY;
        for (int i = startX; i < countX + startX; i++)
        {
            console[i, startY] = walls[0];
            console[i, sYcY] = walls[0];
        }
        for (int i = startY; i < countY + startY; i++)
        {
            console[startX, i] = walls[1];
            console[sXcX, i] = walls[1];
        }
        console[startX, startY] = walls[2];
        console[startX, sYcY] = walls[4];
        console[sXcX, startY] = walls[3];
        console[sXcX, sYcY] = walls[5];
    }
    private void StringWriter(string text, int startX, int startY, int lengthOfBlock, int heightOfBlock, bool? clearBox)
    {
        if (clearBox == true) BoxClear(startX, startY, lengthOfBlock, heightOfBlock);
        int tempXPosition = startX, tempLengthOfBlock = lengthOfBlock;
        int blockMultiplication = heightOfBlock * lengthOfBlock;
        if (blockMultiplication < text.Length) text = "... " + text.Substring(text.Length - blockMultiplication + 4);
        foreach (char c in text)
        {
            if (tempLengthOfBlock > 0)
            {
                console[tempXPosition, startY] = c;
                tempLengthOfBlock--;
                tempXPosition++;
            }
            else
            {
                tempLengthOfBlock = lengthOfBlock;
                tempXPosition = startX;
                startY++;
                console[tempXPosition, startY] = c;
                tempLengthOfBlock--;
                tempXPosition++;
            }
        }
    }
    private void ListWriter(string[] text, int startX, int startY, int lengthOfBlock, int heightOfBlock, bool truncateEnd)
    {
        BoxClear(startX, startY, lengthOfBlock, heightOfBlock);
        int tempXPosition = startX;
        heightOfBlock -= 2;
        int lsX = lengthOfBlock + startX;
        for (int i = startX; i < lsX; i++) console[i, startY] = arrows[0];
        startY++;
        for (int i = 0; i < text.Length; i++)
        {
            if (lengthOfBlock < text[i].Length)
            {
                if (truncateEnd) text[i] = text[i].Substring(0, lengthOfBlock - 4) + " ...";
                else text[i] = "... " + text[i].Substring(text[i].Length - lengthOfBlock + 4);
            }
            foreach (char c in text[i])
            {
                console[tempXPosition, startY] = c;
                tempXPosition++;
            }
            tempXPosition = startX;
            startY++;
            heightOfBlock--;
            if (heightOfBlock == 0) break;
        }
        if (heightOfBlock != 0) startY += heightOfBlock;
        for (int i = startX; i < lsX; i++) console[i, startY] = arrows[1];
    }
    private void UpdateConsoleBlock(int startX, int startY, int lengthOfBlock, int heightOfBlock)
    {
        lock (console)
        {
            for (int i = startY; i < heightOfBlock + startY; i++)
            {
                for (int j = startX; j < lengthOfBlock + startX; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(console[j, i]);
                }
            }
        }
    }
    private void UpdateConsoleBlockWithHighlights(int startX, int startY, int lengthOfBlock, int heightOfBlock, int? selectedItem, int? selectedItem2)
    {
        lock (console)
        {
            int row;
            selectedItem++;
            selectedItem2++;
            for (int i = startY; i < heightOfBlock + startY; i++)
            {
                row = i - startY;
                if (row == selectedItem2)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (row == selectedItem)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                for (int j = startX; j < lengthOfBlock + startX; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(console[j, i]);
                }
                if (selectedItem != null || selectedItem2 != null)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}