using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library 
using System.Linq;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using Assets.Script;
using System;
using Assets.Script.Utils;

public class EnemyPathFindingController : MonoBehaviour
{
    private enum TargetType
    {
        Tank, Flag, TeamZone
    }
    private enum TargettedColor
    {
        DifferentFromMyTeam, SameHasMyTeam
    }

    private const float MAXIMUM_ANGLE_TO_TARGET_TOLERANCE = 30f;
    private const float RECALCULATE_INTERVAL = 3f;
    private const float DETECTION_RADIUS = 2f;
    private const float DISTANCE_TO_WAYPOINT_TOLERANCE = 0.4f;

    private int PathTargetIndex = 0;
    private NetPlayerAvatar netPlayerAvatar;

    public GameObject targetAquisitionAlgo;
    [Tooltip("The layers that the navigator can not pass through.")]
    public LayerMask obstacles;

    //Components
    private BoxCollider2D theCollider;

    private bool IsMoving = false;
    private bool IsRotating = false;
    private bool HasPath = false;
    //public bool HasFlag = false;

    public GameObject target;

    [Header("Navigator options")]
    public float gridSize = 0.5f; //increase patience or gridSize for larger maps
    Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")] 
    [SerializeField] bool searchShortcut = true; 
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid = false; 
    List <Vector2> path;
    List<Vector2> pathLeftToGo = new List<Vector2>();
    TankController tankController;

    void Awake()
    {
        theCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector2>(GetDistance,GetNeighbourNodes,1000); //increase patience or gridSize for larger maps
        theCollider = GetComponent<BoxCollider2D>();
        tankController = gameObject.GetComponentInChildren<TankController>();

        // Sometimes the tank can be stuck being unable to get to its target position so we
        // recompute a path every x seconds to avoid that.  Also permit to adjust direction faster when
        // moving towards a far position
        StartCoroutine(GetNewCommandEveryXSec());
        netPlayerAvatar = gameObject.GetComponentInParent<NetPlayerAvatar>();
    }

    IEnumerator GetNewCommandEveryXSec()
    {
        yield return new WaitForSeconds(RECALCULATE_INTERVAL);
        target = AcquireTarget(null);
        if (target != null)
        {
            ChangeTargetTo(target);
            GetMoveCommand((Vector2)target.transform.position);
            StartCoroutine(GetNewCommandEveryXSec());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get player tank as target
        if (target == null || UnityUtils.IsNullOrDestroyed(target))
        {
            target = AcquireTarget(null);
            if (target == null)
            {
                // No tank in sight!
                return;
            }
            //Debug.Log(gameObject.name + " target acquired.");
        }
        if (!tankController.hasFlag) CheckForCloserTarget();
        if (!IsMoving)
        {
            // Get the first step of a moving path
            GetMoveCommand((Vector2)target.transform.position);
        }

        // Check for shooting line of sight
        bool isEnemyInDirectLineOfSight = IsEnemyInDirectLineOfSight();
        
        if (isEnemyInDirectLineOfSight)
        {
            tankController.Fire();
        }
        tankController.TurnTurretTowardsPosition(target.transform.position);
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(transform.position, pathLeftToGo[PathTargetIndex]);
    }

    private void FixedUpdate()
    {
        // Controls rotation and movement
        if (HasPath && pathLeftToGo.Count > PathTargetIndex)
        {
            Vector3 dir = (Vector3)pathLeftToGo[PathTargetIndex] - transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            float diffAngle = Quaternion.Angle(transform.rotation, targetRotation);
            // Prorize rotation towards target vs moving...
            if (IsRotating || diffAngle > MAXIMUM_ANGLE_TO_TARGET_TOLERANCE)
            {
                IsRotating = true;
                RotateToward(targetRotation);
            }
            else
            {
                tankController.Move(0, 1);

                IsMoving = true;
                float distanceToTarget = GetDistanceToTarget();
                float distanceToWaypoint = Vector2.Distance((Vector2)transform.position, pathLeftToGo[PathTargetIndex]);
                if (distanceToWaypoint < DISTANCE_TO_WAYPOINT_TOLERANCE)
                {
                    if (PathTargetIndex == 0)
                    {
                        PathTargetIndex++;
                    }
                    else
                    {
                        pathLeftToGo.RemoveAt(0);
                        IsMoving = false;
                    }
                }
            }
        }

        netPlayerAvatar.UpdatePositionServerRpc(transform.position, transform.rotation);
    }

    private GameObject AcquireTarget(GameObject exceptThisObject)
    {
        GameMap map = GameObject.FindObjectOfType<GameMap>();
        Vector3 fovMin = map.aiFovMin.position;
        Vector3 fovMax = map.aiFovMax.position;

        Color myTeamColor = GetComponent<PlayerController>().playerInfo.teamColor;
        int layerMask = LayerMask.GetMask("Tanks");
        TargetType targetType = TargetType.Tank;
        TargettedColor targettedColor = TargettedColor.DifferentFromMyTeam;
        if (GameManager.SceneInstance.CurrentGameMode.isRequiresFlags())
        {
            if (tankController.hasFlag)
            {
                layerMask = LayerMask.GetMask("TeamZones");
                targetType = TargetType.TeamZone;
                targettedColor = TargettedColor.SameHasMyTeam;
            }
            else if (!PlayerUtils.IsTeamHasFlag(myTeamColor))
            {
                layerMask = LayerMask.GetMask("Flags");
                targetType = TargetType.Flag;
            }
            else 
            {
                //Debug.Log("Team has flag, targetting another tank...");
            }
        }

        Collider2D[] colliders = Physics2D.OverlapAreaAll(fovMin, fovMax, layerMask);

        GameObject target = null;
        if (colliders.Length > 0)
        {
            List<GameObject> exclusions = new List<GameObject>();
            exclusions.Add(this.gameObject);
            // Excludes teammates
            foreach (Collider2D collider in colliders)
            {
                Color colliderColor = Color.black;
                if (targetType == TargetType.Tank)
                {
                    PlayerController colliderPlayerController = collider.GetComponent<PlayerController>();
                    colliderColor = colliderPlayerController.playerInfo.teamColor;
                }
                else if (targetType == TargetType.Flag || targetType == TargetType.TeamZone)
                {
                    colliderColor = collider.GetComponentInChildren<SpriteRenderer>().color;
                }
                bool isColliderSameColorAsMyTeam = UnityUtils.IsSameColorIgnoringAlpha(colliderColor, myTeamColor);
                if (targettedColor == TargettedColor.SameHasMyTeam && !isColliderSameColorAsMyTeam ||
                    targettedColor == TargettedColor.DifferentFromMyTeam && isColliderSameColorAsMyTeam)
                {
                    exclusions.Add(collider.gameObject);
                }
            }
            if (exceptThisObject != null) exclusions.Add(exceptThisObject);
            target = targetAquisitionAlgo?.GetComponent<ITargetAcquisitionAlgo>().AcquireTarget(this.gameObject, exclusions, colliders);
        }
        return target;
    }

    private bool IsEnemyInDirectLineOfSight()
    {
        Vector2 vectorTowardsTargetTank = target.transform.position - gameObject.transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, vectorTowardsTargetTank.normalized, vectorTowardsTargetTank.magnitude);
        bool hitObstacle = false;
        GameObject hitEnemyTank = null;
        int i = 0;
        while (!hitObstacle && i < hits.Length)
        {
            RaycastHit2D hit = hits[i];
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit != gameObject && objectHit.GetComponentInChildren<TankController>() != null)
            {
                hitEnemyTank = objectHit;
            }
            if (hit.collider.gameObject != gameObject && objectHit.GetComponentInChildren<TankController>() == null)
            {
                hitObstacle = true;
            }
            i++;
        }

