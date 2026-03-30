using UnityEngine;
using LSL;

/// <summary>
/// This script connects to an OpenViBE LSL stream and translates 
/// Motor Imagery (Left/Right hand) commands into physical movement in Unity.
/// </summary>
public class BCIReader : MonoBehaviour
{
    [Header("LSL Settings")]
    [Tooltip("Must match the Marker stream name in OpenViBE LSL Export box")]
    public string StreamName = "Unity_BCI";

    [Header("Game Object Control")]
    [Tooltip("The 3D object you want to move with your mind")]
    public GameObject objectToMove;
    public float moveSpeed = 5f;

    // LSL Variables
    private StreamInfo[] streamInfos;
    private StreamInlet inlet;
    private double[] sample = new double[1]; // We only expect one value (the stimulation code)

    void Start()
    {
        Debug.Log("Searching for BCI Stream...");

        // Resolve the LSL stream by its name
        streamInfos = LSL.LSL.resolve_stream("name", StreamName, 1, 0.0);

        if (streamInfos.Length > 0)
        {
            // Open the inlet to receive data
            inlet = new StreamInlet(streamInfos[0]);
            Debug.Log("Successfully Connected to OpenViBE!");
        }
        else
        {
            Debug.LogError("Stream not found! Make sure OpenViBE is playing and LSL Export name is correct.");
        }
    }

    void Update()
    {
        // If connected, continuously check for new brain commands
        if (inlet != null)
        {
            double timeout = 0.0; // Do not block the main Unity thread
            double timestamp = inlet.pull_sample(sample, timeout);

            // If a new sample is received
            if (timestamp != 0.0)
            {
                double brainCommand = sample[0];
                Debug.Log("Brain Command Received: " + brainCommand);

                // Translate GDF standard codes into movement
                if (objectToMove != null)
                {
                    if (brainCommand == 769) // OVTK_GDF_Left (Left Hand Motor Imagery)
                    {
                        objectToMove.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                    }
                    else if (brainCommand == 770) // OVTK_GDF_Right (Right Hand Motor Imagery)
                    {
                        objectToMove.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
}