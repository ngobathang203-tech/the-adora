using System;
using System.Collections.Generic;
using System.Linq;
namespace DoAnCN_Net.DAL
{
    public class DatSanhDAL
    {
        //dataDataContext db =  dataDataContext.Create();
        public List<DatSanh> GetAll()
        {
            var db = dataDataContext.Create();
            return db.DatSanhs.ToList();
        }
        public List<DatSanh> GetTopDatSanh()
        {
            var db = dataDataContext.Create();
            return db.DatSanhs
                     .OrderByDescending(x => x.NgayToChuc)
                     .Take(5)
                     .ToList();
        }
        public List<DatSanh> GetByKhachHang(int maKhachHang)
        {
            var db = dataDataContext.Create();
            return db.DatSanhs
                     .Where(x => x.MaKhachHang == maKhachHang)
                     .OrderByDescending(x => x.NgayToChuc)
                     .ToList();
        }
        public bool Insert(DatSanh ds)
        {
            try
            {
                var db = dataDataContext.Create();
                db.DatSanhs.InsertOnSubmit(ds);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(DatSanh ds)
        {
            try
            {
                var db = dataDataContext.Create();
                DatSanh old = db.DatSanhs
                                .FirstOrDefault(x => x.MaDat == ds.MaDat);
                if (old == null) return false;
                old.MaSanh = ds.MaSanh;
                old.MaKhachHang = ds.MaKhachHang;
                old.MaLoai = ds.MaLoai;
                old.MaMenu = ds.MaMenu;
                old.TenSuKien = ds.TenSuKien;
                old.NgayToChuc = ds.NgayToChuc;
                old.GioBatDau = ds.GioBatDau;
                old.GioKetThuc = ds.GioKetThuc;
                old.TrangThai = ds.TrangThai;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int maDat)
        {
            try
            {
                var db = dataDataContext.Create();
                DatSanh ds = db.DatSanhs
                               .FirstOrDefault(x => x.MaDat == maDat);
                if (ds == null) return false;
                db.DatSanhs.DeleteOnSubmit(ds);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public DatSanh FindById(int maDat)
        {
            var db = dataDataContext.Create();
            return db.DatSanhs
                     .FirstOrDefault(x => x.MaDat == maDat);
        }
    }
}