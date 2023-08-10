using WMPLib;
using MusicConsoleTool;

namespace MusicPlayerTool;

class MusicPlayer
{
    private WindowsMediaPlayer windowsMediaPlayerMain, windowsMediaPlayerHelper;
    private WindowsMediaPlayer? windowsMediaPlayerBufor;
    private MusicConsole musicConsole;
    public MusicPlayer(MusicConsole musicConsole, short startVolume = 100)
    {
        this.musicConsole = musicConsole;
        windowsMediaPlayerMain = new WindowsMediaPlayer();
        windowsMediaPlayerMain.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        windowsMediaPlayerMain.MediaError += new _WMPOCXEvents_MediaErrorEventHandler(TrackLost);
        windowsMediaPlayerMain.settings.volume = startVolume;
        windowsMediaPlayerHelper = new WindowsMediaPlayer();
        windowsMediaPlayerHelper.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        windowsMediaPlayerHelper.MediaError += new _WMPOCXEvents_MediaErrorEventHandler(TrackLost);
        windowsMediaPlayerHelper.settings.volume = startVolume;
    }
    public string TrackPath => windowsMediaPlayerMain.URL;
    public string TrackDuration => windowsMediaPlayerMain.currentMedia != null ? windowsMediaPlayerMain.currentMedia.durationString : "00:00";
    public string TrackCurrentDuration => windowsMediaPlayerMain.controls.currentPositionString;
    public short PlayerVolume => (short)windowsMediaPlayerMain.settings.volume;
    public bool IsLoaded => windowsMediaPlayerMain.currentMedia != null ? (windowsMediaPlayerMain.currentMedia.duration > 0d ? true : false) : false;
    public bool IsPlayerMute => windowsMediaPlayerMain.settings.mute;
    public bool IsPlayerJustStarted => windowsMediaPlayerMain.controls.currentPosition < 2.0d ? true : false;
    public void Start(string path)
    {
        windowsMediaPlayerMain.controls.stop();
        if (File.Exists(path))
        {
            windowsMediaPlayerMain.URL = musicConsole.DirectoryFileManager.Path + "\\" + path;
            RewindTrack(double.MinValue);
            windowsMediaPlayerMain.controls.play();
            musicConsole.nowPlaying = true;
        }
        else
        {
            musicConsole.nowPlaying = false;
            bool changedFolder = musicConsole.DirectoryFileManager.Refresh();
            if (changedFolder || musicConsole.DirectoryFileManager.CountOfFiles == 0)
            {
                if (changedFolder)
                {
                    musicConsole.currentFolderIdx = 0;
                    musicConsole.UpdatePath();
                    musicConsole.UpdateFolders();
                }
                musicConsole.currentFileIdx = musicConsole.currentFileIdxMemory = -2;
                if (musicConsole.DirectoryFileManager.CountOfFiles > 0) musicConsole.cursorFileIdx = 0;
                else musicConsole.cursorFileIdx = -2;
            }
            else
            {
                musicConsole.currentFileIdx = (musicConsole.currentFileIdx - 1) < 0 ? 0 : (musicConsole.currentFileIdx - 1) < musicConsole.DirectoryFileManager.CountOfFiles ? (musicConsole.currentFileIdx - 1) : (musicConsole.DirectoryFileManager.CountOfFiles - 1);
                Start(musicConsole.DirectoryFileManager.ArrayOfFiles[musicConsole.currentFileIdx]);
            }
        }
        musicConsole.UpdateFiles();
        musicConsole.UpdateTrack();
    }
    public void Pause() => windowsMediaPlayerMain.controls.pause();
    public void Play() => windowsMediaPlayerMain.controls.play();
    public void ChangeVolume(short count)
    {
        windowsMediaPlayerMain.settings.mute = false;
        windowsMediaPlayerHelper.settings.mute = false;
        windowsMediaPlayerMain.settings.volume += count;
        windowsMediaPlayerHelper.settings.volume += count;
    }
    public void ChangeMute(bool value) => windowsMediaPlayerMain.settings.mute = windowsMediaPlayerHelper.settings.mute = value;
    public void RewindTrack(double count)
    {
        try
        {
            windowsMediaPlayerMain.controls.currentPosition += count;
        }
        catch (Exception) { }
    }
    private void TrackEnded(int state)
    {
        if (state == (int)WMPPlayState.wmppsMediaEnded)
        {
            musicConsole.nowPlaying = false;
            if (musicConsole.currentFileIdx != -2)
            {
                musicConsole.currentFileIdx = (musicConsole.currentFileIdx + 1) < musicConsole.DirectoryFileManager.CountOfFiles ? ++musicConsole.currentFileIdx : 0;
                windowsMediaPlayerBufor = windowsMediaPlayerMain;
                windowsMediaPlayerMain = windowsMediaPlayerHelper;
                windowsMediaPlayerHelper = windowsMediaPlayerBufor;
                windowsMediaPlayerHelper.URL = null;
                Start(musicConsole.DirectoryFileManager.ArrayOfFiles[musicConsole.currentFileIdx]);
            }
        }
    }
    private void TrackLost(object mediaObject)
    {
        windowsMediaPlayerMain.controls.pause();
        musicConsole.nowPlaying = false;
    }
}