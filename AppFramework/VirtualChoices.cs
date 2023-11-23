namespace AppFramework
{
    public class VirtualChoices
    {
        public string RollNo { get; set; }
        public string VChoiceWithRank { get; set; }
        public int ChoiceCount { get; set; }
        public int VChoiceCount { get; set; }
        public VirtualChoices(string rollNo, string vChoiceWithRank, int choiceCount, int vChoiceCount)
        {
            this.RollNo = rollNo;
            this.VChoiceWithRank = vChoiceWithRank;
            this.ChoiceCount = choiceCount;
            this.VChoiceCount = vChoiceCount;
        }

        public VirtualChoices(string rollNo, string vChoiceWithRank, int choiceCount, int vChoiceCount, VirtualChoice[] vChoices)
        {
            this.RollNo = rollNo;
            this.VChoiceWithRank = vChoiceWithRank;
            this.ChoiceCount = choiceCount;
            this.VChoiceCount = vChoiceCount;
            this.VChoices = vChoices;
        }

        public VirtualChoices(string rollNo, string vChoiceWithRank)
        {
            this.RollNo = rollNo;
            this.VChoiceWithRank = vChoiceWithRank;
            this.ChoiceCount = 0;
            this.VChoiceCount = 0;
        }

        public VirtualChoice[] VChoices;
    }

    public class VirtualChoice
    {
        public string VChoice { get; set; }
        public double Rank { get; set; }
        public VirtualChoice(string vChoiceWithRank)
        {
            string[] choiceDetails = vChoiceWithRank.Split(':');
            VChoice = choiceDetails[0];
            Rank = System.Convert.ToDouble(choiceDetails[1]);
        }

        
    }

}
