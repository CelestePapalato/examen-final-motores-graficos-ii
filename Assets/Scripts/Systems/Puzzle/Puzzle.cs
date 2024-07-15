using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSystem
{
    public abstract class Puzzle : MonoBehaviour
    {
        protected bool _completed = false;
        public bool Completed
        {
            get { return _completed; }
            protected set
            {
                if (_completed != value)
                {
                    _completed = value;
                    PuzzleStateUpdated.Invoke();
                }
            }
        }

        public UnityEvent PuzzleStateUpdated;

    }
}