using UnityEngine;

public class EnemyBasePresenter : MonoBehaviour
{
    [SerializeField]
    private Transform _transform;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private float _movementSpeed;

    private int _enemyHealth;
    public int EnemyHealth
    {
        get 
        {
            return _enemyHealth;
        }
        set
        {
            if (_enemyHealth <= 0)
            {
                Destroy(this.gameObject);
                return;
            }
            _enemyHealth = value;
        }
    }

    private void Awake()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.Find("Player").transform;
        }
    }


    // Update is called once per frame
    void Update()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, _playerTransform.position, _movementSpeed * Time.deltaTime);
    }

    public void DamageEnemy()
    {
        EnemyHealth--;
    }
}
