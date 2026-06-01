using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class HoaDonBLL
    {
        private readonly HoaDonDAL _dal = new HoaDonDAL();
        private readonly DatSanhDAL _dsDal = new DatSanhDAL();

        public List<HoaDon> GetAll() => _dal.GetAll();
        public HoaDon GetById(int maHoaDon) => _dal.GetById(maHoaDon);
        public HoaDon GetByMaDat(int maDat) => _dal.GetByMaDat(maDat);
        public List<DoanhThuThang> GetDoanhThuTheoThang(int nam) => _dal.GetDoanhThuTheoThang(nam);

        public List<HoaDon> Filter(List<HoaDon> source, string keyword, DateTime? tuNgay, DateTime? denNgay)
        {
            IEnumerable<HoaDon> q = source;

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(h => h.MaHoaDon.ToString().Contains(keyword) ||
                                 h.MaDat.ToString().Contains(keyword));

            if (tuNgay.HasValue)
                q = q.Where(h => h.NgayLap.HasValue && h.NgayLap.Value.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                q = q.Where(h => h.NgayLap.HasValue && h.NgayLap.Value.Date <= denNgay.Value.Date);

            return q.ToList();
        }

        public void AddHoaDon(int maDat, decimal tongTien)
        {
            if (maDat <= 0)
                throw new Exception("Mã đặt sảnh không hợp lệ.");
            if (tongTien < 0)
                throw new Exception("Tổng tiền không được âm.");
            if (_dal.DaCoHoaDon(maDat))
                throw new Exception("Đơn đặt sảnh này đã có hóa đơn.");

            var ds = _dsDal.FindById(maDat);
            if (ds == null)
                throw new Exception("Không tìm thấy đơn đặt sảnh.");

            _dal.Add(new HoaDon
            {
                MaDat = maDat,
                TongTien = tongTien,
                NgayLap = DateTime.Now
            });
        }

        public void UpdateHoaDon(int maHoaDon, decimal tongTien)
        {
            if (tongTien < 0)
                throw new Exception("Tổng tiền không được âm.");
            var hd = _dal.GetById(maHoaDon);
            if (hd == null)
                throw new Exception("Không tìm thấy hóa đơn.");
            hd.TongTien = tongTien;
            _dal.Update(hd);
        }

        public void DeleteHoaDon(int maHoaDon)
        {
            _dal.Delete(maHoaDon);
        }

        // Stats
        public int TongSo(List<HoaDon> list) => list.Count;
        public decimal TongDoanhThu(List<HoaDon> list) => list.Sum(h => h.TongTien ?? 0);
        public decimal TrungBinh(List<HoaDon> list) => list.Count == 0 ? 0 : list.Average(h => h.TongTien ?? 0);
        public decimal CaoNhat(List<HoaDon> list) => list.Count == 0 ? 0 : list.Max(h => h.TongTien ?? 0);
    }
}