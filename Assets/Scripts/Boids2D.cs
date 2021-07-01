using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Boids2D : MonoBehaviour
{
//UI Elements
    private Slider repulsionRadius;
    private Slider repulsionConstant;
    private Slider alignmentRadius;
    private Slider alignmentConstant;
    private Slider cohesionRadius;
    private Slider cohesionConstant;
    private Slider numBoids;
    private Text repulsionRadiusText;
    private Text repulsionConstantText;
    private Text alignmentRadiusText;
    private Text alignmentConstantText;
    private Text cohesionRadiusText;
    private Text cohesionConstantText;
    private Text numBoidsText;

    private const float xborder = 3;
    private const float yborder = 1.5f;
    private int num_boids = 10;
    
    public GameObject target;
    private Vector2 kTarget;
    private float kTargetRadius;

    public GameObject avoid;
    private Vector2 kAvoidCenter;
    private float kAvoidSize;

    private float kDeltaT;
    private const float boidSize = 0.16f;
// Forces influence radius: repulsion < alignment < cohesion
    private float kRepulsionRadius = 1f;
    private float kAlignmentRadius = 3.5f;
    private float kCohesionRadius = 6f;

// Forces constants
    private float kRepulsionConstant = 1f;
    private float kAlignmentConstant = .3f;
    private float kCohesionConstant = .6f;
    private float kTargetRotation = 1f;
    private float kTargetCentripetal = 1f;

    public GameObject boids_prefab;
    private List<GameObject> boid = new List<GameObject>();

    private float updateTime = 0f;
/*
* Load and start sliders and instantiate/create boids
*/
    private void Start()
    {
        UILoad();
        SlidersStart();
        CreateBoids();
    }
/*
 * Load sliders and texts at runtime
 */
    void UILoad()
    {
        repulsionRadius = GameObject.Find("RepulsionR").GetComponent<Slider>();
        repulsionConstant = GameObject.Find("RepulsionK").GetComponent<Slider>();
        alignmentRadius = GameObject.Find("AlignmentR").GetComponent<Slider>();
        alignmentConstant = GameObject.Find("AlignmentK").GetComponent<Slider>();
        cohesionRadius = GameObject.Find("CohesionR").GetComponent<Slider>();
        cohesionConstant = GameObject.Find("CohesionK").GetComponent<Slider>();
        numBoids = GameObject.Find("NumBoids").GetComponent<Slider>();
        repulsionRadiusText = GameObject.Find("RepRadiusText").GetComponent<Text>();
        repulsionConstantText = GameObject.Find("RepConstText").GetComponent<Text>();
        alignmentRadiusText = GameObject.Find("AlignRadiusText").GetComponent<Text>();
        alignmentConstantText = GameObject.Find("AlignConstText").GetComponent<Text>();
        cohesionRadiusText = GameObject.Find("CoheRadiusText").GetComponent<Text>();
        cohesionConstantText = GameObject.Find("CoheConstText").GetComponent<Text>();
        numBoidsText = GameObject.Find("NumBoidsText").GetComponent<Text>();
    }
/*
* Sliders start value
*/
    void SlidersStart()
    {
        repulsionRadius.value = kRepulsionRadius;
        alignmentRadius.value = kAlignmentRadius;
        cohesionRadius.value = kCohesionRadius;

        repulsionConstant.value = kRepulsionConstant;
        alignmentConstant.value = kAlignmentConstant;
        cohesionConstant.value = kCohesionConstant;

        numBoids.value = num_boids;
    }
/*
 * Instantiate boids at random positions and velocities 
 */
    private void CreateBoids()
    {
        Vector2 pos;
        kDeltaT = Time.deltaTime;
        pos = new Vector2(Random.Range(-xborder, xborder), Random.Range(-yborder, yborder));
        target.transform.position = new Vector3(pos.x, pos.y, 0f);
        pos = new Vector2(Random.Range(-xborder, xborder), Random.Range(-yborder, yborder));
        avoid.transform.position = new Vector3(pos.x, pos.y, 0f);

        for (int i = 0; i < num_boids; i++)
        {
            boid.Add(GameObject.Instantiate(boids_prefab));
            pos = new Vector2(Random.Range(-xborder, xborder), Random.Range(-yborder, yborder));
            while (Vector2.SqrMagnitude(pos - kAvoidCenter) <= kAvoidSize)
            {
                pos = new Vector2(Random.Range(-xborder, xborder), Random.Range(-yborder, yborder));
            }
            boid[i].GetComponent<Fish>().SetPosition(pos);
            boid[i].GetComponent<Fish>().SetVelocity(Vector2.ClampMagnitude(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), 1f));
        }
    }
