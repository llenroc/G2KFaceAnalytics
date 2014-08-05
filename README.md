Gen. II Kinect for Windows – Face Analyics
==============

In this post I will walk you through the steps to generate basic face analytics.

![K4W logo](http://www.kinectingforwindows.com/wp-content/themes/twentyten/images/headers/logo.jpg)

## Face Analytics ##
This is how the face analytics will look like.

    <FaceAnalytics xmlns="http://schemas.datacontract.org/2004/07/Codit.Summit.Core.FaceTracking" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    	<KinectId>Not-Available-Yet</KinectId>
    	<BodyId>72057594037929353</BodyId>
    	<TrackDuration>0:00:22.0431966</TrackDuration>
    	<FeatureAnalytics>
    		<FaceFeatureAnalytics>
    			<FaceFeature>Engaged</FaceFeature>
    			<DetectionResult>No</DetectionResult>
    			<Percentage>81.14</Percentage>
    			<BodyId>72057594037929353</BodyId>
    			<FaceFeatureDetails>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>Yes</DetectionResult>
    					<Percentage>17</Percentage>
    				</FaceFeatureDetailsAnalytics>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>Maybe</DetectionResult>
    					<Percentage>2</Percentage>
    				</FaceFeatureDetailsAnalytics>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>No</DetectionResult>
    					<Percentage>81</Percentage>
    				</FaceFeatureDetailsAnalytics>
    			</FaceFeatureDetails>
    		</FaceFeatureAnalytics>
    		<FaceFeatureAnalytics>
    			<FaceFeature>WearingGlasses</FaceFeature>
    			<DetectionResult>Unknown</DetectionResult>
    			<Percentage>47.81</Percentage>
    			<BodyId>72057594037929353</BodyId>
    			<FaceFeatureDetails>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>Maybe</DetectionResult>
    					<Percentage>6</Percentage>
    				</FaceFeatureDetailsAnalytics>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>Yes</DetectionResult>
    					<Percentage>13</Percentage>
    				</FaceFeatureDetailsAnalytics>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>Unknown</DetectionResult>
    					<Percentage>48</Percentage>
    				</FaceFeatureDetailsAnalytics>
    				<FaceFeatureDetailsAnalytics>
    					<DetectionResult>No</DetectionResult>
    					<Percentage>33</Percentage>
    				</FaceFeatureDetailsAnalytics>
    			</FaceFeatureDetails>
    		</FaceFeatureAnalytics>
    	</FeatureAnalytics>
    </FaceAnalytics>

