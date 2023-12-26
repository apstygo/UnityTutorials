using System;
using UnityEngine;

public class Clock: MonoBehaviour {
    [SerializeField]
    Transform hoursPivot, minutesPivot, secondsPivot;

    void Update() {
        TimeSpan time = DateTime.Now.TimeOfDay;
        hoursPivot.localRotation = Quaternion.Euler(0, 0, -30 * (float)time.TotalHours);
        minutesPivot.localRotation = Quaternion.Euler(0, 0, -6 * (float)time.TotalMinutes);
        secondsPivot.localRotation = Quaternion.Euler(0, 0, -6 * (float)time.TotalSeconds);
    }
}
