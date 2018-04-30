namespace ScreenGrid.Models
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Anemic model
    /// </summary>
    public class NativeWindowState
    {
        public IntPtr Handle { get; set; }

        public string ClassName { get; set; }

        public string Caption { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "[{0:X8}] {1} '{2}' {3}x{4}", this.Handle.ToInt32(), this.ClassName, this.Caption, this.Width, this.Height);
        }
    }
}
