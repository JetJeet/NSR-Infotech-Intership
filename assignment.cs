using UnityEngine;
using GoogleARCore;

public class ARImageVideoPlayer : MonoBehaviour
{
    public AugmentedImageTracker imageTracker; // Reference to the image tracker
    public AugmentedImageVisualizer imageVisualizerPrefab; // Prefab for visualizing the tracked image
    public GameObject videoPlayerPrefab; // Prefab for the video player

    private AugmentedImageVisualizer imageVisualizer; // Instance of the visualizer for the tracked image
    private GameObject currentVideoPlayer; // Instance of the current video player

    private bool isVideoPlaying = false; // Flag to check if video is playing

    private void Update()
    {
        // Check if there are any tracked images
        if (imageTracker == null || imageTracker.Count == 0)
        {
            // If no tracked images, destroy the video player if it's active
            if (currentVideoPlayer != null)
            {
                Destroy(currentVideoPlayer);
                isVideoPlaying = false;
            }
            return;
        }

        // Loop through the tracked images
        foreach (var image in imageTracker)
        {
            // Check if the image is being tracked and it's not already visualized
            if (image.TrackingState == TrackingState.Tracking && imageVisualizer == null)
            {
                // Instantiate the visualizer prefab
                imageVisualizer = Instantiate(imageVisualizerPrefab, image.CenterPose.position, image.CenterPose.rotation);
                imageVisualizer.Image = image;
            }
            // Check if the image is not being tracked and it's already visualized
            else if (image.TrackingState == TrackingState.Stopped && imageVisualizer != null)
            {
                // Destroy the visualizer
                Destroy(imageVisualizer.gameObject);
                imageVisualizer = null;
            }

            // If the image is being tracked and a video is not already playing
            if (image.TrackingState == TrackingState.Tracking && !isVideoPlaying)
            {
                // Instantiate the video player prefab and position it above the image
                currentVideoPlayer = Instantiate(videoPlayerPrefab, image.CenterPose.position + new Vector3(0, 0.1f, 0), image.CenterPose.rotation);
                // Play the video
                currentVideoPlayer.GetComponent<VideoPlayer>().Play();
                isVideoPlaying = true;
            }
            // If the image is not being tracked and a video is playing
            else if ((image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused) && isVideoPlaying)
            {
                // Stop and destroy the video player
                currentVideoPlayer.GetComponent<VideoPlayer>().Stop();
                Destroy(currentVideoPlayer);
                isVideoPlaying = false;
            }
        }
    }
}
c