using WMPLib;
using MusicConsoleTool;

namespace MusicPlayerTool;

class MusicPlayer
{
    private WindowsMediaPlayer windowsMediaPlayer;
    private MusicConsole musicConsole;
    public MusicPlayer(MusicConsole musicConsole, short startVolume = 100)
    {
        this.musicConsole = musicConsole;
        windowsMediaPlayer = new WindowsMediaPlayer();
        windowsMediaPlayer.settings.volume = startVolume;
    }
    public string TrackPath => windowsMediaPlayer.URL;
    public string TrackDuration => windowsMediaPlayer.currentMedia != null ? windowsMediaPlayer.currentMedia.durationString : "00:00";
    public string TrackCurrentDuration => windowsMediaPlayer.controls.currentPositionString;
    public short PlayerVolume => (short)windowsMediaPlayer.settings.volume;
    public bool IsLoaded => windowsMediaPlayer.currentMedia != null ? (windowsMediaPlayer.currentMedia.duration > 0d ? true : false) : false;
    public bool IsPlayerMute => windowsMediaPlayer.settings.mute;
    public bool IsPlayerJustStarted => windowsMediaPlayer.controls.currentPosition < 2.0d ? true : false;
    public void Start(string path)
    {
        windowsMediaPlayer.controls.stop();
        if (File.Exists(path))
        {
            short oldVolume = (short)windowsMediaPlayer.settings.volume;
            bool oldMute = windowsMediaPlayer.settings.mute;
            windowsMediaPlayer.close();
            windowsMediaPlayer = new WindowsMediaPlayer();
            windowsMediaPlayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
            windowsMediaPlayer.MediaError += new _WMPOCXEvents_MediaErrorEventHandler(TrackLost);
            windowsMediaPlayer.settings.volume = oldVolume;
            windowsMediaPlayer.settings.mute = oldMute;
            windowsMediaPlayer.URL = path;
            windowsMediaPlayer.controls.play();
            musicConsole.nowPlaying = true;
        }
        else
        {
            bool changedFolder = musicConsole.DirectoryFileManager.Refresh();
            musicConsole.nowPlaying = false;
            if (changedFolder || musicConsole.DirectoryFileManager.CountOfFiles == 0)
            {
                short oldVolume = (short)windowsMediaPlayer.settings.volume;
                bool oldMute = windowsMediaPlayer.settings.mute;
                windowsMediaPlayer.close();
                windowsMediaPlayer = new WindowsMediaPlayer();
                windowsMediaPlayer.settings.volume = oldVolume;
                windowsMediaPlayer.settings.mute = oldMute;
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
    public void Pause() => windowsMediaPlayer.controls.pause();
    public void Play() => windowsMediaPlayer.controls.play();
    public void ChangeVolume(short count)
    {
        windowsMediaPlayer.settings.mute = false;
        windowsMediaPlayer.settings.volume += count;
    }
    public void ChangeMute(bool value) => windowsMediaPlayer.settings.mute = value;
    public void RewindTrack(double count) => windowsMediaPlayer.controls.currentPosition += count;
    private void TrackEnded(int state)
    {
        if (state == (int)WMPPlayState.wmppsMediaEnded)
        {
            musicConsole.nowPlaying = false;
            if (musicConsole.currentFileIdx != -2)
            {
                musicConsole.currentFileIdx = (musicConsole.currentFileIdx + 1) < musicConsole.DirectoryFileManager.CountOfFiles ? ++musicConsole.currentFileIdx : 0;
                Start(musicConsole.DirectoryFileManager.ArrayOfFiles[musicConsole.currentFileIdx]);
            }
        }
    }
    private void TrackLost(object mediaObject)
    {
        windowsMediaPlayer.controls.pause();
        musicConsole.nowPlaying = false;
    }
}