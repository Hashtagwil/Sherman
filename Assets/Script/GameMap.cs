using Assets.Script.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameMap : MonoBehaviour
{
    private const int MAX_FLAG_SPAWN_ATTEMPTS = 50;

    public NetGame netGame;

    public GameObject Flag;
    public GameObject Zone;
    public Transform aiFovMin;
    public Transform aiFovMax;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SpawnFlagsAndZones()
    { 
        if (GameManager.SceneInstance.CurrentGameMode.isRequiresFlags())
        {
            Dictionary<Color, List<Vector2>> positionsPerTeam = new Dictionary<Color, List<Vector2>>();
            GameObject[] players = PlayerUtils.GetAllPlayer();
            foreach (var player in players)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                List<Vector2> positions = new List<Vector2>();
                if (positionsPerTeam.ContainsKey(pc.playerInfo.teamColor))
                {
                    positions = positionsPerTeam[pc.playerInfo.teamColor];
                }
                else
                {
                    positionsPerTeam.Add(pc.playerInfo.teamColor, positions);
                    netGame.InstantiateZonesFlagClientRpc(pc.tankController.transform.position, new Color(pc.playerInfo.teamColor.r, pc.playerInfo.teamColor.g, pc.playerInfo.teamColor.b));
                }
                positions.Add(player.transform.position);
            }

            foreach (Color team in positionsPerTeam.Keys)
            {
                List<Vector2> positions = positionsPerTeam[team];
                float averageX = 0;
                float averageY = 0;
                foreach (Vector2 position in positions)
                {
                    averageX += position.x;
                    averageY += position.y;
                }
                averageX = averageX / positions.Count;
                averageY = averageY / positions.Count;

                Vector3 spawnPosition = new Vector3(averageX, averageY, -5f);
                bool isOverlapping = true;
                int attempt = 0;
                int allLayersExceptFlags = ~LayerMask.GetMask("Flags");
                List<Vector3> spawnCandidates = new List<Vector3>();
                spawnCandidates.Add(spawnPosition);
                while (isOverlapping && spawnCandidates.Count > 0 && attempt < MAX_FLAG_SPAWN_ATTEMPTS)
                {
                    Vector3 positionTested = spawnCandidates[0];
                    Collider2D[] collidersOverlapping = Physics2D.OverlapBoxAll(positionTested, Flag.GetComponent<BoxCollider2D>().size * Flag.transform.localScale.x, 0, allLayersExceptFlags);
                    collidersOverlapping = UnityUtils.RemoveTriggersFromColliders(collidersOverlapping);
                    isOverlapping = (collidersOverlapping.Length != 0) ? true : false;
                    if (isOverlapping)
                    {
                        // To debug attempts...
                        //GameObject attemptFlag = Instantiate(Flag, positionTested, Quaternion.identity);
                        //Destroy(attemptFlag.GetComponent<Rigidbody2D>());
                        spawnCandidates.Remove(positionTested);
                        spawnCandidates.Add(new Vector3(positionTested.x + 0.2f, positionTested.y + 0.2f, positionTested.z));
                        spawnCandidates.Add(new Vector3(positionTested.x - 0.2f, positionTested.y + 0.2f, positionTested.z));
                        spawnCandidates.Add(new Vector3(positionTested.x + 0.2f, positionTested.y - 0.2f, positionTested.z));
                        spawnCandidates.Add(new Vector3(positionTested.x - 0.2f, positionTested.y - 0.2f, positionTested.z));
                        attempt++;
                    }
                    else
                    {
                        netGame.InstantiateFlagsServerRpc(positionTested, team);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateZonesFlag(Vector3 pos, Color color)
    {
        GameObject zone = Instantiate(Zone, pos, Quaternion.identity);
        color.a = zone.GetComponentInChildren<SpriteRenderer>().color.a;
        zone.GetComponentInChildren<SpriteRenderer>().color = color;
    }
}
