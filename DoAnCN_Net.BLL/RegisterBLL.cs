using DoAnCN_Net.DAL;

namespace DoAnCN_Net.BLL
{
    public class RegisterBLL
    {
        private readonly RegisterDAL dal = new RegisterDAL();

        public string Register(string name, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Vui lòng nhập họ tên!";

            if (string.IsNullOrWhiteSpace(phone))
                return "Vui lòng nhập số điện thoại!";

            if (phone.Length < 9)
                return "Số điện thoại không hợp lệ!";

            return dal.Register(name, phone, email);
        }
    }
}