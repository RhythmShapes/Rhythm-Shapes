using shape;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalysis
{

    public static class PostAnalysisTools
    {
        public static int FirstBeatTime(Shape[] shapes, float bpm)
        {
            float bps = bpm / 60;
            float maxStartTime = shapes[0].TimeToPress + 3 * bps;
            int k = 0;
            List<int> startIndexes = new List<int>();
            while (shapes[k].TimeToPress < maxStartTime)
            {
                startIndexes.Add(k);
            }
            int maxCount = 0;
            int startIndex = 0;
            foreach (int index in startIndexes)
            {
                int count = 0;
                for (int i = 0; i < shapes.Length; i++)
                {
                    float timeDiff = shapes[i].TimeToPress - shapes[index].TimeToPress;
                    if (Mathf.Abs(Mathf.Round(timeDiff)-timeDiff)*bps < 1 / 6)
                    {
                        count++;
                    }
                }
                if (count > maxCount)
                {
                    startIndex = index;
                    maxCount = count;
                }
            }
            return startIndex;
        }
        public static void snapNotesToBPMGrid(Shape[] shapes, float bpm, float firstNoteTime, float precision)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                float shift = -(((shapes[i].TimeToPress - firstNoteTime) % precision) - precision);
                shapes[i].ShiftTimeToPress(shift);
            }
        }

        public static int LengthInBeats(Shape[] shapes, float bpm, float firstNoteTime)
        {
            int end = shapes.Length - 1;
            return Mathf.FloorToInt( (shapes[end].TimeToPress - firstNoteTime) * bpm / 60);
        }

        public static Shape[] MelodicalPatternRepetition(Shape[] shapes, float bpm, float firstNoteTime, float matchingThreshold)
        {
            float bps = bpm / 60;
            int segmentLengthInBeats = LengthInBeats(shapes, bpm, firstNoteTime);
            int segmentLengthInBars = segmentLengthInBeats / 4;
            if (segmentLengthInBars < 8)
            {
                return shapes;
            }
            /*
             * Couper shapes en deux, puis comparer la première moitié à la deuxième.
             * ATTENTION, il faut couper à un nombre entier de mesures (1 mesure = 4 beats), et de sorte à avoir un nombre de mesures égal des deux côtés
             * Cela signifie donc qu'il faut un nombre de mesures pair.
             * Pour comparer on peut utliser par exemple le ratio intersection/union, s'il est proche de 1 alors il y a ressemblance
             * Puis s'il y a ressemblance, on assigne l'union des deux parties à chacune des deux parties
             */
            //Séparation de shapes en deux:
            if (segmentLengthInBars % 2 == 0)
            {
                List<Shape> firstHalf = new List<Shape>();
                List<Shape> secondHalf = new List<Shape>();
                for (int k = 0; k < shapes.Length; k++)
                {
                    if (shapes[k].TimeToPress < firstNoteTime + segmentLengthInBeats / (bps * 2))
                    {
                        firstHalf.Add(shapes[k]);
                    }
                    else
                    {
                        secondHalf.Add(shapes[k]);
                    }
                }
                List<Shape> union = new List<Shape>();
                List<Shape> intersection = new List<Shape>();

                float timeDelta = secondHalf[0].TimeToPress - firstHalf[0].TimeToPress;

                int i = 0;
                int j = 0;
                while (i < firstHalf.Count && j < secondHalf.Count)
                {
                    if (Mathf.Approximately(firstHalf[i].TimeToPress, secondHalf[j].TimeToPress))
                    {
                        intersection.Add(firstHalf[i]);
                        i++;
                        j++;
                    }
                    else
                    {
                        if (firstHalf[i].TimeToPress < secondHalf[j].TimeToPress)
                        {
                            union.Add(firstHalf[i]);
                            i++;
                        }
                        else
                        {
                            union.Add(secondHalf[j]);
                            j++;
                        }
                    }
                }
                if (i < firstHalf.Count)
                {
                    for (int k = j; k < secondHalf.Count; k++)
                    {
                        union.Add(secondHalf[k]);
                    }
                }
                else
                {
                    for (int k = j; k < firstHalf.Count; k++)
                    {
                        union.Add(firstHalf[k]);
                    }
                }
                float iou = intersection.Count / union.Count;
                if (iou > matchingThreshold)
                {
                    union.AddRange(union);
                    return union.ToArray();
                }
                else
                {
                    Shape[] shapes_l = MelodicalPatternRepetition(firstHalf.ToArray(), bpm, firstNoteTime, matchingThreshold);
                    Shape[] shapes_r = MelodicalPatternRepetition(secondHalf.ToArray(), bpm, FirstBeatTime(secondHalf.ToArray(),bpm), matchingThreshold);
                    for (int k = 0; k < shapes.Length; k++)
                    {
                        if (k < shapes_l.Length)
                        {
                            shapes[k] = shapes_l[k];
                        }
                        else
                        {
                            shapes[k] = shapes_r[k - shapes_l.Length];
                        }
                    }
                    return shapes;
                }

            }
            //Si le nombre de mesures est impair, on enlève la dernière et on réessaie.
            else
            {

                List<Shape> shortenedSong = new List<Shape>();
                float shortenedSongLength = (segmentLengthInBars - 1) * 4 / bps;

                for (int i = 0; shapes[i].TimeToPress < shortenedSongLength; i++)
                {
                    shortenedSong.Add(shapes[i]);
                }
                return MelodicalPatternRepetition(shortenedSong.ToArray(), bpm, firstNoteTime, matchingThreshold);
            }
        }
    }

}
