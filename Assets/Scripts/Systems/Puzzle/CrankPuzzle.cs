using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PuzzleSystem
{
    public class CrankPuzzle : PuzzleInteractable
    {
        [Header("Crank Puzzle")]

        [SerializeField] float timeToComplete;
        [SerializeField]
        [Range(0f, 1f)] float slowDownFactor;

        public UnityEvent<float> onWheelValueUpdated;

        bool isInteracting = false;

        Slider stateSlider;

        void Start()
        {
            StopAllCoroutines();
            stateSlider = GetComponentInChildren<Slider>();
            StopInteraction();
            StartCoroutine(WheelValueUpdate());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            StopInteraction();
        }

        public override void Interact()
        {
            base.Interact();
            isInteracting = true;
        }

        public override void StopInteraction()
        {
            base.StopInteraction();
            isInteracting = false;
        }

        IEnumerator WheelValueUpdate()
        {
            float wheelValue = 0f;
            stateSlider.value = wheelValue;
            float timer = 0f;
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
                stateSlider.value = 1 - wheelValue;
                onWheelValueUpdated?.Invoke(wheelValue);
            }
            Completed = true;
        }

        public void Restart()
        {
            StopAllCoroutines();
            StopInteraction();
            _completed = false;
            if (!stateSlider)
            {
                stateSlider = GetComponentInChildren<Slider>();
            }
            StartCoroutine(WheelValueUpdate());
        }
    }
}