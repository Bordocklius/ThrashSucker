using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TrashSucker.Presenters
{
    public class SpawnerBase: MonoBehaviour
    {
        public Transform Transform;

        public GameObject[] SpawnItems;
        public int SpawnCount = 0;
        public float SpawnRadius = 1f;
        public Shape Shape = Shape.Circle;
        public Vector2 BoxSize = new Vector2(0, 0);
        public Vector3 SpawnOffset = new Vector3(0, 0, 0);

        private void Start()
        {
            if (Transform == null)
                Transform = this.transform;
        }       

        public Vector3 FindValidSpawnPoint()
        {
            Vector3 randomPoint;

            switch (Shape)
            {
                case Shape.Circle:
                    randomPoint = Transform.position + Random.insideUnitSphere * SpawnRadius;
                    break;
                case Shape.Box:
                    float halfWidth = BoxSize.x * 0.5f;
                    float halfHeight = BoxSize.y * 0.5f;
                    randomPoint = Transform.position + new Vector3(Random.Range(-halfWidth, halfWidth), 0, Random.Range(-halfHeight, halfHeight));
                    break;
                default:
                    randomPoint = Transform.position;
                    break;
            }

            randomPoint.y = 3f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, SpawnRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            else
            {
                Debug.Log("Spawning in center");
                return Transform.position;
            }

        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            switch (Shape)
            {
                case Shape.Circle:
                    Gizmos.DrawWireSphere(Transform.position, SpawnRadius);
                    break;
                case Shape.Box:
                    Vector3 size = new Vector3(BoxSize.x, 0, BoxSize.y);
                    Gizmos.DrawWireCube(Transform.position, size);
                    break;
                default:
                    break;
            }
        }

    }

    public enum Shape
    {
        Circle,
        Box
    }
}
