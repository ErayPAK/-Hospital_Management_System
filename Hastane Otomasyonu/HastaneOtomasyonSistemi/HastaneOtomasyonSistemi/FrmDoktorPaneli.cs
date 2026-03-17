using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL kütüphanesini ekledik.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmDoktorPaneli : Form
    {
        public FrmDoktorPaneli()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // GLOBAL DEĞİŞKENLER
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi(); // Veritabanı bağlantı nesnesi.

        // KRİTİK DEĞİŞKEN: 
        // Güncelleme işleminde kullanıcının TC'yi değiştirip değiştirmediğini anlamak için
        // veritabanındaki eski TC'yi hafızada tutmamız gerekiyor.
        string orjinalTC = "";

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmDoktorPaneli_Load(object sender, EventArgs e)
        {
            // 1. DOKTORLARI LİSTELEME
            // Form açılır açılmaz mevcut doktorları tabloya yani DataGridView'e yüklüyoruz.
            DataTable dt1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da1.Fill(dt1);
            dataGridView1.DataSource = dt1;

            // 2. BRANŞLARI COMBOBOX'A ÇEKME
            // Admin elle "Göz", "Dahiliye" yazmasın, listeden seçsin diye
            // Branşlar tablosundaki isimleri ComboBox aracına dolduruyoruz.
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0]);
            }
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // EKLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            // ADIM 1: KAYIT KONTROLÜ
            // Aynı TC numarasıyla ikinci bir doktor eklenmesini engelliyoruz.
            SqlCommand komutKontrol = new SqlCommand("Select Count(*) From Tbl_Doktorlar where DoktorTC=@p1", bgl.baglanti());
            komutKontrol.Parameters.AddWithValue("@p1", MskTC.Text);

            // ExecuteScalar: Tek bir değer döner.
            int sayi = Convert.ToInt32(komutKontrol.ExecuteScalar());
            bgl.baglanti().Close();

            if (sayi > 0)
            {
                // ADIM 2: HATA DURUMU
                // Eğer veritabanında bu TC varsa uyarı ver ve işlemi iptal et.
                MessageBox.Show("Bu TC Kimlik Numarasına sahip bir doktor zaten kayıtlı! \nLütfen TC'yi kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // ADIM 3: KAYIT İŞLEMİ
                // TC yoksa, yeni doktoru güvenle ekleyebiliriz.
                SqlCommand komut = new SqlCommand("insert into Tbl_Doktorlar (DoktorAd,DoktorSoyad,DoktorBrans,DoktorTC,DoktorSifre) values (@d1,@d2,@d3,@d4,@d5)", bgl.baglanti());
                komut.Parameters.AddWithValue("@d1", TxtAd.Text);
                komut.Parameters.AddWithValue("@d2", TxtSoyad.Text);
                komut.Parameters.AddWithValue("@d3", CmbBrans.Text);
                komut.Parameters.AddWithValue("@d4", MskTC.Text);
                komut.Parameters.AddWithValue("@d5", TxtSifre.Text);

                komut.ExecuteNonQuery(); // Komutu çalıştır.
                bgl.baglanti().Close();

                MessageBox.Show("Doktor Başarıyla Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ADIM 4: LİSTEYİ YENİLEME
                // Yeni eklenen doktorun listede görünmesi için tabloyu tekrar çekiyoruz.
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
                da1.Fill(dt1);
                dataGridView1.DataSource = dt1;
            }
        }

        // ---------------------------------------------------------
        // SİLME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnSil_Click(object sender, EventArgs e)
        {
            // 1. KULLANICI SEÇİM YAPMIŞ MI?
            if (MskTC.Text == "")  //Eğer text boşsa uyarı mesajı ver.
            {
                MessageBox.Show("Lütfen silinecek doktoru listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. SON ONAY
            DialogResult secim = MessageBox.Show(MskTC.Text + " TC numaralı doktoru silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (secim == DialogResult.Yes)
            {
                // TRY-CATCH BLOĞU:
                // SQL Server'da yazdığımız 'trg_DoktorSilinemez' trigger'ı, eğer doktorun randevusu varsa hata verir.
                // O hatayı programın çökmeden yakalaması için try-catch kullanıyoruz.
                try
                {
                    SqlConnection baglanti = bgl.baglanti();

                    SqlCommand komut = new SqlCommand("Delete from Tbl_Doktorlar where DoktorTC=@p1", baglanti);
                    komut.Parameters.AddWithValue("@p1", MskTC.Text);

                    komut.ExecuteNonQuery(); // Silme komutunu gönder.

                    baglanti.Close();

                    // Hata oluşmazsa yani trigger izin verirse burası çalşır:
                    MessageBox.Show("Doktor başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Listeyi Yenile
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter da1 = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
                    da1.Fill(dt1);
                    dataGridView1.DataSource = dt1;

                    // Bilgileri temizle
                    TxtAd.Text = ""; TxtSoyad.Text = ""; CmbBrans.Text = ""; MskTC.Text = ""; TxtSifre.Text = "";
                }
                catch (SqlException ex)
                {
                    // BURASI ÇOK ÖNEMLİ: 
                    // SQL Trigger bir hata verirse program buraya düşer.
                    // ex.Message diyerek SQL'den gelen "Doktorun ileri tarihli randevusu var!" mesajını kullanıcıya gösteririz.
                    MessageBox.Show("İŞLEM BAŞARISIZ! \n\nSebep: " + ex.Message, "Veritabanı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bgl.baglanti().Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Beklenmedik bir hata oluştu: " + ex.Message);
                }
            }
        }

        // ---------------------------------------------------------
        // GÜNCELLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            // 1. GÜVENLİK KONTROLÜ
            // Kullanıcı veritabanında kayıtlı olan TC numarasını değiştirmeye çalışırsa
            // sistem tutarlılığı bozulur. Bu yüzden hafızadaki 'orjinalTC' ile kutudaki 'MskTC'yi kıyaslıyoruz.
            if (MskTC.Text != orjinalTC) //Eğer TC yi değişmeye çalışırlarsa bu blloğa girlir ve hata verilit.
            {
                MessageBox.Show("Doktorların TC Kimlik Numarası değiştirilemez! Lütfen eski TC numarasını yazınız veya işlemi iptal ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // İşlemi iptal et.
            }

            // 2. GÜNCELLEME SORGUSU
            // TC değişmediyse diğer bilgileri güncelle.
            SqlCommand komut = new SqlCommand("Update Tbl_Doktorlar set DoktorAd=@d1,DoktorSoyad=@d2,DoktorBrans=@d3,DoktorSifre=@d5 where DoktorTC=@d4", bgl.baglanti());
            komut.Parameters.AddWithValue("@d1", TxtAd.Text);
            komut.Parameters.AddWithValue("@d2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@d3", CmbBrans.Text);
            komut.Parameters.AddWithValue("@d4", MskTC.Text); // Where koşulu için TC
            komut.Parameters.AddWithValue("@d5", TxtSifre.Text);

            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Doktor Bilgileri Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Listeyi Yenile
            DataTable dt1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da1.Fill(dt1);
            dataGridView1.DataSource = dt1;
        }

        // ---------------------------------------------------------
        // LİSTEDEN SEÇME
        // ---------------------------------------------------------
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Tablodaki satıra çift tıklandığında bilgileri kutucuklara doldur. DataGridView'in özelliğidir.
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            TxtAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            TxtSoyad.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            CmbBrans.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            MskTC.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            TxtSifre.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();

            // KRİTİK ADIM:
            // Veriler kutulara dolduğu anda, bu doktorun gerçek TC'sini hafızaya alıyoruz.
            // Güncelleme butonuna basıldığında bu değişkeni kontrol edeceğiz.
            orjinalTC = MskTC.Text;
        }

        // ---------------------------------------------------------
        // TEMİZLEME BUTONU
        // ---------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            // Yeni kayıt için kutuları boşalt.
            TxtAd.Text = "";
            TxtSoyad.Text = "";
            CmbBrans.Text = "";
            MskTC.Text = "";
            TxtSifre.Text = "";

            // Kullanıcı hemen yazmaya başlayabilsin diye imleci Ad kutusuna fokusla. 
            TxtAd.Focus();
        }
    }
}