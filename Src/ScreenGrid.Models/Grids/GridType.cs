namespace ScreenGrid.Models.Grids
{
    public enum GridType
    {
        None,

        /// <summary>
        /// Rule of Thirds
        /// </summary>
        Thirds,

        /// <summary>
        /// One side of the picture is divided into two, and then each half is divided into three parts. 
        /// The adjacent side is divided so that the lines connecting the resulting points form a diagonal frame. 
        /// According to the Diagonal Rule, important elements of the picture should be placed along these
        /// diagonals:
        /// </summary>
        DiagonalOfThirds,

        GoldenRatio,

        /// <summary>
        /// Main diagonal with two perpendiculars from corners
        /// </summary>
        GoldenTriangle,

        GoldenDiagonal,

        FibonacciRectangles,

        GoldenSpiral,

        // TODO: Implement normal (golden ratio) / stretched to window grids
    }
}
