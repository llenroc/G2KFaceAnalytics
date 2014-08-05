using System;
using System.Collections.Generic;
using System.Linq;
using K4W.Face.Analytics.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace K4W.Face.Analytics.Trackers
{
    internal class FaceTracker
    {
        #region Public Properties
        private ulong _bodyId = 0;
        /// <summary>
        /// The Id of the tracked body
        /// </summary>
        public ulong BodyId
        {
            get { return _bodyId; }
        }

        private string _kinectId = "Not-Available-Yet";
        /// <summary>
        /// The Id of the tracking Kinect sensor
        /// </summary>
        public string KinectId
        {
            get { return _kinectId; }
        }

        private FaceFrameFeatures _faceFeatures = FaceFrameFeatures.None;
        /// <summary>
        /// The set of tracked features
        /// </summary>
        public FaceFrameFeatures FaceFeatures
        {
            get { return _faceFeatures; }
        }
        #endregion Public Properties

        #region Private Properties
        /// <summary>
        /// Timestamp when the tracker started
        /// </summary>
        private DateTime _startTracking;

        /// <summary>
        /// A Face source for a tracked body
        /// </summary>
        private FaceFrameSource _faceSource;

        /// <summary>
        /// Reader for the face frames fromthe face source
        /// </summary>
        private FaceFrameReader _faceReader;

        /// <summary>
        /// Set of tracked features
        /// </summary>
        private Dictionary<FaceProperty, FaceFeatureTracker> _featureAnalytics = new Dictionary<FaceProperty, FaceFeatureTracker>();
        #endregion Private Properties


        /// <summary>
        /// Default CTOR
        /// </summary>
        public FaceTracker() { }

        /// <summary>
        /// Extended CTOR
        /// </summary>
        /// <param name="bodyId">Id of the tracked body</param>
        /// <param name="faceFeatures">Set of requested face features to track</param>
        /// <param name="kinect">Kinect sensor that is tracking</param>
        public FaceTracker(ulong bodyId, FaceFrameFeatures faceFeatures, KinectSensor kinect)
        {
            // Pin-point start of tracking
            _startTracking = DateTime.Now;

            // Save variables
            _bodyId = bodyId;
            _faceFeatures = faceFeatures;
            // _kinectId = kinect.UniqueKinectId --> NotImplementedYet

            // Create a new source with body TrackingId
            _faceSource = new FaceFrameSource(kinect)
            {
                FaceFrameFeatures = _faceFeatures,
                TrackingId = bodyId
            };

            // Create new reader
            _faceReader = _faceSource.OpenReader();

            Console.WriteLine(String.Format("Tracker for body #{0} started.", _bodyId));

            // Initialize FaceFeatureTrackers
            InitialiseFeatureTrackers();

            // Wire events
            _faceReader.FrameArrived += OnFaceFrameArrived;
            _faceSource.TrackingIdLost += OnTrackingLost;
        }


        /// <summary>
        /// Initialize trackers for all face features
        /// </summary>
        private void InitialiseFeatureTrackers()
        {
            if (!_featureAnalytics.ContainsKey(FaceProperty.Engaged) && _faceFeatures.HasFlag(FaceFrameFeatures.FaceEngagement))
                _featureAnalytics.Add(FaceProperty.Engaged, new FaceFeatureTracker(FaceProperty.Engaged, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.Happy) && _faceFeatures.HasFlag(FaceFrameFeatures.Happy))
                _featureAnalytics.Add(FaceProperty.Happy, new FaceFeatureTracker(FaceProperty.Happy, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.LeftEyeClosed) && _faceFeatures.HasFlag(FaceFrameFeatures.LeftEyeClosed))
                _featureAnalytics.Add(FaceProperty.LeftEyeClosed, new FaceFeatureTracker(FaceProperty.LeftEyeClosed, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.LookingAway) && _faceFeatures.HasFlag(FaceFrameFeatures.LookingAway))
                _featureAnalytics.Add(FaceProperty.LookingAway, new FaceFeatureTracker(FaceProperty.LookingAway, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.MouthMoved) && _faceFeatures.HasFlag(FaceFrameFeatures.MouthMoved))
                _featureAnalytics.Add(FaceProperty.MouthMoved, new FaceFeatureTracker(FaceProperty.MouthMoved, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.MouthOpen) && _faceFeatures.HasFlag(FaceFrameFeatures.MouthOpen))
                _featureAnalytics.Add(FaceProperty.MouthOpen, new FaceFeatureTracker(FaceProperty.MouthOpen, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.RightEyeClosed) && _faceFeatures.HasFlag(FaceFrameFeatures.RightEyeClosed))
                _featureAnalytics.Add(FaceProperty.RightEyeClosed, new FaceFeatureTracker(FaceProperty.RightEyeClosed, _bodyId));

            if (!_featureAnalytics.ContainsKey(FaceProperty.WearingGlasses) && _faceFeatures.HasFlag(FaceFrameFeatures.Glasses))
                _featureAnalytics.Add(FaceProperty.WearingGlasses, new FaceFeatureTracker(FaceProperty.WearingGlasses, _bodyId));
        }

        /// <summary>
        /// Process Face Frames
        /// </summary>
        private void OnFaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            // Retrieve the face reference
            FaceFrameReference faceRef = e.FrameReference;

            if (faceRef == null) return;

            // Acquire the face frame
            using (FaceFrame faceFrame = faceRef.AcquireFrame())
            {
                if (faceFrame == null) return;

                // Retrieve the face frame result
                FaceFrameResult frameResult = faceFrame.FaceFrameResult;

                if (frameResult != null)
                {
                    // Update trackers
                    UpdateTrackers(frameResult);
                }
            }
        }

        /// <summary>
        /// Update the FaceFeatureTrackers
        /// </summary>
        /// <param name="frameResult">Face tracking frame</param>
        private void UpdateTrackers(FaceFrameResult frameResult)
        {
            // Loop all trackers
            foreach (FaceProperty feature in _featureAnalytics.Keys)
            {
                // Track the detection results
                _featureAnalytics[feature].Track(frameResult.FaceProperties[feature]);
            }
        }

        /// <summary>
        /// We lost track of the body and analytics are generated
        /// </summary>
        private void OnTrackingLost(object sender, TrackingIdLostEventArgs e)
        {
            Console.WriteLine(String.Format("Tracker for body #{0} lost.", e.TrackingId));

            // Create analytics for each feature
            List<FaceFeatureAnalytics> featureAnalytics = _featureAnalytics.Values.Select(fft => FaceFeatureAnalytics.Analyse(fft, _bodyId)).ToList();

            // Notify listeners 
            FaceAnalyticsAvailable(new FaceAnalytics(_kinectId, _bodyId, featureAnalytics, (DateTime.Now - _startTracking)));
        }

        // Custom event to throw when tracking is lost & analytics are available
        public event FaceAnalyticsAvailableHandler FaceAnalyticsAvailable;
        public delegate void FaceAnalyticsAvailableHandler(FaceAnalytics fa);


        /// <summary>
        /// Overrride Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FaceTracker))
                return false;

            FaceTracker castedObj = (FaceTracker)obj;

            if (castedObj.BodyId != _bodyId)
                return false;

            return true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        public override string ToString()
        {
            return String.Format("Face-tracker for body #{0}", _bodyId);
        }
    }
}
