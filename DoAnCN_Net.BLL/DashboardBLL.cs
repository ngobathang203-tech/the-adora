using DoAnCN_Net.DAL;
using System.Collections.Generic;

namespace DoAnCN_Net.BLL
{
    public class DashboardBLL
    {
        private readonly DashboardDAL dal = new DashboardDAL();

        public List<DashboardRow> GetDashboardData() => dal.GetTopDatSanh();
        public int GetTotalOrders() => dal.GetTotalOrders();
        public decimal GetTotalRevenue() => dal.GetTotalRevenue();
        public int GetTodayParties() => dal.GetTodayParties();
        public int GetPendingOrders() => dal.GetPendingOrders();

        /// Doanh thu theo tháng của năm <paramref name="nam"/> (mặc định năm hiện tại).
        public List<DoanhThuChartRow> GetDoanhThuTheoThang(int nam)
            => dal.GetDoanhThuTheoThang(nam);

        /// Số lượng đơn theo từng trạng thái.
        public List<TrangThaiChartRow> GetThongKeTrangThai()
            => dal.GetThongKeTrangThai();
    }
}