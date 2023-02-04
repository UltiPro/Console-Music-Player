using WMPLib;

namespace AudioPlayerTool;

class AudioPlayer
{
    private WindowsMediaPlayer WMP;
    private short volumePlayer, bufVolumePlayer;
    private bool isMute;
    public AudioPlayer()
    {
        WMP = new WindowsMediaPlayer();
        volumePlayer = bufVolumePlayer = 5;
        isMute = false;
    }
    public short VolumePlayer { get { return volumePlayer; } }
    public string CurrentTrackDuration { get { return WMP.controls.currentPositionString; } }
    public string TrackDuration { get { return WMP.currentMedia.durationString; } }
    public bool TrackStatusStopped { get { return WMP.playState == WMPLib.WMPPlayState.wmppsStopped; } }
    public void Start(string path)
    {
        Stop();
        WMP.settings.volume = volumePlayer;
        WMP.URL = path;
        WMP.controls.play();
    }
    public void Pause() => WMP.controls.pause();
    public void Resume() => WMP.controls.play();
    public void Stop() => WMP.controls.stop();
    public void ChangeVolume(short count)
    {
        isMute = false;
        volumePlayer += count;
        WMP.settings.volume = volumePlayer;
    }
    public void Mute()
    {
        isMute = true;
        bufVolumePlayer = volumePlayer;
        volumePlayer = 0;
        WMP.settings.volume = volumePlayer;
    }
    public void UnMute()
    {
        if (isMute)
        {
            volumePlayer = bufVolumePlayer;
            WMP.settings.volume = volumePlayer;
        }
    }
    public void SkipTrack(int count) => WMP.controls.currentPosition += count;
}