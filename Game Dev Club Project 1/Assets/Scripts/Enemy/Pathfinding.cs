using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField] private int rayCount = 12;

    [Header("Target Settings")]
    [SerializeField] private string targetTag = "Enemy Target";
    [SerializeField] private string repelTag = "Enemy Repel";
    [SerializeField] private float movementWeight = 1f;
    [SerializeField] private float targetDistanceWeight = 1f;
    [SerializeField] private float repelWeight = 1f;

    private Vector3[] rayDirections;
    private float[] rayLengths;
    private Vector3 bestDirection;
    private float bestRayLength = 0f;

    private List<TargetData> targetsData = new List<TargetData>();
    private List<TargetData> obstaclesData = new List<TargetData>();

    private GameObject[] targets;
    private GameObject[] obstacles;

    private void Start()
    {
        InitializeRays();

        targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject target in targets)
            targetsData.Add(new TargetData(target.transform.position, targetDistanceWeight, target));

        obstacles = GameObject.FindGameObjectsWithTag(repelTag);
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != this.gameObject)
                obstaclesData.Add(new TargetData(obstacle.transform.position, repelWeight, obstacle));
        }
    }

    private void OnValidate()
    {
        InitializeRays();
    }

    private void InitializeRays()
    {
        rayDirections = new Vector3[rayCount];
        rayLengths = new float[rayCount];

        float angleStep = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;

            rayDirections[i] = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }
    }

    private void Update()
    {
        //Vector3 move = GetBestDir().normalized * 2f * Time.deltaTime;

        //transform.position += new Vector3(move.x, 0, move.z);
    }

    private Vector3 GetBestDir()
    {
        rayLengths = new float[rayCount];

        foreach (var target in targetsData)
        {
            Vector3 targetPos = target.targetGameObject != null
                ? target.targetGameObject.transform.position
                : target.position;

            Vector3 toTarget = targetPos - transform.position;
            toTarget.y = 0;

            float distance = toTarget.magnitude;
            float distanceWeight = targetDistanceWeight / Mathf.Clamp(distance / 2f, 0.5f, 3f);

            float attractionStart = 3f;
            float attractionEnd = 1f;
            float minNegative = -1f;

            if (distance < attractionStart)
            {
                float t = Mathf.Clamp01((distance - attractionEnd) / (attractionStart - attractionEnd));
                distanceWeight = distanceWeight * t + minNegative * (1f - t);
            }

            AddWeightedDirection(toTarget, distanceWeight, rayDirections, rayLengths, 2f);
        }

        foreach (var obstacle in obstaclesData)
        {
            Vector3 targetPos = obstacle.targetGameObject != null
                ? obstacle.targetGameObject.transform.position
                : obstacle.position;

            Vector3 toTarget = targetPos - transform.position;
            toTarget.y = 0;

            float distance = toTarget.magnitude;
            float distanceWeight = -repelWeight * (1f / distance);

            if (Mathf.Abs(distanceWeight) < 0.1f)
                distanceWeight = 0;

            AddWeightedDirection(toTarget, distanceWeight, rayDirections, rayLengths, 0.65f);
        }

        bestRayLength = 0f;
        bestDirection = Vector3.zero;

        for (int i = 0; i < rayCount; i++)
        {
            float length = rayLengths[i];
            if (length > bestRayLength)
            {
                bestRayLength = length;
                bestDirection = rayDirections[i];
            }
        }

        return bestDirection;
    }

    public void AddWeightedDirection(
        Vector3 targetDir,
        float weight,
        Vector3[] rayDirections,
        float[] rayLengths,
        float offset)
    {
        targetDir.Normalize();

        for (int i = 0; i < rayDirections.Length; i++)
        {
            float dot = Vector3.Dot(rayDirections[i].normalized, targetDir) * weight;

            if (offset == 2)
                rayLengths[i] += dot;
            else
                rayLengths[i] += 1 - Mathf.Abs(dot - offset);
        }
    }

    private void OnDrawGizmos()
    {
        if (rayDirections == null || rayDirections.Length != rayCount)
            InitializeRays();

        for (int i = 0; i < rayCount; i++)
        {
            float length = rayLengths[i];

            //regular rays
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position,
                transform.position + rayDirections[i] * (length / Mathf.Max(bestRayLength, 0.01f)));
        }

        //best ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + bestDirection * 1);

        Gizmos.color = new Color(0.3f, 0.8f, 0.8f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}

[System.Serializable]
public class TargetData
{
    public GameObject targetGameObject;
    public Vector3 position;
    public float weight;

    public TargetData(Vector3 position, float weight, GameObject targetGameObject)
    {
        this.position = position;
        this.weight = weight;
        this.targetGameObject = targetGameObject;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public void SetPos(Vector3 pos)
    {
        this.position = pos;
    }
}

