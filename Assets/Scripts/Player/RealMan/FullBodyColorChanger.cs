using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class FullBodyColorChanger : SkinnedMeshColorChanger
    {
        [SerializeField] private List<MeshColorPart> _colorParts;
        [Serializable]
        public class MeshColorPart
        {
            public SkinnedMeshRenderer SkinnedMeshRenderer;
            public Color BasicColor;
        }

        public void InitBasicColors()
        {
            if (_colorParts == null)
                _colorParts = new List<MeshColorPart>();
            _skinnedMeshRenderer.ForEach(m =>
            {
                _colorParts.Add(new MeshColorPart()
                {
                    SkinnedMeshRenderer = m,
                    BasicColor = m.materials[0].GetColor(_colorName)
                });
            });
        }

        public override void SetColor(Color color)
        {
            foreach (var colorPart in _colorParts)
            {
                colorPart.SkinnedMeshRenderer.materials[0].SetColor(_colorName, color);
            }
        }

        public void ResetColor()
        {
            foreach (var colorPart in _colorParts)
            {
                colorPart.SkinnedMeshRenderer.materials[0].SetColor(_colorName, colorPart.BasicColor);
            }
        }
    }
}
