using System;
using System.Linq;
using System.Windows.Media;

namespace ScreenGrid.ViewModels
{
    public class ColorItemViewModel
    {
        private Color color;

        public ColorItemViewModel(Color color)
        {
            this.color = color;
        }

        public Color Color => this.color;

        public Brush Brush => new SolidColorBrush(this.Color);

        public string Name => GetColorName(this.Color);

        private static string GetColorName(Color color)
        {
            var colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => (Color)p.GetValue(null, null) == color);
            return colorProperty != null ? colorProperty.Name : "???";
        }

        public override bool Equals(Object obj)
        {
            return (obj is ColorItemViewModel) && (this == (ColorItemViewModel)obj);
        }

        public override int GetHashCode() => this.color.GetHashCode();

        public static bool operator ==(ColorItemViewModel a, ColorItemViewModel b) => a.Color == b.Color;

        public static bool operator !=(ColorItemViewModel a, ColorItemViewModel b) => !(a == b);
    }
}
