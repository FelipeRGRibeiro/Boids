using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predator : MonoBehaviour
{
    private const float xborder = 3;
    private const float yborder = 1.5f;
    public Slider speed;
    public Text speedText;
    private string backupText;
    private float DeltaT;

    private void Start()
    {
        backupText = speedText.text;
        DeltaT = Time.deltaTime;
    }

    float updateTime = 0;
    Vector3 increment = new Vector3(0f, 1f, 0f);
    Vector3 dir = new Vector3(0f, 1f, 0f);

    void FixedUpdate()
    {
        SlidersUpdate();
        if (updateTime <= 0)
        {
            increment = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            increment = Vector3.ClampMagnitude(increment, 1f);
            //OuterWolrds();
            //dir = -increment;
            //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //transform.position += increment * DeltaT;
            updateTime = 2;
        }
        OuterWolrds();
        dir = -increment;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position += increment * DeltaT;

        updateTime -= Time.deltaTime;
    }
    void SlidersUpdate()
    {
        DeltaT = speed.value * Time.deltaTime;
        speedText.text = backupText + " " + speed.value.ToString("F1");
    }
    void OuterWolrds()
    {
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
