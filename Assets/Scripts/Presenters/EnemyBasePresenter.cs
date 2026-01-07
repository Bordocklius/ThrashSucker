using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TrashSucker.Models;
using TrashSucker.Models.Enemies;
using TrashSucker.Presenters;
using TrashSucker.Singleton;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    [SelectionBase]
    public class EnemyBasePresenter : PresenterBaseClass<Enemybase>, IEnemyMovement
    {
        public bool IsActive;

        [Header("Movement")]
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private Transform _playerTransform;
        [SerializeField]
        private float _movementSpeed;
        [SerializeField]
        private float _randomRadius;
        [SerializeField]
        private float _detectionEnterRadius;
        [SerializeField]
        private float _detectionExitRadius;
        [SerializeField]
        private float _minWanderDistance;
        [SerializeField]
        private float _wanderTime;
        public NavMeshAgent NavMeshAgent;
        
        

        [Space(10)]
        [Header("Enemy stats")]
        public float StartingHP;
        public float Damage;
        public float KnockbackStrength = 8f;

        public List<MaterialType> EnemyWeaknesses;
        public List<MaterialType> EnemyResistances;

        private float _healthProgress => Model.Health / StartingHP;

        [Space(10)]
        [Header("Visual")]
        [SerializeField]
        private MeshRenderer _meshRenderer;        
        [SerializeField]
        private Material _flashMaterial;
        [SerializeField]
        private float _flashTime;
        [SerializeField]
        private ParticleSystem _bloodParticles;
        private Material _enemyMaterial;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _damageClip;
        [SerializeField]
        private Image _healthBar;

        private Coroutine _flashDamage;

        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Enemybase.Health)))
            {
                UpdateHealthBar();
                if (Model.Health <= 0)
                    Model_OnEnemyDeath();
            }
        }

        protected override void ModelSetInitialisation(Enemybase previousModel)
        {
            //base.ModelSetInitialisation(previousModel);
            //if (previousModel != null)
            //{
            //    previousModel.TTLExpired -= OnTTLExpired;
            //}
            //Model.TTLExpired += OnTTLExpired;
        }

        private void Awake()
        {
            Model = new Enemybase(EnemyWeaknesses, EnemyResistances, StartingHP, Damage, this, _randomRadius);
            if (_playerTransform == null)
            {
                _playerTransform = GameObject.Find("Player").transform;
            }

            NavMeshAgent.speed = _movementSpeed;
            Model.DetectionEnterRadius = _detectionEnterRadius;
            Model.DetectionExitRadius = _detectionExitRadius;
            Model.MinTargetDistance = _minWanderDistance;
            Model.WanderTimeout = _wanderTime;
            NavMeshAgent.enabled = false;

            if(_meshRenderer == null)
                _meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            _enemyMaterial = _meshRenderer.material;
        }

        private void Start()
        {
            if(!IsActive)
            {
                Model.IsActive = false;
            }
            else
                Model.IsActive = true;
            UpdateHealthBar();
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                NavMeshAgent.Warp(hit.point);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            //if(Vector3.Distance(NavMeshAgent.transform.position, _playerTransform.position) <= 20)
            //{
            //    NavMeshAgent.destination = _playerTransform.position;            
            //}

            //_transform.position = Vector3.MoveTowards(_transform.position, _playerTransform.position, _movementSpeed * Time.deltaTime);
        }


        public void DamageEnemy(SuckableObject suckableObject, bool playSound)
        {
            _bloodParticles.Play();
            if(playSound)
                _audioSource.PlayOneShot(_damageClip);

            Model.OnEnemyShot(suckableObject);
            _flashDamage = StartCoroutine(FlashDamage());
        }

        public void DamageEnemy(float damage, bool playSound)
        {
            _bloodParticles.Play();
            if (playSound)
                _audioSource.PlayOneShot(_damageClip);

            Model.DealDamage(damage);
            _flashDamage = StartCoroutine(FlashDamage());
        }

        private IEnumerator FlashDamage()
        {            
            _meshRenderer.material = _flashMaterial;
            yield return new WaitForSeconds(_flashTime);
            _meshRenderer.material = _enemyMaterial;
        }

        public virtual void Model_OnEnemyDeath()
        {
            _bloodParticles.Play();
            NavMeshAgent.enabled = false;
            StartCoroutine(DeathAnimation());
        }

        private IEnumerator DeathAnimation()
        {
            while(!_bloodParticles.isStopped)
            {
                yield return null;
            }
            Singleton<GameManager>.Instance.Enemies.Remove(this.gameObject);
            Destroy(this.gameObject);
        }

        private void OnDrawGizmos()
        {
            if(Vector3.Distance(transform.position, _playerTransform.position) <= _detectionEnterRadius)
                Gizmos.DrawLine(NavMeshAgent.transform.position, _playerTransform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionEnterRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _detectionExitRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _randomRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, NavMeshAgent.destination);
        }

        private void UpdateHealthBar() => _healthBar.fillAmount = _healthProgress;

        #region IEnemyMovement implementation
        // Request positions
        public Vector3 GetSelfPosition()
        {
            return NavMeshAgent.transform.position;
        }

        public Vector3 GetTargetPosition()
        {
            return _playerTransform.position;
        }

        // Request movement
        public void RequestMoveTo(Vector3 destination)
        {
            NavMeshAgent.destination = destination;
        }

        public bool TryPickRandomNavMeshPoint(float radius, out Vector3 point)
        {
            const int maxAttempts = 10;
            const float sampleSearchRadius = 1.5f;

            Vector3 origin = GetSelfPosition();

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * radius;
                Vector3 candidate = origin + new Vector3(randomOffset.x, 0f, randomOffset.z);

                NavMeshHit hit;
                if (!NavMesh.SamplePosition(candidate, out hit, sampleSearchRadius, NavMesh.AllAreas))
                    continue;

                // Validate reachability using NavMesh path calculation
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(NavMeshAgent.transform.position, hit.position, NavMesh.AllAreas, path)
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    point = hit.position;
                    return true;
                }
            }

            point = Vector3.zero;
            return false;
        }

        // Arival
        public bool IsAtDestination()
        {
            if (NavMeshAgent == null)
                return true;

            if (NavMeshAgent.pathPending)
                return false;

            // Consider arrived when remaining distance is within stopping distance + small epsilon
            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance + 0.1f)
                return true;

            return false;
        }

        public float GetDistanceToPlayer()
        {
            return Vector3.Distance(this.transform.position, _playerTransform.position);
        }
        #endregion

    }

}
