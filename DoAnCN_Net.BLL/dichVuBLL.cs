using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class dichVuBLL
    {
        private readonly dichVuDAL _dal = new dichVuDAL();

        public List<DichVu> GetAll() => _dal.GetAll();
        public DichVu GetById(int maDichVu) => _dal.GetById(maDichVu);
        public Dictionary<int, int> GetSoSuKienDung() => _dal.GetSoSuKienDung();

        public List<DichVu> Filter(List<DichVu> source, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return source;
            return source.Where(d =>
                d.TenDichVu.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();
        }

        public void AddDichVu(string tenDichVu, decimal gia, string moTa = null)
        {
            Validate(tenDichVu, gia, editingMa: null);
            _dal.Add(new DichVu
            {
                TenDichVu = tenDichVu.Trim(),
                Gia = gia,
                MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim()
            });
        }

        public void UpdateDichVu(int maDichVu, string tenDichVu, decimal gia, string moTa = null)
        {
            Validate(tenDichVu, gia, editingMa: maDichVu);
            _dal.Update(new DichVu
            {
                MaDichVu = maDichVu,
                TenDichVu = tenDichVu.Trim(),
                Gia = gia,
                MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim()
            });
        }

        public void DeleteDichVu(int maDichVu)
        {
            _dal.Delete(maDichVu);
        }

        public int TongSo(List<DichVu> list) => list.Count;
        public decimal GiaMin(List<DichVu> list) => list.Count == 0 ? 0 : list.Min(d => d.Gia ?? 0m);
        public decimal GiaMax(List<DichVu> list) => list.Count == 0 ? 0 : list.Max(d => d.Gia ?? 0m);

        private void Validate(string tenDichVu, decimal gia, int? editingMa)
        {
            if (string.IsNullOrWhiteSpace(tenDichVu))
                throw new Exception("Tên dịch vụ không được để trống.");
            if (tenDichVu.Trim().Length < 3)
                throw new Exception("Tên dịch vụ phải có ít nhất 3 ký tự.");
            if (gia < 0)
                throw new Exception("Giá không được âm.");
            if (_dal.TenDaTonTai(tenDichVu.Trim(), editingMa))
                throw new Exception($"Tên dịch vụ \"{tenDichVu.Trim()}\" đã tồn tại.");
        }
    }
}