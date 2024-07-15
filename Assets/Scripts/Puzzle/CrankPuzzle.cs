using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleSystem
{
    public class CrankPuzzle : PuzzleInteractable
    {
        [SerializeField] float timeToComplete;
        [SerializeField]
        [Range(0f, 1f)] float slowDownFactor;

        float wheelValue = 0f;
        float timer = 0f;

        bool isInteracting = false;

        Slider stateSlider;

        void Start()
        {
            stateSlider = GetComponentInChildren<Slider>();
            stateSlider.value = wheelValue;
            StopInteraction();
            StartCoroutine(WheelValueUpdate());
        }

        public override void Interact()
        {
            isInteracting = true;
        }

        public override void StopInteraction()
        {
            isInteracting = false;
        }

        IEnumerator WheelValueUpdate()
        {
            while (wheelValue < 1f)
            {
                if(timeToComplete == 0)
                {
                    break;
                }
                yield return null;
                timer += Time.deltaTime * ((isInteracting) ? 1 : -slowDownFactor);
                timer = Mathf.Max(timer, 0);
                wheelValue = timer/timeToComplete;
                stateSlider.value = wheelValue;
            }
            Completed = true;
        }
    }
}