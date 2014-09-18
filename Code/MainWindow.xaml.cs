using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using K4W.Face.Analytics.Data;
using K4W.Face.Analytics.Serialization;
using K4W.Face.Analytics.Trackers;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace K4W.Face.Analytics
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance of Kinect sensor
        /// </summary>
        private KinectSensor _kinect;

        /// <summary>
        /// Reading body language
        /// </summary>
        private BodyFrameReader _bodyReader;

        /// <summary>
        /// Collection of bodies
        /// </summary>
        private Body[] _bodies;

        /// <summary>
        /// Requested face features
        /// </summary>
        private const FaceFrameFeatures _faceFrameFeatures = FaceFrameFeatures.MouthMoved
                                                                | FaceFrameFeatures.MouthOpen
                                                                | FaceFrameFeatures.LeftEyeClosed
                                                                | FaceFrameFeatures.RightEyeClosed
                                                                | FaceFrameFeatures.LookingAway
                                                                | FaceFrameFeatures.Happy
                                                                | FaceFrameFeatures.FaceEngagement
                                                                | FaceFrameFeatures.Glasses;

        /// <summary>
        /// Holds all the face trackers
        /// </summary>
        private Dictionary<ulong, FaceTracker> _trackers = new Dictionary<ulong, FaceTracker>();

        /// <summary>
        /// Output folder for the analytics
        /// </summary>
        private string _analyticsFolder = "C:/Temp/Analytics";

        /// <summary>
        /// Default CTOR
        /// </summary>
        public MainWindow()
        {
            // Create temp-folder if required
            if (!Directory.Exists(_analyticsFolder))
                Directory.CreateDirectory(_analyticsFolder);

            // Initialize Components
            InitializeComponent();

            // Initialize Kinect
            InitializeKinect();
        }

        /// <summary>
        /// Initialize Kinect
        /// </summary>
        private void InitializeKinect()
        {
            // Get Kinect sensor
            _kinect = KinectSensor.GetDefault();

            if (_kinect == null) return;

            // Initialize Camera
            InitializeCamera();

            // Initialize Body
            InitializeBody();

            // Start receiving
            _kinect.Open();
        }

        #region Body Tracking
        private void InitializeBody()
        {
            // Open the body reader
            _bodyReader = _kinect.BodyFrameSource.OpenReader();

            // Wire handler when new frames arrive
            _bodyReader.FrameArrived += OnBodiesArrive;
        }

        /// <summary>
        /// Handle the new body frames
        /// </summary>
        private async void OnBodiesArrive(object sender, BodyFrameArrivedEventArgs e)
        {
            // Retrieve the body reference
            BodyFrameReference bodyRef = e.FrameReference;

            if (bodyRef == null) return;

            // Acquire the body frame
            using (BodyFrame frame = bodyRef.AcquireFrame())
            {
                if (frame == null) return;

                // Create a new collection when required
                if (_bodies == null || _bodies.Count() != frame.BodyCount)
                    _bodies = new Body[frame.BodyCount];

                // Refresh the bodies
                frame.GetAndRefreshBodyData(_bodies);

                // Start tracking faces
                foreach (Body body in _bodies)
                {
                    if (body.IsTracked)
                    {
                        // Create a new tracker if required
                        if (!_trackers.ContainsKey(body.TrackingId))
                        {
                            FaceTracker tracker = new FaceTracker(body.TrackingId, _faceFrameFeatures, _kinect);
                            tracker.FaceAnalyticsAvailable += OnFaceAnalyticsAvailable;

                            // Add to dictionary
                            _trackers.Add(body.TrackingId, tracker);
                        }
                    }
                }
            }
        }
        #endregion Body Tracking

        #region Analytics
        /// <summary>
        /// Process the new Face Analytics
        /// </summary>
        /// <param name="fa"></param>
        private void OnFaceAnalyticsAvailable(FaceAnalytics fa)
        {
            // Close reader for this body
            _trackers[fa.BodyId].Close();
            _trackers.Remove(fa.BodyId);

            // Compose filename
            string fileName = string.Format("{0}/Face-Tracking-{1}.xml", _analyticsFolder, fa.BodyId);

            // Serialize to string
            string serializedAnalytics = GenericSerializer<FaceAnalytics>.SerializeToString(fa);

            // Convert to byte array
            byte[] rawAnalytics = Encoding.UTF8.GetBytes(serializedAnalytics);

            // Flush to disk
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(rawAnalytics, 0, rawAnalytics.Length);
            }
        }
        #endregion Analytics

        #region CAMERA
        /// <summary>
        /// Color WriteableBitmap linked to our UI
        /// </summary>
        private WriteableBitmap _colorBitmap = null;
        /// <summary>
        /// Array of color pixels
        /// </summary>
        private byte[] _colorPixels = null;

        /// <summary>
        /// FrameReader for our coloroutput
        /// </summary>
        private ColorFrameReader _colorReader = null;
        /// <summary>
        /// Size fo the RGB pixel in bitmap
        /// </summary>
        private readonly int _bytePerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        private void InitializeCamera()
        {
            if (_kinect == null) return;

            // Get frame description for the color output
            FrameDescription desc = _kinect.ColorFrameSource.FrameDescription;

            // Get the framereader for Color
            _colorReader = _kinect.ColorFrameSource.OpenReader();

            // Allocate pixel array
            _colorPixels = new byte[desc.Width * desc.Height * _bytePerPixel];

            // Create new WriteableBitmap
            _colorBitmap = new WriteableBitmap(desc.Width, desc.Height, 96, 96, PixelFormats.Bgr32, null);

            // Link WBMP to UI
            CameraImage.Source = _colorBitmap;

            // Hook-up event
            _colorReader.FrameArrived += OnColorFrameArrived;
        }
        /// <summary>
        /// Process color frames & show in UI
        /// </summary>
        private void OnColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // Get the reference to the color frame
            ColorFrameReference colorRef = e.FrameReference;

            if (colorRef == null) return;

            // Acquire frame for specific reference
            ColorFrame frame = colorRef.AcquireFrame();

            // It's possible that we skipped a frame or it is already gone
            if (frame == null) return;

            using (frame)
            {
                // Get frame description
                FrameDescription frameDesc = frame.FrameDescription;

                // Check if width/height matches
                if (frameDesc.Width == _colorBitmap.PixelWidth && frameDesc.Height == _colorBitmap.PixelHeight)
                {
                    // Copy data to array based on image format
                    if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
                    {
                        frame.CopyRawFrameDataToArray(_colorPixels);
                    }
                    else frame.CopyConvertedFrameDataToArray(_colorPixels, ColorImageFormat.Bgra);

                    // Copy output to bitmap
                    _colorBitmap.WritePixels(
                            new Int32Rect(0, 0, frameDesc.Width, frameDesc.Height),
                            _colorPixels,
                            frameDesc.Width * _bytePerPixel,
                            0);
                }
            }
        }
        #endregion CAMERA
    }
}
