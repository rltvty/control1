using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMeter : MonoBehaviour
{
    private Transform bar;
    private SpriteRenderer barSprite;
    private Vector3 level;

    // Start is called before the first frame update
    private void Awake()
    {
        bar = transform.Find("Bar");
        barSprite = bar.Find("BarSprite").GetComponent<SpriteRenderer>();
        level = new Vector3(0f, 1f);

    }

    public void SetLevel(float normalizedLevel)
    {
        level.x = normalizedLevel;
        bar.localScale = level;
        if (normalizedLevel < 0.5f)
        {
            barSprite.color = Color.green;
        } 
        else if (normalizedLevel < 0.9f)
        {
            barSprite.color = Color.cyan;
        }
        else
        {
            barSprite.color = Color.red;
        }
    }
}
