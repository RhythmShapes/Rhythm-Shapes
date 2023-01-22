using shape;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalysis
{

    public static class PostAnalysisTools
    {
        public static float FirstBeatTime(AudioClip clip, float bpm)
        {
            float bps = bpm / 60;
            float spb = 1 / bps;
            int beatIndexOffset = 0;
            float maxStartTime = AudioAnalysis.AudioTools.timeFromIndex(0, clip) + 3 * bps;
            int k = 0;
            float firstBeatTime = 0;
            float[] data = new float[clip.channels * clip.samples];
            List<int> startIndexes = new List<int>();
            while (AudioAnalysis.AudioTools.timeFromIndex(k, clip) < maxStartTime)
            {
                startIndexes.Add(k);
            }
            int maxCount = 0;
            foreach (int index in startIndexes)
            {
                int count = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    float nbBeat = data[i] * bps;
                    if (Mathf.Abs(Mathf.RoundToInt(nbBeat) - nbBeat) < 1 / 6)
                    {
                        count++;
                    }
                }
                if (count > maxCount)
                {
                    firstBeatTime = AudioAnalysis.AudioTools.timeFromIndex(index, clip);
                    maxCount = count;
                }
            }
            return firstBeatTime;
        }
        public static void snapNotesToBPMGrid(Shape[] shapes, float bpm, float firstNoteTime, float precision)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                float shift = -(((shapes[i].TimeToPress - firstNoteTime) % precision) - precision);
                shapes[i].ShiftTimeToPress(shift);
            }
        }
    }

}
