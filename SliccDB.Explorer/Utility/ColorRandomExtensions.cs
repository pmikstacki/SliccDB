using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Media;

namespace SliccDB.Explorer.Utility
{
    public class RandomColorExtension : MarkupExtension
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var colors = new List<System.Windows.Media.Color>
            {
                Color.FromRgb(50,120,140),
                Color.FromRgb(140,150,50),
                Color.FromRgb(80,120,90),
                Color.FromRgb(200,120,90),
                Color.FromRgb(90,100,100),
            };

            return new SolidColorBrush(colors[new Random().Next(0, 4)] );
        }
    }
}