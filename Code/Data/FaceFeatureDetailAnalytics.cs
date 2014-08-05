using System;
using Microsoft.Kinect;
using System.Runtime.Serialization;

namespace K4W.Face.Analytics.Data
{
    [DataContract]
    internal class FaceFeatureDetailsAnalytics
    {
        private DetectionResult _detectionResult;
        /// <summary>
        /// Most common detection result
        /// </summary>
        [DataMember]
        public DetectionResult DetectionResult
        {
            get { return _detectionResult; }
            private set { _detectionResult = value; }
        }

        private double _percentage = 0;
        /// <summary>
        /// Percentage of occurence
        /// </summary>
        [DataMember]
        public double Percentage
        {
            get { return _percentage; }
            private set { _percentage = value; }
        }

        /// <summary>
        /// Counter of this DetectionResult during tracking
        /// </summary>
        private double _counter = 0;

        /// <summary>
        /// Default CTOR
        /// </summary>
        public FaceFeatureDetailsAnalytics(DetectionResult detResult, double counter)
        {
            _detectionResult = detResult;
            _counter = counter;
        }

        /// <summary>
        /// Calculate the occurence percentage
        /// </summary>
        /// <param name="totalOccurences">Total counter occurences</param>
        public void CalculatePercentage(double totalOccurences)
        {
            _percentage = Math.Round((((double)_counter / (double)totalOccurences) * 100));
        }


        /// <summary>
        /// Overrride Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FaceFeatureDetailsAnalytics))
                return false;

            FaceFeatureDetailsAnalytics castedObj = (FaceFeatureDetailsAnalytics)obj;

            if (castedObj.DetectionResult != _detectionResult)
                return false;

            return true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        public override string ToString()
        {
            return string.Format("'{1}' tracked for {2}%", _detectionResult, Math.Round(_percentage, 2));
        }
    }
}
