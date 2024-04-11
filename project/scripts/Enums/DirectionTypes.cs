using System;

namespace SlimeGame
{
    [Flags]
    public enum DirectionTypes 
    {
        None = 0,
        Face = 1,
        Edge = 2,
        Corner = 4,

        Horizontal = 8,
        Vertical = 16,

        HorizontalFace = Face | Horizontal,
        VerticalFace = Face | Vertical,

        HorizontalEdge = Face | Horizontal,
        VerticalEdge = Face | Vertical,

        HorizontalCorner = Face | Horizontal,
        VerticalCorner = Face | Vertical,

        FaceEdge = Face | Edge,
        HorizontalFaceEdge = Horizontal | Face | Edge,
        VerticalFaceEdge = Vertical | Face | Edge,

        FaceCorner = Face | Corner,
        HorizontalFaceCorner = Horizontal | Face | Corner,
        VerticalFaceCorner = Vertical | Face | Corner,

        EdgeCorner = Edge | Corner,
        HorizontalEdgeCorner = Horizontal | Edge | Corner,
        VerticalEdgeCorner = Vertical | Edge | Corner,


        All = Face | Edge | Corner,
    }   
}
