using shape;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{

    public static class PostAnalysisTools
    {
        public static int FirstBeatIndex(ShapeDescription[] shapes, float bpm)
        {
            float bps = bpm / 60;
            float maxStartTime = shapes[0].timeToPress + 3 / bps;
            int k = 0;
            List<int> startIndexes = new List<int>();
            while (k<shapes.Length && shapes[k].timeToPress < maxStartTime)
            {
                startIndexes.Add(k);
                k++;
            }
            int maxCount = 0;
            int startIndex = 0;
            foreach (int index in startIndexes)
            {
                int count = 0;
                for (int i = 0; i < shapes.Length; i++)
                {
                    float timeDiff = shapes[i].timeToPress - shapes[index].timeToPress;
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
        public static void snapNotesToBPMGrid(ShapeDescription[] shapes, float bpm, float firstNoteTime, float precision)
            /*
             * NB: precision désigne la précision en fraction de beat.
             */
        {
            precision = precision * 60 / bpm;
            Debug.Log("PRECISION: " + precision);
            for (int i = 0; i < shapes.Length; i++)
            {
                float shift = (((shapes[i].timeToPress - firstNoteTime) % precision) - precision);
                shapes[i].timeToPress -= shift;
            }
        }

        public static int LengthInBeats(ShapeDescription[] shapes, float bpm, float firstNoteTime)
        {
            int end = shapes.Length - 1;
            if(end <= 0)
            {
                return 0;
            }
            return Mathf.FloorToInt( (shapes[end].timeToPress - firstNoteTime) * bpm / 60);
        }

        public static void RoundTimingToMilliseconds(ShapeDescription[] shapes)
        {
            foreach(ShapeDescription shape in shapes)
            {
                shape.timeToPress = Mathf.Round(shape.timeToPress*1000)/1000;
            }
        }
        public static ShapeDescription[] RythmicPatternRepetition(ShapeDescription[] shapes, float bpm, float firstNoteTime, float matchingThreshold)
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
                List<ShapeDescription> firstHalf = new List<ShapeDescription>();
                List<ShapeDescription> secondHalf = new List<ShapeDescription>();
                for (int k = 0; k < shapes.Length; k++)
                {
                    if (shapes[k].timeToPress < firstNoteTime + segmentLengthInBeats / (bps * 2))
                    {
                        firstHalf.Add(shapes[k]);
                    }
                    else
                    {
                        secondHalf.Add(shapes[k]);
                    }
                }
                List<ShapeDescription> union = new List<ShapeDescription>();
                List<ShapeDescription> intersection = new List<ShapeDescription>();

                float timeDelta = secondHalf[0].timeToPress - firstHalf[0].timeToPress;

                int i = 0;
                int j = 0;
                while (i < firstHalf.Count && j < secondHalf.Count)
                {
                    if (Mathf.Approximately(firstHalf[i].timeToPress, secondHalf[j].timeToPress))
                    {
                        intersection.Add(firstHalf[i]);
                        i++;
                        j++;
                    }
                    else
                    {
                        if (firstHalf[i].timeToPress < secondHalf[j].timeToPress)
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
                float iou = (float)intersection.Count / union.Count;
                if (iou > matchingThreshold)
                {
                    Debug.Log("PATTERN DETECTED SUCCESSFULLY");
                    union.AddRange(union);
                    return union.ToArray();
                }
                else
                {
                    ShapeDescription[] shapes_l = RythmicPatternRepetition(firstHalf.ToArray(), bpm, firstNoteTime, matchingThreshold);
                    ShapeDescription[] shapes_r = RythmicPatternRepetition(secondHalf.ToArray(), bpm, secondHalf[FirstBeatIndex(secondHalf.ToArray(),bpm)].timeToPress, matchingThreshold);
                    for (int k = 0; k < shapes_l.Length + shapes_r.Length; k++)
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

                List<ShapeDescription> shortenedSong = new List<ShapeDescription>();
                float shortenedSongLength = (segmentLengthInBars - 1) * 4 / bps;

                for (int i = 0; shapes[i].timeToPress < shortenedSongLength; i++)
                {
                    shortenedSong.Add(shapes[i]);
                }
                return RythmicPatternRepetition(shortenedSong.ToArray(), bpm, firstNoteTime, matchingThreshold);
            }
        }
    }

}
