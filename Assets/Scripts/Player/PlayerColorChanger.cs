using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class PlayerColorChanger : MonoBehaviourPun
    {
        [SerializeField] private List<MeshRenderer> _meshRendersToChangeColor;

        [SerializeField] private Color[] _colorVariants;

        private void Start()
        {
            PickRandomColor();
        }
        
        private void PickRandomColor()
        {
            var rndColorIndex = Random.Range(0, _colorVariants.Length);
            if (photonView.IsMine)
            {                
                photonView.RPC("SetColor", RpcTarget.AllBuffered, rndColorIndex);
            }

        }

        [PunRPC]
        public void SetColor(int colorIndex)
        {
            var unpackedColor = _colorVariants[colorIndex];
            foreach (var meshRend in _meshRendersToChangeColor)
            {
                meshRend.materials[0].SetColor("_Color", unpackedColor);
            }
        }
    }
}
