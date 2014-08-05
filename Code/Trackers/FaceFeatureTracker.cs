using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace K4W.Face.Analytics.Trackers
{
    internal class FaceFeatureTracker
    {
        #region Public Properties
        private FaceProperty _faceProperty;
        /// <summary>
        /// Property that it keeps track of
        /// </summary>
        public FaceProperty FaceProperty
        {
            get { return _faceProperty; }
        }

        private ulong _bodyId = 0;
        /// <summary>
        /// Id of the tracked body
        /// </summary>
        public ulong BodyId
        {
            get { return _bodyId; }
        }

        private Dictionary<DetectionResult, int> _tracking = new Dictionary<DetectionResult, int>();
        /// <summary>
        /// Detection results
        /// </summary>
        public Dictionary<DetectionResult, int> Results
        {
            get { return _tracking; }
        }
        #endregion Public Properties


        /// <summary>
        /// Default CTOR
        /// </summary>
        public FaceFeatureTracker() { }

        /// <summary>
        /// Extended CTOR
        /// </summary>
        /// <param name="faceProp">Face property that it will keep track of</param>
        /// <param name="bodyId">Id of the tracked body</param>
        public FaceFeatureTracker(FaceProperty faceProp, ulong bodyId)
        {
            _faceProperty = faceProp;
            _bodyId = bodyId;
        }


        /// <summary>
        /// Track another detection
        /// </summary>
        /// <param name="detectionResult">Result of the detection</param>
        public void Track(DetectionResult detectionResult)
        {
            // Add new detection result if not present yet
            if (!_tracking.ContainsKey(detectionResult)) _tracking.Add(detectionResult, 0);

            // Increment the tracking value
            _tracking[detectionResult]++;
        }

        /// <summary>
        /// Overrride Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FaceFeatureTracker))
                return false;

            FaceFeatureTracker castedObj = (FaceFeatureTracker)obj;

            if (castedObj.FaceProperty != _faceProperty || castedObj.BodyId != _bodyId)
                return false;

            return true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        public override string ToString()
        {
            return String.Format("'{0}' Tracker for body #{1}", _faceProperty, _bodyId);
        }
    }
}
