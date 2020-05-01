using UnityEngine;

namespace RealWorldVRGame
{
    public class PlayerUnitStats : UnitStats
    {
        [SerializeField] protected GameObject _deathItemPrefab;
        [SerializeField] protected bool _isShowDeathItem = true;
        protected GameObject _currentDeathItem;
        protected readonly string HandName = "Default_simple|Hand_R";
        
        private void ShowDeathItem()
        {
            if (_currentDeathItem == null)
            {
                var root = transform.root;
                var hand = root.Find(HandName);
                if (hand != null)
                {
                    _currentDeathItem = Instantiate(_deathItemPrefab, hand);
                } 
            }
            else
            {
                _currentDeathItem.SetActive(true);
            }
        }

        private void HideDeathItem()
        {
            if (_currentDeathItem != null)
            {
                _currentDeathItem.SetActive(false);
            }
        }

        public override void Death()
        {
            base.Death();
            if (_isShowDeathItem)
            {
                ShowDeathItem();
            }
        }

        public override void ResurrectUnit()
        {
            base.ResurrectUnit();
            if (_isShowDeathItem)
            {
                HideDeathItem();
            }
        }
    }
}
