namespace ScreenGrid.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Media;

    public class ColorItemViewModel
    {
        private Color color;

        public ColorItemViewModel(Color color)
        {
            this.color = color;
        }

        public Color Color 
        {
            get
            {
                return this.color;
            }
        }

        public Brush Brush
        { 
            get
            { 
                return new SolidColorBrush(this.Color);
            }
        }
        
        public string Name
        {
            get
            {
                return GetColorName(this.Color);
            }
        }

        private static string GetColorName(Color color)
        {
            var colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => (Color)p.GetValue(null, null) == color);
            return colorProperty != null ? colorProperty.Name : "???";
        }

        public override bool Equals(Object obj)
        {
            return ((obj is ColorItemViewModel) && (this == (ColorItemViewModel)obj));
        }

        public override int GetHashCode()
        {
            return this.color.GetHashCode();
        }

        public static bool operator ==(ColorItemViewModel a, ColorItemViewModel b)
        {
            return (a.Color == b.Color);
        }

        public static bool operator !=(ColorItemViewModel a, ColorItemViewModel b)
        {
            return (!(a == b));
        }
    }
}
