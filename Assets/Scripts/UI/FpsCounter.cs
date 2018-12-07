using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private const float FPS_UPDATE_INTERVAL = 0.5f;
    private float fpsAccum = 0;
    private int fpsFrames = 0;
    private float fpsTimeLeft = FPS_UPDATE_INTERVAL;
    private float fps = 0;

    private Text fpsCounter;

    private void Start()
    {
        fpsCounter = GetComponent<Text>();
    }

    void Update()
    {
        fpsTimeLeft -= Time.deltaTime;
        fpsAccum += Time.timeScale / Time.deltaTime;
        fpsFrames++;

        if (fpsTimeLeft <= 0)
        {
            fps = fpsAccum / fpsFrames;
            fpsTimeLeft = FPS_UPDATE_INTERVAL;
            fpsAccum = 0;
            fpsFrames = 0;
        }
        fpsCounter.text = Mathf.RoundToInt(fps).ToString() + " FPS";
    }
}