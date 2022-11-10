using UnityEngine;
using utils.XML;

namespace AudioAnalysis
{
    public class BasicBPM
    {
        [SerializeField]
        private const int SAMPLE_SIZE = 2048;
        [SerializeField]
        private static int beatWindowSize;
        public static LevelDescription AnalyseMusic(AudioClip clip)
        {
            //TODO : REPLACE WITH ANALYSIS CODE
            Debug.LogWarning("TODO : REPLACE WITH ANALYSIS CODE");
            
            string dataPath = "Levels/LevelTest/Data";
            
            TextAsset xml = Resources.Load<TextAsset>(dataPath);

            float[][] fullSpectrum = AudioTools.FFT(clip, SAMPLE_SIZE);
            float[] averageEnergyPerSample = new float[fullSpectrum.Length];
            int[] beatType = new int[fullSpectrum.Length];
            float overallAverageEnergy = 0;

            for (int i = 0; i < averageEnergyPerSample.Length; i++)
            {
                averageEnergyPerSample[i] = AudioTools.AverageEnergy(fullSpectrum[i]);
                overallAverageEnergy += averageEnergyPerSample[i];
            }
            overallAverageEnergy /= averageEnergyPerSample.Length;

            beatType = new int[averageEnergyPerSample.Length];

            for (int i = 0; i < averageEnergyPerSample.Length; i++)
            {
                beatType[i] = 0;
                if (averageEnergyPerSample[i] < overallAverageEnergy)
                {
                    beatType[i] = -1;
                    continue;
                }

                int windowStart = Mathf.Max(0, i - beatWindowSize / 2);
                int windowEnd = Mathf.Min(averageEnergyPerSample.Length, i + beatWindowSize / 2);

                for (int j = windowStart; j < windowEnd; j++)
                {
                    if (averageEnergyPerSample[i] < averageEnergyPerSample[j])
                    {
                        beatType[i] = -1;
                        break;
                    }
                }
            }

            return  XmlHelpers.DeserializeFromXML<LevelDescription>(xml);
        }
    }
}
