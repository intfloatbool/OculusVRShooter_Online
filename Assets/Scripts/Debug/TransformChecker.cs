using Passer;
using UnityEngine;

namespace DebugTesting
{
    public class TransformChecker : MonoBehaviour
    {
        [SerializeField] private Transform _correctTransform;
        [SerializeField] private Transform _checkingTransform;

        [ContextMenu("Check correct transform")]
        public void CheckCorrectTransfrom()
        {
            var correctChilds = _correctTransform.GetComponentsInChildren<Transform>(true);
            var checkingChilds = _checkingTransform.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < correctChilds.Length; i++)
            {
                var correctChild = correctChilds[i];
                var checkingChild = checkingChilds[i];
                
                CheckTransfrom(correctChild, checkingChild);
            }
        }

        private void CheckTransfrom(Transform correct, Transform checking)
        {
            if (correct.localPosition != checking.localPosition)
            {
                Debug.Log($"{checking.gameObject.name} localPosition not match! \n" +
                          $"correct: {correct.localPosition} " +
                          $"incorrect: {checking.localPosition}");
                
                checking.localPosition = correct.localPosition;
            }
            
            if (correct.localEulerAngles != checking.localEulerAngles)
            {
                Debug.Log($"{checking.gameObject.name} localEulerAngles not match! \n" +
                          $"correct: {correct.localEulerAngles} \n" +
                          $"incorrect: {checking.localEulerAngles}");
                checking.localEulerAngles = correct.localEulerAngles;
            }
            
            if (correct.localScale != checking.localScale)
            {
                Debug.Log($"{checking.gameObject.name} localScale not match! \n" +
                          $"correct: {correct.localScale} \n" +
                          $"incorrect: {checking.localScale}");
                checking.localScale = correct.localScale;
            }
        }
    } 
}

