using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace RealWorldVRGame
{
    public class SkinnedMeshColorChanger : MonoBehaviour
    {
        [SerializeField] protected List<SkinnedMeshRenderer> _skinnedMeshRenderer;
        [SerializeField] protected string _colorName = "_Color";
        [SerializeField] protected Color _currentColor;
        public Color CurrentColor => _currentColor;
        
        public virtual void SetColor(Color color)
        {
            foreach (var meshRend in _skinnedMeshRenderer)
            {
                meshRend.materials[0].SetColor(_colorName, color);
            }

            _currentColor = color;
        }
    }   
}

