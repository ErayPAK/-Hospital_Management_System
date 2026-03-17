using System;
using System.Data;
using System.Drawing; // Resim (Image, Bitmap) işlemleri için gerekli kütüphane.
using System.IO;      // Input/Output: İnternetten gelen veriyi RAM'de tutmak için gerekli.
using System.Net;     // Network: İnternetten dosya indirmek ve SSL/TLS güvenlik protokolleri için gerekli.
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmTahlilSonuclari : Form
    {
        public FrmTahlilSonuclari()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // GLOBAL DEĞİŞKENLER
        // ---------------------------------------------------------
        // Doktor veya Hasta detay formundan buraya TC taşınıyor.
        public string TCnumara;
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmTahlilSonuclari_Load(object sender, EventArgs e)
        {
            // Veritabanından gelen sonuçları tutacak sanal tablo.
            DataTable dt = new DataTable();

            // Sadece bu hastaya ait tahlilleri getir. HastaTC'sine göre yapılır bu.
            SqlCommand komut = new SqlCommand("Select TestAd, TestSonuc, DoktorOnay From Tbl_Testler where HastaTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", TCnumara);

            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt); // Tabloyu doldur.
            dataGridView1.DataSource = dt; // DataGridView'e bağla.
        }

        // ---------------------------------------------------------
        // SATIRA TIKLAMA OLAYI
        // ---------------------------------------------------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 1. HATA ÖNLEME: Eğer kullanıcı yanlışlıkla başlık satırına tıklarsa işlem yapma.
            if (e.RowIndex < 0) return;

            // 2. VERİYİ ALMA
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            // Seçilen satırdaki Test Adı (Örn: "Kan Tahlili" veya "Akciğer Röntgeni")
            string testAdi = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

            // Seçilen satırdaki Sonuç (Örn: "Demir eksikliği..." veya "https://resimlinki.com/xray.jpg")
            string sonucDegeri = dataGridView1.Rows[secilen].Cells[1].Value.ToString();

            // 3. METİN KUTUSUNU DOLDURMA
            // İster yazı olsun ister link, önce metin kutusunda gösterilir.
            RchSonucDetay.Text = sonucDegeri;



            // 4. KARAR MEKANİZMASI: BU BİR RESİM Mİ?
            // Eğer test adında "Röntgen" geçiyorsa VEYA sonuç değeri bir internet linki (http) ise;
            if (testAdi.Contains("Röntgen") || sonucDegeri.Contains("http"))
            {
                // Önceki resim kalıntısını temizle.
                PcbResim.Image = null;

                // İNTERNETTEN İNDİRME İŞLEMİ
                try
                {                                                                                               //Bu kısında yapay zeka çok kullanıldı.
                    // GÜVENLİK PROTOKOLÜ (TLS 1.2):
                    // Modern web siteleri eski güvenlik protokollerini kabul etmez.
                    // C# uygulamasının güncel bir tarayıcı gibi davranması için bu ayarı yapıyoruz.
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    // WEB İSTEMCİSİ (SANAL TARAYICI) OLUŞTURMA
                    using (WebClient client = new WebClient())
                    {
                        // KİMLİK GİZLEME:
                        // Bazı sunucular robotları/yazılımları engeller.
                        // Biz burada "Merhaba, ben bir Chrome tarayıcısıyım" diyerek sunucuyu kandırıyoruz.
                        client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                        // DOSYAYI İNDİRME:
                        // Resmi bilgisayara "dosya" olarak kaydetmiyoruz.
                        // Resmi "Byte Dizisi" (0 ve 1'ler) olarak RAM'e indiriyoruz.
                        // Bu yöntem çok daha hızlıdır ve diskte çöp dosya bırakmaz.
                        byte[] resimBytes = client.DownloadData(sonucDegeri);

                        // STREAM İŞLEMİ:
                        // İndirdiğimiz byte yığınını, C#'ın anlayacağı bir resim formatına çeviriyoruz.
                        using (MemoryStream ms = new MemoryStream(resimBytes))
                        {
                            PcbResim.Image = Image.FromStream(ms); // Resmi göster.
                            PcbResim.Visible = true; // PictureBox'ı görünür yap.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Link kırık olabilir, internet olmayabilir. Program çökmesin diye hatayı yakalıyoruz.
                    MessageBox.Show("Resim yüklenemedi. Olası sebepler: \n1. İnternet bağlantısı yok.\n2. Link kırık.\n\nHata detayı: " + ex.Message);
                }
            }
            else
            {
                // Eğer bu bir resim değilse (Örn: Kan tahlili ise), resim kutusunu gizle.
                PcbResim.Visible = false;
            }
        }
    }
}