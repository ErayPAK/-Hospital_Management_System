using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient; // Veritabanı (SQL) komutlarını kullanabilmek için gerekli kütüphane.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmBransPaneli : Form
    {
        public FrmBransPaneli()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Projenin genelinde kullandığımız veritabanı bağlantı sınıfını çağırıyoruz.
        // Bu nesne bizim SQL ile iletişim kuran köprümüzdür.
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmBransPaneli_Load(object sender, EventArgs e)
        {
            // Form açıldığı anda branş listesinin yani DataGridView'in ekrana gelmesini sağlıyoruz.

            DataTable dt = new DataTable(); // Verileri geçici olarak tutacak sanal bir tablo oluşturduk.

            // Veritabanından tüm branşları çeken sorguyu hazırladık.
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());

            da.Fill(dt); // Veritabanından gelen bilgileri sanal tabloya doldurduk.
            dataGridView1.DataSource = dt; // Tabloyu ekrandaki listeye yani DataGridView'e bağladık.
        }

        // ---------------------------------------------------------
        // EKLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            // ADIM 1: AYNI İSİMLE KAYIT VAR MI KONTROLÜ
            // Kullanıcı "Göz" branşını eklemek istiyor ama sistemde zaten varsa tekrar eklememeli.

            SqlCommand komutKontrol = new SqlCommand("Select Count(*) From Tbl_Branslar where BransAd=@p1", bgl.baglanti());
            komutKontrol.Parameters.AddWithValue("@p1", TxtBrans.Text);

            // ExecuteScalar: Sorgu sonucunda tek bir sayı döneceği için kullanılır.
            int sayi = Convert.ToInt32(komutKontrol.ExecuteScalar());
            bgl.baglanti().Close(); // İşimiz bitince bağlantıyı kapatıyoruz.

            // ADIM 2: KARAR MEKANİZMASI
            if (sayi > 0)
            {
                // Eğer veritabanından 0'dan büyük bir sayı dönerse, kayıt var demektir.
                MessageBox.Show("Bu branş adı zaten sistemde kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // ADIM 3: KAYIT YOKSA EKLEME İŞLEMİ
                SqlCommand komut = new SqlCommand("insert into Tbl_Branslar (BransAd) values (@b1)", bgl.baglanti());
                komut.Parameters.AddWithValue("@b1", TxtBrans.Text); // Parametre kullanarak SQL Injection'ı önlüyoruz.

                komut.ExecuteNonQuery(); // Ekleme işlemini çalıştır.
                bgl.baglanti().Close();

                MessageBox.Show("Branş Başarıyla Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ADIM 4: LİSTEYİ YENİLEME
                // Ekleme yapıldıktan sonra listenin güncel halini tekrar çekiyoruz ki yeni branş hemen görünsün.
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // ---------------------------------------------------------
        // SİLME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnSil_Click(object sender, EventArgs e)
        {
            // GÜVENLİK KONTROLÜ:
            // Silme işlemi ID üzerinden yapılır. Kullanıcı listeden seçim yapmadıysa ID kutusu boş olur.
            // Yanlışlıkla silmeyi önlemek için önce ID var mı diye bakıyoruz.
            if (Txtid.Text == "")
            {
                MessageBox.Show("Lütfen silmek istediğiniz branşı listeden seçiniz.\nSadece isim yazarak silme işlemi yapılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Metodu burada durdur, aşağıya inme.
            }

            // SİLME KOMUTU
            SqlCommand komut = new SqlCommand("Delete From Tbl_Branslar where Bransid=@b1", bgl.baglanti());
            komut.Parameters.AddWithValue("@b1", Txtid.Text); // Txtid değerine göre siliyoruz.

            komut.ExecuteNonQuery(); // Silme işlemini onayla.
            bgl.baglanti().Close();

            MessageBox.Show("Branş Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // LİSTEYİ YENİLEME
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            // TEMİZLİK: Kutuları boşaltalım.
            Txtid.Text = "";
            TxtBrans.Text = "";
        }

        // ---------------------------------------------------------
        // GÜNCELLEME İŞLEMİ
        // ---------------------------------------------------------
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            // Seçili olan ID'yi bulur ve ismini kullanıcının yeni yazdığı isimle değiştirir.
            SqlCommand komut = new SqlCommand("Update Tbl_Branslar set BransAd=@b1 where Bransid=@b2", bgl.baglanti());

            komut.Parameters.AddWithValue("@b1", TxtBrans.Text); // Yeni İsim
            komut.Parameters.AddWithValue("@b2", Txtid.Text);    // Hangi ID?

            komut.ExecuteNonQuery(); // Güncellemeyi uygula.
            bgl.baglanti().Close();

            MessageBox.Show("Branş Bilgisi Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // LİSTEYİ YENİLEME
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        // ---------------------------------------------------------
        // HÜCREYE TIKLAMA OLAYI
        // ---------------------------------------------------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kullanıcı listeden bir satıra tıkladığında, o satırdaki bilgileri
            // yukarıdaki TextBox'lara taşır. Böylece güncelleme veya silme yapabiliriz.

            int secilen = dataGridView1.SelectedCells[0].RowIndex; // Tıklanan satırın numarasını al.

            Txtid.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();    // ID'yi al -> Txtid'ye yaz.
            TxtBrans.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString(); // Branş Adını al -> TxtBrans'a yaz.
        }
    }
}