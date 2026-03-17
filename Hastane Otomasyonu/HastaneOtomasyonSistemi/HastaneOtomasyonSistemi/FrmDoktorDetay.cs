using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL kütüphanesini ekledik.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmDoktorDetay : Form
    {
        public FrmDoktorDetay()
        {
            InitializeComponent();
        }


        void HastalariListele(string kelime)
        {
            // Arama prosedürünü çağırıyoruz
            DataTable dt = new DataTable();

            // Eğer kutu boşsa hepsini getirir, doluysa filtreler (SQL'deki Like mantığı)
            SqlCommand komut = new SqlCommand("sp_HastaArama", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@AranacakKelime", kelime);

            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt);
            dataGridView2.DataSource = dt;
        }



        // ---------------------------------------------------------
        // GLOBAL DEĞİŞKENLER VE BAĞLANTI
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi(); // Veritabanı bağlantı köprüsü.

        // Giriş formundan buraya giriş yapan doktorun TC'sini taşıyoruz.
        // public olması, dışarıdan erişilebilir olmasını sağlar.
        public string TC;

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmDoktorDetay_Load(object sender, EventArgs e)
        {
            // 1. ADIM: DOKTOR BİLGİLERİNİ GETİRME
            LblTC.Text = TC; // Giriş ekranından gelen TC'yi ekrana yaz.

            // Sadece bu TC'ye ait doktorun Ad ve Soyadını çeken sorgu.
            SqlCommand komut = new SqlCommand("Select DoktorAd, DoktorSoyad From Tbl_Doktorlar where DoktorTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", LblTC.Text);

            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                // Gelen veriyi birleştirip label'a yazıyoruz.
                LblAdSoyad.Text = dr[0] + " " + dr[1];
            }
            bgl.baglanti().Close();

            // 2. ADIM: RANDEVU LİSTESİNİ ÇEKME (SQL FONKSİYONU)
            DataTable dt = new DataTable();

            // BURASI ÖNEMLİ: 
            // Normalde "Select * From..." diyebilirdik. Ancak SQL'de yazdığımız 'fn_RandevuDetayliDurum'
            // fonksiyonunu kullanarak, randevunun durumunu (Geçmiş, Dolu, Müsait) metin olarak çekiyoruz.
            // Bu sayede C# tarafında ekstra if-else yazmaktan kurtulduk.

            SqlDataAdapter da = new SqlDataAdapter(
                "Select RandevuTarih, RandevuSaat, RandevuBrans, HastaTC, HastaSikayet, " +
                "dbo.fn_RandevuDetayliDurum(RandevuDurum, RandevuTarih) as DurumBilgisi " +
                "From Tbl_Randevular Where RandevuDoktor='" + LblAdSoyad.Text + "'", bgl.baglanti());

            da.Fill(dt); // Verileri sanal tabloya doldur.
            dataGridView1.DataSource = dt; // Tabloyu ekrandaki listeye bağla.

            HastalariListele(""); // Boş gönderiyoruz ki hepsi gelsin
        }

        // ---------------------------------------------------------
        // REÇETE VE TANI GÜNCELLEME
        // ---------------------------------------------------------
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            // Doktor hastayı muayene ettikten sonra reçeteyi sisteme kaydeder.

            // Listeden bir hasta seçili mi?
            if (dataGridView1.SelectedCells.Count > 0)
            {
                // Seçili satırın indeksini al.
                int secilen = dataGridView1.SelectedCells[0].RowIndex;

                // Güncelleme işlemini RandevuID üzerinden yapmalıyız. 
                // DataGridView'in 0. hücresinde ID var.
                string randevuId = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

                // GÜNCELLEME SORGUSU
                // Sadece ilgili randevunun "RandevuRecete" sütununu güncelledik.
                SqlCommand komut = new SqlCommand("Update Tbl_Randevular set RandevuRecete=@p1 where Randevuid=@p2", bgl.baglanti());

                komut.Parameters.AddWithValue("@p1", RchRecete.Text); // Reçete metni
                komut.Parameters.AddWithValue("@p2", randevuId);      // Hangi randevu?

                komut.ExecuteNonQuery(); // İşlemi uygula.
                bgl.baglanti().Close();

                MessageBox.Show("Reçete ve Tanı başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Hasta seçilmediyse uyarı ver.
                MessageBox.Show("Lütfen listeden işlem yapılacak hastayı seçiniz.");
            }
        }

        // ---------------------------------------------------------
        // LİSTEYE TIKLANINCA ŞİKAYETİ GÖSTERME
        // ---------------------------------------------------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Doktor listeden bir hastaya tıkladığında, o hastanın şikayetini
            // sağ taraftaki büyük kutuya taşıyoruz ki doktor rahat okusun.
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            // 7. hücrede "HastaSikayet" verisi vardır.
            RchSikayet.Text = dataGridView1.Rows[secilen].Cells[7].Value.ToString();
        }

        // ---------------------------------------------------------
        // DİĞER FORMLARA GEÇİŞLER
        // ---------------------------------------------------------
        private void BtnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Programı tamamen kapatır.
        }

        private void BtnDuyurular_Click(object sender, EventArgs e)
        {
            // Duyurular penceresini açar.
            FrmDuyurular fr = new FrmDuyurular();
            fr.Show();
        }

        private void BtnLabSonuc_Click(object sender, EventArgs e)
        {
            // TAHLİL SONUÇLARINI GÖRÜNTÜLEME
            // Burada formlar arası veri taşıma işlemi yapıyoruz.

            if (dataGridView1.SelectedCells.Count > 0)
            {
                FrmTahlilSonuclari fr = new FrmTahlilSonuclari();

                // Seçili hastanın TC'sini alıp diğer forma gönderiyoruz.
                int secilen = dataGridView1.SelectedCells[0].RowIndex;
                fr.TCnumara = dataGridView1.Rows[secilen].Cells["HastaTC"].Value.ToString();

                fr.Show(); // Diğer formu aç.
            }
            else
            {
                MessageBox.Show("Lütfen sonuçlarını görmek istediğiniz hastayı seçiniz.");
            }
        }

        // ---------------------------------------------------------
        // DOKTOR BİLGİ DÜZENLEME
        // ---------------------------------------------------------
        private void BtnGuncelle1_Click(object sender, EventArgs e)
        {
            // Burada normal SQL sorgusu yerine STORED PROCEDURE kullanıyoruz.
            // Çünkü şifre değiştirme işlemi kritik bir işlemdir ve kontrolü SQL tarafında (SP) yapılır.

            if (TxtEskiSifre.Text == "")
            {
                MessageBox.Show("Güvenliğiniz için lütfen eski şifrenizi giriniz.");
                return;
            }

            // Prosedürümüzü çağırıyoruz:
            SqlCommand komut = new SqlCommand("sp_DoktorSifreGuncelle", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure; // Bunun bir SP olduğunu belirttik.

            // GİRİŞ PARAMETRELERİ
            komut.Parameters.AddWithValue("@DoktorTC", MskTC.Text);
            komut.Parameters.AddWithValue("@EskiSifre", TxtEskiSifre.Text);
            komut.Parameters.AddWithValue("@YeniSifre", TxtYeniSifre.Text);

            // ÇIKIŞ PARAMETRESİ
            // SQL Prosedürü bize geriye bir sayı döndürecek (1: Başarılı, 0: Başarısız).
            // Bunu yakalamak için özel bir parametre tanımlıyoruz.
            SqlParameter returnDeger = new SqlParameter();
            returnDeger.Direction = ParameterDirection.ReturnValue;
            komut.Parameters.Add(returnDeger);

            komut.ExecuteNonQuery(); // Prosedürü çalıştır.

            // SQL'den dönen cevabı alıyoruz.
            int sonuc = (int)returnDeger.Value;

            // SONUCU KONTROL ET
            if (sonuc == 1)
            {
                // SQL "1" döndürdüyse işlem başarılıdır.
                MessageBox.Show("Şifreniz ve bilgileriniz başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // SQL "0" veya başka bir şey döndürdüyse eski şifre yanlıştır.
                MessageBox.Show("Eski şifrenizi yanlış girdiniz! İşlem iptal edildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bgl.baglanti().Close();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Başlık satırına tıklamayı engelle
            if (e.RowIndex < 0) return;

            // Tıklanan satırı al
            int secilen = dataGridView2.SelectedCells[0].RowIndex;

            // Grid'deki TC sütununun kaçıncı sırada olduğuna dikkat et. 
            // SQL sorgumuzda: id(0), Ad(1), Soyad(2), TC(3), Tel(4) sırasıyla çektik.
            // Bu yüzden Cells[3] TC numarasını verir.

            string secilenTC = dataGridView2.Rows[secilen].Cells[3].Value.ToString();

            // Ortadaki Randevu Panelinde bulunan TC kutusuna (MskTC veya TxtTC) aktar
            MskTC.Text = secilenTC;
        }

        private void TxtHastaAra_TextChanged(object sender, EventArgs e)
        {
            HastalariListele(TxtHastaAra.Text);
        }
    }
}