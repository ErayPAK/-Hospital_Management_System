using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL kütüphanesini ekledik.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmSekreterPaneli : Form
    {
        public FrmSekreterPaneli()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI VE GLOBAL DEĞİŞKENLER
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi(); // Veritabanı bağlantı nesnesi.

        // GÜVENLİK KİLİDİ:
        // Güncelleme işlemi sırasında kullanıcının TC kimlik numarasını değiştirip değiştirmediğini
        // kontrol etmek için hafızada tuttuğumuz değişken.
        string orjinalTC = "";

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmSekreterPaneli_Load(object sender, EventArgs e)
        {
            // Form açıldığında sekreter listesini getir.
            DataTable dt = new DataTable(); // Verileri tutacak sanal tablo.

            // Tüm sekreterleri veritabanından çeken sorgu.
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Sekreterler", bgl.baglanti());

            da.Fill(dt); // Tabloyu doldur.
            dataGridView1.DataSource = dt; // Ekrana DataGridView'e yansıt.
        }



        // ---------------------------------------------------------
        // EKLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            // AYNI TC İLE KAYIT VAR MI?
            // Sisteme aynı kişiyi iki kere eklememek için önce sayıyoruz.
            SqlCommand komutKontrol = new SqlCommand("Select Count(*) From Tbl_Sekreterler where SekreterTC=@p1", bgl.baglanti());
            komutKontrol.Parameters.AddWithValue("@p1", MskTC.Text);

            // ExecuteScalar: Sorgu sonucunda tek bir sayı döner.
            int sayi = Convert.ToInt32(komutKontrol.ExecuteScalar());
            bgl.baglanti().Close();

            if (sayi > 0)
            {
                // Eğer veritabanında bu TC varsa (Sayı > 0), işlemi durdur ve uyar.
                MessageBox.Show("Bu TC numarasına sahip bir sekreter zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // KAYIT YOKSA EKLE 
                SqlCommand komut = new SqlCommand("insert into Tbl_Sekreterler (SekreterAdSoyad,SekreterTC,SekreterSifre) values (@p1,@p2,@p3)", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", TxtAdSoyad.Text);
                komut.Parameters.AddWithValue("@p2", MskTC.Text);
                komut.Parameters.AddWithValue("@p3", TxtSifre.Text);

                komut.ExecuteNonQuery(); // Kaydı yap.
                bgl.baglanti().Close();

                MessageBox.Show("Sekreter Başarıyla Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Listeyi yenile ki yeni kayıt hemen görünsün.
                FrmSekreterPaneli_Load(sender, e);
            }
        }

        // ---------------------------------------------------------
        // SİLME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnSil_Click(object sender, EventArgs e)
        {
            // TC Numarasına göre silme işlemi yapıyoruz.
            SqlCommand komut = new SqlCommand("Delete from Tbl_Sekreterler where SekreterTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", MskTC.Text);

            komut.ExecuteNonQuery(); // Sil.
            bgl.baglanti().Close();

            MessageBox.Show("Sekreter Kaydı Silindi", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            // Listeyi yenile.
            FrmSekreterPaneli_Load(sender, e);
        }

        // ---------------------------------------------------------
        // GÜNCELLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            // 1. GÜVENLİK KONTROLÜ
            // Kullanıcı listeden birini seçtiğinde TC'sini 'orjinalTC' değişkenine atmıştık.
            // Eğer şu an kutuda yazan TC ile orjinal TC aynı değilse, kullanıcı TC'yi değiştirmeye çalışıyor demektir.
            if (MskTC.Text != orjinalTC)
            {
                MessageBox.Show("TC Kimlik Numarası değiştirilemez!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // İşlemi iptal et.
            }

            // 2. GÜNCELLEME SORGUSU
            // TC hariç (AdSoyad ve Şifre) bilgilerini güncelliyoruz.
            SqlCommand komut = new SqlCommand("Update Tbl_Sekreterler set SekreterAdSoyad=@p1,SekreterSifre=@p3 where SekreterTC=@p2", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", TxtAdSoyad.Text);
            komut.Parameters.AddWithValue("@p2", MskTC.Text); // Where koşulu TC
            komut.Parameters.AddWithValue("@p3", TxtSifre.Text);

            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Sekreter Bilgileri Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Listeyi yenile.
            FrmSekreterPaneli_Load(sender, e);
        }

        // ---------------------------------------------------------
        // LİSTEDEN SEÇME
        // ---------------------------------------------------------
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Listeden bir satıra çift tıklandığında bilgileri kutulara taşı.
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            // DataGridView sütun sırasına göre verileri alıyoruz:
            // 0: ID, 1: AdSoyad, 2: TC, 3: Şifre
            TxtAdSoyad.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            MskTC.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            TxtSifre.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();

            // KRİTİK ADIM:
            // Seçilen kişinin gerçek TC'sini hafızaya alıyoruz. Güncellemede kontrol edeceğiz.
            orjinalTC = MskTC.Text;
        }

        // ---------------------------------------------------------
        // TEMİZLEME BUTONU
        // ---------------------------------------------------------
        private void BtnTemizle_Click(object sender, EventArgs e)
        {
            // Yeni kayıt girmek için kutuları temizle.
            TxtAdSoyad.Text = "";
            MskTC.Text = "";
            TxtSifre.Text = "";
            TxtAdSoyad.Focus(); // İmleci isme fokusla.
        }
    }
}