/*
 * Update is called once per frame
 */ 
    void FixedUpdate()
    {
        kTarget = target.transform.position;
        kTargetRadius = target.GetComponent<CircleCollider2D>().bounds.size.x / 2;
        kAvoidCenter = avoid.transform.position;
        kAvoidSize = avoid.GetComponent<CircleCollider2D>().bounds.size.x / 2;
        SlidersUpdate();
        MoveBoids();
    }
/*
* Update constant values by slider
*/
    void SlidersUpdate()
    {
        kRepulsionRadius = boidSize * repulsionRadius.value;
        repulsionRadiusText.text = "Repulsion Radius: " + repulsionRadius.value.ToString("F1");
        kAlignmentRadius = boidSize * alignmentRadius.value;
        alignmentRadiusText.text = "Alignment Radius: " + alignmentRadius.value.ToString("F1");
        kCohesionRadius = boidSize * cohesionRadius.value;
        cohesionRadiusText.text = "Cohesion Radius: " + cohesionRadius.value.ToString("F1");
        
        kRepulsionConstant = repulsionConstant.value;
        repulsionConstantText.text = "Repulsion Constant: " + repulsionConstant.value.ToString("F1");
        kAlignmentConstant = alignmentConstant.value;
        alignmentConstantText.text = "Alignment Constant: " + alignmentConstant.value.ToString("F1");
        kCohesionConstant = cohesionConstant.value;
        cohesionConstantText.text = "Cohesion Constant: " + cohesionConstant.value.ToString("F1");

        int temp_num_boids = Mathf.RoundToInt(numBoids.value);
        numBoidsText.text = "Number of boids: " + Mathf.RoundToInt(numBoids.value).ToString("F1");
        if (temp_num_boids != num_boids)
        {
            num_boids = temp_num_boids;
            foreach(GameObject Boids in boid)
            {
                Destroy(Boids.gameObject);
            }
            boid.Clear();
            CreateBoids();
        }
    }
/*
* Update boids positions, based on separation, alignment, cohesion and target forces.
* Bounds position to universe limits.
*/
    void MoveBoids()
    {
        Vector2 v1;
        Vector2 v2;
        Vector2 v3;
        Vector2 v4;
        Vector2 velRes;
        int i = 0;

        foreach (GameObject boids in boid)
        {
            if (updateTime <= 0)
            {
                v1 = RepulsionRule(boids, i);
                v2 = AlignmentRule(boids, i);
                v3 = CohesionRule(boids, i);
                v4 = TargetRule(boids);
                velRes = boids.GetComponent<Fish>().GetVelocity() + v1 + v2 + v3 + v4;
                velRes = Vector2.ClampMagnitude(velRes, 1f);
                velRes = RandomChange(velRes);
                velRes = AvoidObstacle(boids.GetComponent<Fish>().GetPosition(), velRes);
                updateTime = Time.deltaTime * 3;
            }
            else
            {
                velRes = boids.GetComponent<Fish>().GetVelocity();
            }
            updateTime -= Time.deltaTime;
            boids.GetComponent<Fish>().SetVelocity(Vector2.ClampMagnitude(velRes,1f));
            boids.GetComponent<Fish>().SetPosition(boids.GetComponent<Fish>().GetPosition() + boids.GetComponent<Fish>().GetVelocity() * kDeltaT);
            i++;
        }
    }

/*
* Calculate cohesion force for boid i
*/
    Vector2 CohesionRule(GameObject boids, int index)
    {
        Vector2 resultantVector = new Vector2(0, 0);
        Vector2 currentPosition = boids.GetComponent<Fish>().GetPosition();
        int numNeighbors = 0;

        for (int j = 0; j < num_boids; j++)
        {
            if (index == j) continue;
            Vector2 otherPosition = boid[j].GetComponent<Fish>().GetPosition();

            if (Mathf.Abs(Vector2.Distance(currentPosition, otherPosition)) < kCohesionRadius)
            {
                numNeighbors++;
                resultantVector += otherPosition;
            }
        }
        if (numNeighbors == 0) return resultantVector;
        resultantVector /= numNeighbors;
        resultantVector -= currentPosition;
        resultantVector *= kCohesionConstant;
        return resultantVector;
    }
