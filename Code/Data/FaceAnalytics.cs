using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace K4W.Face.Analytics.Data
{
    [DataContract]
    internal class FaceAnalytics
    {
        private string _kinectId = string.Empty;
        /// <summary>
        /// Id of the tracking Kinect sensor
        /// </summary>
        [DataMember(Order = 1)]
        public string KinectId
        {
            get { return _kinectId; }
            private set { _kinectId = value; }
        }

        /// <summary>
        /// Id of the tracked bo
        /// </summary>
        private ulong _bodyId = 0;
        [DataMember(Order = 2)]
        public ulong BodyId
        {
            get { return _bodyId; }
            private set { _bodyId = value; }
        }

        /// <summary>
        /// Analytics of the features
        /// </summary>
        private List<FaceFeatureAnalytics> _featureAnalytics = new List<FaceFeatureAnalytics>();
        [DataMember(Order = 4)]
        public List<FaceFeatureAnalytics> FeatureAnalytics
        {
            get { return _featureAnalytics; }
            private set { _featureAnalytics = value; }
        }

        /// <summary>
        /// Duration of the tracking
        /// </summary>
        private TimeSpan _trackDuration = default(TimeSpan);
        [DataMember(Order = 3)]
        public string TrackDuration
        {
            get { return _trackDuration.ToString("g"); }
            private set { _trackDuration = TimeSpan.Parse(value); }
        }



        /// <summary>
        /// Default CTOR
        /// </summary>
        public FaceAnalytics() { }

        /// <summary>
        /// Extended CTOR
        /// </summary>
        /// <param name="kinectId">Id of the tracked Kinect sensor</param>
        /// <param name="bodyId">Id of the tracked body</param>
        /// <param name="featureAnalytics">Analytics of the features</param>
        /// <param name="trackDuration">Duration of the tracking</param>
        public FaceAnalytics(string kinectId, ulong bodyId, List<FaceFeatureAnalytics> featureAnalytics, TimeSpan trackDuration)
        {
            _kinectId = kinectId;
            _bodyId = bodyId;
            _featureAnalytics = featureAnalytics;
            _trackDuration = trackDuration;
        }


        /// <summary>
        /// Overrride Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FaceAnalytics))
                return false;

            FaceAnalytics castedObj = (FaceAnalytics)obj;

            if (castedObj.BodyId != _bodyId || castedObj.KinectId != _kinectId)
                return false;

            return true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        public override string ToString()
        {
            return String.Format("Analytics for Body #{0}", _bodyId);
        }
    }
}
