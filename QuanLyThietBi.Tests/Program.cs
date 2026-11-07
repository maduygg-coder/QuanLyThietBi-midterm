using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThietBi.Entities;
using QuanLyThietBi.BLL;
internal static class Program
{
    private static int count;
    private static readonly DateTime Today = new DateTime(2026,11,7);
    private static ThietBiInput HopLe() => new ThietBiInput
    {
        MaTB=" TB006 ", TenTB=" Máy chiếu mới ", MaLoai=" TRINHCHIEU ",
        NgayMua=Today, SoLuong="3", DonGia="15000000,50"
    };
    private static void Check(bool result,string name)
    {
        if(!result) throw new Exception("FAIL: "+name);
        count++; Console.WriteLine("PASS: "+name);
    }
    private static void Reject(Action action,string name)
    {
        try { action(); } catch(BusinessException) { Check(true,name); return; }
        throw new Exception("FAIL: accepted invalid input: "+name);
    }
    public static void Main()
    {
        var repo=new MemoryThietBi();
        var service=new ThietBiService(repo,new MemoryLoai(),()=>Today);
        service.Nhap(HopLe());
        var saved=repo.Items["TB006"];
        Check(saved.MaTB=="TB006" && saved.TenTB=="Máy chiếu mới" && saved.MaLoai=="TRINHCHIEU","normalize strings before insert");
        Check(saved.DonGia==15000000.50m && saved.NgayMua==Today,"parse decimal and purchase date");
        Reject(()=>service.Nhap(HopLe()),"trimmed duplicate key");
        Reject(()=>service.Nhap(null),"null input");
        Action<ThietBiInput>[] invalid={
            x=>x.MaTB=" ",x=>x.MaTB=new string('A',21),x=>x.TenTB=" ",x=>x.TenTB=new string('A',151),
            x=>x.MaLoai=null,x=>x.MaLoai="UNKNOWN",x=>x.MaLoai=new string('A',11),
            x=>x.NgayMua=Today.AddDays(1),x=>x.NgayMua=new DateTime(1700,1,1),
            x=>x.SoLuong="0",x=>x.SoLuong="-1",x=>x.SoLuong="abc",x=>x.SoLuong="1,5",x=>x.SoLuong="2147483648",
            x=>x.DonGia="0",x=>x.DonGia="-1",x=>x.DonGia="abc",x=>x.DonGia="1.500",x=>x.DonGia="0,001",x=>x.DonGia="10000000000000000"
        };
        for(int i=0;i<invalid.Length;i++)
        {
            var input=HopLe(); input.MaTB="NEW"; invalid[i](input);
            int writes=repo.Writes;
            Reject(()=>service.Nhap(input),"validation "+(i+1));
            Check(repo.Writes==writes,"invalid data never reaches Save "+(i+1));
        }
        var edit=HopLe(); edit.MaTB="DIFFERENT";
        Reject(()=>service.Sua("TB006",edit),"primary key is immutable");
        edit.MaTB="TB006"; edit.TenTB="Đã cập nhật"; edit.SoLuong="12";
        service.Sua("TB006",edit);
        Check(repo.Items.Count==1 && repo.Items["TB006"].TenTB=="Đã cập nhật" && repo.Items["TB006"].SoLuong==12,"update changes existing row only");
        var missing=HopLe(); missing.MaTB="MISSING";
        Reject(()=>service.Sua("MISSING",missing),"update missing row");
        Reject(()=>service.Xoa(" "),"delete without selected key");
        service.Xoa(" TB006 "); Check(repo.Items.Count==0,"delete normalized key");
        Reject(()=>service.Xoa("TB006"),"delete missing row");
        var boundary=HopLe(); boundary.MaTB=new string('A',20); boundary.TenTB=new string('A',150);
        boundary.SoLuong="2147483647"; boundary.DonGia="9999999999999999,99";
        service.Nhap(boundary);
        Check(repo.Items[boundary.MaTB].TenTB.Length==150,"maximum permitted length and numeric boundaries");
        repo.Fail=true; var failed=HopLe(); failed.MaTB="FAIL";
        bool error=false; try { service.Nhap(failed); } catch(InvalidOperationException) { error=true; }
        Check(error && !repo.Items.ContainsKey("FAIL"),"DAL failure propagates without fake success");
        var store=new MemorySettings(); var setup=new DatabaseSetupService(store);
        Reject(()=>setup.TestAndSave(null),"null database configuration");
        Reject(()=>setup.TestAndSave(new DatabaseConnectionOptions {Server="",Database="DB"}),"empty server");
        Reject(()=>setup.TestAndSave(new DatabaseConnectionOptions {Server="server",Database=""}),"empty database");
        Reject(()=>setup.TestAndSave(new DatabaseConnectionOptions {Server=new string('A',256),Database="DB"}),"server length");
        Reject(()=>setup.TestAndSave(new DatabaseConnectionOptions {Server="server",Database=new string('A',129)}),"database length");
        Check(store.Saves==0 && store.Tests==0,"invalid settings never reach DAL");
        setup.TestAndSave(new DatabaseConnectionOptions {Server=" server ",Database=" DB "});
        Check(store.Options.Server=="server" && store.Options.Database=="DB" && store.Order=="test;save;","test server before saving normalized settings");
        store.Fail=true; error=false;
        try { setup.TestAndSave(new DatabaseConnectionOptions {Server="bad",Database="DB"}); } catch(InvalidOperationException) { error=true; }
        Check(error && store.Saves==1 && store.Options.Server=="server","failed server check keeps previous settings");
        Console.WriteLine($"{count} checks passed.");
    }
    private sealed class MemoryThietBi : IThietBiRepository
    {
        public readonly Dictionary<string,ThietBi> Items=new Dictionary<string,ThietBi>(StringComparer.OrdinalIgnoreCase);
        public int Writes;
        public bool Fail;
        public bool TonTai(string ma)=>Items.ContainsKey(ma);
        public List<ThietBiRow> LayTatCa()=>Items.Values.Select(x=>new ThietBiRow {MaTB=x.MaTB,TenTB=x.TenTB}).ToList();
        public void Them(ThietBi x) { if(Fail) throw new InvalidOperationException("simulated database failure"); Items.Add(x.MaTB,x); Writes++; }
        public void Sua(string ma,ThietBi x) { Items[ma]=x; Writes++; }
        public void Xoa(string ma) { Items.Remove(ma); Writes++; }
    }
    private sealed class MemoryLoai : ILoaiThietBiRepository
    {
        public bool TonTai(string ma)=>ma=="TRINHCHIEU";
        public List<LoaiThietBi> LayTatCa()=>new List<LoaiThietBi>{new LoaiThietBi {MaLoai="TRINHCHIEU",TenLoai="Thiết bị trình chiếu"}};
    }
    private sealed class MemorySettings : IConnectionSettingsStore
    {
        public int Tests,Saves;
        public bool Fail;
        public string Order="";
        public DatabaseConnectionOptions Options;
        public DatabaseConnectionOptions Read()=>Options;
        public void TestServer(DatabaseConnectionOptions x) { Tests++; Order+="test;"; if(Fail) throw new InvalidOperationException("server unavailable"); }
        public void Save(DatabaseConnectionOptions x) { Saves++; Order+="save;"; Options=x; }
    }
}
