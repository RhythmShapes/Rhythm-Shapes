using System.Collections.Generic;
using UnityEngine;
using shape;

namespace AudioAnalysis
{
    public class DetectPattern : MonoBehaviour
    {
        void DetectedPatterns(List<Shape> shapes)
        {
            /*
            List<List<int>> connectedIndexes = ConnectedIndexes(shapes, 0f);
            int length = connectedIndexes.Count;

            for (int i = 0; i < length; i++)
            {

            }
            */
        }

        List<List<int>> ConnectedIndexes(List<Shape> shapes, float sigma)
        {
            List<List<int>> connectedIndexes = new List<List<int>>();
            List<int> alreadyChecked = new List<int>();

            int length = shapes.Count;

            for (int i = 0; i < length - 2; i++) 
            {
                if (alreadyChecked.Contains(i)) continue;

                List<int> connectedIndex = new List<int>{i};

                Shape currentShape = shapes[i];
                Shape currentNextShape = shapes[i + 1];
                float curDT = DeltaTime(currentShape, currentNextShape);

                // On regarde toutes les shapes apr�s les deux premi�res
                for (int j = i+2; j < length; j++) 
                {
                    // On regarde si la premi�re shape est la m�me
                    Shape observedShape = shapes[j];
                    if(observedShape.Type == currentShape.Type) 
                    {
                        // On regarde si la deuxi�me shape est la m�me
                        Shape observedNextShape = shapes[j + 1];
                        if (observedNextShape.Type == currentNextShape.Type)
                        {
                            float obsDT = DeltaTime(observedShape, observedNextShape);

                            // On regarde si elles ont le m�me �cart temporel � sigma pr�s
                            // (si l'analyse marche parfaitement, sigma = 0 mais pas s�r ici)
                            if (Mathf.Abs(obsDT - curDT) <= sigma)
                            {
                                // La shape i est connect�e � la shape j
                                connectedIndex.Add(j);
                                // Plus besoin de v�rifier pour j
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