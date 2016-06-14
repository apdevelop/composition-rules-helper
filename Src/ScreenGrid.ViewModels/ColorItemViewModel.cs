namespace ScreenGrid.ViewModels
{
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
    }
}
