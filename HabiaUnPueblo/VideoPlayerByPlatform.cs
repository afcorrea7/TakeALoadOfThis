//Andrés Correa 2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//WebGL builds can't handle .mp4 files, this script handles this issue through the StreamingAssets solution
public class VideoPlayerByPlatform : MonoBehaviour
{
    [Tooltip("For playing on PC or Editor")]
    [SerializeField] VideoClip clipAsset;
    [Tooltip("For playing on WebGL")]
    [SerializeField] string clipFileName;
    
    private VideoPlayer thisPlayer;

    void Start(){
        thisPlayer = GetComponent<VideoPlayer>();
        PlayVideoByPlatform();
    }

    void PlayVideoByPlatform(){ //Choose how the video player will recieve its clip based on what platform we are on
        #if UNITY_EDITOR || PLATFORM_STANDALONE
        SetVideoWithClip();
        #endif
        #if PLATFORM_WEBGL
        SetVideoWithURL();
        #endif

    }

    void SetVideoWithClip(){
        thisPlayer.source = VideoSource.VideoClip;
        thisPlayer.clip = clipAsset;
        thisPlayer.Play();
    }

    void SetVideoWithURL(){
        thisPlayer.source = VideoSource.Url;
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, clipFileName+".mp4");
        thisPlayer.url = videoPath;
        thisPlayer.Play();
    }
}