        return (hitEnemyTank != null && !hitObstacle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathLeftToGo.Count - 1; i++) 
        {
            Gizmos.DrawLine(pathLeftToGo[0], pathLeftToGo[1]);
        }
        Gizmos.DrawWireSphere(transform.position, gridSize);
        if (pathLeftToGo.Count > 1)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(pathLeftToGo[PathTargetIndex], new Vector3(0.2f, 0.2f, 0.2f));
        }
        Gizmos.DrawWireSphere(transform.position, DETECTION_RADIUS);

        GameMap map = GameObject.FindObjectOfType<GameMap>();
        Vector3 fovMin = map.aiFovMin.position;
        Vector3 fovMax = map.aiFovMax.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(fovMin, fovMax);

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.transform.position, transform.position);
        }
    }

    private void RotateToward(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * tankController.turnSpeed);

        if (transform.rotation == targetRotation)
            IsRotating = false;
    }

    private void CheckForCloserTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, DETECTION_RADIUS);
        GameObject hitPowerUp = null;
        GameObject hitEnemyTank = null;
        int i = 0;
        while (!hitPowerUp && i < hits.Length)
        {
            Collider2D hit = hits[i];
            GameObject objectHit = hit.gameObject;
            if (objectHit != gameObject && objectHit.GetComponentInChildren<TankController>() != null)
            {
                if (objectHit.GetComponent<PlayerController>().playerInfo.teamColor != GetComponent<PlayerController>().playerInfo.teamColor)
                {
                    hitEnemyTank = objectHit;
                }
            }
            if (hit.gameObject != gameObject && objectHit.CompareTag("PowerUp"))
            {
                hitPowerUp = objectHit;
            }
            i++;
        }
        if (hitPowerUp)
        {
            ChangeTargetTo(hitPowerUp);
        }
        else if (hitEnemyTank)
        {
            ChangeTargetTo(hitEnemyTank);
        }
    }

    private void ChangeTargetTo(GameObject newTarget)
    {
        HasPath = false;
        IsMoving = false;
        target = newTarget;
    }

    void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            if (searchShortcut && path.Count>0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }
            //path.Insert(0, transform.position);
            HasPath = true;
        }
        else
        {
            // Algo find no path, we go straight towards target!
            path = new List<Vector2>();
            path.Add(transform.position);
            path.Add(target);
            HasPath = true;
        }
        PathTargetIndex = 0;
    }

    /// <summary>
    /// Finds closest point on the grid
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector2 GetClosestNode(Vector2 target) 
    {
        return new Vector2(Mathf.Round(target.x/gridSize)*gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }

    /// <summary>
    /// A distance approximation. 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    float GetDistance(Vector2 A, Vector2 B) 
    {
        return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
    }

    /// <summary>
    /// Finds possible conenctions and the distances to those connections on the grid.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Dictionary<Vector2,float> GetNeighbourNodes(Vector2 pos) 
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i=-1;i<2;i++)
        {
            for (int j=-1;j<2;j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j)*gridSize;
                if (!Physics2D.Linecast(pos,pos+dir, obstacles))
                {
                    neighbours.Add(GetClosestNode( pos + dir), dir.magnitude);
                }
            }

        }
        return neighbours;
    }

    
    List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();
        
        for (int i=0;i<path.Count;i++)
        {
            newPath.Add(path[i]);
            for (int j=path.Count-1;j>i;j-- )
            {
                if (!Physics2D.Linecast(path[i],path[j], obstacles))
                {
                    
                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
}
