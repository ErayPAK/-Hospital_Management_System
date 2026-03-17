using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmHastaDetay : Form
    {
        public FrmHastaDetay()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // GLOBAL DEĞİŞKENLER VE BAĞLANTI
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi(); // Veritabanı bağlantı nesnesi

        // Giriş formundan gelen Hasta TC'sini burada tutuyoruz.
        public string tc;

        // ---------------------------------------------------------
        // METOT: SAATLERİ LİSTELEME
        // ---------------------------------------------------------
        // Bu metot, seçilen doktorun ve günün randevularına bakar.
        // Sadece "BOŞ" olan saatleri hesaplayıp ComboBox'a ekler.
        void SaatleriListele()
        {
            CmbSaat.Items.Clear(); // Önce listenin içini temizle, üst üste binmesin.

            // KORUMA: Eğer kullanıcı henüz Branş veya Doktor seçmediyse saat hesabı yapma, çık.
            if (string.IsNullOrEmpty(CmbBrans.Text) || string.IsNullOrEmpty(CmbDoktor.Text))
            {
                return;
            }

            // 1. ADIM: DOLU SAATLERİ VERİTABANINDAN ÇEKME
            // Seçilen branş, doktor ve tarihteki 'Dolu' (Durum=1) randevuların saatlerini getir.
            SqlCommand komut = new SqlCommand("Select RandevuSaat From Tbl_Randevular where RandevuBrans=@p1 AND RandevuDoktor=@p2 AND RandevuDurum=1 AND RandevuTarih=@p3", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", CmbBrans.Text);
            komut.Parameters.AddWithValue("@p2", CmbDoktor.Text);
            komut.Parameters.AddWithValue("@p3", DtpTarih.Value.Date);

            SqlDataReader dr = komut.ExecuteReader();

            // Veritabanından gelen dolu saatleri geçici bir listeye atıyoruz.
            // Bu listeyi birazdan referans olarak kullanacağız.
            List<string> doluSaatler = new List<string>();
            while (dr.Read())
            {
                doluSaatler.Add(dr[0].ToString());
            }
            bgl.baglanti().Close();

            // 2. ADIM: TÜM MESAİ SAATLERİNİ DÖNGÜYLE OLUŞTURMA
            // Mesai Başlangıç: 09:00 - Bitiş: 16:00
            DateTime baslangic = DateTime.Parse("09:00");
            DateTime bitis = DateTime.Parse("16:00");

            // Döngü 09:00'dan başlar, 16:00'a kadar 15'er dakika artarak devam eder.
            while (baslangic <= bitis)
            {
                string saatDegeri = baslangic.ToString("HH:mm"); // Örn: "09:15"

                // 3. ADIM: FİLTRELEME
                // Eğer oluşturduğumuz 'saatDegeri', veritabanından çektiğimiz 'doluSaatler' listesinde YOKSA;
                // demek ki bu saat boştur, listeye ekleyebiliriz.
                if (!doluSaatler.Contains(saatDegeri))
                {
                    CmbSaat.Items.Add(saatDegeri);
                }

                baslangic = baslangic.AddMinutes(15); // Bir sonraki satte geç.
            }
        }

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmHastaDetay_Load(object sender, EventArgs e)
        {
            // 1. KİŞİSEL BİLGİLERİ GETİRME
            LblTC.Text = tc; // Girişten gelen TC'yi yaz.

            // İsim Soyisim Çekme
            SqlCommand komut = new SqlCommand("Select HastaAd, HastaSoyad From Tbl_Hastalar where HastaTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", LblTC.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0] + " " + dr[1]; // Ad ve Soyadı birleştirip yaz.
            }
            bgl.baglanti().Close();

            // 2. RANDEVU GEÇMİŞİNİ GETİRME
            // Hastanın daha önce aldığı tüm randevuları listele.
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular where HastaTC=" + tc, bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            // 3. BRANŞLARI YÜKLEME
            // Randevu alma kısmındaki ilk basamak olan Branşları doldur.
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0]);
            }
            bgl.baglanti().Close();

            // Geçmiş tarihe randevu alınmasını engelle.
            DtpTarih.MinDate = DateTime.Now;

            // -----------------------------------------------------------------------
            // 4. RANDEVU ONAY/HATIRLATMA SİSTEMİ
            // -----------------------------------------------------------------------
            // Mantık: Sistem açıldığında otomatik olarak "Yarın" tarihli bir randevu var mı diye bakar.
            // Varsa kullanıcıya sorar: "Gelecek misin?"
            // Evet -> Onaylandı (1) yap.
            // Hayır -> Randevuyu iptal et.

            string yarin = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"); // Yarının tarihini bul.

            // Sorgu: Yarın için bu hastaya ait ve henüz onaylanmamış (0) randevu var mı?
            SqlCommand komutOnay = new SqlCommand("Select Randevuid, RandevuBrans, RandevuSaat From Tbl_Randevular where HastaTC=@p1 and RandevuTarih=@p2 and RandevuOnay=0", bgl.baglanti());
            komutOnay.Parameters.AddWithValue("@p1", LblTC.Text);
            komutOnay.Parameters.AddWithValue("@p2", yarin);

            SqlDataReader drOnay = komutOnay.ExecuteReader();

            // Eğer kayıt varsa (yani yarın randevusu varsa) içeri gir:
            while (drOnay.Read())
            {
                string randevuId = drOnay[0].ToString();
                string brans = drOnay[1].ToString();
                string saat = drOnay[2].ToString();

                // Kullanıcıya Soru Sor (DialogResult)
                DialogResult secim = MessageBox.Show("Yarın saat " + saat + "'de " + brans + " polikliniğinde randevunuz bulunmaktadır. \n\nRandevuya gelecek misiniz? \n(Hayır derseniz randevunuz İPTAL edilecektir.)", "Randevu Hatırlatma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (secim == DialogResult.Yes)
                {
                    // HASTA 'EVET' DEDİ: Durumu Onaylandı (1) yap.
                    drOnay.Close(); // Güncelleme yapbilmek için önce okuyucuyu kapatıyoruz..

                    SqlCommand komutGuncelle = new SqlCommand("Update Tbl_Randevular set RandevuOnay=1 where Randevuid=@k1", bgl.baglanti());
                    komutGuncelle.Parameters.AddWithValue("@k1", randevuId);
                    komutGuncelle.ExecuteNonQuery();

                    MessageBox.Show("Randevunuz onaylanmıştır. Teşekkür ederiz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // HASTA 'HAYIR' DEDİ: Randevuyu iptala et.
                    drOnay.Close();

                    SqlCommand komutSil = new SqlCommand("Delete From Tbl_Randevular where Randevuid=@k1", bgl.baglanti());
                    komutSil.Parameters.AddWithValue("@k1", randevuId);
                    komutSil.ExecuteNonQuery();

                    MessageBox.Show("Randevunuz iptal edilmiştir.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // LİSTEYİ YENİLEME
                // Onaylama veya Silme işlemi yapıldığı için ekrandaki listeyi güncelliyoruz.
                DataTable dtOnay = new DataTable();
                SqlDataAdapter daOnay = new SqlDataAdapter("Select * From Tbl_Randevular where HastaTC=" + tc, bgl.baglanti());
                daOnay.Fill(dtOnay);
                dataGridView1.DataSource = dtOnay;

                break; // Bir tane hatırlatma yeterli, döngüden çık.
            }
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // BRANŞ SEÇİLİNCE
        // ---------------------------------------------------------
        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Branş değiştiği an eski doktorları ve saatleri temizlemeliyiz.
            CmbDoktor.Items.Clear();
            CmbSaat.Items.Clear();

            // Seçilen branşa ait doktorları getir.
            SqlCommand komut3 = new SqlCommand("Select DoktorAd, DoktorSoyad From Tbl_Doktorlar where DoktorBrans=@p1", bgl.baglanti());
            komut3.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr3 = komut3.ExecuteReader();
            while (dr3.Read())
            {
                CmbDoktor.Items.Add(dr3[0] + " " + dr3[1]);
            }
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // DOKTOR SEÇİLİNCE
        // ---------------------------------------------------------
        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Doktor belli olduğuna göre artık boş saatleri hesaplayabiliriz.
            SaatleriListele();
        }

        // ---------------------------------------------------------
        // TARİH DEĞİŞİNCE
        // ---------------------------------------------------------
        private void DtpTarih_ValueChanged(object sender, EventArgs e)
        {
            // Tarih değişirse o günün doluluk oranları farklı olacağı için saatleri yeniden hesapla.
            SaatleriListele();
        }

        // ---------------------------------------------------------
        // RANDEVU AL BUTONU
        // ---------------------------------------------------------
        private void BtnRandevuAl_Click(object sender, EventArgs e)
        {
            // Veri kaydetme işlemini SQL tarafında yazdığımız 'sp_RandevuAl' prosedürü ile yapıyoruz.
            // Bu yöntem kod kirliliğini önler ve güvenlidir.
            SqlCommand komut = new SqlCommand("sp_RandevuAl", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure; // Bunun bir Prosedür olduğunu belirt.

            // Parametreleri gönder
            komut.Parameters.AddWithValue("@Tarih", DtpTarih.Value.Date);
            komut.Parameters.AddWithValue("@Saat", CmbSaat.Text);
            komut.Parameters.AddWithValue("@Brans", CmbBrans.Text);
            komut.Parameters.AddWithValue("@Doktor", CmbDoktor.Text);
            komut.Parameters.AddWithValue("@HastaTC", LblTC.Text);
            komut.Parameters.AddWithValue("@Sikayet", RchSikayet.Text);

            komut.ExecuteNonQuery(); // Kaydet
            bgl.baglanti().Close();

            MessageBox.Show("Randevu Başarıyla Alındı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Randevu alındıktan sonra listeyi yenile.
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular where HastaTC=" + LblTC.Text, bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            // Alınan saat artık dolu olduğu için, ComboBox listesinden çıkarıması lazım.
            // Bu yüzden hesaplamayı tkrar çalıştırıyoruz.
            SaatleriListele();
        }

        // ---------------------------------------------------------
        // TAHLİL SONUÇLARI EKRANI
        // ---------------------------------------------------------
        private void BtnSonuclar_Click(object sender, EventArgs e)
        {
            FrmTahlilSonuclari fr = new FrmTahlilSonuclari();
            fr.TCnumara = LblTC.Text; // TC'yi diğer forma taşı.
            fr.Show();
        }
    }
}