using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private Vector2 velocity;
    
    public Vector2 GetVelocity()
    {
        return velocity;
    }
    public void SetVelocity(Vector2 vel)
    {
        velocity = vel;
    }
    public Vector2 GetPosition()
    {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }
    public void SetPosition(Vector2 pos)
    {
        SetAngle(pos-GetPosition());
        this.transform.position = new Vector3(pos.x, pos.y, 0);
    }
    public void SetAngle(Vector2 pos)
    {
        pos = Vector2.ClampMagnitude(pos, 1f);
        if (pos != Vector2.zero)
        {
            Vector3 dir = pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

}
