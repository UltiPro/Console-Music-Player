using System.Diagnostics;
using DirectoryFileManagerTool;
using AudioPlayerTool;
using LoggerTool;

namespace MusicConsoleTool;

class MusicConsole
{
    private char[] walls = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
    private char[] arrows = { '▲', '▼' };
    private char[] volumeDots = { '●', '○' };
    private char[] notes = { '♪', '♫' };
    private char[,] console;
    private int[] consoleAnimation;
    private int currentFolderIdx, currentFileIdx;
    private bool initTheards, nowPlaying;
    private DirectoryFileManager DFM;
    private AudioPlayer AP;
    private Thread? threadTimer, threadAnimation, threadAnimation2;
    public MusicConsole()
    {
        currentFileIdx = 0;
        currentFolderIdx = 0;
        initTheards = true;
        nowPlaying = false;
        console = new char[160, 40];
        consoleAnimation = new int[(console.GetLength(0) / 2) - 4];
        DFM = new DirectoryFileManager();
        AP = new AudioPlayer();
    }
    public void Start()
    {
        InitMusicConsole();
        bool start = true;
        ConsoleKeyInfo inputFromKeyboard;
        while (start)
        {
            inputFromKeyboard = Console.ReadKey(true);
            switch (inputFromKeyboard.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentFolderIdx > 0 /* && blockChanging */)
                    {
                        currentFolderIdx--;
                        UpdateFolders(currentFolderIdx);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (currentFolderIdx < DFM.CountOfFolders - 1 /*&& blockChanging*/)
                    {
                        currentFolderIdx++;
                        UpdateFolders(currentFolderIdx);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentFileIdx > 0 /*&& blockChanging*/)
                    {
                        currentFileIdx--;
                        UpdateFiles(currentFileIdx);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (currentFileIdx < DFM.CountOfFiles - 1 /*&& blockChanging*/)
                    {
                        currentFileIdx++;
                        UpdateFiles(currentFileIdx);
                    }
                    break;
                case ConsoleKey.Enter:
                    if (currentFolderIdx == 0 && DFM.ReturnDirectory) DFM.ChangeFolderBack();
                    else DFM.ChangeFolder(DFM.ArrayOfFolders[currentFolderIdx]);
                    currentFolderIdx = currentFileIdx = 0;
                    UpdatePath();
                    UpdateFolders(currentFolderIdx);
                    UpdateFiles(currentFileIdx);
                    break;
                case ConsoleKey.Spacebar:
                    try
                    {
                        AP.Start(DFM.ArrayOfFiles[currentFileIdx]);
                        UpdateTrack(DFM.ArrayOfFiles[currentFileIdx]);
                        if (threadTimer != null && threadAnimation != null && threadAnimation2 != null && initTheards)
                        {
                            threadTimer.Start();
                            threadAnimation.Start();
                            threadAnimation2.Start();
                            initTheards = false;
                            nowPlaying = true;
                        }
                        else nowPlaying = true;
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.Message);
                    }
                    break;
                case ConsoleKey.F1:
                    AP.UnMute();
                    UpdateVolume(AP.VolumePlayer);
                    break;
                case ConsoleKey.F2:
                    if (AP.VolumePlayer > 0)
                    {
                        AP.ChangeVolume(-1);
                        UpdateVolume(AP.VolumePlayer);
                    }
                    break;
                case ConsoleKey.F3:
                    if (AP.VolumePlayer < 100)
                    {
                        AP.ChangeVolume(1);
                        UpdateVolume(AP.VolumePlayer);
                    }
                    break;
                case ConsoleKey.F4:
                    AP.Mute();
                    UpdateVolume(AP.VolumePlayer);
                    break;
                case ConsoleKey.F5:
                    AP.Pause();
                    nowPlaying = false;
                    break;
                case ConsoleKey.F6:
                    AP.SkipTrack(-5);
                    break;
                case ConsoleKey.F7:
                    AP.Resume();
                    nowPlaying = true;
                    break;
                case ConsoleKey.F8:
                    AP.SkipTrack(5);
                    break;
                case ConsoleKey.I:
                    try
                    {
                        Process.Start("notepad.exe", "Information.txt");
                    }
                    catch (Exception e)
                    {
                        Logger.SaveLog(e.Message);
                    }
                    break;
                case ConsoleKey.Escape:
                    if (threadTimer != null) threadTimer.Interrupt();
                    if (threadAnimation != null) threadAnimation.Interrupt();
                    if (threadAnimation2 != null) threadAnimation2.Interrupt();
                    start = false;
                    Console.Clear();
                    break;
            }
        }
    }
    private void InitMusicConsole()
    {
        Console.Title = "Console Music Player";
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.TreatControlCAsInput = true;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetWindowSize(console.GetLength(0), console.GetLength(1));
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        StartMenu();
        InitMusicConsoleDraw();
        threadTimer = new Thread(() => UpdateTimer());
        threadAnimation = new Thread(() => UpdateAnimation());
        threadAnimation2 = new Thread(() => UpdateAnimation2());
    }
    private void InitMusicConsoleDraw()
    {
        DrawBaseConsole();
        DrawConsole();
        UpdatePath();
        UpdateFolders(0);
        UpdateFiles(0);
        UpdateVolume(AP.VolumePlayer);
        UpdateAnimationPassive();
    }
    private void StartMenu()
    {
        Console.Clear();
        for (int i = 0; i < 10; i++) Console.WriteLine();
        try
        {
            using (StreamReader sr = new StreamReader("Logo.txt"))
            {
                String? line;
                while ((line = sr.ReadLine()) != null) Console.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            for (int i = 0; i < (Console.WindowWidth / 2) - 10; i++) Console.Write(" ");
            Console.Write("Console Music Player");
            Logger.SaveLog(e.Message);
        }
        Console.Write("\n\n\n");
        string pressButton = "<<< Press Any Button To Continue >>>";
        Console.SetCursorPosition((Console.WindowWidth / 2) - (pressButton.Length / 2), 24);
        Console.Write(pressButton);
        Console.ReadKey();
        Console.Clear();
    }
    private void InformationMenu()
    {
        Console.Clear();
        Console.WriteLine("test");
        Console.ReadKey();
        Console.Clear();
    }
    private void DrawConsole()
    {
        for (int i = 0; i < Console.WindowHeight - 1; i++)
        {
            for (int j = 0; j < Console.WindowWidth; j++) Console.Write(console[j, i]);
        }
        for (int i = 0; i < Console.WindowWidth - 1; i++) Console.Write(console[i, 39]);
        Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 1);
        Console.Write(walls[5]);
        Console.MoveBufferArea(Console.WindowWidth - 2, Console.WindowHeight - 1, 1, 1, Console.WindowWidth - 1, Console.WindowHeight - 1, walls[0], Console.ForegroundColor, Console.BackgroundColor);
    }
    private void UpdatePath()
    {
        string newPath = DFM.Path;
        if (!(newPath[newPath.Length - 1] == '\\')) newPath += "\\";
        StringWriter(newPath, 1, 3, (Console.WindowWidth / 4) - 2, 3, true);
        UpdateConsoleBlock(1, 3, (Console.WindowWidth / 4) - 2, 3, null);
    }
    private void UpdateFolders(int indexOfSelected)
    {
        ListWriter(DFM.ArrayOfFolders, 1, 9, (Console.WindowWidth / 4) - 2, ((Console.WindowHeight / 4) * 3) - 10, true);
        UpdateConsoleBlock(1, 9, (Console.WindowWidth / 4) - 2, ((Console.WindowHeight / 4) * 3) - 10, indexOfSelected);
    }
    private void UpdateTrack(string trackToDisplay)
    {
        trackToDisplay = trackToDisplay.Remove(trackToDisplay.Length - 4);
        BoxClear((Console.WindowWidth / 4) + 1, 1, ((Console.WindowWidth / 4) * 2) - 1, 1);
        StringWriter(trackToDisplay, (Console.WindowWidth / 2) - (trackToDisplay.Length / 2), 1, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
        UpdateConsoleBlock((Console.WindowWidth / 4), 1, ((Console.WindowWidth / 4) * 2) - 1, 1, null);
    }
    private void UpdateAnimationPassive()
    {
        BoxClear((Console.WindowWidth / 4) + 1, 3, (Console.WindowWidth / 4) * 2 - 2, (Console.WindowHeight / 4) * 3 - 3);
        string infoText = "PAUSED: Resume Or Select New Track";
        StringWriter(infoText, (Console.WindowWidth / 2) - (infoText.Length / 2), (Console.WindowHeight / 4 * 3) / 2, (Console.WindowWidth / 2) - 2, 1, false);
        UpdateConsoleBlock((Console.WindowWidth / 4) + 1, 0, (Console.WindowWidth / 4) * 2 - 2, (Console.WindowHeight / 4) * 3 - 3, null);
    }
    private void UpdateAnimation()
    {
        Random rnd = new Random();
        int rndNumber = rnd.Next(0, (Console.WindowHeight / 4) * 3 - 9);
        while (true)
        {
            if (nowPlaying)
            {

                for (int i = 0; i < consoleAnimation.Length; i++)
                {
                    consoleAnimation[i] = rndNumber;
                    rndNumber = rnd.Next(0, (Console.WindowHeight / 4) * 3 - 9);
                }
                int z = 0;
                for (int i = (Console.WindowWidth / 4) + 2; i < (Console.WindowWidth / 4) + 2 + (Console.WindowWidth / 4) * 2 - 4; i++)
                {
                    for (int j = (Console.WindowHeight / 4) * 3 - 4; j > 5; j--)
                    {
                        if (j > 8 + consoleAnimation[z]) console[i, j] = volumeDots[0];
                        else console[i, j] = ' ';
                    }
                    z++;
                }
                UpdateConsoleBlock((Console.WindowWidth / 4) + 2, 6, (Console.WindowWidth / 4) * 2 - 4, (Console.WindowHeight / 4) * 3 - 9, null);
                Thread.Sleep(500);
            }
            else
            {
                UpdateAnimationPassive();
                Thread.Sleep(1000);
            }
        }
    }
    private void UpdateAnimation2()
    {
        Random rnd = new Random();
        BoxClear((Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) + 5, (Console.WindowWidth / 4) * 2, 2);
        while (true)
        {
            if (nowPlaying)
            {
                int col = rnd.Next(0, 10) + (Console.WindowWidth / 4) + 1;
                int row = ((Console.WindowHeight / 4) * 3) + 5;
                while (col <= (Console.WindowWidth / 4 * 3) - 2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                col = rnd.Next(0, 10) + (Console.WindowWidth / 4) + 1;
                row++;
                while (col <= (Console.WindowWidth / 4 * 3) - 2)
                {
                    console[col, row] = notes[col % 2];
                    col += rnd.Next(0, 10);
                }
                UpdateConsoleBlock((Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) + 5, (Console.WindowWidth / 4) * 2, 2, null);
                BoxClear((Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) + 5, (Console.WindowWidth / 4) * 2, 2);
            }
            Thread.Sleep(1000);
        }
    }
    private void UpdateTimer()
    {
        while (true)
        {
            BoxClear((Console.WindowWidth / 4) + 1, ((Console.WindowHeight / 4) * 3) - 2, ((Console.WindowWidth / 4) * 2) - 2, 1);
            string outText = "<<< " + (AP.CurrentTrackDuration != "" ? AP.CurrentTrackDuration : "--:--") + " | " + AP.TrackDuration + " >>>";
            StringWriter(outText, (Console.WindowWidth / 2) - (outText.Length / 2), ((Console.WindowHeight / 4) * 3) - 2, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
            UpdateConsoleBlock((Console.WindowWidth / 4) + 1, ((Console.WindowHeight / 4) * 3) - 2, ((Console.WindowWidth / 4) * 2) - 2, 1, null);
            Thread.Sleep(1000);
            if (AP.TrackStatusStopped) nowPlaying = false;
        }
    }
    private void UpdateFiles(int indexOfSelected)
    {
        ListWriter(DFM.ArrayOfFiles, ((Console.WindowWidth / 4) * 3) + 1, 3, (Console.WindowWidth / 4) - 2, ((Console.WindowHeight / 4) * 3) - 4, false);
        UpdateConsoleBlock(((Console.WindowWidth / 4) * 3) + 1, 3, (Console.WindowWidth / 4) - 2, ((Console.WindowHeight / 4) * 3) - 4, indexOfSelected);
    }
    private void UpdateVolume(int currentVolume)
    {
        string volumeDisplay = " Value: [";
        for (int i = 0; i < currentVolume / 10; i++) volumeDisplay += volumeDots[0] + " ";
        for (int i = 0; i < 10 - (currentVolume / 10); i++) volumeDisplay += volumeDots[1] + " ";
        volumeDisplay = volumeDisplay.Remove(volumeDisplay.Length - 1) + "]  " + currentVolume + " %";
        StringWriter(volumeDisplay, ((Console.WindowWidth / 4) * 3) + 1, ((Console.WindowHeight / 4) * 3) + 3, (Console.WindowWidth / 4) - 2, 1, true);
        UpdateConsoleBlock(((Console.WindowWidth / 4) * 3) + 1, ((Console.WindowHeight / 4) * 3) + 3, (Console.WindowWidth / 4) - 2, 1, null);
    }
    private void UpdateConsoleBlock(int xPosition, int yPosition, int lengthOfBlock, int heightOfBlock, int? selectedItem)
    {
        lock (console)
        {
            for (int i = yPosition; i < heightOfBlock + yPosition; i++)
            {
                for (int j = xPosition; j < lengthOfBlock + xPosition; j++)
                {
                    if (i - yPosition == selectedItem + 1)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.SetCursorPosition(j, i);
                    Console.Write(console[j, i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
    private void DrawBaseConsole()
    {
        FrameDrawer(0, 0, (Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3) - 1);
        FrameDrawer((Console.WindowWidth / 4), 0, ((Console.WindowWidth / 4) * 2) - 1, ((Console.WindowHeight / 4) * 3) - 1);
        FrameDrawer(((Console.WindowWidth / 4) * 3), 0, (Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3) - 1);
        FrameDrawer(0, ((Console.WindowHeight / 4) * 3), Console.WindowWidth - 1, (Console.WindowHeight / 4) - 1);
        WallDrawer((Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3), (Console.WindowHeight / 4) - 1);
        WallDrawer(((Console.WindowWidth / 4) * 3), ((Console.WindowHeight / 4) * 3), (Console.WindowHeight / 4) - 1);
        LineDrawer(0, 2, (Console.WindowWidth / 4) - 1);
        LineDrawer(0, 6, (Console.WindowWidth / 4) - 1);
        LineDrawer(0, 8, (Console.WindowWidth / 4) - 1);
        LineDrawer((Console.WindowWidth / 4), 2, ((Console.WindowWidth / 4) * 2) - 1);
        LineDrawer((Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) - 3, ((Console.WindowWidth / 4) * 2) - 1);
        LineDrawer(((Console.WindowWidth / 4) * 3), 2, (Console.WindowWidth / 4) - 1);
        LineDrawer(0, ((Console.WindowHeight / 4) * 3) + 2, (Console.WindowWidth / 4) - 1);
        LineDrawer(((Console.WindowWidth / 4) * 3), ((Console.WindowHeight / 4) * 3) + 2, (Console.WindowWidth / 4) - 1);
        LineDrawer(((Console.WindowWidth / 4) * 3), ((Console.WindowHeight / 4) * 3) + 4, (Console.WindowWidth / 4) - 1);
        LineDrawer((Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3) + 7, (Console.WindowWidth / 2) + 1);
        WallDrawer((Console.WindowWidth / 2) - 1, ((Console.WindowHeight / 4) * 3) + 7, 2);
        LineDrawerConnected((Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3) + 2, (Console.WindowWidth / 2) + 1);
        LineDrawerRightConnected((Console.WindowWidth / 4) - 1, ((Console.WindowHeight / 4) * 3) + 4, (Console.WindowWidth / 2) + 1);
        StringWriter("PATH", ((Console.WindowWidth / 4) / 2) - 2, 1, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("OTHER LOCATIONS", ((Console.WindowWidth / 4) / 2) - 8, 7, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Choose first track...", (Console.WindowWidth / 2) - 11, 1, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
        StringWriter("Choose first track...", (Console.WindowWidth / 2) - 11, ((Console.WindowHeight / 4) * 3) - 2, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
        StringWriter("FILES", (Console.WindowWidth - (Console.WindowWidth / 8)) - 3, 1, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("FOLDER & FILE CONTROLS", ((Console.WindowWidth / 4) / 2) - 11, ((Console.WindowHeight / 4) * 3) + 1, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Arrow Up    | Folder Up", 2, ((Console.WindowHeight / 4) * 3) + 3, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Arrow Down  | Folder Down", 2, ((Console.WindowHeight / 4) * 3) + 4, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Arrow Left  | File Up", 2, ((Console.WindowHeight / 4) * 3) + 5, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Arrow Right | File Down", 2, ((Console.WindowHeight / 4) * 3) + 6, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Enter       | Choose Folder", 2, ((Console.WindowHeight / 4) * 3) + 7, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("Space       | Choose File", 2, ((Console.WindowHeight / 4) * 3) + 8, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("TRACK CONTROLS", (Console.WindowWidth / 2) - 7, ((Console.WindowHeight / 4) * 3) + 1, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
        StringWriter("(F5) Pause | (F6) Skip Back | (F7) Resume | (F8) Skip Front", (Console.WindowWidth / 2) - 29, ((Console.WindowHeight / 4) * 3) + 3, ((Console.WindowWidth / 4) * 2) - 1, 1, false);
        StringWriter("♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪", (Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) + 5, ((Console.WindowWidth / 4) * 2), 1, false);
        StringWriter("♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫", (Console.WindowWidth / 4), ((Console.WindowHeight / 4) * 3) + 6, ((Console.WindowWidth / 4) * 2), 1, false);
        StringWriter("(I) More Information", (Console.WindowWidth / 4) + (Console.WindowWidth / 8) - 10, ((Console.WindowHeight / 4) * 3) + 8, (Console.WindowWidth / 4), 1, false);
        StringWriter("(ESC) Turn Off", (Console.WindowWidth / 2) + (Console.WindowWidth / 8) - 7, ((Console.WindowHeight / 4) * 3) + 8, (Console.WindowWidth / 4), 1, false);
        StringWriter("VOLUME", (Console.WindowWidth - (Console.WindowWidth / 8)) - 3, ((Console.WindowHeight / 4) * 3) + 1, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("(F1) Unmute", ((Console.WindowWidth / 4) * 3) + 2, ((Console.WindowHeight / 4) * 3) + 5, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("(F2) Volume Down", ((Console.WindowWidth / 4) * 3) + 2, ((Console.WindowHeight / 4) * 3) + 6, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("(F3) Volume Up", ((Console.WindowWidth / 4) * 3) + 2, ((Console.WindowHeight / 4) * 3) + 7, (Console.WindowWidth / 4) - 2, 1, false);
        StringWriter("(F4) Mute", ((Console.WindowWidth / 4) * 3) + 2, ((Console.WindowHeight / 4) * 3) + 8, (Console.WindowWidth / 4) - 2, 1, false);
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
        if (heightOfBlock * lengthOfBlock < textCopied.Length) textCopied = "... " + textCopied.Substring(textCopied.Length - (heightOfBlock * lengthOfBlock) + 3);
        foreach (char c in textCopied)
        {
            if (tempLengthOfBlock != 0)
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