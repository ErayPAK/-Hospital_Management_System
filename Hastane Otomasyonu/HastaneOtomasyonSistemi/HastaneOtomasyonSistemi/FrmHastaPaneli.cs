using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL kütüphanesini ekledik.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmHastaPaneli : Form
    {
        public FrmHastaPaneli()
        {
            InitializeComponent();
        }


        void HastalariListele(string kelime)
        {
            // 1. Tabloyu oluştur
            DataTable dt = new DataTable();

            // 2. Hazır olan SP'yi çağır
            SqlCommand komut = new SqlCommand("sp_HastaArama", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@AranacakKelime", kelime);

            // 3. Verileri çek ve doldur
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt);

            // 4. Grid'e yansıt (Senin formundaki gridin adı dataGridView1 ise)
            dataGridView1.DataSource = dt;
        }




        // ---------------------------------------------------------
        // BAĞLANTI VE GLOBAL DEĞİŞKENLER
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi(); // Veritabanı bağlantı nesnesi.

        // GÜNCELLEME KONTROLÜ İÇİN KRİTİK DEĞİŞKEN:
        // Kullanıcı listeden bir hastayı seçtiğinde, o hastanın orijinal TC'sini buraya kaydedeceğiz.
        // Eğer kullanıcı güncelleme yaparken TC'yi değiştirmeye kalkarsa bu değişkenle kıyaslayıp engelleyeceğiz.
        string orjinalTC = "";

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmHastaPaneli_Load(object sender, EventArgs e)
        {

            // Form açılınca parametreyi boş gönder, hepsi gelsin.
            HastalariListele("");


            // Form açıldığında veya işlem yapıldığında hastaları listelemek için:
            DataTable dt = new DataTable(); // Verileri tutacak sanal tablo.

            // Tüm hastaları çeken SQL sorgusu.
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Hastalar", bgl.baglanti());

            da.Fill(dt); // Verileri tabloya doldur.
            dataGridView1.DataSource = dt; // Tabloyu ekrandaki DataGridView'e bağla.

        }

        // ---------------------------------------------------------
        // EKLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            // 1. ADIM: KAYIT KONTROLÜ
            // Eklenmek istenen TC numarası zaten sistemde var mı?
            SqlCommand komutKontrol = new SqlCommand("Select Count(*) From Tbl_Hastalar where HastaTC=@p1", bgl.baglanti());
            komutKontrol.Parameters.AddWithValue("@p1", MskTC.Text);

            // ExecuteScalar: Sorgu sonucunda tek bir sayı döner.
            int sayi = Convert.ToInt32(komutKontrol.ExecuteScalar());
            bgl.baglanti().Close();

            if (sayi > 0)
            {
                // Eğer sayı 0'dan büyükse, kayıt var demektir. Hata ver.
                MessageBox.Show("Bu TC numarasına sahip bir hasta zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // 2. ADIM: KAYIT YOKSA EKLE
                SqlCommand komut = new SqlCommand("insert into Tbl_Hastalar (HastaAd,HastaSoyad,HastaTC,HastaTelefon,HastaSifre,HastaCinsiyet) values (@p1,@p2,@p3,@p4,@p5,@p6)", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", TxtAd.Text);
                komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);
                komut.Parameters.AddWithValue("@p3", MskTC.Text);
                komut.Parameters.AddWithValue("@p4", MskTelefon.Text);
                komut.Parameters.AddWithValue("@p5", TxtSifre.Text);
                komut.Parameters.AddWithValue("@p6", CmbCinsiyet.Text);

                komut.ExecuteNonQuery(); // Ekleme işlemini yap.
                bgl.baglanti().Close();

                MessageBox.Show("Hasta Başarıyla Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // İşlemden sonra listeyi yenile.
                HastalariListele("");
            }
        }

        // ---------------------------------------------------------
        // SİLME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnSil_Click(object sender, EventArgs e)
        {
            // Hastayı TC numarasına göre siliyoruz.
            SqlCommand komut = new SqlCommand("Delete from Tbl_Hastalar where HastaTC=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", MskTC.Text);

            komut.ExecuteNonQuery(); // Silme işlemini yap.
            bgl.baglanti().Close();

            MessageBox.Show("Hasta Silindi", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            // İşlemden sonra listeyi yenile.
            HastalariListele("");
        }

        // ---------------------------------------------------------
        // GÜNCELLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            // 1. GÜVENLİK KONTROLÜ
            // Kullanıcı listeden seçtiği kişinin TC'sini değiştirmeye çalışırsa engelliyoruz.
            if (MskTC.Text != orjinalTC)
            {
                MessageBox.Show("TC Kimlik Numarası değiştirilemez!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // İşlemi iptsl et, veritabanına gitme.
            }

            // 2. GÜNCELLEME SORGUSU
            // TC hariç diğer bilgileri güncelliyoruz.
            SqlCommand komut = new SqlCommand("Update Tbl_Hastalar set HastaAd=@p1,HastaSoyad=@p2,HastaTelefon=@p4,HastaSifre=@p5,HastaCinsiyet=@p6 where HastaTC=@p3", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", TxtAd.Text);
            komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@p3", MskTC.Text); // Where koşulu için TC (Değişmeyen)
            komut.Parameters.AddWithValue("@p4", MskTelefon.Text);
            komut.Parameters.AddWithValue("@p5", TxtSifre.Text);
            komut.Parameters.AddWithValue("@p6", CmbCinsiyet.Text);

            komut.ExecuteNonQuery(); // Güncellmeyi yap.
            bgl.baglanti().Close();

            MessageBox.Show("Hasta Bilgileri Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Listeyi yenile.
            HastalariListele("");
        }

        // ---------------------------------------------------------
        // LİSTEDEN SEÇME
        // ---------------------------------------------------------
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Listeden yani DataGridView'den bir satıra çift tıklandığında, o satırdaki bilgileri kutucuklara doldur.
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            // DataGridView sütun sırasına göre verileri alıyoruz:
            TxtAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            TxtSoyad.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            MskTC.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            MskTelefon.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            TxtSifre.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            CmbCinsiyet.Text = dataGridView1.Rows[secilen].Cells[6].Value.ToString();

            // KRİTİK NOKTA:
            // Veriler kutulara dolduğu an, bu kişinin gerçek TC'sini hafızaya alıyoruz.
            // Güncelleme sırasında bu değişkeni kontrol edeceğiz.
            orjinalTC = MskTC.Text;
        }

        // ---------------------------------------------------------
        // TEMİZLEME BUTONU
        // ---------------------------------------------------------
        private void BtnTemizle_Click(object sender, EventArgs e)
        {
            // Yeni kayıt girmek için tüm kutuları temizle.
            TxtAd.Text = ""; TxtSoyad.Text = ""; MskTC.Text = "";
            MskTelefon.Text = ""; TxtSifre.Text = ""; CmbCinsiyet.Text = "";
            TxtAd.Focus(); // İmleci isme focusla.
        }

        // ---------------------------------------------------------
        // CANLI ARAMA 
        // ---------------------------------------------------------
        private void TxtArama_TextChanged(object sender, EventArgs e)
        {
            // Kutudaki her harf değişiminde listeyi filtrele
            HastalariListele(TxtArama.Text);
        }

        // ---------------------------------------------------------
        // ARA BUTONU
        // ---------------------------------------------------------
        private void BtnAra_Click(object sender, EventArgs e)
        {
            // Yukarıdaki TextChanged olayıyla aynı mantıkta çalışır.
            // Kullanıcı yazıp butona basmak isterse diye eklendi. Bu daha mantıklı. 

            SqlCommand komut = new SqlCommand("sp_HastaArama", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@AranacakKelime", TxtArama.Text);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            bgl.baglanti().Close();
        }
    }
}