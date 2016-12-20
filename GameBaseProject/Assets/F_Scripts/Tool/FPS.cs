using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FPS : MonoBehaviour {

    public float updateInterval = 0.5f;
    public UIPanel panel = null;
    public UILabel label = null;
    public int depth = 2100;

    private float lastTime;
    public int frames = 0;
    static public FPS Instance { get; set; }
    private int lastFPSValue = 0;
    private Dictionary<int, string> fpsDict = new Dictionary<int, string>();
    void Start()
    {
        Instance = this;
        lastTime = Time.time;
        frames = 0;
        panel.depth = depth;
        for (int i = 1; i <= 60; ++i)
        {
            fpsDict.Add(i, i.ToString());
        }
    }

    void Update()
    {
        ++frames;
        float timeNow = Time.time;

        if (timeNow > lastTime + updateInterval)
        {
            int fps = (int)(frames / (timeNow - lastTime));
            frames = 0;
            lastTime = timeNow;

            if (lastFPSValue != (int)fps)
            {
                if (fpsDict.ContainsKey((int)fps))
                {
                    string fpsString = fpsDict[(int)fps];
                    if (fpsString != null)
                    {
                        //label.text = fpsString;
                        lastFPSValue = (int)fps;
                    }
                }
            }
        }
    }
}
