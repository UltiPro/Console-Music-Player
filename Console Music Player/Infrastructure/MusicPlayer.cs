using WMPLib;
using MusicConsoleTool;

namespace MusicPlayerTool;

class MusicPlayer
{
    private WindowsMediaPlayer windowsMediaPlayer;
    private MusicConsole musicConsole;
    public MusicPlayer(MusicConsole musicConsole, short defaultVolume = 50)
    {
        this.musicConsole = musicConsole;
        windowsMediaPlayer = new WindowsMediaPlayer();
        windowsMediaPlayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        windowsMediaPlayer.settings.volume = defaultVolume;
    }
    public string TrackDuration => windowsMediaPlayer.currentMedia != null ? windowsMediaPlayer.currentMedia.durationString : "00:00";
    public string TrackCurrentDuration => windowsMediaPlayer.controls.currentPositionString;
    public short PlayerVolume => (short)windowsMediaPlayer.settings.volume;
    public bool IsPlayerMute => windowsMediaPlayer.settings.mute;
    public bool IsPlayerJustStarted => windowsMediaPlayer.controls.currentPosition < 2.0d ? true : false;
    public void Start(string path)
    {
        if (!File.Exists(path))
        {
            musicConsole.currentFileIdx = (musicConsole.currentFileIdx - 1) < 0 ? 0 : --musicConsole.currentFileIdx;
            TrackEnded(8);
            return;
        }
        windowsMediaPlayer.controls.stop();
        short oldVolume = (short)windowsMediaPlayer.settings.volume;
        bool oldMute = windowsMediaPlayer.settings.mute;
        windowsMediaPlayer = new WindowsMediaPlayer();
        windowsMediaPlayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        windowsMediaPlayer.settings.volume = oldVolume;
        windowsMediaPlayer.settings.mute = oldMute;
        windowsMediaPlayer.URL = path;
        windowsMediaPlayer.controls.play();
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
            musicConsole.DirectoryFileManager.Refresh();
            if (musicConsole.DirectoryFileManager.CountOfFiles == 0)
            {
                windowsMediaPlayer.controls.stop();
                musicConsole.nowPlaying = false;
                return;
            }
            musicConsole.currentFileIdx = (musicConsole.currentFileIdx + 1) < musicConsole.DirectoryFileManager.CountOfFiles ? ++musicConsole.currentFileIdx : 0;
            musicConsole.UpdateFiles();
            Start(musicConsole.DirectoryFileManager.ArrayOfFiles[musicConsole.currentFileIdx]);
            musicConsole.UpdateTrack();
        }
    }
}