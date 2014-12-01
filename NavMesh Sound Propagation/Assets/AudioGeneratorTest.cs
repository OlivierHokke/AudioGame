using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioGeneratorTest : MonoBehaviour {

    public const int MAX_FIR_LENGTH = 512;
    //private const int MAX_FIR_LENGTH = 6;
    private const int MAX_DATA_CACHE_LENGTH = 655360;

    //private float[] filter = new float[] { 0f, 0f, 0f, .5f, .5f, 0f };

    private float normalizer = 0f;
    private float[] data_1 = new float[MAX_DATA_CACHE_LENGTH];
    private int pos_1 = MAX_FIR_LENGTH - 1;
    private float[] data_2 = new float[MAX_DATA_CACHE_LENGTH];
    private int pos_2 = MAX_FIR_LENGTH - 1;

    NavMeshListener listener = null;

    void Start()
    {
        listener = GetComponent<NavMeshListener>();
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        normalizer = 1f;
        //foreach (float f in GetComponent<NavMeshListener>().filter) { normalizer += f; }

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
        if (ReferenceEquals(listener,null)) return 0f;
        if (listener.threadSafeFilters.Count == 0) return 0f;

        List<BaseFilter> filters = listener.threadSafeFilters.Last();
        if (filters == null) filters = listener.threadSafeFilters.Last(1);
        if (listener.threadSafeFilters.Count > 10000)
        {
            listener.threadSafeFilters.Clear();
            listener.threadSafeFilters.Add(filters);

            Debug.Log("Deleting old filters");
        }

        float processed = 0f;
        foreach (BaseFilter filter in filters)
        {
            processed += filter.Apply(data, startIndex);
        }


        return processed;
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
