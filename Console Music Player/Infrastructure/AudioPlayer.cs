using WMPLib;
using MusicConsoleTool;

namespace AudioPlayerTool;

class AudioPlayer
{
    private MusicConsole musicConsole;
    public WindowsMediaPlayer WMP;
    public AudioPlayer(MusicConsole musicConsole, short defaultVolume = 50)
    {
        this.musicConsole = musicConsole;
        WMP = new WindowsMediaPlayer();
        WMP.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(TrackEnded);
        WMP.settings.volume = defaultVolume;
    }
    public short VolumePlayer { get { return (short)WMP.settings.volume; } }
    public string CurrentTrackDuration { get { return WMP.controls.currentPositionString; } }
    public string TrackDuration { get { return WMP.currentMedia.durationString; } }
    public bool TrackStatusStopped { get { return WMP.playState == WMPLib.WMPPlayState.wmppsStopped; } }
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
            musicConsole.DFM.Refresh();
            musicConsole.currentFileIdx = (musicConsole.currentFileIdx + 1) < musicConsole.DFM.CountOfFiles ? ++musicConsole.currentFileIdx : 0;
            musicConsole.UpdateFiles(musicConsole.currentFileIdx);
            Start(musicConsole.DFM.ArrayOfFiles[musicConsole.currentFileIdx]);
            musicConsole.UpdateTrack(musicConsole.DFM.ArrayOfFiles[musicConsole.currentFileIdx]);
        }
    }
}