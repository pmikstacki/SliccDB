using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Edit;

namespace SliccDebugger.Utility
{
    public class CypherLanguage : ProceduralLanguageBase

    {

        public CypherLanguage(EditControl control)

            : base(control)

        {

            this.Name = "Cypher";

            this.FileExtension = "cyp";

            this.ApplyColoring = true;

            this.SupportsIntellisense = false;

            this.SupportsOutlining = true;

            this.TextForeground = Brushes.White;

            Lexem = Application.Current.Resources["CypherLanguageLexems"] as LexemCollection;
            Formats = Application.Current.Resources["CypherLanguageFormats"] as FormatsCollection;

        }

    }
}