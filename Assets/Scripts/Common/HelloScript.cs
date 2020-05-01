using System;
using UnityEngine;

namespace RealWorldVRGame
{
    public class HelloScript : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("I am created! " + gameObject.name);
        }
    } 
}