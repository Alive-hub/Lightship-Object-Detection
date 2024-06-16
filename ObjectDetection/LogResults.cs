using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;

public class LogResults : MonoBehaviour
{
    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;
    [SerializeField] private float confidenceThreshold = .5f;
    
    // Start is called before the first frame update
    void Start()
    {
        _objectDetectionManager.enabled = true;
        _objectDetectionManager.MetadataInitialized += ObjectDetectionManagerOnMetadataInitialized;

    }

    private void ObjectDetectionManagerOnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void OnDestroy()
    {
        _objectDetectionManager.MetadataInitialized -= ObjectDetectionManagerOnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionManagerOnObjectDetectionsUpdated;

    }

    private void ObjectDetectionManagerOnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        string resultString = "";
        var result = obj.Results;

        if (result == null)
        {
            return;
        }

        for (int i = 0; i < result.Count; i++)
        {
            var detection = result[i];
            var categories = detection.GetConfidentCategorizations(confidenceThreshold);

            if (categories.Count <= 0)
            {
                break;
            }
            
            categories.Sort((a,b) => b.Confidence.CompareTo(a.Confidence));

            for (int j = 0; j < categories.Count; j++)
            {
                var categoryToDisplay = categories[j];
                resultString += $"Detected: {categoryToDisplay.CategoryName}  with confidence {categoryToDisplay.Confidence} - ";
            }
            
            Debug.Log(resultString);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
