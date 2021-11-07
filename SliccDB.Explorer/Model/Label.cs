namespace SliccDB.Explorer.Model
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