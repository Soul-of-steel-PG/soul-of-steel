using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Movement {
    [Serializable]
    public class MovementInfo {
        public int Steps;
        public string Direction;

        public MovementInfo(int steps, string direction) {
            Steps = steps;
            Direction = direction;
        }
    }

    [ShowInInspector] public List<MovementInfo> steps;
    public string fullStrings;
    public List<int> degrees;

    public static List<Movement> FromString(string movementString) {
        List<Movement> movements = new List<Movement>();
        string[] lines = movementString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines) {
            string trimmedLine = line.Trim();
            Movement movement = FromStringSingleLine(trimmedLine);
            movements.Add(movement);
        }

        return movements;
    }

    private static Movement FromStringSingleLine(string movementString) {
        List<int> steps = new() { };
        List<string> direction = new() { };
        List<int> degrees = new() { -1 };
        string fullStrings;

        if (movementString.Length > 2) {
            movementString = movementString.Substring(3);
            fullStrings = movementString.Replace("⮂", "");
            List<string> parts = movementString.Split(',').ToList();
            foreach (string part in parts) {
                string localPart = part.Replace(" ", "");

                if (part.Contains("º") || part.Contains("°")) {
                    degrees.Clear();

                    localPart = localPart.Replace("°", "");
                    localPart = localPart.Replace("º", "");
                    localPart = localPart.Replace("⮂", "");

                    if (localPart.Contains("o")) {
                        string[] numbers = localPart.Split('o');

                        foreach (string number in numbers) {
                            Int32.TryParse(number.Trim(), out int numberInt);
                            degrees.Add(numberInt);
                        }
                    }
                    else {
                        Int32.TryParse(localPart, out int numberInt);
                        degrees.Add(numberInt);
                    }
                }
                else {
                    string stepsString = localPart.Substring(0, 1);
                    Int32.TryParse(stepsString, out int numberInt);
                    steps.Add(numberInt);

                    string directionString = localPart.Substring(1);
                    direction.Add(directionString);
                }
            }


            List<MovementInfo> stepsDictionary = new List<MovementInfo>();

            for (int i = 0; i < steps.Count; i++) {
                stepsDictionary.Add(new MovementInfo(steps[0], direction[0]));
            }

            return new Movement {
                steps = stepsDictionary,
                degrees = degrees, // Use the parsed degrees or -1 for no rotation
                fullStrings = fullStrings
            };
        }

        return null;
    }
}