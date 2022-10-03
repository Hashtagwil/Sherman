using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script
{
    [RequireComponent(typeof(NetworkBehaviour))]
    public class NetCarcass : NetworkBehaviour
    {
        [SerializeField] GameObject[] Carcass;
        public int IndexCarcass;
        public Color BodyColor;
        public Color TurretColor;

        public Transform TankTransform;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                InstantiateClientRpc(IndexCarcass, BodyColor, TurretColor);
            }
        }

        private void Update()
        {
            if (IsServer)
            {
                SetPositionClientRpc(TankTransform.position, TankTransform.rotation);
            }
        }

        [ClientRpc]
        void InstantiateClientRpc(int indexCarcass, Color bodyColor, Color turretColor)
        {
            IndexCarcass = indexCarcass;
            BodyColor = bodyColor;
            TurretColor = turretColor;

            InstantiateCarcass();
        }

        private void InstantiateCarcass()
        {
            GameObject DeadTank = Instantiate(Carcass[IndexCarcass], transform);
            UnityUtils.SetLayerAllChildren(DeadTank.transform, 0);
            DeadTank.layer = 0;
            TankController DeadController = DeadTank.GetComponentInChildren<TankController>();
            DeadController.SetBodyColor(BodyColor);
            DeadController.SetTurretColor(TurretColor);
            DeadController.smokeParticleCritical.Play();
            DeadController.PlayerController = null;
            TankTransform = DeadController.transform;
            Destroy(DeadController);
        }

        [ClientRpc]
        void SetPositionClientRpc(Vector3 position, Quaternion rotation)
        {
            TankTransform.SetPositionAndRotation(position, rotation);
        }
    }
}
