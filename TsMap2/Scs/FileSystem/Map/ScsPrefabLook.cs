using System.Collections.Generic;
using System.Drawing;

namespace TsMap2.Scs.FileSystem.Map;

public abstract class ScsPrefabLook {
    public readonly List< PointF > Points;

    protected ScsPrefabLook( List< PointF > points ) => Points = points;

    protected ScsPrefabLook() : this( new List< PointF >() ) { }

    public int   ZIndex { get; set; }
    public Brush Color  { get; set; }

    public void AddPoint( PointF p ) {
        Points.Add( p );
    }

    public void AddPoint( float x, float y ) {
        AddPoint( new PointF( x, y ) );
    }

    public abstract void Draw( Graphics g );
}

public class ScsPrefabRoadLook : ScsPrefabLook {
    public ScsPrefabRoadLook() => ZIndex = 1;
    public float Width { private get; set; }

    public override void Draw( Graphics g ) {
        g.DrawLines( new Pen( Color, Width ), Points.ToArray() );
    }
}

public class ScsPrefabPolyLook : ScsPrefabLook {
    public ScsPrefabPolyLook( List< PointF > points ) : base( points ) { }

    public override void Draw( Graphics g ) {
        g.FillPolygon( Color, Points.ToArray() );
    }
}