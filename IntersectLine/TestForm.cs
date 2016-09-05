using System;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.DesktopEdition;


namespace  IntersectLine
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-96.8531,33.1084,-96.8461,33.106);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay worldMapKitDesktopOverlay = new ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            LineShape lineShape = new LineShape();
            lineShape.Vertices.Add(new Vertex(-96.8511, 33.1089));
            lineShape.Vertices.Add(new Vertex(-96.8504, 33.108));
            lineShape.Vertices.Add(new Vertex(-96.85, 33.1077));
            lineShape.Vertices.Add(new Vertex(-96.8498, 33.1076));
            lineShape.Vertices.Add(new Vertex(-96.8482, 33.1076));

            LineShape lineShape2 = new LineShape();
            lineShape2.Vertices.Add(new Vertex(-96.8497, 33.1084));
            lineShape2.Vertices.Add(new Vertex(-96.8506, 33.1072));

            //MapShapeLayer to display the three features (line to split, splitting line and intersection point).
            MapShapeLayer mapShapeLayer = new MapShapeLayer();

            //For the line to split.
            MapShape lineMapShape1 = new MapShape(new Feature(lineShape));
            lineMapShape1.ZoomLevels.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Green, 2, true);
            lineMapShape1.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapShapeLayer.MapShapes.Add("Line1", lineMapShape1);

            //For the splitting line.
            MapShape lineMapShape2 = new MapShape(new Feature(lineShape2));
            lineMapShape2.ZoomLevels.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Red, 1, true);
            lineMapShape2.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapShapeLayer.MapShapes.Add("Line2", lineMapShape2);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(mapShapeLayer);

            winformsMap1.Overlays.Add(layerOverlay);

            winformsMap1.Refresh();
        }

        private void btnSplitLine_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverLay = (LayerOverlay)winformsMap1.Overlays[1];
            MapShapeLayer mapShapeLayer = (MapShapeLayer)layerOverLay.Layers[0];

            LineShape lineShape = (LineShape)mapShapeLayer.MapShapes["Line1"].Feature.GetShape();
            LineShape lineShape2 = (LineShape)mapShapeLayer.MapShapes["Line2"].Feature.GetShape();

            //Gets the crossing MultipointShape with the PointShape that will be used to calculate the two split lines.
            MultipointShape multipointShape = lineShape.GetCrossing(lineShape2);

            //Uses the GetLineOnLine (Dynamic Segmentation) to get the two split lines from the intersection point.
            LineShape splitLineShape1 = (LineShape)lineShape.GetLineOnALine(StartingPoint.FirstPoint, multipointShape.Points[0]);
            LineShape splitLineShape2 = (LineShape)lineShape.GetLineOnALine(StartingPoint.LastPoint, multipointShape.Points[0]);

            //Displays the two split lines with different colors to distinguish them.
            MapShape splitMapShape1 = new MapShape(new Feature(splitLineShape1));
            splitMapShape1.ZoomLevels.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Orange, 3, true);
            splitMapShape1.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapShapeLayer.MapShapes.Add("SplitLine1", splitMapShape1);

            MapShape splitMapShape2 = new MapShape(new Feature(splitLineShape2));
            splitMapShape2.ZoomLevels.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Turquoise, 3, true);
            splitMapShape2.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapShapeLayer.MapShapes.Add("SplitLine2", splitMapShape2);

            //Displays the intersection point as a reference.
            MapShape pointMapShape = new MapShape(new Feature(multipointShape.Points[0]));
            pointMapShape.ZoomLevels.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle,
                                                                             GeoColor.StandardColors.Red, GeoColor.StandardColors.Black, 1, 9);
            pointMapShape.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapShapeLayer.MapShapes.Add("Point", pointMapShape);
          
            winformsMap1.Refresh(layerOverLay);

            btnSplitLine.Enabled = false;

        }

      
        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

    }
}
