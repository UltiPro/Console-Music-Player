using WMPLib;
using MusicConsoleTool;

namespace MusicPlayerTool;

class MusicPlayer
{
    private MusicConsole musicConsole;
    private WindowsMediaPlayer WMP;
    public MusicPlayer(MusicConsole musicConsole, short defaultVolume = 50)
    {
        this.musicConsole = musicConsole;
        WMP = new WindowsMediaPlayer();
        WMP.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        WMP.settings.volume = defaultVolume;
    }
    public short VolumePlayer => (short)WMP.settings.volume;
    public bool MutePlayer => WMP.settings.mute;
    public bool IsJustStarted => WMP.controls.currentPosition < 2.0d ? true : false;
    public string CurrentTrackDuration => WMP.controls.currentPositionString;
    public string TrackDuration => WMP.currentMedia.durationString;
    public void Start(string path)
    {
        if (!File.Exists(path))
        {
            musicConsole.currentFileIdx = (musicConsole.currentFileIdx - 1) < 0 ? 0 : --musicConsole.currentFileIdx;
            TrackEnded(8);
            return;
        }
        WMP.controls.stop();
        short oldVolume = (short)WMP.settings.volume;
        bool oldMute = WMP.settings.mute;
        WMP = new WindowsMediaPlayer();
        WMP.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        WMP.settings.volume = oldVolume;
        WMP.settings.mute = oldMute;
        WMP.URL = path;
        WMP.controls.play();
    }
    public void Pause() => WMP.controls.pause();
    public void Resume() => WMP.controls.play();
    public void ChangeVolume(short count)
    {
        WMP.settings.mute = false;
        WMP.settings.volume += count;
    }
    public void MuteUnMute(bool value) => WMP.settings.mute = value;
    public void SkipTrack(int count) => WMP.controls.currentPosition += count;
    private void TrackEnded(int state)
    {
        if (state == (int)WMPPlayState.wmppsMediaEnded)
        {
            musicConsole.Dfm.Refresh();
            if (musicConsole.Dfm.CountOfFiles == 0)
            {
                WMP.controls.stop();
                musicConsole.nowPlaying = false;
                return;
            }
            musicConsole.currentFileIdx = (musicConsole.currentFileIdx + 1) < musicConsole.Dfm.CountOfFiles ? ++musicConsole.currentFileIdx : 0;
            musicConsole.UpdateFiles();
            Start(musicConsole.Dfm.ArrayOfFiles[musicConsole.currentFileIdx]);
            musicConsole.UpdateTrack();
        }
    }
}