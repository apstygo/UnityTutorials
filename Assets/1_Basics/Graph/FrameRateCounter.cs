using TMPro;
using UnityEngine;

public class FrameRateCounter: MonoBehaviour {
    public enum DisplayMode { 
        FPS,
        MS
    }

    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1;

    [SerializeField]
    DisplayMode displayMode = DisplayMode.FPS;

    private int currentSampleFrames;
    private float currentSampleDuration;
    private float currentBestDuration = float.MaxValue;
    private float currentWorstDuration;

    void Update() {
        float frameDuration = Time.unscaledDeltaTime;
        currentSampleDuration += frameDuration;
        currentSampleFrames += 1;

        currentBestDuration = Mathf.Min(currentBestDuration, frameDuration);
        currentWorstDuration = Mathf.Max(currentWorstDuration, frameDuration);

        if (currentSampleDuration >= sampleDuration) {
            switch (displayMode) {
                case DisplayMode.FPS:
                    display.SetText(
                        "FPS\n{0:0}\n{1:0}\n{2:0}", 
                        currentSampleFrames / currentSampleDuration,
                        1 / currentWorstDuration,
                        1 / currentBestDuration
                    );
                    break;

                case DisplayMode.MS:
                    display.SetText(
                        "MS\n{0:1}\n{1:1}\n{2:1}", 
                        currentSampleDuration / currentSampleFrames * 1000,
                        currentWorstDuration * 1000,
                        currentBestDuration * 1000
                    );
                    break;
            }
            
            currentSampleFrames = 0;
            currentSampleDuration = 0;
            currentWorstDuration = 0;
            currentBestDuration = float.MaxValue;
        }
    }
}
