using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using K4W.Face.Analytics.Trackers;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace K4W.Face.Analytics.Data
{
    [DataContract]
    internal class FaceFeatureAnalytics
    {
        private ulong _bodyId;
        /// <summary>
        /// Id of the tracked body
        /// </summary>
        [DataMember(Order = 4)]
        public ulong BodyId
        {
            get { return _bodyId; }
            private set { _bodyId = value; }
        }

        private FaceProperty _faceFeature;
        /// <summary>
        /// Feature of the analytics
        /// </summary>
        [DataMember(Order = 1)]
        public FaceProperty FaceFeature
        {
            get { return _faceFeature; }
            private set { _faceFeature = value; }
        }

        private DetectionResult _detectionResult;
        /// <summary>
        /// Most common detection result
        /// </summary>
        [DataMember(Order = 2)]
        public DetectionResult DetectionResult
        {
            get { return _detectionResult; }
            private set { _detectionResult = value; }
        }

        private double _percentage = 0;
        /// <summary>
        /// Percentage of occurence
        /// </summary>
        [DataMember(Order = 3)]
        public double Percentage
        {
            get { return _percentage; }
            private set { _percentage = value; }
        }

        private List<FaceFeatureDetailsAnalytics> _details = new List<FaceFeatureDetailsAnalytics>();
        /// <summary>
        /// Detailed analytics
        /// </summary>
        [DataMember(Order = 5)]
        public List<FaceFeatureDetailsAnalytics> FaceFeatureDetails
        {
            get { return _details; }
            set { _details = value; }
        }


        /// <summary>
        /// Default CTOR
        /// </summary>
        public FaceFeatureAnalytics() { }

        /// <summary>
        /// Extended CTOR
        /// </summary>
        /// <param name="bodyId">Id of tracked body</param>
        /// <param name="faceFeature">Feature of the analytics</param>
        /// <param name="detectionResult">Most common detection result</param>
        /// <param name="perc">Percentage of occurence</param>
        private FaceFeatureAnalytics(ulong bodyId, FaceProperty faceFeature, DetectionResult detectionResult, double perc)
        {
            _bodyId = bodyId;
            _faceFeature = faceFeature;
            _detectionResult = detectionResult;
            _percentage = perc;
        }


        /// <summary>
        /// Analyse the feature tracker
        /// </summary>
        /// <param name="tracker">Tracker for the feature</param>
        /// <param name="bodyId">Id of tracked body</param>
        public static FaceFeatureAnalytics Analyse(FaceFeatureTracker tracker, ulong bodyId)
        {
            if (tracker == null) throw new ArgumentException("Invalid feature tracker", "tracker");
            if (bodyId == 0) throw new ArgumentException("Invalid body Id", "bodyId");

            // Get most frequent result
            DetectionResult frequentResult = DetectionResult.Unknown;

            // Create details list
            List<FaceFeatureDetailsAnalytics> featureDetails = new List<FaceFeatureDetailsAnalytics>();

            int totalOccurences = 0;
            int detectionOcc = -1;

            foreach (KeyValuePair<DetectionResult, int> pair in tracker.Results)
            {
                // Determin if this occured more
                if (pair.Value > detectionOcc)
                {
                    frequentResult = pair.Key;
                    detectionOcc = pair.Value;
                }

                // Add to details list
                featureDetails.Add(new FaceFeatureDetailsAnalytics(pair.Key, pair.Value));

                // Increment total
                totalOccurences += pair.Value;
            }

            double perc = 0;

            featureDetails.ForEach(ffda => ffda.CalculatePercentage(totalOccurences));

            // Calculate percentage
            if (tracker.Results.ContainsKey(frequentResult))
                perc = Math.Round((((double)tracker.Results[frequentResult] / (double)totalOccurences) * 100), 2);

            return new FaceFeatureAnalytics(bodyId, tracker.FaceProperty, frequentResult, perc) { FaceFeatureDetails = featureDetails };
        }

        /// <summary>
        /// Overrride Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FaceFeatureAnalytics))
                return false;

            FaceFeatureAnalytics castedObj = (FaceFeatureAnalytics)obj;

            if (castedObj.BodyId != _bodyId || castedObj.Percentage != _percentage || castedObj.DetectionResult != _detectionResult || castedObj.FaceFeature != _faceFeature)
                return false;

            return true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        public override string ToString()
        {
            return String.Format("Analytics for feature '{0}' for body #{1}'", _faceFeature, _bodyId);
        }
    }
}
