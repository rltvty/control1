using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    [SerializeField] public LevelMeter levelMeter;

    // Start is called before the first frame update
    private void Start()
    {
        float level = 1f;
        float change = -0.01f;
        FunctionPeriodic.Create(() =>
        {

            level += change;
            if (level <= 0f)
            {
                level = 0f;
                change = change * -1;
            }
            if (level >= 1f)
            {
                level = 1f;
                change = change * -1;
            }
            levelMeter.SetLevel(level);
        }, .03f);
    }

   
}
