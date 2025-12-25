using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace TrashSucker.Presenters
{
    public class TrashSpawner: SpawnerBase
    {
        private void Start()
        {            
            if (Transform == null)
                Transform = this.transform;

            StartCoroutine(SpawnThrash());
        }

        private IEnumerator SpawnThrash()
        {
            for(int i = 0; i < SpawnCount; i++)
            {             
                Vector3 randomPoint = FindValidSpawnPoint();
                if (randomPoint.y <= 0)
                    Debug.Log(randomPoint.y);
                GameObject itemToSpawn = Instantiate(SpawnItems[Random.Range(0, SpawnItems.Length)], Transform);
                itemToSpawn.transform.position = randomPoint + SpawnOffset;                
            }
            yield return null;
        }        

    }
}
