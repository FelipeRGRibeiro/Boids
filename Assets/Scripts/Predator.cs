using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predator : MonoBehaviour
{
//UI Elements
    public Slider speed;
    public Text speedText;
    

    private string backupText;
    private float kDeltaT;
    private float updateTime = 0;
    private Vector3 increment = new Vector3(0f, 1f, 0f);
    private Vector3 dir = new Vector3(0f, 1f, 0f);
        
    private void Start()
    {
        UILoad();
        kDeltaT = Time.deltaTime;
    }

    void FixedUpdate()
    {
        SlidersUpdate();
        Move();
    }
/*
 * Load sliders and texts at runtime
 */
    private void UILoad()
    {
        if(this.name == "Avoid")
        {
            speed = GameObject.Find("AvoidS").GetComponent<Slider>();
            speedText = GameObject.Find("AvoidSText").GetComponent<Text>();
        }
        else if(this.name == "Target")
        {
            speed = GameObject.Find("TargetS").GetComponent<Slider>();
            speedText = GameObject.Find("TargetSText").GetComponent<Text>();
        }
        backupText = speedText.text;
    }
/*
* Update speed value by slider
*/
    void SlidersUpdate()
    {
        kDeltaT = speed.value * Time.deltaTime;
        speedText.text = backupText + " " + speed.value.ToString("F1");
    }
/*
 * Change position 
 */
    private void Move()
    {
        if (updateTime <= 0)
        {
            increment = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            increment = Vector3.ClampMagnitude(increment, 1f);
            updateTime = 2;
        }
        OuterWorlds();
        dir = -increment;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position += increment * kDeltaT;
        updateTime -= Time.deltaTime;
    }
/*
 * World's limits treatment
 */
    private void OuterWorlds()
    {
        const float xborder = 3;
        const float yborder = 1.5f;
    
        if (Mathf.Abs(transform.position.x + increment.x) >= xborder)
        {
            increment.x = -increment.x;
        }
        if (Mathf.Abs(transform.position.y + increment.y) >= yborder)
        {
            increment.y = -increment.y;
        }
    }
}