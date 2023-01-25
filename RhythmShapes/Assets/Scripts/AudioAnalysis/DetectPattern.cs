using System.Collections.Generic;
using UnityEngine;
using shape;

namespace AudioAnalysis
{
    public class DetectPattern : MonoBehaviour
    {
        List<List<List<Shape>>> PatternBeginEnd(List<Shape> shapes, float sigma)
        {
            List<List<List<Shape>>> listpatern = new List<List<List<Shape>>>();

            List<List<int>> connectedIndexes = ConnectedIndexes(shapes, sigma);

            int length_left = connectedIndexes.Count;
            int index = 0;

            while (length_left - index > 0)
            {
                List<List<Shape>> pattern = new List<List<Shape>>();

                List<int> connectedIndex = connectedIndexes[index];
                List<int> connectedIndexNext;

                for (int j = 0; j < connectedIndex.Count; j++)
                {
                    List<Shape> shapesInPattern = new List<Shape>();

                    if (length_left - index == 1)
                    {
                        connectedIndex = connectedIndexes[index];
                        shapesInPattern.Add(shapes[connectedIndex[0]]);
                        shapesInPattern.Add(shapes[connectedIndex[0] + 1]);
                        connectedIndex.RemoveAt(0);

                        index++;

                        pattern.Add(shapesInPattern);
                    }
                    else
                    {
                        connectedIndexNext = connectedIndexes[index + 1];

                        while (connectedIndex[0] == connectedIndexNext[0] - 1)
                        {
                            shapesInPattern.Add(shapes[connectedIndex[0]]);
                            connectedIndex.RemoveAt(0);

                            index++;

                            connectedIndex = connectedIndexes[index];
                            connectedIndexNext = connectedIndexes[index + 1];
                        }

                        // On est sorti de la boucle, connectedIndexes[index] correspond donc à la dernière shape du Pattern
                        connectedIndex = connectedIndexes[index];
                        shapesInPattern.Add(shapes[connectedIndex[0]]);
                        shapesInPattern.Add(shapes[connectedIndex[0] + 1]);
                        connectedIndex.RemoveAt(0);

                        pattern.Add(shapesInPattern);
                    } 
                }
                listpatern.Add(pattern);
            }
            return listpatern;
        }

        List<List<int>> ConnectedIndexes(List<Shape> shapes, float sigma)
        {
            List<List<int>> connectedIndexes = new List<List<int>>();
            List<int> alreadyChecked = new List<int>();

            int length = shapes.Count;

            for (int i = 0; i < length - 3; i++) 
            {
                if (alreadyChecked.Contains(i)) continue;

                List<int> connectedIndex = new List<int>{i};

                Shape currentShape = shapes[i];
                Shape currentNextShape = shapes[i + 1];
                float curDT = DeltaTime(currentShape, currentNextShape);

                // On regarde toutes les shapes après les deux premières
                for (int j = i+2; j < length; j++) 
                {
                    // On regarde si la première shape est la même
                    Shape observedShape = shapes[j];
                    if (observedShape.Type == currentShape.Type) 
                    {
                        // On regarde si la deuxième shape est la même
                        Shape observedNextShape = shapes[j + 1];
                        if (observedNextShape.Type == currentNextShape.Type)
                        {
                            float obsDT = DeltaTime(observedShape, observedNextShape);

                            // On regarde si elles ont le même écart temporel à sigma près
                            // (si l'analyse marche parfaitement, sigma = 0 mais pas sûr ici)
                            if (Mathf.Abs(obsDT - curDT) <= sigma)
                            {
                                // La shape i est connectée à la shape j
                                connectedIndex.Add(j);
                                // Plus besoin de vérifier pour j
                                alreadyChecked.Add(j);
                            }
                        }
                    }
                }
                connectedIndexes.Add(connectedIndex);
            }

            return connectedIndexes;
        }

        float DeltaTime(Shape s1, Shape s2)
        {
            return s2.TimeToPress - s1.TimeToPress;
        }

    }
}