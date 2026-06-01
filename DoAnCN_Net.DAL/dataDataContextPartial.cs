namespace DoAnCN_Net.DAL
{
    public partial class dataDataContext
    {
        public static dataDataContext Create()
        {
            string cs = "Data Source= DESKTOP-IGQEI1S\\SQLEXPRESS;" +
                        "Initial Catalog=Adora;" +
                        "Integrated Security=True;" +
                        "TrustServerCertificate=True";
            return new dataDataContext(cs);
        }
    }
}   