using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using deVoid.Utils;
using System;

// the slider must call BeginScrub, Scrub and EndScrub!

public class VideoContentContainer : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Slider[] seekSliders;
    [SerializeField] private Text[] elapsedTimeTexts;
    [SerializeField] private Text[] totalTimeTexts;
    [SerializeField] private GameObject[] shownWhilePause;
    [SerializeField] private GameObject[] shownWhilePlay;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    VideoPlayer videoPlayer;
    bool wasPlayingOnScrub = false;
    bool isScrubbing = false;
    bool isInitialized = false;

    private Coroutine waitingUntilPlaying;

    bool _showControls;
    public bool showControls { 
        set 
        {
            foreach(var seekSlider in seekSliders)
                seekSlider.gameObject.SetActive(value);
            foreach(var o in shownWhilePause)
                o.SetActive(value);
            foreach(var o in shownWhilePlay)
                o.SetActive(value);

            _showControls = value;
        }
        get { return _showControls; }
    }

    void Awake() {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        videoPlayer.errorReceived += (player, error) => {
            InitializePlay();
            // Should seek to current place (but that makes the problem much worse)
            // this is due to an error I keep getting and cant find a workaround for
            // https://answers.unity.com/questions/1704521/videoplayer-error-on-seek.html
        };

        Signals.Get<ScrubVideoSignal>().AddListener(DoScrub);
        Signals.Get<PlayVideoSignal>().AddListener(DoPlay);
        Signals.Get<PauseVideoSignal>().AddListener(DoPause);
    }

    public void ShowContent(VideoFile file) {
        var url = file.url;
        if(App.cache.HasCached(url))
            url = App.cache.GetCachedPath(url);

        rawImage.color = Color.gray;
        videoPlayer.url = url;
        isInitialized = false;
        DoPlay();
    }

    public void Pause() {
        App.guideSync.PauseVideo();
        App.guideSync.SetVideoTime(videoPlayer.time);
    }

    public void Play() {
        App.guideSync.PlayVideo();
    }

    void InitializePlay() {

        if (waitingUntilPlaying != null)
            StopCoroutine(waitingUntilPlaying);
        videoPlayer.Play();
        waitingUntilPlaying = StartCoroutine(WaitUntilPlaying().Then(Initialize));
    }

    // Workaround for flickering at beginning of frame
    // https://stackoverflow.com/questions/49639033/rawimage-flicker-when-changing-from-texture2d-to-videoplayer-texture
    IEnumerator WaitUntilPlaying() {
        yield return new WaitUntil(() => videoPlayer.frame > 1);
    }

    void Initialize() { 
        rawImage.texture = videoPlayer.texture;
        rawImage.color = Color.white;

        foreach (var seekSlider in seekSliders)
        {
            seekSlider.minValue = 0;
            seekSlider.maxValue = (float)videoPlayer.length;
        }

        isInitialized = true;
        aspectRatioFitter.aspectRatio = (float)videoPlayer.texture.width / (float)videoPlayer.texture.height;
    }

    public void BeginScrub() {
        wasPlayingOnScrub = videoPlayer.isPlaying;
        isScrubbing = true;
        
        App.guideSync.PauseVideo();
    }

    public void Scrub(float value) {
        App.guideSync.SetVideoTime(value);
    }

    public void EndScrub() {
        if(wasPlayingOnScrub)
            App.guideSync.PlayVideo();
        isScrubbing = false;
    }

    void DoPlay()
    {
        if (!isInitialized)
            InitializePlay();
        else
            videoPlayer.Play();
    }

    void DoPause()
    {
        videoPlayer.Pause();
    }

    void DoScrub(double time)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.time = time;
    }

    void Update() {
        if(showControls)
        {
            if(!isScrubbing && videoPlayer.isPlaying)
                foreach(var seekSlider in seekSliders)
                        seekSlider.SetValueWithoutNotify((float) videoPlayer.clockTime);
            
            var showPause = (videoPlayer.isPaused && !isScrubbing) || (isScrubbing && !wasPlayingOnScrub);
                foreach(var o in shownWhilePause)
                    o.SetActive(showPause);

            foreach(var o in shownWhilePlay)
                    o.SetActive(!showPause);

            // a warning: do not use C# ? null checks on editor-assigned game objects
            // https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/?_ga=2.211228565.1230526425.1583864158-955451264.1581701711
            foreach(var elapsedTime in elapsedTimeTexts)
                elapsedTime.text = Helpers.TimeSpanToString(TimeSpan.FromSeconds(videoPlayer.clockTime));

            foreach(var totalTime in totalTimeTexts)
                totalTime.text = Helpers.TimeSpanToString(TimeSpan.FromSeconds(videoPlayer.length));
        }
    }
}