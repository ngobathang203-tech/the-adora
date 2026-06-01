using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class DanhGiaSanhBLL
    {
        private readonly DanhGiaSanhDAL _dal = new DanhGiaSanhDAL();

        public List<DanhGiaSanh> GetBySanh(int maSanh)
            => _dal.GetBySanh(maSanh);

        public List<DanhGiaSanh> GetAll()
            => _dal.GetAll();

        public double GetRating(int maSanh)
            => _dal.GetRatingTrungBinh(maSanh);

        public List<SoSaoThongKe> GetThongKeSao(int maSanh)
            => _dal.GetThongKeSao(maSanh);

        public List<DanhGiaSanh> Filter(List<DanhGiaSanh> source,
                                        string keyword,
                                        int? soSao,
                                        DateTime? tuNgay,
                                        DateTime? denNgay)
        {
            IEnumerable<DanhGiaSanh> q = source;

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(d =>
                        d.MaDanhGia.ToString().Contains(keyword) ||
                        (d.NoiDung ?? "").ToLower().Contains(keyword.ToLower()));

            if (soSao.HasValue)
                q = q.Where(d => d.SoSao == soSao.Value);

            if (tuNgay.HasValue)
                q = q.Where(d => d.NgayDanhGia.HasValue &&
                                 d.NgayDanhGia.Value.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                q = q.Where(d => d.NgayDanhGia.HasValue &&
                                 d.NgayDanhGia.Value.Date <= denNgay.Value.Date);

            return q.OrderByDescending(d => d.NgayDanhGia).ToList();
        }

        public void GuiDanhGia(int maSanh, int maKhachHang, int soSao, string noiDung)
        {
            if (soSao < 1 || soSao > 5)
                throw new Exception("Số sao phải từ 1 đến 5.");
            if (string.IsNullOrWhiteSpace(noiDung))
                throw new Exception("Vui lòng nhập nội dung đánh giá.");
            if (noiDung.Trim().Length < 5)
                throw new Exception("Nội dung đánh giá quá ngắn (tối thiểu 5 ký tự).");

            _dal.Add(new DanhGiaSanh
            {
                MaSanh = maSanh,
                MaKhachHang = maKhachHang,
                SoSao = soSao,
                NoiDung = noiDung.Trim(),
                NgayDanhGia = DateTime.Now
            });
        }

        public void XoaDanhGia(int maDanhGia)
        {
            var dg = _dal.GetById(maDanhGia);
            if (dg == null)
                throw new Exception("Không tìm thấy đánh giá.");
            _dal.Delete(maDanhGia);
        }

        public int TongSo(List<DanhGiaSanh> list) => list.Count;
        public double TrungBinh(List<DanhGiaSanh> list)
            => list.Count == 0 ? 0 : list.Average(d => (double)d.SoSao);
        public int CaoNhat(List<DanhGiaSanh> list)
            => list.Count == 0 ? 0 : list.Max(d => (int)d.SoSao);
    }
}