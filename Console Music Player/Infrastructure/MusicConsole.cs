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
    private int[] consoleAnimation;
    private char[] walls = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
    private char[] arrows = { '▲', '▼' };
    private char[] dots = { '●', '○' };
    private char[] notes = { '♪', '♫' };
    private readonly int widthOfWindow, widthOfWindow_m1, widthOfWindow_m2;
    private readonly int widthOfWindow1_8;
    private readonly int widthOfWindow1_4, widthOfWindow1_4_m1, widthOfWindow1_4_m2, widthOfWindow1_4_p1, widthOfWindow1_4_p2;
    private readonly int widthOfWindow1_2, widthOfWindow1_2_m1, widthOfWindow1_2_m2, widthOfWindow1_2_m4, widthOfWindow1_2_m11, widthOfWindow1_2_p1;
    private readonly int widthOfWindow3_4, widthOfWindow3_4_m2, widthOfWindow3_4_p1, widthOfWindow3_4_p2;
    private readonly int widthOfWindow_m_widthOfWindow1_8_m3;
    private readonly int heightOfWindow, heightOfWindow_m1;
    private readonly int heightOfWindow1_4, heightOfWindow1_4_m1;
    private readonly int heightOfWindow3_4, heightOfWindow3_4_m1, heightOfWindow3_4_m2, heightOfWindow3_4_m3, heightOfWindow3_4_m4, heightOfWindow3_4_m6;
    private readonly int heightOfWindow3_4_m9, heightOfWindow3_4_m10, heightOfWindow3_4_m12, heightOfWindow3_4_p1, heightOfWindow3_4_p2, heightOfWindow3_4_p3;
    private readonly int heightOfWindow3_4_p4, heightOfWindow3_4_p5, heightOfWindow3_4_p6, heightOfWindow3_4_p7, heightOfWindow3_4_p8;
    private int currentFolderIdx, cursorFileIdx;
    private bool launchApp;
    public DirectoryFileManager DirectoryFileManager => directoryFileManager;
    public int currentFileIdx;
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
        catch (System.ArgumentOutOfRangeException e)
        {
            Logger.SaveLog(e.Message);
            System.Environment.Exit(-1);
        }
        try
        {
            using (StreamReader file = new(AppContext.BaseDirectory + "/settings"))
            {
                directoryFileManager = new DirectoryFileManager(file.ReadLine());
                short tempVolume;
                if (short.TryParse(file.ReadLine(), out tempVolume)) musicPlayer = new MusicPlayer(this, tempVolume);
                else musicPlayer = new MusicPlayer(this);
                file.Close();
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
        widthOfWindow1_2_m4 = widthOfWindow1_2 - 4;
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
        consoleAnimation = new int[widthOfWindow1_2_m4];
        currentFolderIdx = 0;
        currentFileIdx = -2;
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
                    break;
                case ConsoleKey.DownArrow:
                    if (currentFolderIdx < directoryFileManager.CountOfFolders - 1)
                    {
                        currentFolderIdx++;
                        UpdateFolders();
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (cursorFileIdx > 0 && cursorFileIdx != -2)
                    {
                        cursorFileIdx--;
                        UpdateFiles();
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (cursorFileIdx < directoryFileManager.CountOfFiles - 1 && cursorFileIdx != -2)
                    {
                        cursorFileIdx++;
                        UpdateFiles();
                    }
                    break;
                case ConsoleKey.Enter:
                    directoryFileManager.ChangeFolder(directoryFileManager.ArrayOfFolders[currentFolderIdx]);
                    currentFolderIdx = 0;
                    currentFileIdx = -2;
                    if (directoryFileManager.CountOfFiles > 0) cursorFileIdx = 0;
                    else cursorFileIdx = -2;
                    UpdatePath();
                    UpdateFolders();
                    UpdateFiles();
                    break;
                case ConsoleKey.Spacebar:
                    currentFileIdx = cursorFileIdx;
                    nowPlaying = true;
                    musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    UpdateFiles();
                    UpdateTrack();
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
                    directoryFileManager.Refresh();
                    if (directoryFileManager.CountOfFiles == 0)
                    {
                        musicPlayer.Pause();
                        nowPlaying = false;
                        continue;
                    }
                    if (!musicPlayer.IsPlayerJustStarted)
                    {
                        musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                        continue;
                    }
                    currentFileIdx = (currentFileIdx - 1) < 0 ? directoryFileManager.CountOfFiles - 1 : --currentFileIdx;
                    UpdateFiles();
                    musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    UpdateTrack();
                    break;
                case ConsoleKey.F6:
                    musicPlayer.RewindTrack(-10);
                    break;
                case ConsoleKey.F7:
                    if (nowPlaying) musicPlayer.Pause();
                    else musicPlayer.Play();
                    nowPlaying = !nowPlaying;
                    break;
                case ConsoleKey.F8:
                    musicPlayer.RewindTrack(10);
                    break;
                case ConsoleKey.F9:
                    if (currentFileIdx == -2) continue;
                    directoryFileManager.Refresh();
                    if (directoryFileManager.CountOfFiles == 0)
                    {
                        musicPlayer.Pause();
                        nowPlaying = false;
                        continue;
                    }
                    currentFileIdx = (currentFileIdx + 1) < directoryFileManager.CountOfFiles ? ++currentFileIdx : 0;
                    UpdateFiles();
                    musicPlayer.Start(directoryFileManager.ArrayOfFiles[currentFileIdx]);
                    UpdateTrack();
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
                        using (StreamWriter file = new(AppContext.BaseDirectory + "/settings", false))
                        {
                            file.WriteLine(directoryFileManager.Path);
                            file.WriteLine(musicPlayer.PlayerVolume.ToString());
                            file.Close();
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
            using (StreamReader sr = new(AppContext.BaseDirectory + "/Logo.txt"))
            {
                String? line;
                while ((line = sr.ReadLine()) != null) Console.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Logger.SaveLog(e.Message);
            for (int i = 0; i < widthOfWindow1_2 - 10; i++) Console.Write(" ");
            Console.Write("Console Music Player");
        }
        Console.Write("\n\n\n");
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
        UpdateAnimationPassive();
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
        StringWriter("Choose first track...", widthOfWindow1_2_m11, heightOfWindow3_4_m2, widthOfWindow1_2_m1, 1, false);
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
    private void UpdatePath()
    {
        string newPath = directoryFileManager.Path;
        if (!(newPath[newPath.Length - 1] == '\\')) newPath += "\\";
        StringWriter(newPath, 1, 3, widthOfWindow1_4_m2, 3, true);
        UpdateConsoleBlock(1, 3, widthOfWindow1_4_m2, 3, null, null);
    }
    private void UpdateFolders()
    {
        int skipFolders = currentFolderIdx / heightOfWindow3_4_m12;
        ListWriter(directoryFileManager.ArrayOfFolders.Skip(skipFolders * heightOfWindow3_4_m12).Take(heightOfWindow3_4_m12).ToArray(), 1, 9, widthOfWindow1_4_m2, heightOfWindow3_4_m10, true);
        UpdateConsoleBlock(1, 9, widthOfWindow1_4_m2, heightOfWindow3_4_m10, (currentFolderIdx % heightOfWindow3_4_m12), null);
    }
    public void UpdateFiles()
    {
        int skipFiles = cursorFileIdx / heightOfWindow3_4_m6;
        int skipFilesMultiplication = skipFiles * heightOfWindow3_4_m6;
        ListWriter(directoryFileManager.ArrayOfFiles.Skip(skipFilesMultiplication).Take(heightOfWindow3_4_m6).ToArray(), widthOfWindow3_4_p1, 3, widthOfWindow1_4_m2, heightOfWindow3_4_m4, false);
        UpdateConsoleBlock(widthOfWindow3_4_p1, 3, widthOfWindow1_4_m2, heightOfWindow3_4_m4, (cursorFileIdx % heightOfWindow3_4_m6), (currentFileIdx >= skipFilesMultiplication && currentFileIdx <= skipFilesMultiplication + heightOfWindow3_4 - 7) ? (currentFileIdx % heightOfWindow3_4_m6) : null);
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
        StringWriter(volumeDisplay, widthOfWindow3_4_p1, heightOfWindow3_4_p3, widthOfWindow1_4_m2, 1, true);
        UpdateConsoleBlock(widthOfWindow3_4_p1, heightOfWindow3_4_p3, widthOfWindow1_4_m2, 1, null, null);
    }
    public void UpdateTrack()
    {
        string trackToDisplay = directoryFileManager.ArrayOfFiles[currentFileIdx].Remove(directoryFileManager.ArrayOfFiles[currentFileIdx].Length - 4);
        if (trackToDisplay.Length > widthOfWindow1_2 - 3) trackToDisplay = trackToDisplay.Substring(0, widthOfWindow1_2 - 8) + " ...";
        BoxClear(widthOfWindow1_4_p1, 1, widthOfWindow1_2_m1, 1);
        StringWriter(trackToDisplay, widthOfWindow1_2 - (trackToDisplay.Length / 2), 1, widthOfWindow1_2_m1, 1, false);
        UpdateConsoleBlock(widthOfWindow1_4, 1, widthOfWindow1_2_m1, 1, null, null);
    }
    private void UpdateTimer()
    {
        while (launchApp)
        {
            {
                BoxClear(widthOfWindow1_4_p1, heightOfWindow3_4_m2, widthOfWindow1_2_m2, 1);
                string outText = "<<< " + (musicPlayer.TrackCurrentDuration != "" ? musicPlayer.TrackCurrentDuration : "--:--") + " | " + musicPlayer.TrackDuration + " >>>";
                StringWriter(outText, widthOfWindow1_2 - (outText.Length / 2), heightOfWindow3_4_m2, widthOfWindow1_2_m1, 1, false);
                UpdateConsoleBlock(widthOfWindow1_4_p1, heightOfWindow3_4_m2, widthOfWindow1_2_m2, 1, null, null);
                Thread.Sleep(200);
            }
        }
    }
    private void UpdateAnimationPassive()
    {
        BoxClear(widthOfWindow1_4_p1, 3, widthOfWindow1_2_m2, heightOfWindow3_4_m3);
        string infoText = "PAUSED: Resume Or Select New Track";
        StringWriter(infoText, widthOfWindow1_2 - (infoText.Length / 2), heightOfWindow3_4 / 2, widthOfWindow1_2_m2, 1, false);
        UpdateConsoleBlock(widthOfWindow1_4_p1, 0, widthOfWindow1_2_m2, heightOfWindow3_4_m3, null, null);
    }
    private void UpdateAnimation()
    {
        Random rnd = new Random();
        int rndNumber = rnd.Next(0, heightOfWindow3_4_m9);
        while (launchApp)
        {
            if (nowPlaying)
            {
                for (int i = 0; i < consoleAnimation.Length; i++)
                {
                    consoleAnimation[i] = rndNumber;
                    rndNumber = rnd.Next(0, heightOfWindow3_4_m9);
                }
                int z = 0;
                for (int i = widthOfWindow1_4_p2; i < widthOfWindow1_4_p2 + widthOfWindow1_2_m4; i++)
                {
                    for (int j = heightOfWindow3_4_m4; j > 5; j--)
                    {
                        if (j > 8 + consoleAnimation[z]) console[i, j] = dots[0];
                        else console[i, j] = ' ';
                    }
                    z++;
                }
                UpdateConsoleBlock(widthOfWindow1_4_p2, 6, widthOfWindow1_2_m4, heightOfWindow3_4_m9, null, null);
            }
            else UpdateAnimationPassive();
            Thread.Sleep(200);
        }
    }
    private void UpdateAnimation2()
    {
        Random rnd = new Random();
        BoxClear(widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 2);
        while (launchApp)
        {
            if (nowPlaying)
            {
                int col = rnd.Next(0, 10) + widthOfWindow1_4_p1;
                int row = heightOfWindow3_4_p5;
                while (col <= widthOfWindow3_4_m2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                col = rnd.Next(0, 10) + widthOfWindow1_4_p1;
                row++;
                while (col <= widthOfWindow3_4_m2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                UpdateConsoleBlock(widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 2, null, null);
                BoxClear(widthOfWindow1_4, heightOfWindow3_4_p5, widthOfWindow1_2, 2);
            }
            Thread.Sleep(200);
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
        string textCopied = text;
        if (clearBox != null && clearBox == true) BoxClear(startX, startY, lengthOfBlock, heightOfBlock);
        int tempXPosition = startX, tempLengthOfBlock = lengthOfBlock;
        int blockMultiplication = heightOfBlock * lengthOfBlock;
        if (blockMultiplication < textCopied.Length) textCopied = "... " + textCopied.Substring(textCopied.Length - blockMultiplication + 4);
        foreach (char c in textCopied)
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
        string[] copiedText = new string[text.Length];
        text.CopyTo(copiedText, 0);
        BoxClear(startX, startY, lengthOfBlock, heightOfBlock);
        int tempXPosition = startX;
        heightOfBlock -= 2;
        int lsX = lengthOfBlock + startX;
        for (int i = startX; i < lsX; i++) console[i, startY] = arrows[0];
        startY++;
        for (int i = 0; i < copiedText.Length; i++)
        {
            if (lengthOfBlock < copiedText[i].Length)
            {
                if (truncateEnd) copiedText[i] = copiedText[i].Substring(0, lengthOfBlock - 4) + " ...";
                else copiedText[i] = "... " + copiedText[i].Substring(copiedText[i].Length - lengthOfBlock + 4);
            }
            foreach (char c in copiedText[i])
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
    private void UpdateConsoleBlock(int startX, int startY, int lengthOfBlock, int heightOfBlock, int? selectedItem, int? selectedItem2)
    {
        lock (console)
        {
            for (int i = startY; i < heightOfBlock + startY; i++)
            {
                if (i - startY == selectedItem2 + 1)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (i - startY == selectedItem + 1)
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