/*
* Calculate separation force for boid i
*/
    Vector2 RepulsionRule(GameObject boids, int index)
    {
        Vector2 resultantVector = new Vector2(0, 0);
        Vector2 currentPosition = boids.GetComponent<Fish>().GetPosition();
        int numNeighbors = 0;

        for (int j = 0; j < num_boids; j++)
        {
            if (index == j) continue;
            Vector2 otherPosition = boid[j].GetComponent<Fish>().GetPosition();

            if (Mathf.Abs(Vector2.Distance(currentPosition, otherPosition)) < kRepulsionRadius && Mathf.Abs(Vector3.Distance(currentPosition, otherPosition)) > 0)
            {
                numNeighbors++;
                resultantVector += (currentPosition - otherPosition)/Mathf.Pow(Vector2.Distance(currentPosition, otherPosition), 2);
            }
        }
        if (numNeighbors == 0) return resultantVector;

        resultantVector *= kRepulsionConstant/numNeighbors;
        return resultantVector;
    }
/*
* Calculate alignment force for boid i
*/
    Vector2 AlignmentRule(GameObject boids, int index)
    {
        Vector2 resultantVector = new Vector2(0, 0);
        Vector2 currentPosition = boids.GetComponent<Fish>().GetPosition();
        Vector2 currentVelocity = boids.GetComponent<Fish>().GetVelocity();
        int numNeighbors = 0;

        for (int j = 0; j < num_boids; j++)
        {
            if (index == j) continue;
            Vector2 otherPosition = boid[j].GetComponent<Fish>().GetPosition();
            Vector2 otherVelocity = boid[j].GetComponent<Fish>().GetVelocity();
            if (Mathf.Abs(Vector2.Distance(currentPosition, otherPosition)) < kAlignmentRadius)
            {
                numNeighbors++;
                resultantVector += otherVelocity;
            }
        }

        if (numNeighbors == 0) return resultantVector;

        resultantVector *= kAlignmentConstant/numNeighbors;
        return resultantVector;
    }
/*
* Calculate target force for boid i.
* The target force pulls the boid to a circumference around targetCenter,
* plus a counter-clockwise rotation force
*/
    Vector2 TargetRule(GameObject boids)
    {
        Vector2 resultantVector = new Vector2(0, 0);
        Vector2 currentPosition = boids.GetComponent<Fish>().GetPosition();
        Vector2 currentVelocity = boids.GetComponent<Fish>().GetVelocity();
        Vector2 rotation;
        Vector2 centripetal;
        float angle = 90;

        centripetal = currentPosition - kTarget;
        if(Vector2.SqrMagnitude(centripetal) < kTargetRadius)
        {
            centripetal = -centripetal;
            angle = -angle;
        }
        centripetal = Vector2.ClampMagnitude(centripetal, 1f);

        rotation.x = centripetal.x * Mathf.Cos(angle) - centripetal.y * Mathf.Sin(angle);
        rotation.y = centripetal.x * Mathf.Sin(angle) + centripetal.y * Mathf.Cos(angle);
        resultantVector = rotation * kTargetRotation - centripetal * kTargetCentripetal;
        return resultantVector;
    }
/*
 * Calculate particle pseudo-random changes
 */ 
    Vector2 RandomChange(Vector2 vel)
    {
        Vector2 resultantVector = vel;
        float angle;

        if (Random.Range(0f, 1f) <= 0.5f) return resultantVector;
        angle = 60 * Random.Range(0f, 2f) - 30;
        resultantVector.x = vel.x * Mathf.Cos(angle) - vel.y * Mathf.Sin(angle);
        resultantVector.y = vel.x * Mathf.Sin(angle) + vel.y * Mathf.Cos(angle);
        return resultantVector;
    }
/*
* Change the velocity vector direction if there's an iminent
* collision with the obstacle.Rotates velocity vector
* the least ammount until collision is avoided.
*/
    Vector2 AvoidObstacle(Vector2 pos, Vector2 vel)
    {
        Vector2 resultantVector = vel;
        float angle = 15;
        int sign = 1;
        int cont = 0;
        Vector3 rotSign;
        Vector2 nextPosition = pos + vel * kDeltaT;
        Vector2 nextDistance = kAvoidCenter - nextPosition;
        if (Vector2.SqrMagnitude(nextDistance) > kAvoidSize) return resultantVector;
        rotSign = Vector3.Cross(new Vector3(nextDistance.x, nextDistance.y, 0f), new Vector3(vel.x, vel.y, 0f));
        if (Mathf.Sign(rotSign.z) < 0)
        {
            sign = -1;
        }
        while(Vector2.SqrMagnitude(nextDistance) < kAvoidSize && cont<10)
        {
            resultantVector.x = vel.x * Mathf.Cos(sign * angle) - vel.y * Mathf.Sin(sign * angle);
            resultantVector.y = vel.x * Mathf.Sin(sign * angle) + vel.y * Mathf.Cos(sign * angle);
            nextPosition = pos + vel * kDeltaT;
            nextDistance = kAvoidCenter - nextPosition;
            cont++;
        }
        return resultantVector;
    }
}