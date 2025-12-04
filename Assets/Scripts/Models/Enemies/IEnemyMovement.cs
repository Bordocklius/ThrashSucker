using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThrashSucker.Models.Enemies
{
    public interface IEnemyMovement
    {
        // Request positions
        public Vector3 GetSelfPosition();
        public Vector3 GetTargetPosition();

        // Request movement
        public void RequestMoveTo(Vector3 destination);
        public bool TryPickRandomNavMeshPoint(float radius, out Vector3 point);

        // Arival
        public bool IsAtDestination();
        public float GetDistanceToPlayer();
    }
}
