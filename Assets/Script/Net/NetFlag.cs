using System;
using System.Collections;
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
    public class NetFlag : NetworkBehaviour
    {
        public bool Attached = false;
        public TankController attachedTankController;
        Vector3 initialPosition;
        public Color _color;
        public int DebugPhysicOwner;
        private ulong _tankManagingFlagPhysics;
        public ulong TankManagingFlagPhysics
        {
            get { return _tankManagingFlagPhysics; }
            set {
                _tankManagingFlagPhysics = value;
                DebugPhysicOwner = (int)_tankManagingFlagPhysics;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                InitializeClientRpc(_color);
                TankManagingFlagPhysics = NetworkManager.Singleton.LocalClientId;
            }

            initialPosition = transform.position;
        }

        [ClientRpc]
        private void InitializeClientRpc(Color color)
        {
            _color = color;
            gameObject.GetComponentInChildren<SpriteRenderer>().color = color;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TankController tc = collision.gameObject.GetComponent<TankController>();
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();

            if (!Attached && tc != null && !IsOurTeamFlag(tc) && tc.netPlayerAvatar.IsOwner)
            {
                Vector2 newPosition2D = collision.GetContact(0).point;
                transform.position =  new Vector3(newPosition2D.x, newPosition2D.y, transform.position.z);
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                AttacheFlagServerRpc(tc.PlayerController.playerInfo.Player, collision.GetContact(0).point);
                AttachFlag(tc.PlayerController.playerInfo.Player, newPosition2D);
                TransferOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
                TankManagingFlagPhysics = NetworkManager.Singleton.LocalClientId;
            }
            else if (!Attached && tc != null && tc.netPlayerAvatar.IsOwner && IsOurTeamFlag(tc))
            {
                TransferOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
                TankManagingFlagPhysics = NetworkManager.Singleton.LocalClientId;
            }
            else if (!Attached && projectile != null && projectile.Owner.netPlayerAvatar.IsOwner)
            {
                if (TankManagingFlagPhysics != NetworkManager.Singleton.LocalClientId)
                {
                    // Physics was controlled by another client, need to apply a force manually the first time
                    // as collision will not be processed because of Static rigid body
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    Vector2 dir = collision.contacts[0].point - (Vector2)transform.position;
                    dir = -dir.normalized;
                    GetComponent<Rigidbody2D>().AddForce(collision.relativeVelocity * dir);
                }
                TransferOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
                TankManagingFlagPhysics = NetworkManager.Singleton.LocalClientId;
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsFlagAttachedToThisGameInstanceTank() && collision.gameObject.CompareTag("TeamZone"))
            {
                int player = attachedTankController.netPlayerAvatar._playerInfo.Player;
                Color colliderColor = collision.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                if (!UnityUtils.IsSameColorIgnoringAlpha(colliderColor, GetComponentInChildren<SpriteRenderer>().color))
                {
                    attachedTankController = null;
                    FlagDepositServerRpc(player);
                }
            }
        }

        private void Update()
        {
            if (Attached && UnityUtils.IsNullOrDestroyed(attachedTankController))
            {
                // Flag holder was destroyed!
                ResetFlagState(false);
            }

            if (TankManagingFlagPhysics == NetworkManager.Singleton.LocalClientId)
            {
                SetPositionServerRpc(transform.position, transform.rotation);
            }
        }

        private bool IsOurTeamFlag(TankController tankController)
        {
            bool teamFlag = false;
            if (tankController.PlayerController.playerInfo.teamColor == GetComponentInChildren<SpriteRenderer>().color)
            {
                teamFlag = true;
            }
            return teamFlag;
        }

        private bool IsFlagAttachedToThisGameInstanceTank()
        {
            return attachedTankController != null && attachedTankController.netPlayerAvatar._playerInfo.NetPlayer == NetworkManager.Singleton.LocalClientId;
        }

        [ServerRpc(RequireOwnership = false)]
        private void TransferOwnershipServerRpc(ulong ownerId)
        {
            TransferOwnershipClientRpc(ownerId);
        }

        [ClientRpc]
        private void TransferOwnershipClientRpc(ulong ownerId)
        {
            TankManagingFlagPhysics = ownerId;
            if (NetworkManager.Singleton.LocalClientId != ownerId)
            {
                // We switch the body type of non-controlling clients to static to avoid their physics to mess with the position assignments from the server
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
            else
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPositionServerRpc(Vector3 pos, Quaternion rot)
        {
            SetPositionClientRpc(pos, rot);
        }

        [ClientRpc]
        private void SetPositionClientRpc(Vector3 pos, Quaternion rot)
        {
            if ((TankManagingFlagPhysics != NetworkManager.Singleton.LocalClientId))
            {
                transform.SetPositionAndRotation(pos, rot);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AttacheFlagServerRpc(int player, Vector2 attachPoint)
        {
            AttacheFlagClientRpc(player, attachPoint);
        }

        [ClientRpc]
        private void AttacheFlagClientRpc(int player, Vector2 attachPoint)
        {
            AttachFlag(player, attachPoint);
        }

        private void AttachFlag(int player, Vector2 attachPoint)
        { 
            TankController tc = FindObjectsOfType<TankController>().FirstOrDefault(x => x.PlayerController.playerInfo.Player == player);
            if (tc != null && this.GetComponent<HingeJoint2D>() == null)
            {
                HingeJoint2D joint = this.AddComponent<HingeJoint2D>();
                joint.connectedBody = tc.gameObject.GetComponent<Rigidbody2D>();
                joint.enableCollision = true;
                transform.position = new Vector3(attachPoint.x, attachPoint.y, transform.position.z);
                attachedTankController = tc;
                attachedTankController.FlagTaken();
                Attached = true;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void FlagDepositServerRpc(int player)
        {
            FlagDepositClientRpc(player);
            FindObjectOfType<NetGame>().IncreaseFlagsCapturedClientRpc(player);
        }

        [ClientRpc]
        private void FlagDepositClientRpc(int player)
        {
            TankController tc = FindObjectsOfType<TankController>().FirstOrDefault(x => x.PlayerController.playerInfo.Player == player);
            if (tc != null)
            {
                tc.FlagCaptured();
                ResetFlagState(true);
            }
        }

        private void ResetFlagState(bool resetToOriginalPosition)
        {
            Destroy(GetComponent<HingeJoint2D>());
            Attached = false;
            attachedTankController = null;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if (resetToOriginalPosition) transform.position = initialPosition;
            TankManagingFlagPhysics  = 0;
        }
    }
}
