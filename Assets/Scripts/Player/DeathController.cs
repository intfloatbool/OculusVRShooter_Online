using Passer;
using UnityEngine;

namespace RealWorldVRGame
{
    public class DeathController : MonoBehaviour, IController
    {
        [Header("Prefab to instantiate when death")]
        [SerializeField] private DeathBody _deathBodyPrefab;
        [SerializeField] private Color _deathColor = Color.black;
        
        [Header("Runtime references.")]
        [SerializeField] private HumanoidControl _humanoid;
        [SerializeField] private DeathBody _currentDeathBody;
        /// <summary>
        /// Origin color changer
        /// </summary>
        [SerializeField] private SkinnedMeshColorChanger _meshColorChanger;
        [SerializeField] private FullBodyColorChanger _fullBodyColorChanger;
        [SerializeField] private UnitStats _unitStats;
        [SerializeField] private Color _basicColor;

        public void InitializeController()
        {
            _humanoid = GetComponent<HumanoidControl>();
            Debug.Assert(_humanoid != null, "_humanoid != null");
            
            _meshColorChanger = GetComponentInChildren<SkinnedMeshColorChanger>();
            Debug.Assert(_meshColorChanger != null, "_meshColorChanger != null");
            _basicColor = _meshColorChanger.CurrentColor;
            
            _unitStats = GetComponentInChildren<UnitStats>();
            Debug.Assert(_unitStats != null, "_unitStats != null");

            _fullBodyColorChanger = GetComponentInChildren<FullBodyColorChanger>();
            Debug.Assert(_fullBodyColorChanger != null, "_fullBodyColorChanger != null");
        }

        public void Death()
        {
            if (_currentDeathBody == null)
            {
                _currentDeathBody = CreateDeathBody();
                
                //Death copy color changer for init basic color of player
                var colorChanger = _currentDeathBody.GetComponent<SkinnedMeshColorChanger>();
                Debug.Assert(colorChanger != null, "colorChanger != null");
                if (colorChanger != null)
                {
                    colorChanger.SetColor(_basicColor);
                }
            }
            else
            {
                _currentDeathBody.gameObject.SetActive(true);
            }
            
            _fullBodyColorChanger.SetColor(_deathColor);
            _currentDeathBody.transform.position = _humanoid.transform.position;
            _currentDeathBody.transform.rotation = _humanoid.transform.rotation;
            
            _currentDeathBody.StartDeath();
        }

        public void Resurrection()
        {
            _currentDeathBody.Reset();
            _fullBodyColorChanger.ResetColor();
            _currentDeathBody.gameObject.SetActive(false);
        }

        private DeathBody CreateDeathBody()
        {
            return Instantiate(_deathBodyPrefab);
        }
    }
}

