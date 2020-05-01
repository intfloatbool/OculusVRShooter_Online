using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RealWorldVRGame
{
    public class Grenade : MonoBehaviourPun
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        public Rigidbody Rb => _rb;
        [SerializeField] private float _damageRadius = 1f;
        [SerializeField] private float _timeToBlow = 4f;
        [SerializeField] private GameObject _blowEffect;
        [SerializeField] private int _minDamage = 10;
        [SerializeField] private int _maxDamage = 20;
        [SerializeField] private GameObject _grenadeBody;
        [SerializeField] private float _decayTime = 2f;
        private int RandomDamage => Random.Range(_minDamage, _maxDamage);
        private bool _isBanged;
        private IEnumerator Start()
        {
            Debug.Assert(_blowEffect != null, "_blowEffect != null");
            Debug.Assert(_grenadeBody != null, "_grenadeBody != null");
            Debug.Assert(_rb != null, "_rb != null");
            Debug.Assert(_collider != null, "_collider != null");
            if (photonView.IsMine)
            {
                yield return StartCoroutine(GrenadeCoroutine());
            }
            else
            {
                //Disable remote physics
                _rb.isKinematic = true;
                _rb.useGravity = false;
                _collider.enabled = false;
            }
        }

        private IEnumerator GrenadeCoroutine()
        {
            yield return new WaitForSeconds(_timeToBlow);
            photonView.RPC("BangRPC", RpcTarget.All, RandomDamage);
            yield return new WaitForSeconds(_decayTime);
            photonView.RPC("DestroyRPC", RpcTarget.All);
        }

        [PunRPC]
        public void DestroyRPC()
        {
            Destroy(gameObject);
        }

        [PunRPC]
        public void BangRPC(int damage)
        {
            Bang(damage);
        }
        private void Bang(int damage)
        {
            _grenadeBody.gameObject.SetActive(false);
            _blowEffect.gameObject.SetActive(true);

            var bodies = Physics.OverlapSphere(transform.position, _damageRadius);
            for (int i = 0; i < bodies.Length; i++)
            {
                var col = bodies[i];
                if (col != null)
                {
                    var unitStats = col.GetComponent<UnitStats>();
                    if (unitStats != null)
                    {
                        unitStats.MakeDamage(damage);
                    }
                }
            }

            _isBanged = true;
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (_isBanged && _rb != null)
                {
                    _rb.velocity = Vector3.zero;
                }
            }
        }
    }
}
