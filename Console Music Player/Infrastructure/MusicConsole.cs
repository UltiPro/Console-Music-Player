using System.Diagnostics;
using DirectoryFileManagerTool;
using MusicPlayerTool;
using LoggerTool;

namespace MusicConsoleTool;

class MusicConsole
{
    private readonly int widthOfWindow, widthOfWindow3_4, widthOfWindow2_4, widthOfWindow1_4, widthOfWindow1_8;
    private readonly int heightOfWindow, heightOfWindow3_4, heightOfWindow1_4;
    private char[] walls = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
    private char[] arrows = { '▲', '▼' };
    private char[] volumeDots = { '●', '○' };
    private char[] notes = { '♪', '♫' };
    private char[,] console;
    private int[] consoleAnimation;
    private int currentFolderIdx;
    private bool initThreads, launchApp;
    public bool nowPlaying;
    private DirectoryFileManager DFM;
    private MusicPlayer MP;
    private Thread? threadTimer, threadAnimation, threadAnimation2;
    public int currentFileIdx, cursorFileIdx;
    public DirectoryFileManager Dfm => DFM;
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
            Console.SetWindowSize(console.GetLength(0), console.GetLength(1));
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
        catch (System.ArgumentOutOfRangeException e)
        {
            Logger.SaveLog(e.ToString());
        }
        widthOfWindow = Console.WindowWidth;
        widthOfWindow1_8 = Console.WindowWidth / 8;
        widthOfWindow1_4 = Console.WindowWidth / 4;
        widthOfWindow2_4 = Console.WindowWidth / 2;
        widthOfWindow3_4 = Console.WindowWidth / 4 * 3;
        heightOfWindow = Console.WindowHeight;
        heightOfWindow1_4 = Console.WindowHeight / 4;
        heightOfWindow3_4 = Console.WindowHeight / 4 * 3;
        initThreads = true;
        nowPlaying = launchApp = false;
        consoleAnimation = new int[widthOfWindow2_4 - 4];
        string tempPath = "";
        short tempVolume = 50;
        try
        {
            using (StreamReader file = new(AppContext.BaseDirectory + "/settings"))
            {
                string? output = file.ReadLine();
                if (output != null) tempPath = output;
                output = file.ReadLine();
                if (!short.TryParse(output, out tempVolume)) throw new FormatException("Settings has invalid format of data.");
                file.Close();
            }
        }
        catch (Exception e)
        {
            tempVolume = 50;
            tempPath = "";
            Logger.SaveLog(e.ToString());
        }
        DFM = new DirectoryFileManager(tempPath);
        MP = new MusicPlayer(this, tempVolume);
        currentFolderIdx = 0;
        currentFileIdx = -2;
        if (DFM.CountOfFiles > 0) cursorFileIdx = 0;
        else cursorFileIdx = -2;
        threadTimer = new Thread(() => UpdateTimer());
        threadAnimation = new Thread(() => UpdateAnimation());
        threadAnimation2 = new Thread(() => UpdateAnimation2());
    }
    public void Start()
    {
        StartMenu();
        InitMusicConsole();
        launchApp = true;
        ConsoleKeyInfo inputFromKeyboard;
        while (launchApp)
        {
            inputFromKeyboard = Console.ReadKey(true);
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
                    if (currentFolderIdx < DFM.CountOfFolders - 1)
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
                    if (cursorFileIdx < DFM.CountOfFiles - 1 && cursorFileIdx != -2)
                    {
                        cursorFileIdx++;
                        UpdateFiles();
                    }
                    break;
                case ConsoleKey.Enter:
                    DFM.ChangeFolder(DFM.ArrayOfFolders[currentFolderIdx]);
                    currentFolderIdx = 0;
                    currentFileIdx = -2;
                    if (DFM.CountOfFiles > 0) cursorFileIdx = 0;
                    else cursorFileIdx = -2;
                    UpdatePath();
                    UpdateFolders();
                    UpdateFiles();
                    break;
                case ConsoleKey.Spacebar:
                    try
                    {
                        currentFileIdx = cursorFileIdx;
                        MP.Start(DFM.ArrayOfFiles[currentFileIdx]);
                        UpdateTrack();
                        if (threadTimer != null && threadAnimation != null && threadAnimation2 != null && initThreads)
                        {
                            threadTimer.Start();
                            threadAnimation.Start();
                            threadAnimation2.Start();
                            initThreads = false;
                            nowPlaying = true;
                        }
                        else nowPlaying = true;
                        DFM.Refresh();
                        UpdateFiles();
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.Message);
                    }
                    break;
                case ConsoleKey.F1:
                    MP.MuteUnMute(true);
                    UpdateVolume(MP.VolumePlayer);
                    break;
                case ConsoleKey.F2:
                    if (MP.VolumePlayer > 0)
                    {
                        MP.ChangeVolume(-1);
                        UpdateVolume(MP.VolumePlayer);
                    }
                    break;
                case ConsoleKey.F3:
                    if (MP.VolumePlayer < 100)
                    {
                        MP.ChangeVolume(1);
                        UpdateVolume(MP.VolumePlayer);
                    }
                    break;
                case ConsoleKey.F4:
                    MP.MuteUnMute(false);
                    UpdateVolume(MP.VolumePlayer);
                    break;
                case ConsoleKey.F5:
                    if (currentFileIdx == -2) continue;
                    DFM.Refresh();
                    if (DFM.CountOfFiles == 0)
                    {
                        MP.Pause();
                        nowPlaying = false;
                        continue;
                    }
                    if (!MP.IsJustStarted)
                    {
                        MP.Start(DFM.ArrayOfFiles[currentFileIdx]);
                        continue;
                    }
                    currentFileIdx = (currentFileIdx - 1) < 0 ? DFM.CountOfFiles - 1 : --currentFileIdx;
                    UpdateFiles();
                    MP.Start(DFM.ArrayOfFiles[currentFileIdx]);
                    UpdateTrack();
                    break;
                case ConsoleKey.F6:
                    MP.SkipTrack(-10);
                    break;
                case ConsoleKey.F7:
                    if (nowPlaying) MP.Pause();
                    else MP.Resume();
                    nowPlaying = !nowPlaying;
                    break;
                case ConsoleKey.F8:
                    MP.SkipTrack(10);
                    break;
                case ConsoleKey.F9:
                    if (currentFileIdx == -2) continue;
                    DFM.Refresh();
                    if (DFM.CountOfFiles == 0)
                    {
                        MP.Pause();
                        nowPlaying = false;
                        continue;
                    }
                    currentFileIdx = (currentFileIdx + 1) < DFM.CountOfFiles ? ++currentFileIdx : 0;
                    UpdateFiles();
                    MP.Start(DFM.ArrayOfFiles[currentFileIdx]);
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
                            file.WriteLine(DFM.Path);
                            file.WriteLine(MP.VolumePlayer.ToString());
                            file.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.ToString());
                    }
                    launchApp = false;
                    Thread.Sleep(500);
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
            for (int i = 0; i < widthOfWindow2_4 - 10; i++) Console.Write(" ");
            Console.Write("Console Music Player");
            Logger.SaveLog(e.Message);
        }
        Console.Write("\n\n\n");
        string pressButton = "<<< Press Any Button To Continue >>>";
        Console.SetCursorPosition(widthOfWindow2_4 - (pressButton.Length / 2), 24);
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
        UpdateVolume(MP.VolumePlayer);
        UpdateAnimationPassive();
    }
    private void DrawConsole()
    {
        for (int i = 0; i < heightOfWindow - 1; i++)
        {
            for (int j = 0; j < widthOfWindow; j++) Console.Write(console[j, i]);
        }
        for (int i = 0; i < widthOfWindow - 1; i++) Console.Write(console[i, 39]);
        Console.SetCursorPosition(widthOfWindow - 2, heightOfWindow - 1);
        Console.Write(walls[5]);
        Console.MoveBufferArea(widthOfWindow - 2, heightOfWindow - 1, 1, 1, widthOfWindow - 1, heightOfWindow - 1, walls[0], Console.ForegroundColor, Console.BackgroundColor);
    }
    private void UpdatePath()
    {
        string newPath = DFM.Path;
        if (!(newPath[newPath.Length - 1] == '\\')) newPath += "\\";
        StringWriter(newPath, 1, 3, widthOfWindow1_4 - 2, 3, true);
        UpdateConsoleBlock(1, 3, widthOfWindow1_4 - 2, 3, null, null);
    }
    private void UpdateFolders()
    {
        int skipFolders = currentFolderIdx / (heightOfWindow3_4 - 12);
        ListWriter(DFM.ArrayOfFolders.Skip(skipFolders * (heightOfWindow3_4 - 12)).Take((heightOfWindow3_4 - 12)).ToArray(), 1, 9, widthOfWindow1_4 - 2, heightOfWindow3_4 - 10, true);
        UpdateConsoleBlock(1, 9, widthOfWindow1_4 - 2, heightOfWindow3_4 - 10, (currentFolderIdx % (heightOfWindow3_4 - 12)), null);
    }
    public void UpdateTrack()
    {
        string trackToDisplay = DFM.ArrayOfFiles[currentFileIdx].Remove(DFM.ArrayOfFiles[currentFileIdx].Length - 4);
        if (trackToDisplay.Length > widthOfWindow1_4 * 2 - 3) trackToDisplay = trackToDisplay.Substring(0, widthOfWindow1_4 * 2 - 8) + " ...";
        BoxClear(widthOfWindow1_4 + 1, 1, widthOfWindow2_4 - 1, 1);
        StringWriter(trackToDisplay, widthOfWindow2_4 - (trackToDisplay.Length / 2), 1, widthOfWindow2_4 - 1, 1, false);
        UpdateConsoleBlock(widthOfWindow1_4, 1, widthOfWindow2_4 - 1, 1, null, null);
    }
    private void UpdateAnimationPassive()
    {
        BoxClear(widthOfWindow1_4 + 1, 3, widthOfWindow1_4 * 2 - 2, heightOfWindow3_4 - 3);
        string infoText = "PAUSED: Resume Or Select New Track";
        StringWriter(infoText, widthOfWindow2_4 - (infoText.Length / 2), heightOfWindow3_4 / 2, widthOfWindow2_4 - 2, 1, false);
        UpdateConsoleBlock(widthOfWindow1_4 + 1, 0, widthOfWindow1_4 * 2 - 2, heightOfWindow3_4 - 3, null, null);
    }
    private void UpdateAnimation()
    {
        Random rnd = new Random();
        int rndNumber = rnd.Next(0, heightOfWindow3_4 - 9);
        while (launchApp)
        {
            if (nowPlaying)
            {
                for (int i = 0; i < consoleAnimation.Length; i++)
                {
                    consoleAnimation[i] = rndNumber;
                    rndNumber = rnd.Next(0, heightOfWindow3_4 - 9);
                }
                int z = 0;
                for (int i = widthOfWindow1_4 + 2; i < widthOfWindow1_4 + 2 + widthOfWindow1_4 * 2 - 4; i++)
                {
                    for (int j = heightOfWindow3_4 - 4; j > 5; j--)
                    {
                        if (j > 8 + consoleAnimation[z]) console[i, j] = volumeDots[0];
                        else console[i, j] = ' ';
                    }
                    z++;
                }
                UpdateConsoleBlock(widthOfWindow1_4 + 2, 6, widthOfWindow1_4 * 2 - 4, heightOfWindow3_4 - 9, null, null);
                Thread.Sleep(100);
            }
            else
            {
                UpdateAnimationPassive();
                Thread.Sleep(100);
            }
        }
    }
    private void UpdateAnimation2()
    {
        Random rnd = new Random();
        BoxClear(widthOfWindow1_4, heightOfWindow3_4 + 5, widthOfWindow1_4 * 2, 2);
        while (launchApp)
        {
            if (nowPlaying)
            {
                int col = rnd.Next(0, 10) + widthOfWindow1_4 + 1;
                int row = heightOfWindow3_4 + 5;
                while (col <= widthOfWindow3_4 - 2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                col = rnd.Next(0, 10) + widthOfWindow1_4 + 1;
                row++;
                while (col <= widthOfWindow3_4 - 2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                UpdateConsoleBlock(widthOfWindow1_4, heightOfWindow3_4 + 5, widthOfWindow1_4 * 2, 2, null, null);
                BoxClear(widthOfWindow1_4, heightOfWindow3_4 + 5, widthOfWindow1_4 * 2, 2);
            }
            Thread.Sleep(100);
        }
    }
    private void UpdateTimer()
    {
        while (launchApp)
        {
            {
                BoxClear(widthOfWindow1_4 + 1, heightOfWindow3_4 - 2, widthOfWindow2_4 - 2, 1);
                string outText = "<<< " + (MP.CurrentTrackDuration != "" ? MP.CurrentTrackDuration : "--:--") + " | " + MP.TrackDuration + " >>>";
                StringWriter(outText, widthOfWindow2_4 - (outText.Length / 2), heightOfWindow3_4 - 2, widthOfWindow2_4 - 1, 1, false);
                UpdateConsoleBlock(widthOfWindow1_4 + 1, heightOfWindow3_4 - 2, widthOfWindow2_4 - 2, 1, null, null);
                Thread.Sleep(100);
            }
        }
    }
    public void UpdateFiles()
    {
        int skipFiles = cursorFileIdx / (heightOfWindow3_4 - 6);
        ListWriter(DFM.ArrayOfFiles.Skip(skipFiles * (heightOfWindow3_4 - 6)).Take((heightOfWindow3_4 - 6)).ToArray(), widthOfWindow3_4 + 1, 3, widthOfWindow1_4 - 2, heightOfWindow3_4 - 4, false);
        UpdateConsoleBlock(widthOfWindow3_4 + 1, 3, widthOfWindow1_4 - 2, heightOfWindow3_4 - 4, (cursorFileIdx % (heightOfWindow3_4 - 6)), (currentFileIdx >= skipFiles * (heightOfWindow3_4 - 6) && currentFileIdx <= skipFiles * (heightOfWindow3_4 - 6) + heightOfWindow3_4 - 7) ? (currentFileIdx % (heightOfWindow3_4 - 6)) : null);
    }
    private void UpdateVolume(int currentVolume)
    {
        string volumeDisplay = " Value: [";
        if (MP.MutePlayer) volumeDisplay += " Muted  ";
        else
        {
            for (int i = 0; i < currentVolume / 10; i++) volumeDisplay += volumeDots[0] + " ";
            for (int i = 0; i < 10 - (currentVolume / 10); i++) volumeDisplay += volumeDots[1] + " ";
        }
        volumeDisplay = volumeDisplay.Remove(volumeDisplay.Length - 1) + "]  " + (MP.MutePlayer ? 0 : currentVolume) + " %";
        StringWriter(volumeDisplay, widthOfWindow3_4 + 1, heightOfWindow3_4 + 3, widthOfWindow1_4 - 2, 1, true);
        UpdateConsoleBlock(widthOfWindow3_4 + 1, heightOfWindow3_4 + 3, widthOfWindow1_4 - 2, 1, null, null);
    }
    private void UpdateConsoleBlock(int xPosition, int yPosition, int lengthOfBlock, int heightOfBlock, int? selectedItem, int? selectedItem2)
    {
        lock (console)
        {
            for (int i = yPosition; i < heightOfBlock + yPosition; i++)
            {
                if (i - yPosition == selectedItem2 + 1)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (i - yPosition == selectedItem + 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                for (int j = xPosition; j < lengthOfBlock + xPosition; j++)
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
    private void DrawBaseConsole()
    {
        FrameDrawer(0, 0, widthOfWindow1_4 - 1, heightOfWindow3_4 - 1);
        FrameDrawer(widthOfWindow1_4, 0, widthOfWindow2_4 - 1, heightOfWindow3_4 - 1);
        FrameDrawer(widthOfWindow3_4, 0, widthOfWindow1_4 - 1, heightOfWindow3_4 - 1);
        FrameDrawer(0, heightOfWindow3_4, widthOfWindow - 1, heightOfWindow1_4 - 1);
        WallDrawer(widthOfWindow1_4 - 1, heightOfWindow3_4, heightOfWindow1_4 - 1);
        WallDrawer(widthOfWindow3_4, heightOfWindow3_4, heightOfWindow1_4 - 1);
        LineDrawer(0, 2, widthOfWindow1_4 - 1);
        LineDrawer(0, 6, widthOfWindow1_4 - 1);
        LineDrawer(0, 8, widthOfWindow1_4 - 1);
        LineDrawer(widthOfWindow1_4, 2, widthOfWindow2_4 - 1);
        LineDrawer(widthOfWindow1_4, heightOfWindow3_4 - 3, widthOfWindow2_4 - 1);
        LineDrawer(widthOfWindow3_4, 2, widthOfWindow1_4 - 1);
        LineDrawer(0, heightOfWindow3_4 + 2, widthOfWindow1_4 - 1);
        LineDrawer(widthOfWindow3_4, heightOfWindow3_4 + 2, widthOfWindow1_4 - 1);
        LineDrawer(widthOfWindow3_4, heightOfWindow3_4 + 4, widthOfWindow1_4 - 1);
        LineDrawer(widthOfWindow1_4 - 1, heightOfWindow3_4 + 7, widthOfWindow2_4 + 1);
        WallDrawer(widthOfWindow2_4 - 1, heightOfWindow3_4 + 7, 2);
        LineDrawerConnected(widthOfWindow1_4 - 1, heightOfWindow3_4 + 2, widthOfWindow2_4 + 1);
        LineDrawerRightConnected(widthOfWindow1_4 - 1, heightOfWindow3_4 + 4, widthOfWindow2_4 + 1);
        StringWriter("PATH", widthOfWindow1_8 - 2, 1, widthOfWindow1_4 - 2, 1, false);
        StringWriter("OTHER LOCATIONS", widthOfWindow1_8 - 8, 7, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Choose first track...", widthOfWindow2_4 - 11, 1, widthOfWindow2_4 - 1, 1, false);
        StringWriter("Choose first track...", widthOfWindow2_4 - 11, heightOfWindow3_4 - 2, widthOfWindow2_4 - 1, 1, false);
        StringWriter("FILES", (widthOfWindow - widthOfWindow1_8) - 3, 1, widthOfWindow1_4 - 2, 1, false);
        StringWriter("FOLDER & FILE CONTROLS", widthOfWindow1_8 - 11, heightOfWindow3_4 + 1, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Arrow Up    | Folder Up", 2, heightOfWindow3_4 + 3, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Arrow Down  | Folder Down", 2, heightOfWindow3_4 + 4, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Arrow Left  | File Up", 2, heightOfWindow3_4 + 5, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Arrow Right | File Down", 2, heightOfWindow3_4 + 6, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Enter       | Choose Folder", 2, heightOfWindow3_4 + 7, widthOfWindow1_4 - 2, 1, false);
        StringWriter("Space       | Choose File", 2, heightOfWindow3_4 + 8, widthOfWindow1_4 - 2, 1, false);
        StringWriter("TRACK CONTROLS", widthOfWindow2_4 - 7, heightOfWindow3_4 + 1, widthOfWindow2_4 - 1, 1, false);
        StringWriter("(F5) Previous | (F6) Back | (F7) Pause Play | (F8) Forwad | (F9) Next", widthOfWindow2_4 - 35, heightOfWindow3_4 + 3, widthOfWindow2_4 - 1, 1, false);
        StringWriter("♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪", widthOfWindow1_4, heightOfWindow3_4 + 5, widthOfWindow2_4, 1, false);
        StringWriter("♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫", widthOfWindow1_4, heightOfWindow3_4 + 6, widthOfWindow2_4, 1, false);
        StringWriter("(I) More Information", widthOfWindow1_4 + widthOfWindow1_8 - 10, heightOfWindow3_4 + 8, widthOfWindow1_4, 1, false);
        StringWriter("(ESC) Turn Off", widthOfWindow2_4 + widthOfWindow1_8 - 7, heightOfWindow3_4 + 8, widthOfWindow1_4, 1, false);
        StringWriter("VOLUME", (widthOfWindow - widthOfWindow1_8) - 3, heightOfWindow3_4 + 1, widthOfWindow1_4 - 2, 1, false);
        StringWriter("(F1) Mute", widthOfWindow3_4 + 2, heightOfWindow3_4 + 5, widthOfWindow1_4 - 2, 1, false);
        StringWriter("(F2) Volume Down", widthOfWindow3_4 + 2, heightOfWindow3_4 + 6, widthOfWindow1_4 - 2, 1, false);
        StringWriter("(F3) Volume Up", widthOfWindow3_4 + 2, heightOfWindow3_4 + 7, widthOfWindow1_4 - 2, 1, false);
        StringWriter("(F4) Unmute", widthOfWindow3_4 + 2, heightOfWindow3_4 + 8, widthOfWindow1_4 - 2, 1, false);
    }
    private void BoxClear(int xPosition, int yPosition, int lengthOfBlock, int heightOfBlock)
    {
        for (int i = yPosition; i < heightOfBlock + yPosition; i++)
        {
            for (int j = xPosition; j < lengthOfBlock + xPosition; j++) console[j, i] = ' ';
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
        for (int i = startX; i < countX + startX; i++)
        {
            console[i, startY] = walls[0];
            console[i, startY + countY] = walls[0];
        }
        for (int i = startY; i < countY + startY; i++)
        {
            console[startX, i] = walls[1];
            console[startX + countX, i] = walls[1];
        }
        console[startX, startY] = walls[2];
        console[startX, startY + countY] = walls[4];
        console[startX + countX, startY] = walls[3];
        console[startX + countX, startY + countY] = walls[5];
    }
    private void StringWriter(string text, int xPosition, int yPosition, int lengthOfBlock, int heightOfBlock, bool? clearBox)
    {
        string textCopied = text;
        if (clearBox != null && clearBox == true) BoxClear(xPosition, yPosition, lengthOfBlock, heightOfBlock);
        int tempXPosition = xPosition, tempLengthOfBlock = lengthOfBlock;
        if (heightOfBlock * lengthOfBlock < textCopied.Length) textCopied = "... " + textCopied.Substring(textCopied.Length - (heightOfBlock * lengthOfBlock) + 4);
        foreach (char c in textCopied)
        {
            if (tempLengthOfBlock > 0)
            {
                console[tempXPosition, yPosition] = c;
                tempLengthOfBlock--;
                tempXPosition++;
            }
            else
            {
                tempLengthOfBlock = lengthOfBlock;
                tempXPosition = xPosition;
                yPosition++;
                console[tempXPosition, yPosition] = c;
                tempLengthOfBlock--;
                tempXPosition++;
            }
        }
    }
    private void ListWriter(string[] text, int xPosition, int yPosition, int lengthOfBlock, int heightOfBlock, bool truncateEnd)
    {
        string[] copiedText = new string[text.Length];
        text.CopyTo(copiedText, 0);
        BoxClear(xPosition, yPosition, lengthOfBlock, heightOfBlock);
        int tempXPosition = xPosition;
        heightOfBlock -= 2;
        for (int i = xPosition; i < lengthOfBlock + xPosition; i++) console[i, yPosition] = arrows[0];
        yPosition++;
        for (int i = 0; i < copiedText.Length; i++)
        {
            if (lengthOfBlock < copiedText[i].Length)
            {
                if (truncateEnd) copiedText[i] = copiedText[i].Substring(0, lengthOfBlock - 4) + " ...";
                else copiedText[i] = "... " + copiedText[i].Substring(copiedText[i].Length - (lengthOfBlock) + 4);
            }
            foreach (char c in copiedText[i])
            {
                console[tempXPosition, yPosition] = c;
                tempXPosition++;
            }
            tempXPosition = xPosition;
            yPosition++;
            heightOfBlock--;
            if (heightOfBlock == 0) break;
        }
        if (heightOfBlock != 0) yPosition += heightOfBlock;
        for (int i = xPosition; i < lengthOfBlock + xPosition; i++) console[i, yPosition] = arrows[1];
    }
}