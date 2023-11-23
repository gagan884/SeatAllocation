namespace AppFramework
{
    public class AllotmentDetail
    {
        public string Rollno { get; set; }
        public string Instcd { get; set; }
        public string Brcd { get; set; }
        public int  OptNo{ get; set; }
        public string Sequence { get; set; }
        public int IsRetained { get; set; }

        public AllotmentDetail(string rollno,string instcd,string brcd, string sequence, int optNo,int isRetained)
        {
            Rollno = rollno;
            Instcd = instcd;
            Brcd = brcd;
            OptNo = optNo;
            Sequence = sequence;
            IsRetained = isRetained;
        }

    }
}
