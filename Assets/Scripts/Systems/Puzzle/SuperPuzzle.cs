using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem
{
    public class SuperPuzzle : Puzzle
    {
        Puzzle[] puzzles;

        private void Awake()
        {
            puzzles = GetComponentsInChildren<Puzzle>();
        }

        private void OnEnable()
        {
            foreach (var puzzle in puzzles)
            {
                puzzle.PuzzleStateUpdated.AddListener(OnStateUpdate);
            }
        }

        private void OnDisable()
        {
            foreach (var puzzle in puzzles)
            {
                puzzle.PuzzleStateUpdated?.RemoveListener(OnStateUpdate);
            }
        }

        private void OnStateUpdate()
        {
            if (Completed) { return; }
            bool isCompleted = true;
            foreach (var puzzle in puzzles)
            {
                isCompleted &= puzzle.Completed;
            }
            if (isCompleted)
            {
                Completed = true;
            }
        }
    }
}
