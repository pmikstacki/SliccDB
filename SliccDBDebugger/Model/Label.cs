namespace SliccDebugger.Model
{
    public class LabelModel
    {
        public string Label { get; set; }

        public override string ToString()
        {
            return ":"+Label;
        }
    }
}