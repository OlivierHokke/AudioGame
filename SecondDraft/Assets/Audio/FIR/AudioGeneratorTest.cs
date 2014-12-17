using UnityEngine;
using System.Collections;

public class AudioGeneratorTest : MonoBehaviour {

    private const int MAX_FIR_LENGTH = 1000;
    private const int MAX_DATA_CACHE_LENGTH = 655360;

    private float normalizer = 0f;
    public float[] fir = {   0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                             0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f };
    private float[] data_1 = new float[MAX_DATA_CACHE_LENGTH];
    private int pos_1 = MAX_FIR_LENGTH - 1;
    private float[] data_2 = new float[MAX_DATA_CACHE_LENGTH];
    private int pos_2 = MAX_FIR_LENGTH - 1;



    void OnAudioFilterRead(float[] data, int channels)
    {
        normalizer = 0f;
        foreach (float f in fir) { normalizer += f; }

        // set data into channel cache
        for (int i = 0; i < data.Length; i++)
        {
            if (i % channels == 0) // check channel
            {

                data_1[pos_1] = data[i];
                data[i] = firFilter(data_1, pos_1);
                pos_1++;

                if (pos_1 >= MAX_DATA_CACHE_LENGTH)
                {
                    for (int q = 0; q < MAX_FIR_LENGTH; q++)
                    {
                        data_1[q] = data_1[MAX_DATA_CACHE_LENGTH - (MAX_FIR_LENGTH - q)]; // copy from end of array, back to front
                    }
                    pos_1 = MAX_FIR_LENGTH - 1; // set position back to beginning
                }
            }
            else
            {

                data_2[pos_2] = data[i];
                data[i] = firFilter(data_2, pos_2);
                pos_2++;

                if (pos_2 >= MAX_DATA_CACHE_LENGTH)
                {
                    for (int q = 0; q < MAX_FIR_LENGTH; q++)
                    {
                        data_2[q] = data_2[MAX_DATA_CACHE_LENGTH - (MAX_FIR_LENGTH - q)]; // copy from end of array, back to front
                    }
                    pos_2 = MAX_FIR_LENGTH - 1; // set position back to beginning
                }
            }
        }
    }

    private float firFilter(float[] data, int startIndex)
    {
        float processed = 0f;
        int i = fir.Length - 1;
        int x = startIndex;
        while (i >= 0)
        {
            if (fir[i] > 0.0001f)
            {
                processed += data[x] * fir[i];
            }
            i--; x--;
        }
        return processed / normalizer;
    }

    /*float time = 0f;
    public int alternatorBase = 1028;
    int alternatorCurrent = 4;
    int alternatorSide = -1;
    private int samples;

    void OnAudioFilterRead(float[] data, int channels)
    {
        samples = data.Length;
        //System.Random r = new System.Random();
        for (int i = 0; i < data.Length; i++)
        {
            if (i % channels == 1)
            {
                data[i] = alternatorSide / 2f;
            }
            else
            {
                data[i] = Mathf.Sin(time) - 0.5f;
            }
            Debug.Log(audio.timeSamples);
            alternatorCurrent--;
            if (alternatorCurrent < 0)
            {
                alternatorCurrent = alternatorBase;
                alternatorSide = -alternatorSide;
            }

            time += 0.005f;
        }
    }*/
}
